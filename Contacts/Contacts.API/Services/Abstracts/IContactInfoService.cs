using Contacts.API.Dto;

namespace Contacts.API.Services.Abstracts
{
    public interface IContactInfoService
    {
        Task<bool> AddAsync(AddContactInfoDto addContactInfoDto, CancellationToken cancellationToken);
        Task<bool> RemoveAsync(Guid infoId, CancellationToken cancellationToken);
    }
}
