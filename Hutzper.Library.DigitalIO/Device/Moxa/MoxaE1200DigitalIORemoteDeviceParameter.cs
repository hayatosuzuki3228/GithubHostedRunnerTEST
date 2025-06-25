using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.DigitalIO.Device.Moxa
{
    /// <summary>
    /// MoxaE1200パラメータ
    /// </summary>
    [Serializable]
    public record MoxaE1200DigitalIORemoteDeviceParameter : ControllerParameterBaseRecord, IDigitalIORemoteDeviceParameter
    {
        #region IDigitalIODeviceParameter

        /// <summary>
        /// デバイスID
        /// </summary>
        [IniKey(true, "")]
        public virtual string DeviceID { get; set; } = string.Empty;

        /// <summary>
        /// 識別
        /// </summary>
        public virtual Common.Drawing.Point Location
        {
            get => this.location.Clone();
        }

        /// <summary>
        /// 入力チャネル
        /// </summary>
        [IniKey(true, new int[0])]
        public int[] InputChannels { get; set; } = Array.Empty<int>();
        //public int[] InputChannels { get; } = Enumerable.Range(0, 4).ToArray();

        /// <summary>
        /// 出力チャネル
        /// </summary>
        [IniKey(true, new int[0])]
        public int[] OutputChannels { get; set; } = Array.Empty<int>();
        //public int[] OutputChannels { get; } = Enumerable.Range(4, 4).ToArray();

        /// <summary>
        /// 入力フィルタリングミリ秒
        /// </summary>
        [IniKey(true, 50, 50, 50, 50)]
        public int[] FilteringTimeMsIntervals { get; set; } = Enumerable.Repeat(50, 4).ToArray();

        #endregion

        #region IDigitalIORemoteDeviceParameter

        /// <summary>
        /// IPアドレス
        /// </summary>
        [IniKey(true, "192.168.127.254")]
        public string IpAddress { get; set; } = "192.168.127.254";

        /// <summary>
        /// ポート番号
        /// </summary>
        [IniKey(true, 502)]
        public int PortNumber { get; set; } = 502;

        /// <summary>
        /// 再接続
        /// </summary>
        [IniKey(true, false)]
        public bool IsReconnectable { get; set; }

        /// <summary>
        /// 再接続試行間隔
        /// </summary>
        [IniKey(true, 0)]
        public int ReconnectionAttemptsIntervalSec { get; set; }

        #endregion

        /// <summary>
        /// パスワード
        /// </summary>
        [IniKey(true, "moxa")]
        public string Password { get; set; } = "moxa";

        protected Common.Drawing.Point location = new();
        protected string fileNameWithoutExtension;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MoxaE1200DigitalIORemoteDeviceParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public MoxaE1200DigitalIORemoteDeviceParameter(Common.Drawing.Point location) : this(location, "MoxaE1200")
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public MoxaE1200DigitalIORemoteDeviceParameter(Common.Drawing.Point location, string fileNameWithoutExtension) : base($"DigitalIO_Device_Y{location.Y + 1:D2}_X{location.X + 1:D2}", "DigitalIODevice", $"{fileNameWithoutExtension}_Y{location.Y + 1:D2}_X{location.X + 1:D2}.ini")
        {
            this.IsHierarchy = false;
            this.fileNameWithoutExtension = fileNameWithoutExtension;
            this.location = location.Clone();
        }
    }
}