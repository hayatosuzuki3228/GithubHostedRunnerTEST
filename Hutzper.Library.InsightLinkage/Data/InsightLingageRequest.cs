using Hutzper.Library.InsightLinkage.Connection;

namespace Hutzper.Library.InsightLinkage.Data;

/// <summary>
/// IInsightLinkageRequest実装
/// </summary>
[Serializable]
public record InsightLinkageRequest : IInsightLinkageRequest
{
    /// <summary>
    /// 要求カウンター
    /// </summary>
    public int RequestCounter { get; set; }

    /// <summary>
    /// 要求ID
    /// </summary>
    public string RequestId { get; set; } = string.Empty;

    /// <summary>
    /// 送信文字列
    /// </summary>
    public string MessageText { get; set; } = string.Empty;

    /// <summary>
    /// アップロードファイル
    /// </summary>
    public IFileUploadRequest? FileUploadRequest { get; set; }

    /// <summary>
    /// リクエスト種別
    /// </summary>
    public InsightRequestType? InsightRequestType { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="requestId"></param>
    /// <param name="mekikiData"></param>
    /// <param name="fileUploadRequest"></param>
    public InsightLinkageRequest(string requestId, IInsightMekikiData mekikiData, IFileUploadRequest? fileUploadRequest, InsightRequestType requestType) : this(requestId, string.Empty, fileUploadRequest, requestType)
    {
        this.MessageText = mekikiData?.ToJsonText() ?? string.Empty;
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="requestId"></param>
    /// <param name="messageText"></param>
    /// <param name="uploadFileInfo"></param>
    public InsightLinkageRequest(string requestId, string messageText, IFileUploadRequest? fileUploadRequest, InsightRequestType requestType)
    {
        this.RequestId = requestId;
        this.MessageText = messageText;
        this.FileUploadRequest = fileUploadRequest;
        this.InsightRequestType = requestType;
    }
}