using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReportApp.Pages.Report_Pages
{
    public class OverviewModel : PageModel
    {
        private readonly ApiService _api;
        private readonly IConfiguration _config;

        public OverviewModel(ApiService api, IConfiguration config)
        {
            _api = api;
            _config = config;
        }

        public List<OverviewDTO> GeneralInfoList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var jwtToken = User.FindFirst("JwtToken")?.Value;

            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToPage("Login");
            }

            try
            {
                string endpoint = _config["ApiSettings:OverviewReportEndpoint"] ?? "api/OverviewGeneralInfoDummyData";

                var results = await _api.GetReportAsync<OverviewDTO>(endpoint, jwtToken);

                if (results != null)
                {
                    GeneralInfoList = results;
                }
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Core API Overview service is currently unreachable.");
            }

            return Page();
        }
    }
}

public record OverviewDTO(int PropertyID, string PropertyName, string Location, int TotalUnits, int OccupiedUnits, int ActiveTickets, decimal GrossRevenue);