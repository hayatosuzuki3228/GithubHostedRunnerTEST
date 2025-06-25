using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hutzper.Project.Mekiki.Migrations
{
    /// <inheritdoc />
    public partial class ModifyInspectionUnitForTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Inspection_DisplayOrder_Enum",
                table: "Inspection_unit",
                sql: "display_order >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Inspection_DisplayOrder_Enum",
                table: "Inspection_unit");
        }
    }
}
