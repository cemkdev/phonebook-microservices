using MassTransit;
using Reports.Worker.Consumers.Projection;
using Reports.Worker.Infrastructure;
using Shared.Contracts;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<MongoContext>();
builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<ContactInfoAddedConsumer>();
    configurator.AddConsumer<ContactInfoRemovedConsumer>();
    configurator.AddConsumer<ContactDeletedConsumer>();

    configurator.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]);

        _configurator.ReceiveEndpoint(RabbitMQSettings.Worker_ContactInfoAddedEventQueue, e => e.ConfigureConsumer<ContactInfoAddedConsumer>(context));
        _configurator.ReceiveEndpoint(RabbitMQSettings.Worker_ContactInfoRemovedEventQueue, e => e.ConfigureConsumer<ContactInfoRemovedConsumer>(context));
        _configurator.ReceiveEndpoint(RabbitMQSettings.Worker_ContactDeletedEventQueue, e => e.ConfigureConsumer<ContactDeletedConsumer>(context));
    });
});

var host = builder.Build();
host.Run();
