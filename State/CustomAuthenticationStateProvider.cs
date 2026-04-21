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
                    return await Task.FromResult(new AuthenticationState(anonymous));

                var gatherClaims = DecryptJWTService.DecryptToken(token);
                if (gatherClaims == null) return await Task.FromResult(new AuthenticationState(anonymous));

                var datetime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(gatherClaims.Expiry));
                if (datetime.UtcDateTime <= DateTime.UtcNow)
                {
                    //await _accountService.RefreshToken(new UserSession()
                    //{
                    //    JWTToken = token.Value
                    //});
                }

                var claimsPrincipal = SetClaimPrincipal(gatherClaims);
                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new AuthenticationState(anonymous));
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

                claimsPrincipal = SetClaimPrincipal(getUserClaims);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
            }
            else
            {
                _constant.JWTToken = null!;
                await DeleteStoredTokensAsync();
            }
            //NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }


        public ClaimsPrincipal SetClaimPrincipal(CustomUserClaims claims)
        {
            if (claims.Email is null) return new ClaimsPrincipal();
            _constant.JWTToken = claims.Jwt;

            Console.WriteLine($"Email: {claims.Email} JWTToken:{_constant.JWTToken}");
            //Console.WriteLine($"Email: {claims.Email}");

            return new ClaimsPrincipal(new ClaimsIdentity(
                new List<Claim>
                {
                    new("FirstName", claims.FirstName!),
                    new("LastName", claims.LastName!),
                    new(ClaimTypes.Email, claims.Email!),
                    new(ClaimTypes.Role, claims.Role!),
                    new("Jwt", claims.Jwt!)
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
