using Domain.DTO;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDTO> Login(LoginRequest request);
        Task<bool> RegistNewUser(UserRegistrationRequest request);
    }
}
