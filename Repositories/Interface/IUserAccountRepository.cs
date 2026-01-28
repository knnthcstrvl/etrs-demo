using Financial.DTO;
using Financial.DTO.Auth;
using Financial.Model;
using static Financial.Responses.CustomResponses;

namespace Financial.Repositories.Interface
{
    public interface IUserAccountRepository
    {
        Task<RegistrationResponse> RegisterAsync(RegisterDto model);
        Task<LoginResponse> LoginAsync(LoginDto model);
        LoginResponse RefreshToken(UserSession userSession);
        Task<UserResponse> GetUsersAsync();
        Task<UserResponse> UpdateUserAsync(UserAccount model);
    }
}
