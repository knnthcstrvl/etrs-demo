using System.ComponentModel.DataAnnotations;

namespace Financial.Model
{
    public class UserAccount
    {
        [Key]
        public int UserAccountId { get; set; }

        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Role { get; set; } = "User";

        [MaxLength(1000)]
        public string DivisionCenterUnit { get; set; } = string.Empty;

        [MaxLength(1000)]
        // AccessLevel stores comma separated access rights (eg: "Add,Edit,Export")
        public string AccessLevel { get; set; } = string.Empty;

        public string RememberToken { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool MustChangePassword { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateUpdated { get; set; }
    }
}
