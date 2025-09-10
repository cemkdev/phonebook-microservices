using MassTransit;
using MongoDB.Driver;
using Reports.Worker.Infrastructure;
using Reports.Worker.Projection;
using Shared.Contracts.ContactInfoEvents;

namespace Reports.Worker.Consumers
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
                .SetOnInsert(nameof(ContactInfoProjection.InfoType), (int)e.InfoType)
                .SetOnInsert(x => x.Content, e.Content);

            await _mongo.ContactInfos.UpdateOneAsync(
                filter: filter,
                update: update,
                options: new UpdateOptions { IsUpsert = true });
        }
    }
}
