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

        public async Task<Contact?> GetAsync(Guid id, CancellationToken cancellationToken)
            => await contactsDbContext.Contacts
                .Include(c => c.ContactInfos)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public async Task<List<Contact>> ListAsync(CancellationToken cancellationToken)
            => await contactsDbContext.Contacts.AsNoTracking().ToListAsync(cancellationToken);

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
                ContactId = id,
                OccurredAt = DateTime.UtcNow
            };
            await publishEndpoint.Publish(contactDeletedEvent, cancellationToken);

            return true;
        }
    }
}
