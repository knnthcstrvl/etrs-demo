using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Dtr
{
    public class BiometricDevice
    {
        [Key]
        public int BiometricDeviceId { get; set; }

        [MaxLength(150)]
        public string DeviceName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string SerialNumber { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(40)]
        public string Status { get; set; } = "Active";

        public ICollection<AttendanceLog> AttendanceLogs { get; set; } = new List<AttendanceLog>();
    }
}
