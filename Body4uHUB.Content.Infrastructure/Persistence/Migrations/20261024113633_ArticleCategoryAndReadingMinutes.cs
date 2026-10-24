using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Body4uHUB.Content.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ArticleCategoryAndReadingMinutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Existing articles get ArticleCategory.Other (4); 0 is not a valid category.
            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 4);

            migrationBuilder.AddColumn<int>(
                name: "ReadingMinutes",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // Approximates Article.CalculateReadingMinutes by counting spaces; the domain recalculates on the next edit.
            migrationBuilder.Sql(
                """
                UPDATE Articles
                SET ReadingMinutes = CEILING((LEN(Content) - LEN(REPLACE(Content, ' ', '')) + 1) / 200.0)
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ReadingMinutes",
                table: "Articles");
        }
    }
}
