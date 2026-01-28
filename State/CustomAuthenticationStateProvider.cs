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
        public IConstantState _constant;

        public CustomAuthenticationStateProvider(IConstantState constantState, ProtectedLocalStorage protectedLocalStorage)
        {
            _constant = constantState;
            _protectedLocalStorage = protectedLocalStorage;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _protectedLocalStorage.GetAsync<string>("c_user_session");

                if (string.IsNullOrEmpty(token.Value))
                    return await Task.FromResult(new AuthenticationState(anonymous));

                var gatherClaims = DecryptJWTService.DecryptToken(token.Value);
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

        public async void UpdateAuthenticationState(string jwtToken)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            if (!string.IsNullOrEmpty(jwtToken))
            {
                _constant.JWTToken = jwtToken;
                await _protectedLocalStorage.SetAsync("c_user_session", jwtToken);

                var getUserClaims = DecryptJWTService.DecryptToken(jwtToken);

                claimsPrincipal = SetClaimPrincipal(getUserClaims);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
            }
            else
            {
                _constant.JWTToken = null!;
                await _protectedLocalStorage.DeleteAsync("c_user_session");
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

    }
}
