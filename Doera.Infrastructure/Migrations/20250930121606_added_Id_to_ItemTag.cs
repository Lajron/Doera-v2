using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Doera.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class added_Id_to_ItemTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TodoItemTags",
                table: "TodoItemTags");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "TodoItemTags",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TodoItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "TodoItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TodoItemTags",
                table: "TodoItemTags",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItemTags_TodoItemId_TagId",
                table: "TodoItemTags",
                columns: new[] { "TodoItemId", "TagId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TodoItemTags",
                table: "TodoItemTags");

            migrationBuilder.DropIndex(
                name: "IX_TodoItemTags_TodoItemId_TagId",
                table: "TodoItemTags");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TodoItemTags");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TodoItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "TodoItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TodoItemTags",
                table: "TodoItemTags",
                columns: new[] { "TodoItemId", "TagId" });
        }
    }
}
