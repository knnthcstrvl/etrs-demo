using Financial.Model.Dtr;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data
{
    public static class ExceptionConditionSeeder
    {
        public static async Task SeedAsync(ProcurementDbContext db)
        {
            var defaults = new (string Name, string Description)[]
            {
                ("Skeletal Workforce", "Employee is reporting under the skeletal workforce arrangement."),
                ("On Travel", "Employee is deployed or travelling to another site."),
                ("Holiday", "Employee is covered by an official holiday schedule."),
                ("On Leave", "Employee is on approved leave.")
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
                        DateCreated = DateTime.UtcNow
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
    }
}
