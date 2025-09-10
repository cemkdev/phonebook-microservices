namespace Shared.Contracts.ContactInfoEvents
{
    public record class ContactInfoRemovedEvent
    {
        public Guid InfoId { get; init; }
        public Guid ContactId { get; init; }
        public short InfoType { get; init; }
        public string Content { get; init; }
    }
}
