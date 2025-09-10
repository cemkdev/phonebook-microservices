using MassTransit;
using MongoDB.Driver;
using Reports.Worker.Infrastructure;
using Reports.Worker.Models.Projection;
using Shared.Contracts.ContactInfoEvents;

namespace Reports.Worker.Consumers.Projection
{
    public class ContactInfoRemovedConsumer(MongoContext _mongo) : IConsumer<ContactInfoRemovedEvent>
    {
        public async Task Consume(ConsumeContext<ContactInfoRemovedEvent> context)
        {
            var infoId = context.Message.InfoId;
            await _mongo.ContactInfos.DeleteOneAsync(Builders<ContactInfoProjection>.Filter.Eq(x => x.Id, infoId));
        }
    }
}
