using MassTransit;
using MongoDB.Driver;
using Reports.Worker.Infrastructure;
using Reports.Worker.Models.Report;
using Shared.Contracts.Enums;
using Shared.Contracts.ReportEvents;

namespace Reports.Worker.Consumers.Report
{
    public class ReportRequestedConsumer(MongoContext _mongo, IConfiguration _configuration) : IConsumer<ReportRequestedEvent>
    {
        public async Task Consume(ConsumeContext<ReportRequestedEvent> context)
        {
            var reportId = context.Message.ReportId;
            var cancellationToken = context.CancellationToken;

            try
            {
                // Yapay olarak geciktir. appsettings'te default 60 sn olarak belirlendi. (Preparing'i ekranda net olarak gorebilmek icin)
                var delaySec = _configuration.GetValue<int?>("Reports:ReportDelay") ?? 0;

                if (delaySec > 0)
                    await Task.Delay(TimeSpan.FromSeconds(delaySec), cancellationToken);

                //throw new Exception("Fake error!....."); // Burasi aktif edilirse, hata alinmasi ve Failed durumu simule edilebilir...

                // Location'i olan satirlar. ContactID ve Location'lari ile birlikte. ContactId'ler distinct edildi.
                var locationRows = await _mongo.ContactInfos
                    .Find(x => x.InfoType == ContactInfoType.Location)
                    .Project(x => new { x.Content, x.ContactId })
                    .ToListAsync(cancellationToken);

                // { "Izmir" => [321321, 352132], "Istanbul" => [123213] } gibi...
                var byLocation = locationRows
                    .GroupBy(r => r.Content)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(r => r.ContactId).Distinct().ToList()
                    );

                // byLocation listesinden Sadece id'leri cek. Zaten distinct ama yine de ekledik, kafa karismasin.
                var allContactIds = byLocation.Values.SelectMany(v => v).Distinct().ToList();

                var phoneCountsByContact = new Dictionary<Guid, int>();
                if (allContactIds.Count > 0)
                {
                    // Yine tum ContactInfos'taki phone'lu satirlardan, allContactIds listesinde bulunan id'lerin oldugu satirlarin(Yani location'li satirlar aslinda) sadece id'lerini list olarak al.
                    // (byLocation yani allContactIds'teki id'lerin oldugu satirlarda phone olmayan satirlar da olabilir. Bu filtre o yuzden)
                    var phones = await _mongo.ContactInfos
                        .Find(x => x.InfoType == ContactInfoType.Phone && allContactIds.Contains(x.ContactId))
                        .Project(x => x.ContactId)
                        .ToListAsync(cancellationToken);

                    // Telefonu olan kisileri telefon sayilarina gore gruplar.
                    phoneCountsByContact = phones
                        .GroupBy(c => c) // { Guid-1, Guid-1 }, { Guid-2 }, { Guid-3, Guid-3, Guid-3 } gibi...
                        .ToDictionary(g => g.Key, g => g.Count()); // { Guid-1: 2, Guid-2: 1, Guid-3: 3 } gibi...
                }

                var items = new List<ReportItem>();
                foreach (var kvp in byLocation) // Konumlari donuyoruz tek tek ReportItem referansli listeyi dolduruyoruz.
                {
                    var location = kvp.Key;
                    var contacts = kvp.Value;

                    var contactCount = contacts.Count;
                    var phoneCount = contacts.Sum(cid => phoneCountsByContact.TryGetValue(cid, out var n) ? n : 0); // Bir konumdaki kisiler listesindeki(cantacts), 'telefonu olanlarin telefon sayilariniphoneCountsByContact' toplar.

                    items.Add(new ReportItem
                    {
                        Location = location,
                        ContactCount = contactCount,
                        PhoneCount = phoneCount
                    });
                }

                var filter = Builders<ReportDocument>.Filter.Eq(x => x.Id, reportId); // Ilgili Report'u cek, event'ten gelen reportId ile.
                var update = Builders<ReportDocument>.Update // Bu report'un gerekli alanlarini guncelle.
                    .Set(x => x.Status, ReportStatus.Completed)
                    .Set(x => x.Items, items)
                    .Set(x => x.Error, null);

                await _mongo.Reports.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                // Failed durumu
                var filter = Builders<ReportDocument>.Filter.Eq(x => x.Id, reportId);
                var update = Builders<ReportDocument>.Update
                    .Set(x => x.Status, ReportStatus.Failed)
                    .Set(x => x.Error, ex.Message);

                await _mongo.Reports.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
            }
        }
    }
}
