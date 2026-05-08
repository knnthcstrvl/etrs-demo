using Financial.Model.Dtr;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data.Seeders
{
    public static class BiometricDeviceSeeder
    {
        public static async Task SeedAsync(EtrsDbContext db)
        {
            if (await db.BiometricDevices.AnyAsync()) return;

            var devices = new List<BiometricDevice>
            {
                new() { DeviceName = "Main Biometric Terminal", SerialNumber = "BIO-0001", Location = "Head Office", Status = "Active" }
            };

            await db.BiometricDevices.AddRangeAsync(devices);
            await db.SaveChangesAsync();
        }
    }
}
