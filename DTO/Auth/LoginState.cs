namespace Financial.DTO.Auth
{
    public class LoginState
    {
        public record LoginRequest(string Username, string Password);

        public record AuthResponse
        {
            public string AccessToken { get; init; } = string.Empty;
            public string RefreshToken { get; init; } = string.Empty;
        }
    }
}
