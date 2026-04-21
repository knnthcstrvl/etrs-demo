using System.ComponentModel.DataAnnotations;

namespace Financial.Model.Dtr
{
    public class BiometricDevice
    {
        [Key]
        public int BiometricDeviceId { get; set; }

        [MaxLength(30)]
        public string DeviceCode { get; set; } = string.Empty;

        [MaxLength(150)]
        public string DeviceName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(50)]
        public string IpAddress { get; set; } = string.Empty;

        [MaxLength(40)]
        public string SyncStatus { get; set; } = "Pending";

        public DateTime? LastSyncedAt { get; set; }

        public bool IsOnline { get; set; } = true;

        public ICollection<AttendanceLog> AttendanceLogs { get; set; } = new List<AttendanceLog>();
    }
}
