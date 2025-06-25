namespace Hutzper.Library.InsightLinkage.Connection;

/// <summary>
/// テキストメッセンジャーパラメータ
/// </summary>
public interface ITextMessengerParameter : IConnectionParameter
{
    /// <summary>
    /// デバイスuuid
    /// </summary>
    public string DeviceUuid { get; set; }

    /// <summary>
    /// 企業uuid
    /// </summary>
    public string CompanyUuid { get; set; }
}