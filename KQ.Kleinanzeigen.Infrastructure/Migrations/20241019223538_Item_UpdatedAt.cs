using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KQ.Kleinanzeigen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Item_UpdatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Items",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Items");
        }
    }
}
