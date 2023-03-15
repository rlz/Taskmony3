using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskmony.Migrations
{
    /// <inheritdoc />
    public partial class RenameTaskFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RepeatsUntil",
                table: "Tasks",
                newName: "RepeatUntil");

            migrationBuilder.RenameColumn(
                name: "RepeatsEvery",
                table: "Tasks",
                newName: "RepeatEvery");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RepeatUntil",
                table: "Tasks",
                newName: "RepeatsUntil");

            migrationBuilder.RenameColumn(
                name: "RepeatEvery",
                table: "Tasks",
                newName: "RepeatsEvery");
        }
    }
}
