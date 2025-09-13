using Shared.Contracts.Enums;

namespace Shared.Contracts.ReportEvents
{
    public class ReportStatusChangedEvent
    {
        public Guid ReportId { get; set; }
        public ReportStatus Status { get; set; }
    }
}
