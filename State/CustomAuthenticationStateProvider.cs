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
        private AuthenticationState? _cachedAuthenticationState;
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
            if (_cachedAuthenticationState is not null)
            {
                return _cachedAuthenticationState;
            }

            try
            {
                var token = await GetStoredTokenAsync();

                if (string.IsNullOrWhiteSpace(token))
                {
                    _cachedAuthenticationState = new AuthenticationState(anonymous);
                    return _cachedAuthenticationState;
                }

                var gatherClaims = DecryptJWTService.DecryptToken(token);
                if (gatherClaims == null)
                {
                    _cachedAuthenticationState = new AuthenticationState(anonymous);
                    return _cachedAuthenticationState;
                }

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
                _cachedAuthenticationState = new AuthenticationState(claimsPrincipal);
                return _cachedAuthenticationState;
            }
            catch
            {
                // Protected browser storage may be unavailable during prerendering.
                // Leave the cache empty so an interactive circuit can retry.
                return new AuthenticationState(anonymous);
            }
        }

        public async Task UpdateAuthenticationState(string jwtToken, bool rememberMe = false)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            if (!string.IsNullOrEmpty(jwtToken))
            {
                _constant.JWTToken = jwtToken;
                await StoreTokenAsync(jwtToken, rememberMe);

                var getUserClaims = DecryptJWTService.DecryptToken(jwtToken);

                claimsPrincipal = getUserClaims is null ? anonymous : SetClaimPrincipal(getUserClaims);
                _cachedAuthenticationState = new AuthenticationState(claimsPrincipal);
                NotifyAuthenticationStateChanged(Task.FromResult(_cachedAuthenticationState));
            }
            else
            {
                _constant.JWTToken = string.Empty;
                await DeleteStoredTokensAsync();
                _cachedAuthenticationState = new AuthenticationState(anonymous);
                NotifyAuthenticationStateChanged(Task.FromResult(_cachedAuthenticationState));
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
