using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReportApp.Pages.Report_Pages
{
    public class MaintenanceRequestModel : PageModel
    {
        private readonly ApiService _api;
        private readonly IConfiguration _config;

        public MaintenanceRequestModel(ApiService api, IConfiguration config)
        {
            _api = api;
            _config = config;
        }

        public List<MaintenanceRequestDTO> MaintenanceRequests { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var jwtToken = User.FindFirst("JwtToken")?.Value;

            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToPage("/Report_Pages/Login");
            }

            try
            {
                string endpoint = _config["ApiSettings:MaintenanceReportEndpoint"] ?? "api/MaintenanceRequestDummyData";

                var results = await _api.GetReportAsync<MaintenanceRequestDTO>(endpoint, jwtToken);

                if (results != null)
                {
                    MaintenanceRequests = results;
                }
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Core API Maintenance system is currently unreachable");
            }

            return Page();
        }
    }
}

public record MaintenanceRequestDTO(int Id, string title, string description, string tenantName, string unitName, string category, string priority, string status);