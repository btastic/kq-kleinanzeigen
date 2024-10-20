using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KQ.Kleinanzeigen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ShippingCost_Shipping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingCost",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "Shipping",
                table: "Items",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Shipping",
                table: "Items");

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingCost",
                table: "Items",
                type: "decimal(18,6)",
                nullable: true);
        }
    }
}
