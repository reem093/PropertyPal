using Microsoft.AspNetCore.Mvc;
using PropertyPal.Mvc.ViewModels;

namespace PropertyPal.Mvc.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult MaintenanceBacklog()
        {
            var backlog = new List<MaintenanceBacklogViewModel>
            {
                new MaintenanceBacklogViewModel
                {
                    Id = 1,
                    PropertyName = "Seef Apartment 12",
                    TenantName = "Ali Hassan",
                    IssueTitle = "Air conditioner not cooling",
                    Priority = "High",
                    Status = "Open",
                    RequestedDate = DateTime.Today.AddDays(-5)
                },
                new MaintenanceBacklogViewModel
                {
                    Id = 2,
                    PropertyName = "Juffair Villa 4",
                    TenantName = "Sara Ahmed",
                    IssueTitle = "Water leakage in kitchen",
                    Priority = "Medium",
                    Status = "In Progress",
                    RequestedDate = DateTime.Today.AddDays(-3)
                }
            };

            return View(backlog);
        }
    }
}