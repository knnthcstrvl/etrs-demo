using Financial.Model;

namespace Financial.Responses
{
    public class CustomResponses
    {
        public record RegistrationResponse(bool Flag = false, string Message = null!);
        public record LoginResponse(bool Flag = false, string Message = null!, string JWTToken = null!);
        public record UserResponse(bool Flag = false, string Message = null!, List<UserAccount> Data = null!);
    }
}
