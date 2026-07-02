using Financial.DTO;
using Financial.DTO.Auth;
using Financial.Model;
using Financial.Responses;
using Financial.Services.Interfaces;
using Financial.State;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static Financial.Responses.CustomResponses;

namespace Financial.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;
        private readonly ProtectedLocalStorage _protectedLocalStorage;
        private readonly ProtectedSessionStorage _protectedSessionStorage;
        private readonly IConstantState _constantss;
        private readonly NavigationManager _navigationManager;

        public AccountService(
            HttpClient httpClient,
            ProtectedLocalStorage protectedLocalStorage,
            ProtectedSessionStorage protectedSessionStorage,
            IConstantState constantss,
            NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _protectedLocalStorage = protectedLocalStorage;
            _protectedSessionStorage = protectedSessionStorage;
            _constantss = constantss;
            _navigationManager = navigationManager;
        }

        public Task<string> GetSample()
        {
            throw new NotImplementedException();
        }

        private async Task GetRefreshToken()
        {
            EnsureBaseAddress();
            ApplyAuthorizationHeader();

            var token = await GetStoredTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            var response = await _httpClient.PostAsJsonAsync("api/account/refresh-token", new UserSession()
            {
                JWTToken = token
            });
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result is null || string.IsNullOrWhiteSpace(result.JWTToken))
            {
                return;
            }

            await StoreTokenAsync(result.JWTToken);

            Console.WriteLine($"Refresh Token: {_constantss.JWTToken} || {result.JWTToken}");
        }

        public async Task<CustomResponses.UserResponse> GetUserAsync()
        {
            EnsureBaseAddress();
            ApplyAuthorizationHeader();

            var response = await _httpClient.GetAsync("api/account/GetUsers");
            var result = await response.Content.ReadFromJsonAsync<UserResponse>();
            return result!;
        }

        public async Task<CustomResponses.LoginResponse> LoginAsync(LoginDto model)
        {
            try
            {
                EnsureBaseAddress();
                ClearAuthorizationHeader();

                var response = await _httpClient.PostAsJsonAsync("api/account/login", model);
                var result = await ReadJsonOrErrorAsync<LoginResponse>(response);

                return result ?? new LoginResponse(false, "Login failed.");
            }
            catch (Exception ex)
            {
                return new LoginResponse(false, $"Login failed: {ex.Message}");
            }
        }

        public async Task<CustomResponses.LoginResponse> RefreshToken(UserSession userSession)
        {
            if (string.IsNullOrWhiteSpace(_constantss.JWTToken))
            {
                return new LoginResponse(false, "No token available.");
            }

            EnsureBaseAddress();
            ApplyAuthorizationHeader();

            var response = await _httpClient.PostAsJsonAsync("api/account/refresh-token", userSession);
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result is null)
            {
                return new LoginResponse(false, "Token refresh failed.");
            }

            if (!string.IsNullOrWhiteSpace(result.JWTToken))
            {
                await StoreTokenAsync(result.JWTToken);
            }

            return result;
        }

        public async Task<CustomResponses.RegistrationResponse> RegisterAsync(RegisterDto model)
        {
            EnsureBaseAddress();
            ApplyAuthorizationHeader();

            var response = await _httpClient.PostAsJsonAsync("api/account/register", model);
            var result = await response.Content.ReadFromJsonAsync<RegistrationResponse>();
            return result!;
        }

        public async Task<CustomResponses.UserResponse> UpdateUserAsync(UserAccount model)
        {
            EnsureBaseAddress();
            ApplyAuthorizationHeader();

            var response = await _httpClient.PostAsJsonAsync("api/account/UpdateUser", model);
            var result = await response.Content.ReadFromJsonAsync<UserResponse>();
            return result!;
        }

        private void EnsureBaseAddress()
        {
            var baseUri = _navigationManager.BaseUri;
            if (_httpClient.BaseAddress is null || _httpClient.BaseAddress.AbsoluteUri != baseUri)
            {
                _httpClient.BaseAddress = new Uri(baseUri);
            }
        }

        private void ApplyAuthorizationHeader()
        {
            if (string.IsNullOrWhiteSpace(_constantss.JWTToken))
            {
                ClearAuthorizationHeader();
                return;
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _constantss.JWTToken);
        }

        private void ClearAuthorizationHeader()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        private async Task<string?> GetStoredTokenAsync()
        {
            var localToken = await _protectedLocalStorage.GetAsync<string>("c_user_session");
            if (!string.IsNullOrWhiteSpace(localToken.Value))
            {
                return localToken.Value;
            }

            var sessionToken = await _protectedSessionStorage.GetAsync<string>("c_user_session");
            return sessionToken.Success ? sessionToken.Value : null;
        }

        private async Task StoreTokenAsync(string jwtToken)
        {
            var persistentToken = await _protectedLocalStorage.GetAsync<string>("c_user_session");
            if (!string.IsNullOrWhiteSpace(persistentToken.Value))
            {
                await _protectedLocalStorage.SetAsync("c_user_session", jwtToken);
                return;
            }

            await _protectedSessionStorage.SetAsync("c_user_session", jwtToken);
        }

        private static async Task<T?> ReadJsonOrErrorAsync<T>(HttpResponseMessage response)
        {
            try
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch
            {
                var content = await response.Content.ReadAsStringAsync();
                var message = string.IsNullOrWhiteSpace(content)
                    ? $"Request failed with status {(int)response.StatusCode}."
                    : content.Trim();

                if (message.Length > 300)
                {
                    message = $"{message[..300]}...";
                }

                if (typeof(T) == typeof(LoginResponse))
                {
                    return (T)(object)new LoginResponse(false, message);
                }

                throw;
            }
        }
    }
}
