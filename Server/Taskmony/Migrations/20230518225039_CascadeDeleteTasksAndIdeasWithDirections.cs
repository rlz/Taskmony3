using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskmony.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteTasksAndIdeasWithDirections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_Directions_DirectionId",
                table: "Ideas");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Directions_DirectionId",
                table: "Tasks");

            migrationBuilder.AddForeignKey(
                name: "FK_Ideas_Directions_DirectionId",
                table: "Ideas",
                column: "DirectionId",
                principalTable: "Directions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Directions_DirectionId",
                table: "Tasks",
                column: "DirectionId",
                principalTable: "Directions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_Directions_DirectionId",
                table: "Ideas");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Directions_DirectionId",
                table: "Tasks");

            migrationBuilder.AddForeignKey(
                name: "FK_Ideas_Directions_DirectionId",
                table: "Ideas",
                column: "DirectionId",
                principalTable: "Directions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Directions_DirectionId",
                table: "Tasks",
                column: "DirectionId",
                principalTable: "Directions",
                principalColumn: "Id");
        }
    }
}
