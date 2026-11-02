using Financial.Model.Dtr;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data.Seeders
{
    public static class ExceptionConditionSeeder
    {
        public static async Task SeedAsync(EtrsDbContext db)
        {
            var defaults = new (string Name, string Description)[]
            {
                ("Skeletal Workforce", "Employee is reporting under the skeletal workforce arrangement."),
                ("On Travel", "Employee is deployed or travelling to another site."),
                ("Holiday", "Employee is covered by an official holiday schedule."),
                ("On Leave", "Employee is on approved leave."),
                ("Official Business", "Employee is assigned to official business outside the office."),
                ("Training", "Employee is attending an approved training or seminar."),
                ("Field Work", "Employee is conducting approved field work."),
                ("Court Appearance", "Employee is required to attend an official court proceeding."),
                ("Medical Appointment", "Employee has an approved medical appointment."),
                ("Disaster Response", "Employee is assigned to disaster response operations."),
                ("Flexible Schedule", "Employee is working under an approved flexible schedule."),
                ("Suspension of Work", "Office work is suspended by an official announcement."),
                ("Client Meeting", "Employee is attending an approved external client meeting."),
                ("Plantation Visit", "Employee is visiting an agricultural or research site."),
                ("Inventory", "Employee is participating in an approved inventory activity."),
                ("Equipment Calibration", "Employee is assigned to equipment calibration work."),
                ("Emergency Leave", "Employee is on approved emergency leave."),
                ("Committee Assignment", "Employee is attending an official committee assignment."),
                ("Remote Field Support", "Employee is supporting field operations remotely."),
                ("Special Project", "Employee is assigned to an approved special project.")
            };

            var existing = await db.ExceptionConditions.ToListAsync();
            var hasChanges = false;

            foreach (var item in defaults)
            {
                var condition = existing.FirstOrDefault(c => string.Equals(c.ConditionName, item.Name, StringComparison.OrdinalIgnoreCase));
                if (condition is null)
                {
                    await db.ExceptionConditions.AddAsync(new ExceptionCondition
                    {
                        ConditionName = item.Name,
                        Description = item.Description,
                        IsActive = true,
                        DateCreated = RandomDate(random)
                    });
                    hasChanges = true;
                    continue;
                }

                if (!condition.IsActive || !string.Equals(condition.Description, item.Description, StringComparison.Ordinal))
                {
                    condition.IsActive = true;
                    condition.Description = item.Description;
                    condition.DateUpdated = DateTime.UtcNow;
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                await db.SaveChangesAsync();
            }
        }

        private static readonly Random random = new();

        private static DateTime RandomDate(Random random)
            => DateTime.UtcNow.AddDays(-random.Next(0, 730)).AddMinutes(-random.Next(0, 1440));
    }
}
