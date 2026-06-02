using Microsoft.AspNetCore.Mvc;

namespace ReportApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OverviewGeneralInfoDummyDataController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<OverviewDTO>> Get()
        {
            // Providing the aggregated mock portfolio overview metrics
            var overviewData = new List<OverviewDTO>()
            {
                new(101, "Grandview Heights", "Downtown Core", 150, 142, 3, 215400.00m),
                new(102, "Oakridge Apartments", "North Suburbs", 85, 71, 12, 94250.00m),
                new(103, "Marina Bay Towers", "Waterfront District", 210, 195, 7, 342000.00m),
                new(104, "Sunset Ridge Commons", "East Valley", 60, 58, 2, 62000.00m)
            };

            return Ok(overviewData);
        }
    }
}