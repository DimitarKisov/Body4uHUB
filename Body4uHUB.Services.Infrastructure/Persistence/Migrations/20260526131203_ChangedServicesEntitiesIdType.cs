using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Body4uHUB.Services.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangedServicesEntitiesIdType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_ServiceOrderId",
                table: "Reviews");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Reviews",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ServiceOrderId",
                table: "Reviews",
                column: "OrderId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_ServiceOrderId",
                table: "Reviews");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Reviews",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ServiceOrderId",
                table: "Reviews",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");
        }
    }
}
