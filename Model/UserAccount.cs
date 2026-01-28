using System.ComponentModel.DataAnnotations;

namespace Financial.Model
{
    public class UserAccount
    {
        [Key]
        public Guid AccountId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string Office { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

    }
}
