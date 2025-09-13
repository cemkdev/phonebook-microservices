using Shared.Contracts.Enums;

namespace Contacts.API.Domain.Entities
{
    public class ContactInfo
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public ContactInfoType InfoType { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public Contact Contact { get; set; }
    }
}
