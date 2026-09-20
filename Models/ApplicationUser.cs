using Microsoft.AspNetCore.Identity;

namespace BestStoreMVC.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Address { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
