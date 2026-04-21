using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Dtr
{
    public class ShiftSchedule
    {
        [Key]
        public int ShiftScheduleId { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public DateTime WorkDate { get; set; }

        [MaxLength(100)]
        public string ShiftName { get; set; } = string.Empty;

        public TimeSpan ScheduledTimeIn { get; set; }

        public TimeSpan ScheduledTimeOut { get; set; }

        public bool IsRestDay { get; set; }

        [MaxLength(50)]
        public string ScheduleSource { get; set; } = "Manual Seed";
    }
}
