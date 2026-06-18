using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financial.Migrations
{
    /// <inheritdoc />
    [Migration("20260615000000_AddMustChangePasswordToUserAccounts")]
    public partial class AddMustChangePasswordToUserAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('dbo.UserAccounts', 'MustChangePassword') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[UserAccounts]
                    ADD [MustChangePassword] bit NOT NULL
                    CONSTRAINT [DF_UserAccounts_MustChangePassword] DEFAULT 0;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('dbo.UserAccounts', 'MustChangePassword') IS NOT NULL
                BEGIN
                    ALTER TABLE [dbo].[UserAccounts] DROP CONSTRAINT IF EXISTS [DF_UserAccounts_MustChangePassword];
                    ALTER TABLE [dbo].[UserAccounts] DROP COLUMN [MustChangePassword];
                END
                """);
        }
    }
}
