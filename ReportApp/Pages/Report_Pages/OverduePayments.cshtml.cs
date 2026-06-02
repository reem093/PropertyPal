using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReportApp.Pages.Report_Pages
{
    public class OverduePaymentsModel : PageModel
    {
        private readonly ApiService _api;
        private readonly IConfiguration _config;

        public OverduePaymentsModel(ApiService api, IConfiguration config)
        {
            _api = api;
            _config = config;
        }

        public List<OverduePaymentsDTO> OverduePayments { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var jwtToken = User.FindFirst("jwtToken")?.Value;

            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToPage("Login");
            }

            try
            {
                string endpoint = _config["ApiSettings:OverduePaymentsEndpoint"] ?? "api/OverduePaymentsDummyData";
                var results = await _api.GetReportAsync<OverduePaymentsDTO>(endpoint, jwtToken);

                if (results != null)
                {
                    OverduePayments = results;
                }
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Core API Financial system is currently unreachable.");
            }

            return Page();
        }
    }
}

public record OverduePaymentsDTO(int Id, string TenantName, string UnitNumber, string BuildingName, decimal AmountOverdue, int DaysPastDue, string Status);