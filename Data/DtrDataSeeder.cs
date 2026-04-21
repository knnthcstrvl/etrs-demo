using Financial.Model;
using Financial.Model.Dtr;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data
{
    public static class DtrDataSeeder
    {
        public static async Task SeedAsync(ProcurementDbContext db)
        {
            await SeedUsersAsync(db);

            if (await db.Employees.AnyAsync())
            {
                return;
            }

            var employees = new List<Employee>
            {
                new()
                {
                    EmployeeNumber = "EMP-1001",
                    FirstName = "John",
                    LastName = "Wick",
                    Department = "Administration",
                    Position = "HR Officer",
                    EmploymentStatus = "Regular",
                    PrimaryShift = "Morning Shift",
                    DateHired = new DateTime(2023, 3, 1),
                    IsActive = true
                },
                new()
                {
                    EmployeeNumber = "EMP-1002",
                    FirstName = "Maria",
                    LastName = "Santos",
                    Department = "Finance",
                    Position = "Payroll Staff",
                    EmploymentStatus = "Regular",
                    PrimaryShift = "Morning Shift",
                    DateHired = new DateTime(2022, 11, 15),
                    IsActive = true
                },
                new()
                {
                    EmployeeNumber = "EMP-1003",
                    FirstName = "Paul",
                    LastName = "Rivera",
                    Department = "IT",
                    Position = "System Support",
                    EmploymentStatus = "Contractual",
                    PrimaryShift = "Flexible Shift",
                    DateHired = new DateTime(2024, 1, 10),
                    IsActive = true
                },
                new()
                {
                    EmployeeNumber = "EMP-1004",
                    FirstName = "Angela",
                    LastName = "Lopez",
                    Department = "Operations",
                    Position = "Field Coordinator",
                    EmploymentStatus = "Regular",
                    PrimaryShift = "Mid Shift",
                    DateHired = new DateTime(2021, 8, 9),
                    IsActive = true
                }
            };

            var devices = new List<BiometricDevice>
            {
                new()
                {
                    DeviceCode = "BIO-01",
                    DeviceName = "Main Gate Scanner",
                    Location = "Main Lobby",
                    IpAddress = "192.168.10.21",
                    SyncStatus = "Connected",
                    LastSyncedAt = DateTime.UtcNow.AddMinutes(-8),
                    IsOnline = true
                },
                new()
                {
                    DeviceCode = "BIO-02",
                    DeviceName = "Finance Office Scanner",
                    Location = "2nd Floor",
                    IpAddress = "192.168.10.22",
                    SyncStatus = "Connected",
                    LastSyncedAt = DateTime.UtcNow.AddMinutes(-12),
                    IsOnline = true
                },
                new()
                {
                    DeviceCode = "BIO-03",
                    DeviceName = "Warehouse Scanner",
                    Location = "Annex Building",
                    IpAddress = "192.168.10.23",
                    SyncStatus = "Needs Attention",
                    LastSyncedAt = DateTime.UtcNow.AddHours(-6),
                    IsOnline = false
                }
            };

            await db.Employees.AddRangeAsync(employees);
            await db.BiometricDevices.AddRangeAsync(devices);
            await db.SaveChangesAsync();

            var today = DateTime.Today;
            var logs = new List<AttendanceLog>
            {
                new() { EmployeeId = employees[0].EmployeeId, BiometricDeviceId = devices[0].BiometricDeviceId, LogTime = today.AddHours(7).AddMinutes(58), LogType = "Time In", VerificationMode = "Fingerprint", IsSynced = true },
                new() { EmployeeId = employees[0].EmployeeId, BiometricDeviceId = devices[0].BiometricDeviceId, LogTime = today.AddHours(17).AddMinutes(7), LogType = "Time Out", VerificationMode = "Fingerprint", IsSynced = true },
                new() { EmployeeId = employees[1].EmployeeId, BiometricDeviceId = devices[1].BiometricDeviceId, LogTime = today.AddHours(8).AddMinutes(14), LogType = "Time In", VerificationMode = "Face", IsSynced = true },
                new() { EmployeeId = employees[1].EmployeeId, BiometricDeviceId = devices[1].BiometricDeviceId, LogTime = today.AddHours(17).AddMinutes(2), LogType = "Time Out", VerificationMode = "Face", IsSynced = true },
                new() { EmployeeId = employees[2].EmployeeId, BiometricDeviceId = devices[0].BiometricDeviceId, LogTime = today.AddHours(9).AddMinutes(6), LogType = "Time In", VerificationMode = "RFID", IsSynced = true },
                new() { EmployeeId = employees[3].EmployeeId, BiometricDeviceId = devices[2].BiometricDeviceId, LogTime = today.AddHours(10).AddMinutes(1), LogType = "Time In", VerificationMode = "Fingerprint", IsSynced = false },
                new() { EmployeeId = employees[2].EmployeeId, BiometricDeviceId = devices[0].BiometricDeviceId, LogTime = today.AddDays(-1).AddHours(8).AddMinutes(3), LogType = "Time In", VerificationMode = "RFID", IsSynced = true },
                new() { EmployeeId = employees[2].EmployeeId, BiometricDeviceId = devices[0].BiometricDeviceId, LogTime = today.AddDays(-1).AddHours(18).AddMinutes(11), LogType = "Time Out", VerificationMode = "RFID", IsSynced = true }
            };

            var schedules = new List<ShiftSchedule>();
            foreach (var employee in employees)
            {
                schedules.Add(new ShiftSchedule
                {
                    EmployeeId = employee.EmployeeId,
                    WorkDate = today,
                    ShiftName = employee.PrimaryShift,
                    ScheduledTimeIn = employee.PrimaryShift == "Mid Shift" ? new TimeSpan(10, 0, 0) : new TimeSpan(8, 0, 0),
                    ScheduledTimeOut = employee.PrimaryShift == "Mid Shift" ? new TimeSpan(19, 0, 0) : new TimeSpan(17, 0, 0),
                    IsRestDay = false
                });
                schedules.Add(new ShiftSchedule
                {
                    EmployeeId = employee.EmployeeId,
                    WorkDate = today.AddDays(1),
                    ShiftName = employee.PrimaryShift,
                    ScheduledTimeIn = employee.PrimaryShift == "Mid Shift" ? new TimeSpan(10, 0, 0) : new TimeSpan(8, 0, 0),
                    ScheduledTimeOut = employee.PrimaryShift == "Mid Shift" ? new TimeSpan(19, 0, 0) : new TimeSpan(17, 0, 0),
                    IsRestDay = false
                });
            }

            var issues = new List<AttendanceIssue>
            {
                new()
                {
                    EmployeeId = employees[1].EmployeeId,
                    AttendanceDate = today,
                    IssueType = "Late Arrival",
                    Severity = "Medium",
                    Status = "Open",
                    Remarks = "14 minutes late based on first biometric tap."
                },
                new()
                {
                    EmployeeId = employees[3].EmployeeId,
                    AttendanceDate = today,
                    IssueType = "Pending Sync",
                    Severity = "High",
                    Status = "Investigating",
                    Remarks = "Latest punch came from an offline device and is waiting for API reconciliation."
                },
                new()
                {
                    EmployeeId = employees[2].EmployeeId,
                    AttendanceDate = today.AddDays(-1),
                    IssueType = "Overtime Review",
                    Severity = "Low",
                    Status = "For Approval",
                    Remarks = "Rendered beyond scheduled shift by more than one hour."
                }
            };

            await db.AttendanceLogs.AddRangeAsync(logs);
            await db.ShiftSchedules.AddRangeAsync(schedules);
            await db.AttendanceIssues.AddRangeAsync(issues);
            await db.SaveChangesAsync();
        }

        private static async Task SeedUsersAsync(ProcurementDbContext db)
        {
            if (await db.UserAccounts.AnyAsync())
            {
                return;
            }

            var users = new List<UserAccount>
            {
                new()
                {
                    Email = "admin@local",
                    FirstName = "System",
                    LastName = "Administrator",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = "Admin",
                    Office = "ICT",
                    IsActive = true
                },
                new()
                {
                    Email = "supervisor@local",
                    FirstName = "Helen",
                    LastName = "Cruz",
                    Password = BCrypt.Net.BCrypt.HashPassword("Supervisor@123"),
                    Role = "Supervisor",
                    Office = "Human Resources",
                    IsActive = true
                },
                new()
                {
                    Email = "viewer@local",
                    FirstName = "Mark",
                    LastName = "Dela Pena",
                    Password = BCrypt.Net.BCrypt.HashPassword("Viewer@123"),
                    Role = "Viewer",
                    Office = "Finance",
                    IsActive = true
                }
            };

            await db.UserAccounts.AddRangeAsync(users);
            await db.SaveChangesAsync();
        }
    }
}
