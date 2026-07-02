using Financial.DTO.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Financial.State
{
    public class DecryptJWTService
    {
        public static CustomUserClaims? DecryptToken(string jwtToken)
        {
            try
            {
                if (string.IsNullOrEmpty(jwtToken)) return new CustomUserClaims();

                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtToken);

                var id = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier);
                var name = token.Claims.FirstOrDefault(_ => _.Type == "Name");
                var username = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Name);
                var role = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Role);
                var exp = token.Claims.FirstOrDefault(_ => _.Type == "exp");

                return new CustomUserClaims(
                    Id: id?.Value ?? string.Empty,
                    Name: name?.Value ?? string.Empty,
                    Username: username?.Value ?? string.Empty,
                    Role: role?.Value ?? string.Empty,
                    Jwt: jwtToken,
                    Expiry: exp?.Value ?? string.Empty);
            }
            catch
            {
                return null;
            }
        }
    }
}
