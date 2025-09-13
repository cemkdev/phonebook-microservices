using Contacts.API.Data.Contexts;
using Contacts.API.Domain.Entities;
using Contacts.API.Services.Abstracts;
using Contacts.API.Services.Implementations;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Enums;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// DbContext
builder.Services.AddDbContext<ContactsDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
});

// Services
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IContactInfoService, ContactInfoService>();

// MassTransit
builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ"]);
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

#region SEED: PostgreSQL (Contacts.API)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContactsDbContext>();

    // Pending migration varsa uygula (yoksa no-op)
    if ((await db.Database.GetPendingMigrationsAsync()).Any())
        await db.Database.MigrateAsync();

    var adaId = Guid.Parse("a00dcd69-60b5-4aed-9a65-ff0f7d1f6707");
    var jamesId = Guid.Parse("5428a4f2-331a-4ab6-b667-2906a98bc7d5");
    var ericId = Guid.Parse("be8d1d2d-f105-45ee-a07e-69b70d9accfd");
    var graceId = Guid.Parse("3da40152-1b3c-40c1-9fe3-0fed919287c8");

    if (!await db.Contacts.AnyAsync())
    {
        db.Contacts.AddRange(new[]
        {
            new Contact { Id = adaId,   FirstName = "Ada",   LastName = "Lovelace", Company = "Analytical Engines Ltd", CreatedAt = DateTime.UtcNow },
            new Contact { Id = jamesId, FirstName = "James", LastName = "Howlett",  Company = "Shiny Knives Ltd",       CreatedAt = DateTime.UtcNow },
            new Contact { Id = ericId,  FirstName = "Eric",  LastName = "Cartman",  Company = "Funny Bully Ltd",        CreatedAt = DateTime.UtcNow },
            new Contact { Id = graceId, FirstName = "Grace", LastName = "Hopper",   Company = "COBOL Inc.",             CreatedAt = DateTime.UtcNow },
        });
        await db.SaveChangesAsync();
    }

    if (!await db.ContactInfos.AnyAsync())
    {
        db.ContactInfos.AddRange(new[]
        {
            // Ada
            new ContactInfo { Id = Guid.Parse("996fd93a-fce3-4b51-8ed6-2d9c535c9852"), ContactId = adaId,   InfoType = ContactInfoType.Location, Content = "Izmir",              CreatedAt = DateTime.UtcNow },
            new ContactInfo { Id = Guid.Parse("d6daf89d-19a7-4aba-818a-6679a4ffdb20"), ContactId = adaId,   InfoType = ContactInfoType.Phone,    Content = "+90 532 000 0001",   CreatedAt = DateTime.UtcNow },
            new ContactInfo { Id = Guid.Parse("da833a32-44b6-480a-9ad3-6eccdc8fb691"), ContactId = adaId,   InfoType = ContactInfoType.Email,    Content = "ada@example.com",    CreatedAt = DateTime.UtcNow },

            // Grace
            new ContactInfo { Id = Guid.Parse("c594156a-7241-44f4-a15d-3c8e695e21b0"), ContactId = graceId, InfoType = ContactInfoType.Location, Content = "Istanbul",           CreatedAt = DateTime.UtcNow },
            new ContactInfo { Id = Guid.Parse("8aba14e4-dea1-41a5-8719-decd714fac86"), ContactId = graceId, InfoType = ContactInfoType.Phone,    Content = "+90 532 000 0002",   CreatedAt = DateTime.UtcNow },

            // James
            new ContactInfo { Id = Guid.Parse("bd9cd529-3e4e-4593-9175-fbd23564ff5b"), ContactId = jamesId, InfoType = ContactInfoType.Phone,    Content = "+90 532 333 3333",   CreatedAt = DateTime.UtcNow },
            new ContactInfo { Id = Guid.Parse("3dac9ec9-d28c-48a7-94d2-aa2f616948bb"), ContactId = jamesId, InfoType = ContactInfoType.Location, Content = "Ankara",              CreatedAt = DateTime.UtcNow },

            // Eric
            new ContactInfo { Id = Guid.Parse("676958cb-5558-4f6b-b34b-085fe36a9391"), ContactId = ericId,  InfoType = ContactInfoType.Phone,    Content = "+90 532 111 1111",   CreatedAt = DateTime.UtcNow },
            new ContactInfo { Id = Guid.Parse("39d6cc36-bc66-449a-8107-98c194fdb26a"), ContactId = ericId,  InfoType = ContactInfoType.Location, Content = "Southpark",          CreatedAt = DateTime.UtcNow },
        });
        await db.SaveChangesAsync();
    }
}
#endregion

app.Run();
