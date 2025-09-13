using Client.Infrastructure.Api;
using Client.Models.Reports.Response;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class ReportsController(IHttpClientFactory httpFactory) : Controller
    {
        private HttpClient Api() => httpFactory.CreateClient(HttpClientNames.Reports);

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var list = await Api().GetFromJsonAsync<List<ReportDocumentResponse>>(ApiRoutes.Reports.List);
            return View(list ?? new List<ReportDocumentResponse>());
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var doc = await Api().GetFromJsonAsync<ReportDocumentResponse>(ApiRoutes.Reports.ById(id));
            if (doc is null) return NotFound();
            return View(doc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Request()
        {
            var resp = await Api().PostAsJsonAsync(ApiRoutes.Reports.Request, new { });

            if (resp.IsSuccessStatusCode)
            {
                TempData["Flash"] = "Report request submitted.";
                TempData["FlashType"] = "success";
            }
            else
            {
                TempData["Flash"] = "Report request failed.";
                TempData["FlashType"] = "danger";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
