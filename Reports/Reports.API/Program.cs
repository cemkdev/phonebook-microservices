using MassTransit;
using Reports.API.Infrastructure;
using Reports.API.Models.Report;
using Shared.Contracts.Enums;
using Shared.Contracts.ReportEvents;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<MongoContext>();
builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]);
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/api/reports", async (MongoContext _mongo, IPublishEndpoint _publish) =>
{
    var reportId = Guid.NewGuid();

    var doc = new ReportDocument
    {
        Id = reportId,
        RequestedAt = DateTime.UtcNow,
        Status = ReportStatus.Preparing,
        Items = new List<ReportItem>(),
        Error = null
    };

    await _mongo.Reports.InsertOneAsync(doc);
    await _publish.Publish(new ReportRequestedEvent { ReportId = reportId });

    return Results.Ok(new { reportId, status = "Preparing" });
});

app.Run();
