using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskmony.Migrations
{
    /// <inheritdoc />
    public partial class RenameActorToModifiedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_ActorId",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "ActorId",
                table: "Notifications",
                newName: "ModifiedById");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ActorId",
                table: "Notifications",
                newName: "IX_Notifications_ModifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_ModifiedById",
                table: "Notifications",
                column: "ModifiedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_ModifiedById",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "ModifiedById",
                table: "Notifications",
                newName: "ActorId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_ModifiedById",
                table: "Notifications",
                newName: "IX_Notifications_ActorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_ActorId",
                table: "Notifications",
                column: "ActorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
