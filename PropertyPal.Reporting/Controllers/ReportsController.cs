using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace PropertyPal.Reporting.Controllers;

public class ReportsController : Controller
{
    private readonly IHttpClientFactory _factory;
    private readonly IConfiguration _configuration;
    public ReportsController(IHttpClientFactory factory, IConfiguration configuration)
    {
        _factory = factory;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index()
    {
        var client = _factory.CreateClient();
        var baseUrl = _configuration["ApiBaseUrl"] ?? "https://localhost:7287";
        var token = await GetToken(client, baseUrl);
        if (token == null)
        {
            ViewBag.Error = "Reporting app could not authenticate with the Web API. Start the API project first.";
            return View(new ReportVm());
        }
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var vm = new ReportVm
        {
            Occupancy = await GetJson(client, $"{baseUrl}/api/reports/occupancy"),
            MaintenanceBacklog = await GetJson(client, $"{baseUrl}/api/reports/maintenance-backlog"),
            OverduePayments = await GetJson(client, $"{baseUrl}/api/reports/overdue-payments")
        };
        return View(vm);
    }

    private async Task<string?> GetToken(HttpClient client, string baseUrl)
    {
        var payload = new { Email = _configuration["ReportingLogin:Email"], Password = _configuration["ReportingLogin:Password"] };
        var response = await client.PostAsync($"{baseUrl}/api/auth/login", new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
        if (!response.IsSuccessStatusCode) return null;
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return doc.RootElement.GetProperty("token").GetString();
    }

    private static async Task<string> GetJson(HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : $"API error: {(int)response.StatusCode}";
    }
}

public class ReportVm
{
    public string Occupancy { get; set; } = "";
    public string MaintenanceBacklog { get; set; } = "";
    public string OverduePayments { get; set; } = "";
}
