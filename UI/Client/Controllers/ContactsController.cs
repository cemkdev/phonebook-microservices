using Client.Infrastructure.Api;
using Client.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class ContactsController(IHttpClientFactory _httpClientFactory) : Controller
    {
        private HttpClient Api() => _httpClientFactory.CreateClient(HttpClientNames.Contacts);

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var list = await Api().GetFromJsonAsync<List<ContactListDto>>(ApiRoutes.Contacts.ContactList);
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var detail = await Api().GetFromJsonAsync<ContactDetailDto>(ApiRoutes.Contacts.ContactById(id));
            if (detail is null) return NotFound();

            return View(detail);
        }
    }
}
