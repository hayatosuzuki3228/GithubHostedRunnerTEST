using Hutzper.Library.Common.Runtime;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Hutzper.Library.InsightLinkage.Data
{
    /// <summary>
    /// mekikiデータ
    /// </summary>
    [DataContract]
    [Serializable]
    public class InsightMekikiData : IInsightMekikiData
    {
        /// <summary>
        /// uuid
        /// </summary>
        [DataMember(Name = "uuid")]
        [JsonPropertyName("uuid")]
        public string Uuid { get; protected set; } = string.Empty;

        /// <summary>
        /// タイムスタンプ
        /// </summary>
        [DataMember(Name = "timestamps")]
        [JsonPropertyName("timestamps")]
        public decimal Timestamps => Convert.ToDecimal(this.DateTime.ToString("yyyyMMddHHmmssffffff"));

        /// <summary>
        /// カテゴリ
        /// </summary>
        [DataMember(Name = "camera_name")]
        [JsonPropertyName("camera_name")]
        public string DataCategory { get; set; } = "all";

        /// <summary>
        /// クラス
        /// </summary>
        [DataMember(Name = "class")]
        [JsonPropertyName("class")]
        public string Class { get; set; } = string.Empty;

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
        public Dictionary<string, double> Options { get; set; } = new();

        /// <summary>
        /// unix時間
        /// </summary>
        [DataMember(Name = "unixtime")]
        [JsonPropertyName("unixtime")]
        public int Unixtime { get; protected set; }

        /// <summary>
        /// 日時
        /// </summary>
        [IgnoreDataMember]
        [JsonIgnore]
        public DateTime DateTime { get; protected set; }

        /// <summary>
        /// 汎用値
        /// </summary>
        [IgnoreDataMember]
        [JsonIgnore]
        public double[] Values { get; protected set; } = Array.Empty<double>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="device_uuid"></param>
        public InsightMekikiData()
        {
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="dateTime"></param>
        public void Initialize(string uuid, DateTime dateTime, int numberOfValue = 10)
        {
            this.Uuid = uuid;
            this.DateTime = dateTime;
            this.Unixtime = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            if (0 < numberOfValue)
            {
                this.Values = Enumerable.Repeat(0d, numberOfValue).ToArray();
            }
            else
            {
                this.Values = Array.Empty<double>();
            }
        }

        /// <summary>
        /// JSON文字列化
        /// </summary>
        /// <returns></returns>
        public virtual string ToJsonText()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            var json = System.Text.Json.JsonSerializer.Serialize(this, options);
            return Regex.Replace(json, @"(^\s{4}.*"":\s*)(\d+)(?!\.\d)", "$1$2.0", RegexOptions.Multiline);
        }

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public virtual IInsightMekikiData? Clone()
        {
            var clonedData = new InsightMekikiData();
            clonedData.Initialize(this.Uuid, this.DateTime);

            PropertyCopier.CopyTo(this, clonedData);

            clonedData.Values = new double[this.Values.Length];
            Array.Copy(this.Values, clonedData.Values, clonedData.Values.Length);

            clonedData.Options = new Dictionary<string, double>();
            foreach (var option in this.Options)
            {
                clonedData.Options.Add(option.Key, option.Value);
            }

            return clonedData;
        }
    }
}