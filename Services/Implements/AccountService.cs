using Financial.DTO;
using Financial.DTO.Auth;
using Financial.Model;
using Financial.Responses;
using Financial.Services.Interfaces;
using Financial.State;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using static Financial.Responses.CustomResponses;

namespace Financial.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;
        private readonly ProtectedLocalStorage _sessionStorage;
        private readonly IConstantState _constantss;

        public AccountService(HttpClient httpClient, ProtectedLocalStorage sessionStorage, IConstantState constantss)
        {
            _httpClient = httpClient;
            _sessionStorage = sessionStorage;
            _constantss = constantss;

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _constantss.JWTToken);
        }

        public Task<string> GetSample()
        {
            throw new NotImplementedException();
        }

        private async Task GetRefreshToken()
        {
            var token = await _sessionStorage.GetAsync<string>("c_user_session");
            var response = await _httpClient.PostAsJsonAsync("api/account/refresh-token", new UserSession()
            {
                JWTToken = token.Value
            });
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            //_constantss.JWTToken = result!.JWTToken;
            await _sessionStorage.SetAsync("c_user_session", result!.JWTToken);
            await _sessionStorage.SetAsync("c_refresh_session", result!.JWTToken);

            Console.WriteLine($"Refresh Token: {_constantss.JWTToken} || {result!.JWTToken}");
        }

        public async Task<CustomResponses.UserResponse> GetUserAsync()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _constantss.JWTToken);

            var response = await _httpClient.GetAsync("api/account/GetUsers");
            var result = await response.Content.ReadFromJsonAsync<UserResponse>();
            return result!;
        }

        public async Task<CustomResponses.LoginResponse> LoginAsync(LoginDto model)
        {
            Console.WriteLine("LoginAsync");
            Console.WriteLine(_httpClient.BaseAddress.ToString());
            var response = await _httpClient.PostAsJsonAsync("api/account/login", model);
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result!;
        }

        public async Task<CustomResponses.LoginResponse> RefreshToken(UserSession userSession)
        {
            if (_constantss.JWTToken == "") return null;

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _constantss.JWTToken);

            var response = await _httpClient.PostAsJsonAsync("api/account/refresh-token", userSession);
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            await _sessionStorage.SetAsync("c_user_session", result!.JWTToken);
            await _sessionStorage.SetAsync("c_refresh_session", result!.JWTToken);

            return result!;
        }

        public async Task<CustomResponses.RegistrationResponse> RegisterAsync(RegisterDto model)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _constantss.JWTToken);

            var response = await _httpClient.PostAsJsonAsync("api/account/register", model);
            var result = await response.Content.ReadFromJsonAsync<RegistrationResponse>();
            return result!;
        }

        public async Task<CustomResponses.UserResponse> UpdateUserAsync(UserAccount model)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _constantss.JWTToken);

            var response = await _httpClient.PostAsJsonAsync("api/account/UpdateUser", model);
            var result = await response.Content.ReadFromJsonAsync<UserResponse>();
            return result!;
        }
    }
}
