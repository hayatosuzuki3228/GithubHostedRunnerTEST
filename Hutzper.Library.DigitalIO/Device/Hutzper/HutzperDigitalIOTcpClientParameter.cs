using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.DigitalIO.Device.Hutzper
{
    /// <summary>
    /// デバッグ用DIOパラメータ
    /// </summary>
    [Serializable]
    public record HutzperDigitalIOTcpClientParameter : ControllerParameterBaseRecord, IDigitalIORemoteDeviceParameter
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
        [IniKey(true, 0, 1, 2, 3)]
        public int[] InputChannels { get; set; } = new int[] { 0, 1, 2, 3 };

        /// <summary>
        /// 出力チャネル
        /// </summary>
        [IniKey(true, 0, 1, 2, 3, 4, 5, 6, 7)]
        public int[] OutputChannels { get; set; } = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };

        /// <summary>
        /// 入力フィルタリングミリ秒
        /// </summary>
        [IniKey(true, "")]
        public int[] FilteringTimeMsIntervals { get; set; } = Array.Empty<int>();

        #endregion

        #region IDigitalIORemoteDeviceParameter

        /// <summary>
        /// IPアドレス
        /// </summary>
        [IniKey(true, "127.0.0.1")]
        public string IpAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// ポート番号
        /// </summary>
        [IniKey(true, 50000)]
        public int PortNumber { get; set; } = 50000;

        /// <summary>
        /// 再接続
        /// </summary>
        [IniKey(true, false)]
        public bool IsReconnectable { get; set; }

        /// <summary>
        /// 再接続試行間隔
        /// </summary>
        [IniKey(true, 3)]
        public int ReconnectionAttemptsIntervalSec { get; set; } = 3;

        #endregion

        protected Common.Drawing.Point location = new();
        protected string fileNameWithoutExtension;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HutzperDigitalIOTcpClientParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public HutzperDigitalIOTcpClientParameter(Common.Drawing.Point location) : this(location, "HutzperDigitalIO")
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public HutzperDigitalIOTcpClientParameter(Common.Drawing.Point location, string fileNameWithoutExtension) : base($"DigitalIO_Device_Y{location.Y + 1:D2}_X{location.X + 1:D2}", "DigitalIODevice", $"{fileNameWithoutExtension}_Y{location.Y + 1:D2}_X{location.X + 1:D2}.ini")
        {
            this.IsHierarchy = false;
            this.fileNameWithoutExtension = fileNameWithoutExtension;
            this.location = location.Clone();
        }
    }
}