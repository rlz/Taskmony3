using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskmony.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurrencePatternTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecurrencePatterns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    RepeatMode = table.Column<byte>(type: "smallint", nullable: false),
                    WeekDays = table.Column<byte>(type: "smallint", nullable: true),
                    RepeatEvery = table.Column<int>(type: "integer", nullable: false),
                    RepeatUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurrencePatterns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurrencePatterns_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecurrencePatterns_TaskId",
                table: "RecurrencePatterns",
                column: "TaskId",
                unique: true);

            migrationBuilder.Sql("""
                INSERT INTO "RecurrencePatterns" ("Id", "TaskId", "RepeatMode", "WeekDays", "RepeatEvery", "RepeatUntil")
                SELECT gen_random_uuid(), "Id", "RepeatMode", "WeekDays", "RepeatEvery", "RepeatUntil" 
                FROM "Tasks"
                WHERE "RepeatMode" IS NOT NULL
            """);

            migrationBuilder.DropColumn(
                name: "RepeatEvery",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "RepeatMode",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "RepeatUntil",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "WeekDays",
                table: "Tasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RepeatEvery",
                table: "Tasks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "RepeatMode",
                table: "Tasks",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RepeatUntil",
                table: "Tasks",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "WeekDays",
                table: "Tasks",
                type: "smallint",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Tasks"
                SET "RepeatMode" = "RecurrencePatterns"."RepeatMode",
                    "WeekDays" = "RecurrencePatterns"."WeekDays",
                    "RepeatEvery" = "RecurrencePatterns"."RepeatEvery",
                    "RepeatUntil" = "RecurrencePatterns"."RepeatUntil"
                FROM "RecurrencePatterns"
                WHERE "Tasks"."Id" = "RecurrencePatterns"."TaskId"
            """);

            migrationBuilder.DropTable(
                name: "RecurrencePatterns");
        }
    }
}
