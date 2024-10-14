using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser<long>
    {
        public string FullName { get; set; }
        public string? ImagePath { get; set; }
    }
}
