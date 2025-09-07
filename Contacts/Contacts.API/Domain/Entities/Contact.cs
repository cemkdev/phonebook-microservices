namespace Contacts.API.Domain.Entities
{
    public class Contact
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<ContactInfo> ContactInfos { get; set; }
    }
}
