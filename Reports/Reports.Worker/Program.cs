using MassTransit;
using MongoDB.Bson;
using Reports.Worker.Consumers.Projection;
using Reports.Worker.Consumers.Report;
using Reports.Worker.Infrastructure;
using Reports.Worker.Models.Projection;
using Shared.Contracts;
using Shared.Contracts.Enums;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<MongoContext>();
builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<ContactInfoAddedConsumer>();
    configurator.AddConsumer<ContactInfoRemovedConsumer>();
    configurator.AddConsumer<ContactDeletedConsumer>();
    configurator.AddConsumer<ReportRequestedConsumer>();

    configurator.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]);

        _configurator.ReceiveEndpoint(RabbitMQSettings.Worker_ContactInfoAddedEventQueue, e => e.ConfigureConsumer<ContactInfoAddedConsumer>(context));
        _configurator.ReceiveEndpoint(RabbitMQSettings.Worker_ContactInfoRemovedEventQueue, e => e.ConfigureConsumer<ContactInfoRemovedConsumer>(context));
        _configurator.ReceiveEndpoint(RabbitMQSettings.Worker_ContactDeletedEventQueue, e => e.ConfigureConsumer<ContactDeletedConsumer>(context));
        _configurator.ReceiveEndpoint(RabbitMQSettings.Worker_ReportRequestedEventQueue, e => e.ConfigureConsumer<ReportRequestedConsumer>(context));
    });
});

var host = builder.Build();

#region SEED: MongoDB (Reports.Worker)
using (var scope = host.Services.CreateScope())
{
    var mongo = scope.ServiceProvider.GetRequiredService<MongoContext>();

    var ciCount = await mongo.ContactInfos.EstimatedDocumentCountAsync();
    if (ciCount == 0)
    {
        var adaId = Guid.Parse("a00dcd69-60b5-4aed-9a65-ff0f7d1f6707");
        var jamesId = Guid.Parse("5428a4f2-331a-4ab6-b667-2906a98bc7d5");
        var ericId = Guid.Parse("be8d1d2d-f105-45ee-a07e-69b70d9accfd");
        var graceId = Guid.Parse("3da40152-1b3c-40c1-9fe3-0fed919287c8");

        await mongo.ContactInfos.InsertManyAsync(new[]
        {
            // Ada
            new ContactInfoProjection { Id = Guid.Parse("996fd93a-fce3-4b51-8ed6-2d9c535c9852"), ContactId = adaId,   InfoType = ContactInfoType.Location, Content = "Izmir" },
            new ContactInfoProjection { Id = Guid.Parse("d6daf89d-19a7-4aba-818a-6679a4ffdb20"), ContactId = adaId,   InfoType = ContactInfoType.Phone,    Content = "+90 532 000 0001" },
            new ContactInfoProjection { Id = Guid.Parse("da833a32-44b6-480a-9ad3-6eccdc8fb691"), ContactId = adaId,   InfoType = ContactInfoType.Email,    Content = "ada@example.com" },

            // Grace
            new ContactInfoProjection { Id = Guid.Parse("c594156a-7241-44f4-a15d-3c8e695e21b0"), ContactId = graceId, InfoType = ContactInfoType.Location, Content = "Istanbul" },
            new ContactInfoProjection { Id = Guid.Parse("8aba14e4-dea1-41a5-8719-decd714fac86"), ContactId = graceId, InfoType = ContactInfoType.Phone,    Content = "+90 532 000 0002" },

            // James
            new ContactInfoProjection { Id = Guid.Parse("bd9cd529-3e4e-4593-9175-fbd23564ff5b"), ContactId = jamesId, InfoType = ContactInfoType.Phone,    Content = "+90 532 333 3333" },
            new ContactInfoProjection { Id = Guid.Parse("3dac9ec9-d28c-48a7-94d2-aa2f616948bb"), ContactId = jamesId, InfoType = ContactInfoType.Location, Content = "adana" },

            // Eric
            new ContactInfoProjection { Id = Guid.Parse("676958cb-5558-4f6b-b34b-085fe36a9391"), ContactId = ericId,  InfoType = ContactInfoType.Phone,    Content = "+90 532 111 1111" },
            new ContactInfoProjection { Id = Guid.Parse("39d6cc36-bc66-449a-8107-98c194fdb26a"), ContactId = ericId,  InfoType = ContactInfoType.Location, Content = "Southpark" },
        });
    }

    // reports — Worker modelinde RequestedAt yok; API okurken gerek duydugu icin BsonDocument ile yaziyoruz.
    var reportsBson = mongo.Reports.Database.GetCollection<BsonDocument>("reports");
    var rCount = await reportsBson.EstimatedDocumentCountAsync();
    if (rCount == 0)
    {
        var completedId = Guid.Parse("23e8e4cc-a4b8-4288-b61e-6f00d53ac344");
        var failedId = Guid.Parse("8fd69723-e467-4e75-9596-b99123709fac");

        var completed = new BsonDocument
        {
            { "_id", completedId.ToString() },
            { "RequestedAt", DateTime.UtcNow.AddMinutes(-10) },
            { "Status", (int)ReportStatus.Completed },
            { "Items", new BsonArray
                {
                    new BsonDocument { { "Location", "Izmir"    }, { "ContactCount", 1 }, { "PhoneCount", 1 } },
                    new BsonDocument { { "Location", "Istanbul" }, { "ContactCount", 1 }, { "PhoneCount", 1 } },
                    new BsonDocument { { "Location", "Southpark"}, { "ContactCount", 1 }, { "PhoneCount", 2 } }
                }
            },
            { "Error", BsonNull.Value }
        };

        var failed = new BsonDocument
        {
            { "_id", failedId.ToString() },
            { "RequestedAt", DateTime.UtcNow.AddMinutes(-5) },
            { "Status", (int)ReportStatus.Failed },
            { "Items", new BsonArray() },
            { "Error", "Fake error!....." }
        };

        await reportsBson.InsertManyAsync(new[] { completed, failed });
    }
}
#endregion

await host.RunAsync();
