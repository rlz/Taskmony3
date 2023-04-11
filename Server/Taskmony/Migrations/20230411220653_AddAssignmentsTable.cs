using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskmony.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignmentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "RefreshTokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssigneeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedById = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assignments_Users_AssignedById",
                        column: x => x.AssignedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assignments_Users_AssigneeId",
                        column: x => x.AssigneeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_AssignedById",
                table: "Assignments",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_AssigneeId",
                table: "Assignments",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_TaskId",
                table: "Assignments",
                column: "TaskId",
                unique: true);

            migrationBuilder.Sql("""
                INSERT INTO "Assignments" ("Id", "TaskId", "AssigneeId", "AssignedById", "CreatedAt")
                SELECT gen_random_uuid(), "Id", "AssigneeId", "AssignedById", now()
                FROM "Tasks"
                WHERE "AssigneeId" IS NOT NULL
            """);
            
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AssignedById",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AssigneeId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssignedById",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssigneeId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AssignedById",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "Tasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "RefreshTokens",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "now()");
            
            migrationBuilder.AddColumn<Guid>(
                name: "AssignedById",
                table: "Tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssigneeId",
                table: "Tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedById",
                table: "Tasks",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssigneeId",
                table: "Tasks",
                column: "AssigneeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AssignedById",
                table: "Tasks",
                column: "AssignedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AssigneeId",
                table: "Tasks",
                column: "AssigneeId",
                principalTable: "Users",
                principalColumn: "Id");
            
            migrationBuilder.Sql("""
                UPDATE "Tasks"
                SET "AssigneeId" = "Assignments"."AssigneeId", "AssignedById" = "Assignments"."AssignedById"
                FROM "Assignments"
                WHERE "Tasks"."Id" = "Assignments"."TaskId"
            """);

            migrationBuilder.DropTable(
                name: "Assignments");
        }
    }
}
