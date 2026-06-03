using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PropertyPal.ViewModels;

namespace PropertyPal.Controllers;

[AllowAnonymous]
public class PublicTrackingController : Controller
{
    private readonly IHttpClientFactory _factory;
    private readonly IConfiguration _configuration;
    public PublicTrackingController(IHttpClientFactory factory, IConfiguration configuration) { _factory = factory; _configuration = configuration; }

    [HttpGet]
    public IActionResult Maintenance() => View(new PublicLookupVm());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Maintenance(PublicLookupVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var baseUrl = _configuration["ApiBaseUrl"] ?? "https://localhost:7173";
        var client = _factory.CreateClient();
        var url = $"{baseUrl}/api/public/maintenance/lookup?ticketNumber={Uri.EscapeDataString(vm.TicketNumber)}&phone={Uri.EscapeDataString(vm.Phone)}";
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode) vm.Error = "No matching maintenance request found. Check the ticket number and registered phone.";
        else vm.ResultJson = await response.Content.ReadAsStringAsync();
        return View(vm);
    }
}
