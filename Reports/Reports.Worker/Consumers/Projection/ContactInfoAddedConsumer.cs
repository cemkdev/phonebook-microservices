using MassTransit;
using MongoDB.Driver;
using Reports.Worker.Infrastructure;
using Reports.Worker.Models.Projection;
using Shared.Contracts.ContactInfoEvents;
using Shared.Contracts.Enums;

namespace Reports.Worker.Consumers.Projection
{
    public class ContactInfoAddedConsumer(MongoContext _mongo) : IConsumer<ContactInfoAddedEvent>
    {
        public async Task Consume(ConsumeContext<ContactInfoAddedEvent> context)
        {
            var e = context.Message;

            var filter = Builders<ContactInfoProjection>.Filter.Eq(x => x.Id, e.InfoId);

            var update = Builders<ContactInfoProjection>.Update
                .SetOnInsert(x => x.Id, e.InfoId)
                .SetOnInsert(x => x.ContactId, e.ContactId)
                .SetOnInsert(nameof(ContactInfoProjection.InfoType), (ContactInfoType)e.InfoType)
                .SetOnInsert(x => x.Content, e.Content);

            await _mongo.ContactInfos.UpdateOneAsync(
                filter: filter,
                update: update,
                options: new UpdateOptions { IsUpsert = true });
        }
    }
}
