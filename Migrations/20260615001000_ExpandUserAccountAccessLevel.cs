using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financial.Migrations
{
    /// <inheritdoc />
    [Migration("20260615001000_ExpandUserAccountAccessLevel")]
    public partial class ExpandUserAccountAccessLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS
                (
                    SELECT 1
                    FROM sys.columns c
                    INNER JOIN sys.objects o ON c.object_id = o.object_id
                    INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
                    WHERE s.name = N'dbo'
                        AND o.name = N'UserAccounts'
                        AND c.name = N'AccessLevel'
                        AND c.max_length > 0
                        AND c.max_length < 2000
                )
                BEGIN
                    ALTER TABLE [dbo].[UserAccounts] ALTER COLUMN [AccessLevel] nvarchar(1000) NOT NULL;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('dbo.UserAccounts', 'AccessLevel') IS NOT NULL
                BEGIN
                    ALTER TABLE [dbo].[UserAccounts] ALTER COLUMN [AccessLevel] nvarchar(200) NOT NULL;
                END
                """);
        }
    }
}
