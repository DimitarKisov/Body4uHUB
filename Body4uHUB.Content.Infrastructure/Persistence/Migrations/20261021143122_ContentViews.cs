using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Body4uHUB.Content.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ContentViews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentViews",
                columns: table => new
                {
                    TargetType = table.Column<byte>(type: "tinyint", nullable: false),
                    TargetId = table.Column<int>(type: "int", nullable: false),
                    VisitorHash = table.Column<byte[]>(type: "binary(32)", fixedLength: true, maxLength: 32, nullable: false),
                    ViewDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentViews", x => new { x.TargetType, x.TargetId, x.VisitorHash, x.ViewDate });
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentViews_ViewDate",
                table: "ContentViews",
                column: "ViewDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentViews");
        }
    }
}
