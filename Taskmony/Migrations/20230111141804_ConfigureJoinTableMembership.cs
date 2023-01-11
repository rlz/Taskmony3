using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskmony.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureJoinTableMembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DirectionUser");

            migrationBuilder.CreateTable(
                name: "Memberships",
                columns: table => new
                {
                    DirectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => new { x.DirectionId, x.UserId });
                    table.ForeignKey(
                        name: "FK_Memberships_Directions_DirectionId",
                        column: x => x.DirectionId,
                        principalTable: "Directions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Memberships_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Memberships_UserId",
                table: "Memberships",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Memberships");

            migrationBuilder.CreateTable(
                name: "DirectionUser",
                columns: table => new
                {
                    DirectionsId = table.Column<Guid>(type: "uuid", nullable: false),
                    MembersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectionUser", x => new { x.DirectionsId, x.MembersId });
                    table.ForeignKey(
                        name: "FK_DirectionUser_Directions_DirectionsId",
                        column: x => x.DirectionsId,
                        principalTable: "Directions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectionUser_Users_MembersId",
                        column: x => x.MembersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DirectionUser_MembersId",
                table: "DirectionUser",
                column: "MembersId");
        }
    }
}
