using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financial.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalStatusFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MovApprovalStatus",
                table: "AttendanceLogsWfh",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "MovApprovedAt",
                table: "AttendanceLogsWfh",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MovRejectedAt",
                table: "AttendanceLogsWfh",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoVerificationApprovalStatus",
                table: "AttendanceLogsWfh",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PhotoVerificationApprovedAt",
                table: "AttendanceLogsWfh",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PhotoVerificationRejectedAt",
                table: "AttendanceLogsWfh",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MovApprovalStatus",
                table: "AttendanceLogs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "MovApprovedAt",
                table: "AttendanceLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MovRejectedAt",
                table: "AttendanceLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoVerificationApprovalStatus",
                table: "AttendanceLogs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PhotoVerificationApprovedAt",
                table: "AttendanceLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PhotoVerificationRejectedAt",
                table: "AttendanceLogs",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovApprovalStatus",
                table: "AttendanceLogsWfh");

            migrationBuilder.DropColumn(
                name: "MovApprovedAt",
                table: "AttendanceLogsWfh");

            migrationBuilder.DropColumn(
                name: "MovRejectedAt",
                table: "AttendanceLogsWfh");

            migrationBuilder.DropColumn(
                name: "PhotoVerificationApprovalStatus",
                table: "AttendanceLogsWfh");

            migrationBuilder.DropColumn(
                name: "PhotoVerificationApprovedAt",
                table: "AttendanceLogsWfh");

            migrationBuilder.DropColumn(
                name: "PhotoVerificationRejectedAt",
                table: "AttendanceLogsWfh");

            migrationBuilder.DropColumn(
                name: "MovApprovalStatus",
                table: "AttendanceLogs");

            migrationBuilder.DropColumn(
                name: "MovApprovedAt",
                table: "AttendanceLogs");

            migrationBuilder.DropColumn(
                name: "MovRejectedAt",
                table: "AttendanceLogs");

            migrationBuilder.DropColumn(
                name: "PhotoVerificationApprovalStatus",
                table: "AttendanceLogs");

            migrationBuilder.DropColumn(
                name: "PhotoVerificationApprovedAt",
                table: "AttendanceLogs");

            migrationBuilder.DropColumn(
                name: "PhotoVerificationRejectedAt",
                table: "AttendanceLogs");
        }
    }
}
