using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Body4uHUB.Content.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovedArticleNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookmarks_ArticleNumber",
                table: "Bookmarks");

            migrationBuilder.DropColumn(
                name: "ArticleNumber",
                table: "Bookmarks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ArticleNumber",
                table: "Bookmarks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_ArticleNumber",
                table: "Bookmarks",
                column: "ArticleNumber");
        }
    }
}
