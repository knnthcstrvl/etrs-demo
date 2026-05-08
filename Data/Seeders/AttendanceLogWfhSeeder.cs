using Financial.Model.Dtr;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data.Seeders
{
    public static class AttendanceLogWfhSeeder
    {
        public static async Task SeedAsync(EtrsDbContext db)
        {
            if (await db.AttendanceLogsWfh.AnyAsync()) return;

            // no initial WFH attendance rows by default
            await db.SaveChangesAsync();
        }
    }
}
