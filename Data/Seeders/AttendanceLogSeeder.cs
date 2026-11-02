using Financial.Model.Dtr;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data.Seeders
{
    public static class AttendanceLogSeeder
    {
        public static async Task SeedAsync(EtrsDbContext db)
        {
            var employees = await db.Employees.ToListAsync();
            if (employees.Count == 0) return;

            var (periodStart, periodEnd) = GetCurrentPeriod();
            var visibleCount = await db.AttendanceLogs.CountAsync(log =>
                log.AttendanceDate >= periodStart && log.AttendanceDate < periodEnd);
            var missing = 20 - visibleCount;
            if (missing <= 0) return;

            var random = new Random();
            var logs = new List<AttendanceLog>();
            for (var i = 0; i < missing; i++)
            {
                var employee = employees[random.Next(employees.Count)];
                var date = RandomDateInPeriod(periodStart, periodEnd, random);
                var timeIn = date.AddHours(8).AddMinutes(random.Next(0, 31));
                var timeOutAm = date.AddHours(12);
                var timeInPm = date.AddHours(13);
                var timeOut = date.AddHours(17).AddMinutes(random.Next(0, 61));
                logs.Add(new AttendanceLog
                {
                    EmployeeId = employee.EmployeeId,
                    IdNumber = employee.IdNumber,
                    LastName = employee.LastName,
                    FirstName = employee.FirstName,
                    MiddleName = employee.MiddleName,
                    DivisionCenterUnit = employee.DivisionCenterUnit,
                    Designation = employee.Designation,
                    AttendanceDate = date,
                    TimeInAm = timeIn,
                    TimeOutAm = timeOutAm,
                    TimeInPm = timeInPm,
                    TimeOutPm = timeOut,
                    Tardiness = timeIn.TimeOfDay > new TimeSpan(8, 0, 0) ? $"{(int)(timeIn - date.AddHours(8)).TotalMinutes} min" : string.Empty,
                    MovApprovalStatus = "Approved",
                    PhotoVerificationApprovalStatus = "Approved"
                });
            }

            await db.AttendanceLogs.AddRangeAsync(logs);
            await db.SaveChangesAsync();
        }

        private static (DateTime Start, DateTime EndExclusive) GetCurrentPeriod()
        {
            var today = DateTime.Today;
            var start = today.Day <= 15
                ? new DateTime(today.Year, today.Month, 1)
                : new DateTime(today.Year, today.Month, 16);
            var end = today.Day <= 15
                ? new DateTime(today.Year, today.Month, 16)
                : new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month)).AddDays(1);
            return (start, end);
        }

        private static DateTime RandomDateInPeriod(DateTime start, DateTime endExclusive, Random random)
            => start.AddDays(random.Next((endExclusive - start).Days));
    }
}
