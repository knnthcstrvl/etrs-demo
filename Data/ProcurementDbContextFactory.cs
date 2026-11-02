using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Financial.Data
{
    public class ProcurementDbContextFactory : IDesignTimeDbContextFactory<EtrsDbContext>
    {
        public EtrsDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<EtrsDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            // Avoid connecting to the database just to discover its version during EF design-time commands.
            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 11)));

            return new EtrsDbContext(optionsBuilder.Options);
        }
    }
}
