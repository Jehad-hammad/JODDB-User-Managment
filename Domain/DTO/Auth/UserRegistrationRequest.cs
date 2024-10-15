using Microsoft.AspNetCore.Http;

namespace Domain.DTO
{
    public class UserRegistrationRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
        public IFormFile? file { get; set; }
        public string? filePath { get; set; }
    }
}
