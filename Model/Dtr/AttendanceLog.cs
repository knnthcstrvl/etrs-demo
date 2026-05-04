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

        public DateTime? TimeInAm { get; set; }
        public DateTime? TimeOutAm { get; set; }
        public DateTime? TimeInPm { get; set; }
        public DateTime? TimeOutPm { get; set; }

        [MaxLength(50)]
        public string Tardiness { get; set; } = string.Empty;

        public DateTime AttendanceDate { get; set; } = DateTime.Today;
    }
}
