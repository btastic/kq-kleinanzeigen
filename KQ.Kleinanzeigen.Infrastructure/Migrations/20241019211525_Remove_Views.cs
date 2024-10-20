using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KQ.Kleinanzeigen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Views : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Views",
                table: "Items");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
