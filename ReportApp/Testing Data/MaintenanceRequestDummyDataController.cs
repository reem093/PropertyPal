using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ReportApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceRequestDummyDataController : ControllerBase
    {

        [HttpGet]
        public ActionResult<List<MaintenanceRequestDTO>> Get()
        {
            var maintenanceRequests = new List<MaintenanceRequestDTO>()
            {
                new(1,  "AC Not Cooling",              "Air conditioner is blowing warm air.",          "Ahmed Ali",      "A101", "HVAC",       "High",     "Submitted"),
                new(2,  "Water Leak",                  "Leak under kitchen sink.",                      "Fatima Hassan",  "B205", "Plumbing",   "Critical", "Assigned"),
                new(3,  "Broken Light",                "Living room light fixture not working.",        "Mohammed Saleh", "C302", "Electrical", "Low",      "In Progress"),
                new(4,  "Door Lock Issue",             "Main entrance lock is difficult to open.",      "Sara Ahmed",     "D110", "Security",   "Medium",   "Resolved"),
                new(5,  "Elevator Noise",              "Elevator making unusual noises.",               "Ali Yusuf",      "Tower A", "Elevator", "High",     "Closed"),

                new(6,  "Blocked Drain",               "Bathroom drain is clogged.",                    "Noor Khalid",    "A203", "Plumbing",   "Medium",   "Submitted"),
                new(7,  "Power Outlet Failure",        "Bedroom outlet has no power.",                  "Hassan Jaber",   "B101", "Electrical", "High",     "Assigned"),
                new(8,  "Window Crack",                "Small crack in bedroom window.",                "Mariam Ali",     "C105", "General",    "Low",      "In Progress"),
                new(9,  "Water Heater Issue",          "No hot water available.",                       "Zainab Abbas",   "D202", "Plumbing",   "Critical", "Resolved"),
                new(10, "Internet Port Fault",         "Wall network port not functioning.",            "Yousef Hamad",   "A304", "IT",         "Low",      "Closed"),

                new(11, "Paint Peeling",               "Paint peeling near balcony.",                   "Ali Hasan",      "B210", "General",    "Low",      "Submitted"),
                new(12, "Smoke Detector Beeping",      "Smoke detector battery warning.",               "Fatima Yusuf",   "C401", "Safety",     "Medium",   "Assigned"),
                new(13, "AC Water Dripping",           "Water dripping from indoor AC unit.",           "Omar Salman",    "D105", "HVAC",       "High",     "In Progress"),
                new(14, "Garage Door Malfunction",     "Garage door not closing properly.",             "Reem Ahmed",     "Villa 3", "Security", "High",     "Resolved"),
                new(15, "Ceiling Water Stain",         "Water stain visible on ceiling.",               "Hasan Ali",      "A402", "Plumbing",   "Critical", "Closed"),

                new(16, "Kitchen Cabinet Repair",      "Cabinet door hinge broken.",                    "Mona Hassan",    "B302", "Carpentry",  "Medium",   "Submitted"),
                new(17, "Floor Tile Damage",           "Cracked tile in hallway.",                      "Jassim Noor",    "C204", "General",    "Low",      "Assigned"),
                new(18, "Generator Check",             "Backup generator warning indicator.",           "Building Mgmt",  "Common Area", "Electrical", "High", "In Progress"),
                new(19, "Water Pressure Low",          "Low water pressure in bathroom.",               "Abbas Karim",    "D404", "Plumbing",   "Medium",   "Resolved"),
                new(20, "Fire Exit Sign Not Lit",      "Emergency exit sign is off.",                   "Property Team",  "Floor 5", "Safety",  "Critical", "Closed")
       
            };

            return Ok(maintenanceRequests);
        }
    }
}
