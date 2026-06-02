using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace ReportApp.Pages.Report_Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApiService _api;
        public LoginModel(ApiService api) => _api = api;

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();


            var tokenJson = await _api.AuthenticateAsync(Username, Password);

            if (tokenJson == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login credentials or unauthorized role access.");
                return Page();
            }

            using var tokenDocument = JsonDocument.Parse(tokenJson);

            string token = tokenDocument.RootElement.GetProperty("access_token").GetString();

            /*
            var token = await _api.AuthenticateAsync(Username, Password);

            if (token == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login credentials or unauthorized role access.");
                return Page();
            }*/



            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            var roleClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "role" || c.Type == ClaimTypes.Role)?.Value;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Username),
                new Claim("JwtToken", token)
            };

            if (!string.IsNullOrEmpty(roleClaim))
                claims.Add(new Claim(ClaimTypes.Role, roleClaim));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToPage("/Report_Pages/Index");
        }

        public void OnGet() { }
    }
}
