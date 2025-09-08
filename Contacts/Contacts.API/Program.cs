using Contacts.API.Data.Contexts;
using Contacts.API.Services.Abstracts;
using Contacts.API.Services.Implementations;
using MassTransit;
using Microsoft.EntityFrameworkCore;

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
        cfg.Host(builder.Configuration["RabbitMQ:Uri"]);
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
