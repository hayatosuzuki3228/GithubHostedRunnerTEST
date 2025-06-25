namespace Hutzper.Project.Mekiki.Models.Config;

public class InspectionUnit
{
    /// <summary>
    /// ID（主キー）
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// AppConfigとの外部キー
    /// </summary>
    public int AppConfigId { get; set; }

    /// <summary>
    /// 検査単位名
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 任意の説明文
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// UI表示や設定構成順の制御に使用される順序番号
    /// </summary>
    public int? DisplayOrder { get; set; }

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
    /// InspectionUnitは一つのAppConfigとの参照を持つ
    /// </summary>

    public AppConfig AppConfig { get; set; } = null!;
}
