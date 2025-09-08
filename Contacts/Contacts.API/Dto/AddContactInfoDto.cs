using Contacts.API.Domain.Enums;

namespace Contacts.API.Dto
{
    public class AddContactInfoDto
    {
        public Guid ContactId { get; set; }
        public ContactInfoType InfoType { get; set; }
        public string Content { get; set; }
    }
}
