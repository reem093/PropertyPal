using Microsoft.AspNetCore.Mvc;

namespace ReportApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OverduePaymentsDummyDataController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<OverduePaymentsDTO>> Get()
        {
            var data = new List<OverduePaymentsDTO>()
            {
                new(1, "John Doe", "A104", "Palm Tower", 1250.00m, 45, "Notice Sent"),
                new(2, "Jane Smith", "B205", "Seaview Residence", 950.00m, 14, "Grace Period"),
                new(3, "Robert Johnson", "V001", "Garden Villas", 4400.00m, 72, "Legal Review"),
                new(4, "Emily Davis", "C301", "City Center Plaza", 1500.00m, 32, "Pending Response")
            };
            return Ok(data);
        }
    }
}