using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hutzper.Project.Mekiki.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "User",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "User",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_User_username",
                table: "User",
                column: "username",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_Role_Enum",
                table: "User",
                sql: "\"role\" IN (0, 1, 2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_username",
                table: "User");

            migrationBuilder.DropCheckConstraint(
                name: "CK_User_Role_Enum",
                table: "User");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "User");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "User");
        }
    }
}
