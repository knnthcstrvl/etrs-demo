using Financial.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financial.Migrations
{
    [DbContext(typeof(EtrsDbContext))]
    [Migration("20260703000000_SimplifyUserAccountName")]
    public partial class SimplifyUserAccountName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS
                (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = N'IX_UserAccounts_Email'
                        AND [object_id] = OBJECT_ID(N'[dbo].[UserAccounts]')
                )
                BEGIN
                    DROP INDEX [IX_UserAccounts_Email] ON [dbo].[UserAccounts];
                END

                IF COL_LENGTH('dbo.UserAccounts', 'Name') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[UserAccounts]
                    ADD [Name] nvarchar(150) NOT NULL
                    CONSTRAINT [DF_UserAccounts_Name] DEFAULT N'';
                END

                IF COL_LENGTH('dbo.UserAccounts', 'FirstName') IS NOT NULL
                   AND COL_LENGTH('dbo.UserAccounts', 'MiddleName') IS NOT NULL
                   AND COL_LENGTH('dbo.UserAccounts', 'LastName') IS NOT NULL
                BEGIN
                    EXEC sp_executesql N'
                        UPDATE [dbo].[UserAccounts]
                        SET [Name] = COALESCE(NULLIF(CONCAT_WS(N'' '', NULLIF([FirstName], N''''), NULLIF([MiddleName], N''''), NULLIF([LastName], N'''')), N''''), [Username])
                        WHERE [Name] = N'''';
                    ';
                END

                IF COL_LENGTH('dbo.UserAccounts', 'LastName') IS NOT NULL
                BEGIN
                    ALTER TABLE [dbo].[UserAccounts] DROP COLUMN [LastName];
                END

                IF COL_LENGTH('dbo.UserAccounts', 'FirstName') IS NOT NULL
                BEGIN
                    ALTER TABLE [dbo].[UserAccounts] DROP COLUMN [FirstName];
                END

                IF COL_LENGTH('dbo.UserAccounts', 'MiddleName') IS NOT NULL
                BEGIN
                    ALTER TABLE [dbo].[UserAccounts] DROP COLUMN [MiddleName];
                END

                IF COL_LENGTH('dbo.UserAccounts', 'Email') IS NOT NULL
                BEGIN
                    ALTER TABLE [dbo].[UserAccounts] DROP COLUMN [Email];
                END

                IF COL_LENGTH('dbo.UserAccounts', 'Designation') IS NOT NULL
                BEGIN
                    ALTER TABLE [dbo].[UserAccounts] DROP COLUMN [Designation];
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('dbo.UserAccounts', 'LastName') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[UserAccounts]
                    ADD [LastName] nvarchar(100) NOT NULL
                    CONSTRAINT [DF_UserAccounts_LastName] DEFAULT N'';
                END

                IF COL_LENGTH('dbo.UserAccounts', 'FirstName') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[UserAccounts]
                    ADD [FirstName] nvarchar(100) NOT NULL
                    CONSTRAINT [DF_UserAccounts_FirstName] DEFAULT N'';
                END

                IF COL_LENGTH('dbo.UserAccounts', 'MiddleName') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[UserAccounts]
                    ADD [MiddleName] nvarchar(100) NOT NULL
                    CONSTRAINT [DF_UserAccounts_MiddleName] DEFAULT N'';
                END

                IF COL_LENGTH('dbo.UserAccounts', 'Email') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[UserAccounts]
                    ADD [Email] nvarchar(150) NOT NULL
                    CONSTRAINT [DF_UserAccounts_Email] DEFAULT N'';
                END

                IF COL_LENGTH('dbo.UserAccounts', 'Designation') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[UserAccounts]
                    ADD [Designation] nvarchar(150) NOT NULL
                    CONSTRAINT [DF_UserAccounts_Designation] DEFAULT N'';
                END

                IF COL_LENGTH('dbo.UserAccounts', 'Name') IS NOT NULL
                BEGIN
                    UPDATE [dbo].[UserAccounts]
                    SET [LastName] = [Name],
                        [Email] = CASE
                            WHEN [Username] LIKE N'%@%' THEN [Username]
                            ELSE CONCAT([Username], N'@etrs.local')
                        END;

                    ALTER TABLE [dbo].[UserAccounts] DROP CONSTRAINT IF EXISTS [DF_UserAccounts_Name];
                    ALTER TABLE [dbo].[UserAccounts] DROP COLUMN [Name];
                END

                IF NOT EXISTS
                (
                    SELECT 1
                    FROM sys.indexes
                    WHERE [name] = N'IX_UserAccounts_Email'
                        AND [object_id] = OBJECT_ID(N'[dbo].[UserAccounts]')
                )
                BEGIN
                    CREATE UNIQUE INDEX [IX_UserAccounts_Email] ON [dbo].[UserAccounts]([Email]);
                END
                """);
        }
    }
}
