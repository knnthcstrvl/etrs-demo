using Financial.Model.Dtr;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data.Seeders
{
    public static class AttendanceLogSeeder
    {
        public static async Task SeedAsync(EtrsDbContext db)
        {
            if (await db.AttendanceLogs.AnyAsync()) return;

            // no initial attendance rows by default
            await db.SaveChangesAsync();
        }
    }
}
