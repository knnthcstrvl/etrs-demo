using Financial.DTO;
using Financial.DTO.Auth;
using Financial.Model;
using static Financial.Responses.CustomResponses;

namespace Financial.Services.Interfaces
{
    public interface IAccountService
    {
        Task<RegistrationResponse> RegisterAsync(RegisterDto model);
        Task<LoginResponse> LoginAsync(LoginDto model);
        Task<LoginResponse> RefreshToken(UserSession userSession);
        Task<string> GetSample();
        Task<UserResponse> GetUserAsync();
        Task<UserResponse> UpdateUserAsync(UserAccount model);
    }
}
