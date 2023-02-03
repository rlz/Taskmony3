using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskmony.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRecurringTaskFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RepeatEvery",
                table: "Tasks",
                newName: "RepeatsEvery");

            migrationBuilder.AddColumn<DateTime>(
                name: "RepeatsUntil",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "WeekDays",
                table: "Tasks",
                type: "smallint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepeatsUntil",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "WeekDays",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "RepeatsEvery",
                table: "Tasks",
                newName: "RepeatEvery");
        }
    }
}
