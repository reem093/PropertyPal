using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReportApp.Pages.Report_Pages
{
    [Authorize(Roles = "PropertyManager")]
    public class OperationalModel : PageModel
    {
        private readonly ApiService _api;
        private readonly IConfiguration _config;

        public OperationalModel(ApiService api, IConfiguration config)
        {
            _api = api;
            _config = config;
        }

        public List<PropertyReportDTO> PropertyReports { get; set; } = new();

        public async Task OnGetAsync()
        {
            var jwtToken = User.FindFirst("JwtToken")?.Value;

            if (!string.IsNullOrEmpty(jwtToken))
            {
                try
                {
                    string endpoint = _config["ApiSettings:UnitsReportEndpoint"] ?? "api/PropertyDummyData";

                    var results = await _api.GetReportAsync<PropertyReportDTO>(endpoint, jwtToken);

                    if (results != null)
                        PropertyReports = results;
                }
                catch (HttpRequestException)
                {
                    ModelState.AddModelError(string.Empty, "Core API System is currently unreachable.");
                }
            }
        }
    }
}

public record PropertyReportDTO(int Id, string BuildingName, string UnitNumber, string Type, string Amenities, int Size, decimal Revenue, string AvailabilityStatus);