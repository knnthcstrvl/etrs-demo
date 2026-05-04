using Financial.Data;
using Financial.DTO;
using Financial.DTO.Auth;
using Financial.Model;
using Financial.Repositories.Interface;
using Financial.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Financial.Responses.CustomResponses;

namespace Financial.Repositories.Implement
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly ProcurementDbContext _context;
        private readonly IConfiguration _config;

        public UserAccountRepository(ProcurementDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<CustomResponses.UserResponse> GetUsersAsync()
        {
            var data = await _context.UserAccounts.ToListAsync();
            return new UserResponse(true, "Success", data);
        }

        private async Task<Model.UserAccount> GetUser(string email)
        {
            return await _context.UserAccounts.FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<CustomResponses.LoginResponse> LoginAsync(LoginDto model)
        {
            var findUser = await GetUser(model.Email);
            if (findUser == null) return new LoginResponse(false, "User not Found!");

            if (!BCrypt.Net.BCrypt.Verify(model.Password, findUser.Password))
            {
                return new LoginResponse(false, "Email/Password not valid");
            }

            string jwtToken = GenerateToken(findUser);

            return new LoginResponse(true, "Login successfully", jwtToken);
        }

        private string GenerateToken(Model.UserAccount user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserAccountId.ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.Role!)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"]!,
                audience: _config["Jwt:Audience"]!,
                claims: userClaims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public CustomResponses.LoginResponse RefreshToken(UserSession userSession)
        {
            throw new NotImplementedException();
        }

        public async Task<CustomResponses.RegistrationResponse> RegisterAsync(RegisterDto model)
        {
            var findUser = await GetUser(model.Email);
            if (findUser != null)
            {
                return new RegistrationResponse(false, "User Already Exist!");
            }

            _context.UserAccounts.Add(
                new UserAccount()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Username = model.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    IsActive = true,
                    DateCreated = DateTime.UtcNow
                });

            await _context.SaveChangesAsync();
            return new RegistrationResponse(true, "Success");
        }

        public async Task<CustomResponses.UserResponse> UpdateUserAsync(Model.UserAccount model)
        {
            throw new NotImplementedException();
        }
    }
}
