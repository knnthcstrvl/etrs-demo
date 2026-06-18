using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financial.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoVerificationToAttendanceLogsWfh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoVerification",
                table: "AttendanceLogsWfh",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoVerification",
                table: "AttendanceLogsWfh");
        }
    }
}
