using Microsoft.AspNetCore.Identity;

namespace PropertyPal.Api.Models 
{
    public class ApplicationUser : IdentityUser
    {
        // Custom properties only
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
    }
}