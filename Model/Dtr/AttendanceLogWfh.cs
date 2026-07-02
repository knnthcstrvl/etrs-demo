using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Dtr
{
    public class AttendanceLogWfh
    {
        [Key]
        public int AttendanceLogWfhId { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        [MaxLength(30)]
        public string IdNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string MiddleName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string DivisionCenterUnit { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Designation { get; set; } = string.Empty;

        public string Target { get; set; } = string.Empty;

        public string Accomplishment { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Mov { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Verification { get; set; } = string.Empty;

        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }

        [MaxLength(50)]
        public string Tardiness { get; set; } = string.Empty;

        [MaxLength(100)]
        public string WorkArrangement { get; set; } = "WFH";

        public int? ExceptionConditionId { get; set; }
        public ExceptionCondition? ExceptionCondition { get; set; }

        public DateTime? TagUpdatedAt { get; set; }

        public DateTime AttendanceDate { get; set; } = DateTime.Today;

        [MaxLength(20)]
        public string MovApprovalStatus { get; set; } = "Pending"; // Pending, Approved, Rejected

        public DateTime? MovApprovedAt { get; set; }

        public DateTime? MovRejectedAt { get; set; }

        [MaxLength(20)]
        public string PhotoVerificationApprovalStatus { get; set; } = "Pending"; // Pending, Approved, Rejected

        public DateTime? PhotoVerificationApprovedAt { get; set; }

        public DateTime? PhotoVerificationRejectedAt { get; set; }
    }
}
