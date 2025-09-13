namespace Client.Models.Responses
{
    public class ContactDetailDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<ContactInfoDto> Info { get; set; } = new();
    }
}
