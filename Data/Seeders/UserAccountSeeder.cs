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

        public static async Task SeedDemoAsync(EtrsDbContext db)
        {
            const string demoAdminUsername = "demo.user001";
            var demoAdmin = await db.UserAccounts.FirstOrDefaultAsync(u => u.Username == demoAdminUsername);
            var adminAccess = string.Join(",", AccessPolicy.AdminAssignableAccess.OrderBy(value => value));

            if (demoAdmin is not null)
            {
                var changed = false;
                if (!string.Equals(demoAdmin.Role, "Admin", StringComparison.Ordinal))
                {
                    demoAdmin.Role = "Admin";
                    changed = true;
                }
                if (!string.Equals(demoAdmin.AccessLevel, adminAccess, StringComparison.Ordinal))
                {
                    demoAdmin.AccessLevel = adminAccess;
                    changed = true;
                }
                if (!demoAdmin.IsActive)
                {
                    demoAdmin.IsActive = true;
                    changed = true;
                }
                if (demoAdmin.MustChangePassword)
                {
                    demoAdmin.MustChangePassword = false;
                    changed = true;
                }
                if (changed)
                {
                    demoAdmin.DateUpdated = DateTime.UtcNow;
                    await db.SaveChangesAsync();
                }
            }
            else
            {
                db.UserAccounts.Add(new UserAccount
                {
                    Name = "Demo Administrator",
                    Username = demoAdminUsername,
                    Password = BCrypt.Net.BCrypt.HashPassword("Demo@123"),
                    Role = "Admin",
                    AccessLevel = adminAccess,
                    DivisionCenterUnit = "Administration",
                    RememberToken = string.Empty,
                    IsActive = true,
                    MustChangePassword = false,
                    DateCreated = DateTime.UtcNow
                });
                await db.SaveChangesAsync();
            }

            var existingCount = await db.UserAccounts.CountAsync();
            var missing = 20 - existingCount;
            if (missing <= 0) return;

            var existingUsernames = await db.UserAccounts.Select(u => u.Username).ToListAsync();
            var random = new Random();
            var demoPassword = BCrypt.Net.BCrypt.HashPassword("Demo@123");
            var users = new List<UserAccount>();

            for (var i = 1; users.Count < missing; i++)
            {
                var username = $"demo.user{i:000}";
                if (existingUsernames.Contains(username, StringComparer.OrdinalIgnoreCase)) continue;

                users.Add(new UserAccount
                {
                    Name = $"Demo User {i:000}",
                    Username = username,
                    Password = demoPassword,
                    Role = i == 1 ? "Admin" : "User",
                    AccessLevel = i == 1 ? adminAccess : string.Empty,
                    DivisionCenterUnit = "Administration",
                    RememberToken = string.Empty,
                    IsActive = true,
                    MustChangePassword = i != 1,
                    DateCreated = DateTime.UtcNow.AddDays(-random.Next(0, 730)).AddMinutes(-random.Next(0, 1440))
                });
            }

            await db.UserAccounts.AddRangeAsync(users);
            await db.SaveChangesAsync();
        }
    }
}
