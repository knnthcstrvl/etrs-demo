using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Dtr
{
    public class AttendanceEmailRequest
    {
        [Key]
        public int AttendanceEmailRequestId { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        [MaxLength(30)]
        public string IdNumber { get; set; } = string.Empty;

        [MaxLength(250)]
        public string EmployeeName { get; set; } = string.Empty;

        [MaxLength(320)]
        public string RecipientEmail { get; set; } = string.Empty;

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }

        [MaxLength(100)]
        public string ReviewedBy { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string FailureReason { get; set; } = string.Empty;

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}
