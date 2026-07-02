using System.ComponentModel.DataAnnotations;

namespace Financial.DTO.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Please enter your username.")]
        public string Username { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
