using Microsoft.AspNetCore.Identity;

namespace WebApi.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string TwoFactorAuthSecret { get; set; }
        
        public bool TwoFactorAuthEnabled { get; set; }
    }
}
