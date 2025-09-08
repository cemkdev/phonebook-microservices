namespace Shared.Contracts.ContactInfoEvents
{
    public record class ContactInfoRemovedEvent
    {
        public Guid ContactId { get; init; }
        public short InfoType { get; init; } // enum
        public string Content { get; init; }
        public DateTime OccurredAt { get; init; }
    }
}
