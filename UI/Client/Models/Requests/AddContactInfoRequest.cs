using Shared.Contracts.Enums;

namespace Client.Models.Requests
{
    public class AddContactInfoRequest
    {
        public Guid ContactId { get; set; }
        public ContactInfoType InfoType { get; set; }
        public string Content { get; set; }
    }
}
