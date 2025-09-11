using Client.Infrastructure.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient(HttpClientNames.Contacts, c =>
{
    var baseUrl = builder.Configuration["Apis:ContactsApi:BaseUrl"] ?? throw new InvalidOperationException("Apis:ContactsApi:BaseUrl is missing.");
    c.BaseAddress = new Uri(baseUrl);
    c.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddHttpClient(HttpClientNames.Reports, c =>
{
    var baseUrl = builder.Configuration["Apis:ReportsApi:BaseUrl"] ?? throw new InvalidOperationException("Apis:ReportsApi:BaseUrl missing.");
    c.BaseAddress = new Uri(baseUrl);
    c.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Contacts}/{action=Index}/{id?}");

app.Run();
