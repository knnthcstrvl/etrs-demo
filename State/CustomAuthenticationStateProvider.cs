using Financial.DTO.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace Financial.State
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());
        private readonly ProtectedLocalStorage _protectedLocalStorage;
        private readonly ProtectedSessionStorage _protectedSessionStorage;
        public IConstantState _constant;

        public CustomAuthenticationStateProvider(
            IConstantState constantState,
            ProtectedLocalStorage protectedLocalStorage,
            ProtectedSessionStorage protectedSessionStorage)
        {
            _constant = constantState;
            _protectedLocalStorage = protectedLocalStorage;
            _protectedSessionStorage = protectedSessionStorage;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await GetStoredTokenAsync();

                if (string.IsNullOrWhiteSpace(token))
                    return new AuthenticationState(anonymous);

                var gatherClaims = DecryptJWTService.DecryptToken(token);
                if (gatherClaims == null) return new AuthenticationState(anonymous);

                if (long.TryParse(gatherClaims.Expiry, out var expirySeconds))
                {
                    var datetime = DateTimeOffset.FromUnixTimeSeconds(expirySeconds);
                    if (datetime.UtcDateTime <= DateTime.UtcNow)
                    {
                        //await _accountService.RefreshToken(new UserSession()
                        //{
                        //    JWTToken = token
                        //});
                    }
                }

                var claimsPrincipal = SetClaimPrincipal(gatherClaims);
                return new AuthenticationState(claimsPrincipal);
            }
            catch
            {
                return new AuthenticationState(anonymous);
            }
        }

        public async void UpdateAuthenticationState(string jwtToken, bool rememberMe = false)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            if (!string.IsNullOrEmpty(jwtToken))
            {
                _constant.JWTToken = jwtToken;
                await StoreTokenAsync(jwtToken, rememberMe);

                var getUserClaims = DecryptJWTService.DecryptToken(jwtToken);

                claimsPrincipal = getUserClaims is null ? anonymous : SetClaimPrincipal(getUserClaims);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
            }
            else
            {
                _constant.JWTToken = string.Empty;
                await DeleteStoredTokensAsync();
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
            }
        }


        public ClaimsPrincipal SetClaimPrincipal(CustomUserClaims claims)
        {
            if (string.IsNullOrWhiteSpace(claims.Id)) return new ClaimsPrincipal();
            _constant.JWTToken = claims.Jwt ?? string.Empty;

            Console.WriteLine($"Username: {claims.Username} JWTToken:{_constant.JWTToken}");

            return new ClaimsPrincipal(new ClaimsIdentity(
                new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, claims.Id ?? string.Empty),
                    new(ClaimTypes.Name, claims.Username ?? string.Empty),
                    new("Name", claims.Name ?? string.Empty),
                    new(ClaimTypes.Role, claims.Role ?? string.Empty),
                    new("Jwt", claims.Jwt ?? string.Empty)
                }, "JwtAuth"));
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

        private async Task StoreTokenAsync(string jwtToken, bool rememberMe)
        {
            if (rememberMe)
            {
                await _protectedLocalStorage.SetAsync("c_user_session", jwtToken);
                await _protectedSessionStorage.DeleteAsync("c_user_session");
                return;
            }

            await _protectedSessionStorage.SetAsync("c_user_session", jwtToken);
            await _protectedLocalStorage.DeleteAsync("c_user_session");
        }

        private async Task DeleteStoredTokensAsync()
        {
            await _protectedLocalStorage.DeleteAsync("c_user_session");
            await _protectedSessionStorage.DeleteAsync("c_user_session");
        }

    }
}
