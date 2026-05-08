using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Dtr
{
    public class BiometricDevice
    {
        [Key]
        public int BiometricDeviceId { get; set; }

        [Required, MaxLength(150)]
        public string DeviceName { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string SerialNumber { get; set; } = string.Empty;

        [Required, MaxLength(40)]
        public string Status { get; set; } = string.Empty;

        public ICollection<AttendanceLog> AttendanceLogs { get; set; } = new List<AttendanceLog>();
    }
}
