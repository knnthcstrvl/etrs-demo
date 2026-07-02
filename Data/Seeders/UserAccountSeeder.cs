using Financial.Model;
using Financial.Services;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data.Seeders
{
    public static class UserAccountSeeder
    {
        public static async Task SeedAdminAsync(EtrsDbContext db)
        {
            const string adminUsername = "admin";

            var existingAdmin = await db.UserAccounts
                .FirstOrDefaultAsync(u => u.Username == adminUsername);

            if (existingAdmin is not null)
            {
                var changed = false;

                if (!string.Equals(existingAdmin.Role, "Admin", StringComparison.Ordinal))
                {
                    existingAdmin.Role = "Admin";
                    changed = true;
                }

                if (!existingAdmin.IsActive)
                {
                    existingAdmin.IsActive = true;
                    changed = true;
                }

                var adminAccess = string.Join(",", AccessPolicy.AdminAssignableAccess.OrderBy(value => value));
                if (!string.Equals(existingAdmin.AccessLevel, adminAccess, StringComparison.Ordinal))
                {
                    existingAdmin.AccessLevel = adminAccess;
                    changed = true;
                }

                if (changed)
                {
                    existingAdmin.DateUpdated = DateTime.UtcNow;
                    await db.SaveChangesAsync();
                }

                return;
            }

            db.UserAccounts.Add(new UserAccount
            {
                Name = "System Administrator",
                Username = adminUsername,
                Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                AccessLevel = string.Join(",", AccessPolicy.AdminAssignableAccess.OrderBy(value => value)),
                DivisionCenterUnit = "Administration",
                RememberToken = string.Empty,
                IsActive = true,
                DateCreated = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
        }
    }
}
