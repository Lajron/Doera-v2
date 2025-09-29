using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doera.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class added_TodoList_Order : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "TodoLists");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "TodoLists",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "TodoLists");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TodoLists",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: true);
        }
    }
}
