using System.ComponentModel.DataAnnotations;

namespace Financial.DTO.Auth
{
    public class RegisterDto : LoginDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required, Compare(nameof(Password)), DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
