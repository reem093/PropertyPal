using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReportApp.Pages.Report_Pages
{
    [Authorize(Roles = "PropertyManager")]
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            var jwtToken = User.FindFirst("JwtToken")?.Value;

            if (string.IsNullOrEmpty(jwtToken))
            {
                return RedirectToPage("Report_Pages/Login");
            }

            return Page();
        }
    }
}
