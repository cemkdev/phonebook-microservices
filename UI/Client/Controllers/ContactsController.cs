using Client.Infrastructure.Api;
using Client.Models.Requests;
using Client.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class ContactsController(IHttpClientFactory _httpClientFactory) : Controller
    {
        private HttpClient Api() => _httpClientFactory.CreateClient(HttpClientNames.Contacts);

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateContactRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.Company))
            {
                ModelState.AddModelError(string.Empty, "All fields are required.");
                return View(request);
            }

            var response = await Api().PostAsJsonAsync(ApiRoutes.Contacts.CreateContact, request);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Could not create the contact.");
                return View(request);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = Api();

            const int maxAttempts = 5;
            const int delayMs = 800;

            List<ContactListDto>? list = null;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    list = await client.GetFromJsonAsync<List<ContactListDto>>(ApiRoutes.Contacts.ContactList);
                    break; // success
                }
                catch (HttpRequestException) when (attempt < maxAttempts)
                {
                    await Task.Delay(delayMs);
                }
                catch (TaskCanceledException) when (attempt < maxAttempts)
                {
                    await Task.Delay(delayMs);
                }
            }

            return View(list ?? new List<ContactListDto>());
        }


        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var detail = await Api().GetFromJsonAsync<ContactDetailDto>(ApiRoutes.Contacts.ContactById(id));
            if (detail is null) return NotFound();

            return View(detail);
        }

        [HttpGet]
        public IActionResult AddInfo(Guid contactId)
        {
            ViewBag.ContactId = contactId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInfo(AddContactInfoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                ModelState.AddModelError(nameof(request.Content), "Content is required.");
                ViewBag.ContactId = request.ContactId;
                ViewBag.SelectedInfoType = request.InfoType; // seçimi korumak için
                return View();
            }

            // Type-Base Duplicate Control (Just Client-Side)
            var detail = await Api().GetFromJsonAsync<ContactDetailDto>(ApiRoutes.Contacts.ContactById(request.ContactId));
            if (detail?.Info != null && detail.Info.Any(i => i.InfoType == request.InfoType))
            {
                ModelState.AddModelError(string.Empty, $"This contact already has an info of type '{request.InfoType}'.");
                ViewBag.ContactId = request.ContactId;
                ViewBag.SelectedInfoType = request.InfoType;
                return View();
            }

            var response = await Api().PostAsJsonAsync(ApiRoutes.Contacts.AddInfo, request);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Could not add the info.");
                ViewBag.ContactId = request.ContactId;
                ViewBag.SelectedInfoType = request.InfoType;
                return View();
            }

            return RedirectToAction(nameof(Details), new { id = request.ContactId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveInfo(Guid contactId, Guid infoId)
        {
            var response = await Api().DeleteAsync(ApiRoutes.Contacts.DeleteInfo(infoId));
            return RedirectToAction(nameof(Details), new { id = contactId });
        }
    }
}
