using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Dtr
{
    public class ExceptionCondition
    {
        [Key]
        public int ExceptionConditionId { get; set; }

        [MaxLength(150)]
        public string ConditionName { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateUpdated { get; set; }
    }
}
