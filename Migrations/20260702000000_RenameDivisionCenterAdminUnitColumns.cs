using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financial.Migrations
{
    [Migration("20260702000000_RenameDivisionCenterAdminUnitColumns")]
    public partial class RenameDivisionCenterAdminUnitColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            RenameColumnIfNeeded(migrationBuilder, "Employees", "DivisionCenterUnit", "DivisionCenterAdminUnit");
            RenameColumnIfNeeded(migrationBuilder, "AttendanceLogs", "DivisionCenterUnit", "DivisionCenterAdminUnit");
            RenameColumnIfNeeded(migrationBuilder, "AttendanceLogsWfh", "DivisionCenterUnit", "DivisionCenterAdminUnit");
            RenameColumnIfNeeded(migrationBuilder, "UserAccounts", "DivisionCenterUnit", "DivisionCenterAdminUnit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            RenameColumnIfNeeded(migrationBuilder, "Employees", "DivisionCenterAdminUnit", "DivisionCenterUnit");
            RenameColumnIfNeeded(migrationBuilder, "AttendanceLogs", "DivisionCenterAdminUnit", "DivisionCenterUnit");
            RenameColumnIfNeeded(migrationBuilder, "AttendanceLogsWfh", "DivisionCenterAdminUnit", "DivisionCenterUnit");
            RenameColumnIfNeeded(migrationBuilder, "UserAccounts", "DivisionCenterAdminUnit", "DivisionCenterUnit");
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
