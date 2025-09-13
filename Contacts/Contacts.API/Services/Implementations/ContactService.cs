using Contacts.API.Data.Contexts;
using Contacts.API.Domain.Entities;
using Contacts.API.Dto;
using Contacts.API.Services.Abstracts;
using MassTransit;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.ContactEvents;

namespace Contacts.API.Services.Implementations
{
    public class ContactService(ContactsDbContext contactsDbContext, IPublishEndpoint publishEndpoint) : IContactService
    {
        public async Task<bool> CreateAsync(CreateContactDto createContactDto, CancellationToken cancellationToken)
        {
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                FirstName = createContactDto.FirstName,
                LastName = createContactDto.LastName,
                Company = createContactDto.Company,
                CreatedAt = DateTime.UtcNow,
                ContactInfos = new List<ContactInfo>()
            };

            contactsDbContext.Add(contact);
            var affected = await contactsDbContext.SaveChangesAsync(cancellationToken);
            return affected > 0;
        }

        public async Task<ContactDetailDto> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            var contact = contactsDbContext
                            .Contacts
                            .AsNoTracking()
                            .Where(c => c.Id == id);

            var contactDto = await contact
                            .Select(c => new ContactDetailDto
                            {
                                Id = c.Id,
                                FirstName = c.FirstName,
                                LastName = c.LastName,
                                Company = c.Company,
                                CreatedAt = c.CreatedAt,
                                Info = c.ContactInfos
                                        .OrderBy(i => i.InfoType)
                                        .Select(i => new ContactInfoDto
                                        {
                                            InfoId = i.Id,
                                            InfoType = i.InfoType,
                                            Content = i.Content
                                        })
                                        .ToList()
                            })
                            .FirstOrDefaultAsync(cancellationToken);

            return contactDto;
        }

        public async Task<List<ContactListDto>> ListAsync(CancellationToken cancellationToken)
        {
            var contacts = contactsDbContext.Contacts.AsNoTracking();

            var contactsDto = await contacts
                            .Select(c => new ContactListDto
                            {
                                Id = c.Id,
                                FirstName = c.FirstName,
                                LastName = c.LastName,
                                Company = c.Company,
                                CreatedAt = c.CreatedAt
                            })
                            .ToListAsync(cancellationToken);

            return contactsDto;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var contact = await contactsDbContext.Contacts.FindAsync([id], cancellationToken);

            if (contact == null)
                return false;

            contactsDbContext.Contacts.Remove(contact); // ContactInfos cascade
            await contactsDbContext.SaveChangesAsync(cancellationToken);

            // Contact Deleted Event Publish
            var contactDeletedEvent = new ContactDeletedEvent
            {
                ContactId = id
            };
            await publishEndpoint.Publish(contactDeletedEvent, cancellationToken);

            return true;
        }
    }
}
