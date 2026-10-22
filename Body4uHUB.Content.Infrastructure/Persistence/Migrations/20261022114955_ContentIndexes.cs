using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Body4uHUB.Content.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ContentIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ForumTopics_Id",
                table: "ForumTopics");

            migrationBuilder.DropIndex(
                name: "IX_Bookmarks_UserId",
                table: "Bookmarks");

            migrationBuilder.DropIndex(
                name: "IX_Articles_AuthorId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_Id",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_PublishedAt",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_Status",
                table: "Articles");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_AuthorId_Status_IsDeleted_PublishedAt",
                table: "Articles",
                columns: new[] { "AuthorId", "Status", "IsDeleted", "PublishedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Status_IsDeleted_PublishedAt",
                table: "Articles",
                columns: new[] { "Status", "IsDeleted", "PublishedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Articles_AuthorId_Status_IsDeleted_PublishedAt",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_Status_IsDeleted_PublishedAt",
                table: "Articles");

            migrationBuilder.CreateIndex(
                name: "IX_ForumTopics_Id",
                table: "ForumTopics",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_UserId",
                table: "Bookmarks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_AuthorId",
                table: "Articles",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Id",
                table: "Articles",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_PublishedAt",
                table: "Articles",
                column: "PublishedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Status",
                table: "Articles",
                column: "Status");
        }
    }
}
