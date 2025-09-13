using Shared.Contracts.Enums;

namespace Contacts.API.Dto
{
    public class ContactInfoDto
    {
        public Guid InfoId { get; set; }
        public ContactInfoType InfoType { get; set; }
        public string Content { get; set; }
    }
}
