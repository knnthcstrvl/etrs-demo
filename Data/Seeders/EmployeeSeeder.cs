using Financial.Model.Dtr;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data.Seeders
{
    public static class EmployeeSeeder
    {
        private static readonly string[] FirstNames = { "Maria", "John", "Ana", "Jose", "Carlo", "Liza", "Mark", "Patricia", "Miguel", "Grace", "Ramon", "Catherine", "Daniel", "Angela", "Paolo", "Rosa", "Eric", "Michelle", "Rafael", "Jasmine" };
        private static readonly string[] LastNames = { "Santos", "Reyes", "Cruz", "Garcia", "Mendoza", "Bautista", "Villanueva", "Ramos", "Aquino", "Castillo", "Rivera", "Gonzales", "Torres", "Flores", "Dela Cruz", "Navarro", "Lim", "Perez", "Diaz", "Mercado" };
        private static readonly string[] Units = { "Soils Survey Division (SSD)", "Agricultural Land Management and Evaluation Division (ALMED)", "Research and Development Division", "Administrative Division", "Regional Soils Laboratory" };
        private static readonly string[] Roles = { "Agriculturist", "Science Research Specialist", "Administrative Officer", "Soil Scientist", "Research Assistant" };

        public static async Task SeedAsync(EtrsDbContext db)
        {
            await NormalizeLegacyDemoIdsAsync(db);

            var count = await db.Employees.CountAsync();
            if (count >= 20) return;

            var existingIds = await db.Employees.Select(e => e.IdNumber).ToListAsync();
            var random = new Random();
            var employees = new List<Employee>();
            for (var i = 1; count + employees.Count < 20; i++)
            {
                var id = $"DEMO-{i:0000}";
                if (existingIds.Contains(id, StringComparer.OrdinalIgnoreCase)) continue;
                employees.Add(new Employee
                {
                    IdNumber = id,
                    FirstName = FirstNames[(i - 1) % FirstNames.Length],
                    MiddleName = new[] { "Lopez", "Garcia", "Dizon", "Santos", "Reyes" }[random.Next(5)],
                    LastName = LastNames[(i - 1) % LastNames.Length],
                    DivisionCenterUnit = Units[random.Next(Units.Length)],
                    Designation = Roles[random.Next(Roles.Length)],
                    Status = random.Next(10) == 0 ? "Inactive" : "Active"
                });
            }

            await db.Employees.AddRangeAsync(employees);
            await db.SaveChangesAsync();
        }

        private static async Task NormalizeLegacyDemoIdsAsync(EtrsDbContext db)
        {
            const string oldPrefix = "BSWM-DEMO-";
            const string newPrefix = "DEMO-";

            var legacyEmployees = await db.Employees
                .Where(employee => employee.IdNumber.StartsWith(oldPrefix))
                .ToListAsync();
            if (legacyEmployees.Count == 0) return;

            var legacyEmployeeIds = legacyEmployees.Select(employee => employee.EmployeeId).ToHashSet();
            var usedIds = (await db.Employees
                    .Where(employee => !legacyEmployeeIds.Contains(employee.EmployeeId))
                    .Select(employee => employee.IdNumber)
                    .ToListAsync())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var replacements = new Dictionary<int, string>();

            foreach (var employee in legacyEmployees)
            {
                var suffix = employee.IdNumber[oldPrefix.Length..];
                var replacement = newPrefix + suffix;
                if (!usedIds.Add(replacement))
                {
                    replacement = $"DEMO-LEGACY-{suffix}";
                    var duplicateIndex = 2;
                    while (!usedIds.Add(replacement))
                    {
                        replacement = $"DEMO-LEGACY-{suffix}-{duplicateIndex++}";
                    }
                }

                employee.IdNumber = replacement;
                replacements[employee.EmployeeId] = replacement;
            }

            var employeeIds = replacements.Keys.ToList();
            var onsiteLogs = await db.AttendanceLogs
                .Where(log => employeeIds.Contains(log.EmployeeId))
                .ToListAsync();
            foreach (var log in onsiteLogs)
            {
                log.IdNumber = replacements[log.EmployeeId];
            }

            var wfhLogs = await db.AttendanceLogsWfh
                .Where(log => employeeIds.Contains(log.EmployeeId))
                .ToListAsync();
            foreach (var log in wfhLogs)
            {
                log.IdNumber = replacements[log.EmployeeId];
            }

            var emailRequests = await db.AttendanceEmailRequests
                .Where(request => employeeIds.Contains(request.EmployeeId))
                .ToListAsync();
            foreach (var request in emailRequests)
            {
                request.IdNumber = replacements[request.EmployeeId];
                request.RecipientEmail = $"{request.IdNumber.ToLowerInvariant()}@example.test";
            }

            await db.SaveChangesAsync();
        }
    }
}
