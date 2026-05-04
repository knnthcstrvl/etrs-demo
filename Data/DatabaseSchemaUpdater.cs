using Financial.Model.Dtr;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data
{
    public static class DatabaseSchemaUpdater
    {
        public static async Task EnsureAttendanceLogWfhColumnsAsync(ProcurementDbContext db)
        {
            var entityType = db.Model.FindEntityType(typeof(AttendanceLogWfh));
            var tableName = entityType?.GetTableName();
            var schema = entityType?.GetSchema() ?? "dbo";

            if (string.IsNullOrWhiteSpace(tableName))
            {
                return;
            }

            var qualifiedTable = $"[{schema}].[{tableName}]";

            await db.Database.ExecuteSqlRawAsync($"""
                IF COL_LENGTH('{schema}.{tableName}', 'Target') IS NULL
                BEGIN
                    ALTER TABLE {qualifiedTable} ADD [Target] nvarchar(max) NOT NULL CONSTRAINT [DF_{tableName}_Target] DEFAULT N'';
                END

                IF COL_LENGTH('{schema}.{tableName}', 'Accomplishment') IS NULL
                BEGIN
                    ALTER TABLE {qualifiedTable} ADD [Accomplishment] nvarchar(max) NOT NULL CONSTRAINT [DF_{tableName}_Accomplishment] DEFAULT N'';
                END

                IF COL_LENGTH('{schema}.{tableName}', 'Mov') IS NULL
                BEGIN
                    ALTER TABLE {qualifiedTable} ADD [Mov] nvarchar(500) NOT NULL CONSTRAINT [DF_{tableName}_Mov] DEFAULT N'';
                END
                """);
        }
    }
}
