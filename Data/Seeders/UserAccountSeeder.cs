using Financial.Model;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data.Seeders
{
    public static class UserAccountSeeder
    {
        public static async Task SeedAdminAsync(EtrsDbContext db)
        {
            const string adminEmail = "admin@etrs.local";
            const string adminUsername = "admin";

            var existingAdmin = await db.UserAccounts
                .FirstOrDefaultAsync(u => u.Email == adminEmail || u.Username == adminUsername);

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

                if (changed)
                {
                    existingAdmin.DateUpdated = DateTime.UtcNow;
                    await db.SaveChangesAsync();
                }

                return;
            }

            db.UserAccounts.Add(new UserAccount
            {
                LastName = "Administrator",
                FirstName = "System",
                MiddleName = string.Empty,
                Email = adminEmail,
                Username = adminUsername,
                Password = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                DivisionCenterUnit = "Administration",
                Designation = "System Administrator",
                RememberToken = string.Empty,
                IsActive = true,
                DateCreated = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
        }
    }
}
