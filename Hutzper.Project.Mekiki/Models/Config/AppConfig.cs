namespace Hutzper.Project.Mekiki.Models.Config;

/// <summary>
/// アプリの設定情報に関するスキーマ
/// </summary>
public class AppConfig
{
    public int Id { get; set; } // Primary Key
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
