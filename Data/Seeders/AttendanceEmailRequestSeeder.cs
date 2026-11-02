using Financial.Model.Dtr;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data.Seeders
{
    public static class AttendanceEmailRequestSeeder
    {
        public static async Task SeedAsync(EtrsDbContext db)
        {
            var employees = await db.Employees.ToListAsync();
            if (employees.Count == 0) return;

            var missing = 20 - await db.AttendanceEmailRequests.CountAsync();
            if (missing <= 0) return;

            var random = new Random();
            var requests = new List<AttendanceEmailRequest>();
            var statuses = new[] { "Sent", "Pending", "Failed" };
            for (var i = 0; i < missing; i++)
            {
                var employee = employees[random.Next(employees.Count)];
                var requestedAt = DateTime.UtcNow.AddDays(-random.Next(0, 365)).AddMinutes(-random.Next(0, 1440));
                var periodStart = new DateTime(requestedAt.Year, requestedAt.Month, 1).AddMonths(-random.Next(0, 3));
                var status = statuses[random.Next(statuses.Length)];
                requests.Add(new AttendanceEmailRequest
                {
                    EmployeeId = employee.EmployeeId,
                    IdNumber = employee.IdNumber,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}",
                    RecipientEmail = $"{employee.IdNumber.ToLowerInvariant()}@example.test",
                    PeriodStart = periodStart,
                    PeriodEnd = periodStart.AddMonths(1).AddDays(-1),
                    Status = status,
                    RequestedAt = requestedAt,
                    ReviewedAt = status == "Pending" ? null : requestedAt.AddMinutes(random.Next(5, 240)),
                    ReviewedBy = status == "Pending" ? string.Empty : "admin",
                    FailureReason = status == "Failed" ? "Sample delivery failure for demo data." : string.Empty
                });
            }

            await db.AttendanceEmailRequests.AddRangeAsync(requests);
            await db.SaveChangesAsync();
        }
    }
}
