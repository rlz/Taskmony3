using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskmony.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignedByToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssignedById",
                table: "Tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedById",
                table: "Tasks",
                column: "AssignedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AssignedById",
                table: "Tasks",
                column: "AssignedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AssignedById",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssignedById",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AssignedById",
                table: "Tasks");
        }
    }
}
