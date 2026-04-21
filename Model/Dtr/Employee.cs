using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Dtr
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [MaxLength(30)]
        public string EmployeeNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Department { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Position { get; set; } = string.Empty;

        [MaxLength(30)]
        public string EmploymentStatus { get; set; } = "Active";

        [MaxLength(150)]
        public string PrimaryShift { get; set; } = string.Empty;

        public DateTime DateHired { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<AttendanceLog> AttendanceLogs { get; set; } = new List<AttendanceLog>();
        public ICollection<ShiftSchedule> ShiftSchedules { get; set; } = new List<ShiftSchedule>();
        public ICollection<AttendanceIssue> AttendanceIssues { get; set; } = new List<AttendanceIssue>();
    }
}
