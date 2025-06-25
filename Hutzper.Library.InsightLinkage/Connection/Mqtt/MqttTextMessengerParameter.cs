using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.InsightLinkage.Connection.Mqtt
{
    /// <summary>
    /// Mqtt テキストメッセンジャーパラメータ
    /// </summary>
    [Serializable]
    public record MqttTextMessengerParameter : ControllerParameterBaseRecord, ITextMessengerParameter
    {
        #region IConnectionParameter

        /// <summary>
        /// 使用するかどうか
        /// </summary>
        [IniKey(true, true)]
        public bool IsUse { get; set; } = true;

        /// <summary>
        /// デバイスuuid
        /// </summary>
        public string DeviceUuid { get; set; } = string.Empty;

        /// <summary>
        /// 企業uuid
        /// </summary>
        public string CompanyUuid { get; set; } = string.Empty;

        #endregion

        /// <summary>
        /// エンドポイント:Host
        /// </summary>
        [IniKey(true, "aryungjxhvjyh-ats.iot.ap-northeast-1.amazonaws.com")]
        public string EndpointHost { get; set; } = "aryungjxhvjyh-ats.iot.ap-northeast-1.amazonaws.com";

        /// <summary>
        /// エンドポイント:ポート
        /// </summary>
        [IniKey(true, 8883)]
        public int EndpointPort { get; set; } = 8883;

        /// <summary>
        /// トピック
        /// </summary>
        [IniKey(true, "mekikibaito/v1")]
        public string Topic { get; set; } = "mekikibaito/v1";

        /// <summary>
        /// CAファイル
        /// </summary>
        [IniKey(true, "certificate.pem.crt")]
        public string FileNameOfRootCA { get; set; } = "certificate.pem.crt";

        /// <summary>
        /// CAファイル
        /// </summary>
        [IniKey(true, "certificate.cert.pfx")]
        public string FileNameOfClientCA { get; set; } = "certificate.cert.pfx";

        /// <summary>
        /// パスワード
        /// </summary>
        [IniKey(true, "")]
        public string PasswordOfClientCA { get; set; } = "";

        /// <summary>
        /// 最大バッファリング数
        /// </summary>
        [IniKey(true, 10)]
        public int MaximumNumberOfBuffers { get; set; } = 10;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public MqttTextMessengerParameter() : this(0)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public MqttTextMessengerParameter(int index) : this(index, typeof(MqttTextMessengerParameter).Name)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public MqttTextMessengerParameter(int index, string fileNameWithoutExtension) : base($"Mqtt_Text_Messaging_{((index < 0) ? 0 : index) + 1:D2}", "MqttTextMessengerParameter", $"{fileNameWithoutExtension}_{((index < 0) ? 0 : index) + 1:D2}.ini")
        {
            this.IsHierarchy = false;
        }
    }
}