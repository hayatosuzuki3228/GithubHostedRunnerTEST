using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.InsightLinkage.Connection;

/// <summary>
/// ファイルアップローダーパラメータ
/// </summary>
public interface IFileUploaderParameter : IConnectionParameter
{
    /// <summary>
    /// アクセスキー ID
    /// </summary>
    public string AccessKeyID { get; set; }

    /// <summary>
    /// シークレットアクセスキー
    /// </summary>
    public string SecretKey { get; set; }

    /// <summary>
    /// プロジェクトUUID
    /// </summary>
    public string ProjectUuid { get; set; }

    /// <summary>
    /// バケット名
    /// </summary>
    [IniKey(true, "insight-image")]
    public string BucketName { get; set; }
}