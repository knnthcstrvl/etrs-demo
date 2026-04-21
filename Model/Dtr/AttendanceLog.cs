using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Dtr
{
    public class AttendanceLog
    {
        [Key]
        public int AttendanceLogId { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public int? BiometricDeviceId { get; set; }
        public BiometricDevice? BiometricDevice { get; set; }

        public DateTime LogTime { get; set; }

        [MaxLength(20)]
        public string LogType { get; set; } = string.Empty;

        [MaxLength(30)]
        public string Source { get; set; } = "Biometric";

        [MaxLength(30)]
        public string VerificationMode { get; set; } = string.Empty;

        public bool IsSynced { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
