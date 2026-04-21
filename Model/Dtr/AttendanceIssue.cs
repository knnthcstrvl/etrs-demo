using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Dtr
{
    public class AttendanceIssue
    {
        [Key]
        public int AttendanceIssueId { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public DateTime AttendanceDate { get; set; }

        [MaxLength(50)]
        public string IssueType { get; set; } = string.Empty;

        [MaxLength(30)]
        public string Severity { get; set; } = string.Empty;

        [MaxLength(30)]
        public string Status { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Remarks { get; set; } = string.Empty;
    }
}
