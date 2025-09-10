using MassTransit;
using MongoDB.Driver;
using Reports.Worker.Infrastructure;
using Reports.Worker.Models.Projection;
using Shared.Contracts.ContactEvents;

namespace Reports.Worker.Consumers.Projection
{
    public class ContactDeletedConsumer(MongoContext _mongo) : IConsumer<ContactDeletedEvent>
    {
        public async Task Consume(ConsumeContext<ContactDeletedEvent> context)
        {
            var contactId = context.Message.ContactId;
            await _mongo.ContactInfos.DeleteManyAsync(Builders<ContactInfoProjection>.Filter.Eq(x => x.ContactId, contactId));
        }
    }
}
