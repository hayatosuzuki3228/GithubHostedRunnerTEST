namespace Hutzper.Project.Mekiki.Models.Config;

/// <summary>
/// アプリの設定情報に関するスキーマ
/// </summary>
public class AppConfig
{
    /// <summary>
    /// ID（主キー）
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// レコード作成日時
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// 最終更新日時
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 外部キー参照のための宣言
    /// AppConfigは複数のInspectionUnitとの参照を持つ
    /// </summary>
    public List<InspectionUnit> inspectionUnits { get; set; } = new();  // 外部キー
}
