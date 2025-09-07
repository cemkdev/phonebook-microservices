using Contacts.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Contacts.API.Data.Contexts
{
    public class ContactsDbContext(DbContextOptions<ContactsDbContext> options) : DbContext(options)
    {
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
    }
}
