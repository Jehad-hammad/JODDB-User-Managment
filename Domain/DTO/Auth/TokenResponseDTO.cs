namespace Domain.DTO
{
    public class TokenResponseDTO
    {
        public string AccessToken { get; set; }
        public string FullName { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
    }
}
