using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Reports.API.Hubs;
using Shared.Contracts.ReportEvents;

namespace Reports.API.Consumers
{
    public class ReportStatusChangedConsumer(IHubContext<ReportsHub> _hub) : IConsumer<ReportStatusChangedEvent>
    {
        public Task Consume(ConsumeContext<ReportStatusChangedEvent> context)
        {
            var m = context.Message;
            return _hub.Clients.Group("reports").SendAsync("ReportUpdated", new { reportId = m.ReportId, status = m.Status });
        }
    }
}
