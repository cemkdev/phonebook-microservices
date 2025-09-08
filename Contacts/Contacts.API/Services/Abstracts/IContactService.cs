using Contacts.API.Domain.Entities;
using Contacts.API.Dto;

namespace Contacts.API.Services.Abstracts
{
    public interface IContactService
    {
        Task<bool> CreateAsync(CreateContactDto createContactDto, CancellationToken cancellationToken);
        Task<Contact?> GetAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Contact>> ListAsync(CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
