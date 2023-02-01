using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taskmony.Migrations
{
    /// <inheritdoc />
    public partial class AddValueObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubscribedAt",
                table: "Subscriptions",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Subscriptions",
                newName: "SubscribedAt");
        }
    }
}
