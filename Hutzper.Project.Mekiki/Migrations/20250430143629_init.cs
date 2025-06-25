using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Hutzper.Project.Mekiki.Migrations;

/// <summary>
/// Userテーブルを作成する初期マイグレーションのためのクラス
/// </summary>
public partial class Init : Migration
{
    /// <summary>
    /// マイグレーションの開始処理
    /// </summary>
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "User",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Key = table.Column<string>(type: "text", nullable: true),
                Value = table.Column<string>(type: "text", nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_User", x => x.Id);
            });
    }

    /// <summary>
    /// マイグレーションの終了処理
    /// </summary>
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "User");
    }
}
