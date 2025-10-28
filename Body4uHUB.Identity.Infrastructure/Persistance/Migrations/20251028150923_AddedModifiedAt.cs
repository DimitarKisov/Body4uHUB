using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Body4uHUB.Identity.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddedModifiedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Users");
        }
    }
}
