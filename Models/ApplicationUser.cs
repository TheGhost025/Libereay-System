using Microsoft.AspNetCore.Identity;

namespace Libereay_System.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; } // Admin or Member
    }
}
