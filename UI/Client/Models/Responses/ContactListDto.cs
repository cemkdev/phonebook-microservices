namespace Client.Models.Responses
{
    public class ContactListDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
