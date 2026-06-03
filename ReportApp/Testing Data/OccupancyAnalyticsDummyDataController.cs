using Microsoft.AspNetCore.Mvc;

namespace ReportApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OccupancyAnalyticsDummyDataController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<BuildingOccupancyDTO>> Get()
        {
            var data = new List<BuildingOccupancyDTO>()
            {
                new(1, "Palm Tower", 150, 142, 0.946, 215400.00m, "Optimal"),
                new(2, "Oakridge Apartments", 85, 71, 0.835, 94250.00m, "Stable"),
                new(3, "Marina Bay Towers", 210, 195, 0.928, 342000.00m, "Optimal"),
                new(4, "Sunset Ridge Commons", 60, 41, 0.683, 62000.00m, "Underperforming")
            };
            return Ok(data);
        }
    }
}