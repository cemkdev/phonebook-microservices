namespace Shared.Contracts.ReportEvents
{
    public record class ReportRequestedEvent
    {
        public Guid ReportId { get; init; }
    }
}
