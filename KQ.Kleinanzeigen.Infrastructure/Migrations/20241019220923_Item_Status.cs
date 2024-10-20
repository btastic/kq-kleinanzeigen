using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KQ.Kleinanzeigen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Item_Status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Items");
        }
    }
}
