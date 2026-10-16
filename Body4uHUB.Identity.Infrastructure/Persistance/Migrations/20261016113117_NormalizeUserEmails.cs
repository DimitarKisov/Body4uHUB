using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Body4uHUB.Identity.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeUserEmails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ContactInfo.Create now stores emails trimmed and lower-cased; bring existing rows in line.
            // IX_Users_Email is unique under the default case-insensitive collation, so this cannot collide.
            migrationBuilder.Sql("UPDATE [Users] SET [Email] = LOWER(LTRIM(RTRIM([Email]))) WHERE [Email] IS NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // The original casing is not recoverable.
        }
    }
}
