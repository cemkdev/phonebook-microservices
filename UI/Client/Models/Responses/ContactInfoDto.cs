using Shared.Contracts.Enums;

namespace Client.Models.Responses
{
    public class ContactInfoDto
    {
        public Guid InfoId { get; set; }
        public ContactInfoType InfoType { get; set; }
        public string Content { get; set; }
    }
}
