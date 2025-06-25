using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hutzper.Project.Mekiki.Migrations
{
    /// <inheritdoc />
    public partial class ModifyInspectionUnitForTestsAboutDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "display_order",
                table: "Inspection_unit",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "display_order",
                table: "Inspection_unit",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
