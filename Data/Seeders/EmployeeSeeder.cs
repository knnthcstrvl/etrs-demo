using Financial.Model.Dtr;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data.Seeders
{
    public static class EmployeeSeeder
    {
        public static async Task SeedAsync(EtrsDbContext db)
        {
            if (await db.Employees.AnyAsync()) return;

            var employees = new List<Employee>
            {
                new() { IdNumber = "BSWM-2026-0001", LastName = "Santos", FirstName = "Maria", MiddleName = "Lopez", DivisionCenterUnit = "Soils Survey Division (SSD)", Designation = "Chief Agriculturist", Status = "Active" },
                new() { IdNumber = "BSWM-2026-0002", LastName = "Reyes", FirstName = "John Paul", MiddleName = "Rivera", DivisionCenterUnit = "Agricultural Land Management and Evaluation Division (ALMED)", Designation = "Chief Agriculturist", Status = "Active" }
            };

            await db.Employees.AddRangeAsync(employees);
            await db.SaveChangesAsync();
        }
    }
}
