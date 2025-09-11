using Client.Infrastructure.Api;
using Client.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class ContactsController(IHttpClientFactory _httpClientFactory) : Controller
    {
        private HttpClient Api() => _httpClientFactory.CreateClient(HttpClientNames.Contacts);

        public async Task<IActionResult> Index()
        {
            var list = await Api().GetFromJsonAsync<List<ContactListDto>>(ApiRoutes.Contacts.ContactList);
            return View(list);
        }
    }
}
