namespace Shared.Contracts.ContactEvents
{
    public record class ContactDeletedEvent
    {
        public Guid ContactId { get; set; }
    }
}
