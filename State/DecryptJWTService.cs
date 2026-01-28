using Financial.DTO.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Financial.State
{
    public class DecryptJWTService
    {
        public static CustomUserClaims DecryptToken(string jwtToken)
        {
            try
            {
                if (string.IsNullOrEmpty(jwtToken)) return new CustomUserClaims();

                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwtToken);

                var id = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier);
                var fname = token.Claims.FirstOrDefault(_ => _.Type == "FirstName");
                var mname = token.Claims.FirstOrDefault(_ => _.Type == "MiddleName");
                var lname = token.Claims.FirstOrDefault(_ => _.Type == "LastName");
                var email = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Email);
                var role = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Role);
                var exp = token.Claims.FirstOrDefault(_ => _.Type == "exp");
                var divisionId = token.Claims.FirstOrDefault(_ => _.Type == "DivisionId");

                return new CustomUserClaims(
                    Id: id.Value,
                    FirstName: fname.Value,
                    MiddleName: mname.Value,
                    LastName: lname.Value,
                    Email: email.Value!,
                    Role: role.Value,
                    Jwt: jwtToken,
                    Expiry: exp.Value,
                    DivisionId: divisionId.Value);
            }
            catch
            {
                return null!;
            }
        }
    }
}
