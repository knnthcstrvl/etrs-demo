using Financial.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financial.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(EtrsDbContext))]
    [Migration("20260702080000_RenameWfhPhotoVerificationToVerification")]
    public partial class RenameWfhPhotoVerificationToVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoVerification",
                table: "AttendanceLogsWfh",
                newName: "Verification");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Verification",
                table: "AttendanceLogsWfh",
                newName: "PhotoVerification");
        }
    }
}
