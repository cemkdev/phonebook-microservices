using MassTransit;
using MongoDB.Driver;
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

// Request a Report
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

    return Results.Ok(new { reportId, status = ReportStatus.Preparing.ToString() });
});

// List Reports
app.MapGet("/api/reports", async (MongoContext mongo, CancellationToken cancellationToken) =>
{
    var list = await mongo.Reports
        .Find(FilterDefinition<ReportDocument>.Empty)
        .SortByDescending(x => x.RequestedAt)
        .ToListAsync(cancellationToken);

    return Results.Ok(list);
});

// Show a Report
app.MapGet("/api/reports/{id:guid}", async (Guid id, MongoContext mongo, CancellationToken cancellationToken) =>
{
    var doc = await mongo.Reports
        .Find(x => x.Id == id)
        .FirstOrDefaultAsync(cancellationToken);

    return doc is not null ? Results.Ok(doc) : Results.NotFound();
});

app.Run();
