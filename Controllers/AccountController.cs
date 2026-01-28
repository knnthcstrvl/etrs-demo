using Financial.DTO;
using Financial.DTO.Auth;
using Financial.Model;
using Financial.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Financial.Responses.CustomResponses;

namespace Financial.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly IUserAccountRepository _userAccountRepository;

        public AccountController(IUserAccountRepository userAccountRepository)
        {
            _userAccountRepository = userAccountRepository;
        }

        [HttpPost("register")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<RegistrationResponse>> RegisterAsync(RegisterDto model)
        {
            Console.WriteLine(JsonConvert.SerializeObject(model));
            var result = await _userAccountRepository.RegisterAsync(model);
            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> LoginAsync(LoginDto model)
        {
            var result = await _userAccountRepository.LoginAsync(model);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public ActionResult<LoginResponse> RefreshToken(UserSession model)
        {
            Console.WriteLine("Token Refreshed");
            var result = _userAccountRepository.RefreshToken(model);
            return Ok(result);
        }

        [HttpGet("Sample")]
        //[Authorize(Roles = "Admin")]
        public ActionResult<string> GetSample()
        {

            return "Authenticated";
        }

        [HttpGet("GetUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponse>> GetUser()
        {
            var result = await _userAccountRepository.GetUsersAsync();
            return Ok(result);
        }

        [HttpPost("UpdateUser")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponse>> UpdateUser(UserAccount model)
        {
            var result = await _userAccountRepository.UpdateUserAsync(model);
            return Ok(result);
        }

    }
}
