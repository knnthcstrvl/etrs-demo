using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financial.Migrations
{
    [Migration("20260702001000_RenameLateUndertimeColumns")]
    public partial class RenameLateUndertimeColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            RenameColumnIfNeeded(migrationBuilder, "AttendanceLogs", "Tardiness", "LateUndertime");
            RenameColumnIfNeeded(migrationBuilder, "AttendanceLogsWfh", "Tardiness", "LateUndertime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            RenameColumnIfNeeded(migrationBuilder, "AttendanceLogs", "LateUndertime", "Tardiness");
            RenameColumnIfNeeded(migrationBuilder, "AttendanceLogsWfh", "LateUndertime", "Tardiness");
        }

        private static void RenameColumnIfNeeded(MigrationBuilder migrationBuilder, string table, string oldName, string newName)
        {
            migrationBuilder.Sql($@"
IF COL_LENGTH(N'[dbo].[{table}]', N'{oldName}') IS NOT NULL
   AND COL_LENGTH(N'[dbo].[{table}]', N'{newName}') IS NULL
BEGIN
    EXEC sp_rename N'[dbo].[{table}].[{oldName}]', N'{newName}', N'COLUMN';
END");
        }
    }
}
