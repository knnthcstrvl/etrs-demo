using Financial.Model.Dtr;
using Microsoft.EntityFrameworkCore;

namespace Financial.Data
{
    public static class DatabaseSchemaUpdater
    {
        public static async Task EnsureAttendanceLogWfhColumnsAsync(EtrsDbContext db)
        {
            var entityType = db.Model.FindEntityType(typeof(AttendanceLogWfh));
            var tableName = entityType?.GetTableName();
            var schema = entityType?.GetSchema() ?? "dbo";

            if (string.IsNullOrWhiteSpace(tableName))
            {
                return;
            }

            var qualifiedTable = $"[{schema}].[{tableName}]";
            var exceptionEntityType = db.Model.FindEntityType(typeof(ExceptionCondition));
            var exceptionTableName = exceptionEntityType?.GetTableName() ?? "ExceptionConditions";
            var exceptionSchema = exceptionEntityType?.GetSchema() ?? "dbo";
            var qualifiedExceptionTable = $"[{exceptionSchema}].[{exceptionTableName}]";
            ValidateSqlIdentifier(schema);
            ValidateSqlIdentifier(tableName);
            ValidateSqlIdentifier(exceptionSchema);
            ValidateSqlIdentifier(exceptionTableName);

#pragma warning disable EF1002
            await db.Database.ExecuteSqlRawAsync($"""
                IF OBJECT_ID(N'{exceptionSchema}.{exceptionTableName}', N'U') IS NULL
                BEGIN
                    CREATE TABLE {qualifiedExceptionTable}
                    (
                        [ExceptionConditionId] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [ConditionName] nvarchar(150) NOT NULL,
                        [Description] nvarchar(300) NOT NULL CONSTRAINT [DF_{exceptionTableName}_Description] DEFAULT N'',
                        [IsActive] bit NOT NULL CONSTRAINT [DF_{exceptionTableName}_IsActive] DEFAULT 1,
                        [DateCreated] datetime2 NOT NULL CONSTRAINT [DF_{exceptionTableName}_DateCreated] DEFAULT SYSUTCDATETIME(),
                        [DateUpdated] datetime2 NULL
                    );
                END

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

                IF COL_LENGTH('{schema}.{tableName}', 'WorkArrangement') IS NULL
                BEGIN
                    ALTER TABLE {qualifiedTable} ADD [WorkArrangement] nvarchar(100) NOT NULL CONSTRAINT [DF_{tableName}_WorkArrangement] DEFAULT N'WFH';
                END

                IF COL_LENGTH('{schema}.{tableName}', 'ExceptionConditionId') IS NULL
                BEGIN
                    ALTER TABLE {qualifiedTable} ADD [ExceptionConditionId] int NULL;
                END

                IF COL_LENGTH('{schema}.{tableName}', 'TagUpdatedAt') IS NULL
                BEGIN
                    ALTER TABLE {qualifiedTable} ADD [TagUpdatedAt] datetime2 NULL;
                END

                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE [name] = 'FK_{tableName}_{exceptionTableName}_ExceptionConditionId'
                )
                BEGIN
                    ALTER TABLE {qualifiedTable}
                    ADD CONSTRAINT [FK_{tableName}_{exceptionTableName}_ExceptionConditionId]
                    FOREIGN KEY ([ExceptionConditionId]) REFERENCES {qualifiedExceptionTable}([ExceptionConditionId]);
                END
                """);
#pragma warning restore EF1002
        }

        private static void ValidateSqlIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier) || identifier.Any(ch => !char.IsLetterOrDigit(ch) && ch != '_'))
            {
                throw new InvalidOperationException($"Unsafe SQL identifier detected: '{identifier}'.");
            }
        }
    }
}
