using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Dtr
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

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

        [MaxLength(30)]
        public string Status { get; set; } = "Active";

        public ICollection<AttendanceLog> AttendanceLogs { get; set; } = new List<AttendanceLog>();
        public ICollection<AttendanceLogWfh> AttendanceLogsWfh { get; set; } = new List<AttendanceLogWfh>();
        public ICollection<AttendanceEmailRequest> AttendanceEmailRequests { get; set; } = new List<AttendanceEmailRequest>();
    }
}
