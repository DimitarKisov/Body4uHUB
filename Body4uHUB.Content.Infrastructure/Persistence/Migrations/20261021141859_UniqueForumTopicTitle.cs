using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Body4uHUB.Content.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UniqueForumTopicTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ForumTopics_Title",
                table: "ForumTopics",
                column: "Title",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ForumTopics_Title",
                table: "ForumTopics");
        }
    }
}
