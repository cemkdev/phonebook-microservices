using Contacts.API.Data.Contexts;
using Contacts.API.Domain.Entities;
using Contacts.API.Dto;
using Contacts.API.Services.Abstracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.ContactInfoEvents;

namespace Contacts.API.Services.Implementations
{
    public class ContactInfoService(ContactsDbContext contactsDbContext, IPublishEndpoint publishEndpoint) : IContactInfoService
    {
        public async Task<bool> AddAsync(AddContactInfoDto addContactInfoDto, CancellationToken cancellationToken)
        {
            var exists = await contactsDbContext.Contacts.AnyAsync(c => c.Id == addContactInfoDto.ContactId, cancellationToken);
            if (!exists) return false;

            var contactInfo = new ContactInfo
            {
                Id = Guid.NewGuid(),
                ContactId = addContactInfoDto.ContactId,
                InfoType = addContactInfoDto.InfoType,
                Content = addContactInfoDto.Content,
                CreatedAt = DateTime.UtcNow
            };
            contactsDbContext.ContactInfos.Add(contactInfo);
            await contactsDbContext.SaveChangesAsync();

            // Info Added Event Publish
            var contactInfoAddedEvent = new ContactInfoAddedEvent
            {
                ContactId = addContactInfoDto.ContactId,
                InfoType = (short)addContactInfoDto.InfoType,
                Content = addContactInfoDto.Content,
                OccurredAt = DateTime.UtcNow
            };
            await publishEndpoint.Publish(contactInfoAddedEvent, cancellationToken);

            return true;
        }

        public async Task<bool> RemoveAsync(Guid infoId, CancellationToken cancellationToken)
        {
            var info = await contactsDbContext.ContactInfos
                       .FirstOrDefaultAsync(x => x.Id == infoId, cancellationToken);
            if (info == null) return false;

            contactsDbContext.ContactInfos.Remove(info);
            await contactsDbContext.SaveChangesAsync();

            // Info Removed Event Publish
            var contactInfoRemovedEvent = new ContactInfoRemovedEvent
            {
                ContactId = info.ContactId,
                InfoType = (short)info.InfoType,
                Content = info.Content,
                OccurredAt = DateTime.UtcNow
            };
            await publishEndpoint.Publish(contactInfoRemovedEvent, cancellationToken);

            return true;
        }
    }
}
