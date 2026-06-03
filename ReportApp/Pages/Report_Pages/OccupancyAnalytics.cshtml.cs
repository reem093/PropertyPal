using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReportApp.Pages.Report_Pages
{
    [Authorize(Roles = "PropertyManager")]
    public class OccupancyAnalyticsModel : PageModel
    {
        private readonly ApiService _api;
        private readonly IConfiguration _config;

        public OccupancyAnalyticsModel(ApiService api, IConfiguration config)
        {
            _api = api;
            _config = config;
        }

        public List<BuildingOccupancyDTO> BuildingStats { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var jwtToken = User.FindFirst("JwtToken")?.Value;

            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToPage("Login");
            }

            try
            {
                string endpoint = _config["ApiSettings:OccupancyAnalyticsEndpoint"] ?? "api/OccupancyAnalyticsDummyData";
                var results = await _api.GetReportAsync<BuildingOccupancyDTO>(endpoint, jwtToken);

                if (results != null)
                {
                    BuildingStats = results;
                }
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Core API Performance system is currently unreachable.");
            }

            return Page();
        }
    }
}

public record BuildingOccupancyDTO(int Id, string BuildingName, int TotalUnits, int OccupiedUnits, double OccupancyRate, decimal TotalMonthlyRevenue, string PortfolioStatus);