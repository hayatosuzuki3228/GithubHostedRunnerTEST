using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Hutzper.Library.InsightLinkage.Data;

/// <summary>
/// mekikiデータ
/// </summary>
public interface IInsightMekikiData
{
    /// <summary>
    /// uuid
    /// </summary>
    [DataMember(Name = "uuid")]
    [JsonPropertyName("uuid")]
    public string Uuid { get; }

    /// <summary>
    /// タイムスタンプ
    /// </summary>
    [DataMember(Name = "timestamps")]
    [JsonPropertyName("timestamps")]
    public decimal Timestamps { get; }

    /// <summary>
    /// カテゴリ
    /// </summary>
    [DataMember(Name = "camera_name")]
    [JsonPropertyName("camera_name")]
    public string DataCategory { get; set; }

    /// <summary>
    /// クラス
    /// </summary>
    [DataMember(Name = "class")]
    [JsonPropertyName("class")]
    public string Class { get; set; }

    /// <summary>
    /// 画像アップロード
    /// </summary>
    [DataMember(Name = "image_uploaded")]
    [JsonPropertyName("image_uploaded")]
    public bool ImageUploaded { get; set; }

    /// <summary>
    /// Option
    /// </summary>
    [DataMember(Name = "option")]
    [JsonPropertyName("option")]
    public Dictionary<string, double> Options { get; set; }

    /// <summary>
    /// unix時間(us)
    /// </summary>
    [DataMember(Name = "unixtime_us")]
    [JsonPropertyName("unixtime_us")]
    public long UnixtimeUs { get; }

    /// <summary>
    /// 日時
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public DateTime DateTime { get; }

    /// <summary>
    /// 汎用値
    /// </summary>
    [IgnoreDataMember]
    [JsonIgnore]
    public double[] Values { get; }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="uuid"></param>
    /// <param name="dateTime"></param>
    public void Initialize(string uuid, DateTime dateTime, int numberOfValue = 10);

    /// <summary>
    /// JSON文字列化
    /// </summary>
    /// <returns></returns>
    public string ToJsonText();

    /// <summary>
    /// 複製
    /// </summary>
    /// <returns></returns>
    public IInsightMekikiData? Clone();
}