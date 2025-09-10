using MassTransit;
using Reports.Worker.Consumers;
using Reports.Worker.Infrastructure;
using Shared.Contracts;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<MongoContext>();
builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<ContactInfoAddedConsumer>();

    configurator.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]);

        _configurator.ReceiveEndpoint(RabbitMQSettings.Worker_ContactInfoAddedEventQueue, e => e.ConfigureConsumer<ContactInfoAddedConsumer>(context));
    });
});

var host = builder.Build();
host.Run();
