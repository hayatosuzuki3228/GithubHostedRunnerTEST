using Hutzper.Library.InsightLinkage.Connection;

namespace Hutzper.Library.InsightLinkage.Data;

/// <summary>
/// Insightへの送信要求
/// </summary>
public interface IInsightLinkageRequest
{
    /// <summary>
    /// 要求カウンター
    /// </summary>
    public int RequestCounter { get; set; }

    /// <summary>
    /// 要求ID
    /// </summary>
    public string RequestId { get; set; }

    /// <summary>
    /// 送信文字列
    /// </summary>
    public string MessageText { get; set; }

    /// <summary>
    /// アップロードファイル
    /// </summary>
    public IFileUploadRequest? FileUploadRequest { get; set; }

    /// <summary>
    /// リクエスト種別
    /// </summary>
    public InsightRequestType? InsightRequestType { get; set; }
}