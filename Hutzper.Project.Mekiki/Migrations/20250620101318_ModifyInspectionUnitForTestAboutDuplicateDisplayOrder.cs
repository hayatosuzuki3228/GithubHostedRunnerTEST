using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hutzper.Project.Mekiki.Migrations
{
    /// <inheritdoc />
    public partial class ModifyInspectionUnitForTestAboutDuplicateDisplayOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Inspection_unit_display_order",
                table: "Inspection_unit",
                column: "display_order",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inspection_unit_display_order",
                table: "Inspection_unit");
        }
    }
}
