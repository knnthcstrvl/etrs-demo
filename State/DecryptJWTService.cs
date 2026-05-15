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
                var fname = token.Claims.FirstOrDefault(_ => _.Type == "FirstName");
                var lname = token.Claims.FirstOrDefault(_ => _.Type == "LastName");
                var email = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Email);
                var role = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Role);
                var exp = token.Claims.FirstOrDefault(_ => _.Type == "exp");

                return new CustomUserClaims(
                    Id: id?.Value ?? string.Empty,
                    FirstName: fname?.Value ?? string.Empty,
                    LastName: lname?.Value ?? string.Empty,
                    Email: email?.Value ?? string.Empty,
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
