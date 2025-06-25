using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.CodeReader.Device.Keyence
{
    /// <summary>
    /// コードリーダーデバイスパラメータ
    /// </summary>
    /// <remarks>KEYENCE SRX</remarks>
    [Serializable]
    public record CodeReaderParameterKeyence_SRX : CodeReaderParameter, ITcpCodeReaderParameter
    {
        /// <summary>
        /// IPアドレス
        /// </summary>
        [IniKey(true, "192.168.100.4")]
        public string IpAddress { get; set; } = "192.168.100.4";

        /// <summary>
        /// ポート番号
        /// </summary>
        [IniKey(true, 9004)]
        public int PortNumber { get; set; } = 9004;

        /// <summary>
        /// コマンド応答タイムアウトミリ秒
        /// </summary>
        [IniKey(true, 500)]
        public int CommandResponseTimeoutMs { get; set; } = 500;

        /// <summary>
        /// フォーカス調整タイムアウトミリ秒
        /// </summary>
        [IniKey(true, 5000)]
        public int FocusTuningTimeoutMs { get; set; } = 5000;

        /// <summary>
        /// 自動チューニングタイムアウトミリ秒
        /// </summary>
        [IniKey(true, 20000)]
        public int AutoTuningTimeoutMs { get; set; } = 20000;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CodeReaderParameterKeyence_SRX() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public CodeReaderParameterKeyence_SRX(Common.Drawing.Point location) : base(location, typeof(CodeReaderParameterKeyence_SRX).Name)
        {
        }
    }
}