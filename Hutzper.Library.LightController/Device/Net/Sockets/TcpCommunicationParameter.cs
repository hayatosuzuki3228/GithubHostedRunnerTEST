using Hutzper.Library.Common.Attribute;

namespace Hutzper.Library.LightController.Device.Net.Sockets
{
    /// <summary>
    /// TCP通信制御パラメータ
    /// </summary>
    [Serializable]
    public record TcpCommunicationParameter : LightControllerParameterBase, ITcpCommunicationParameter
    {
        #region ITcpCommunicationParameter

        /// <summary>
        /// IPアドレス
        /// </summary>
        [IniKey(true, "")]
        public string IpAddress { get; set; } = string.Empty;

        /// <summary>
        /// ポート番号
        /// </summary>
        [IniKey(true, 0)]
        public int PortNumber { get; set; }

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
        /// コンストラクタ
        /// </summary>
        public TcpCommunicationParameter() : this(new Common.Drawing.Point())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="directoryName"></param>
        public TcpCommunicationParameter(Common.Drawing.Point location) : base(location, typeof(TcpCommunicationParameter).Name)
        {
        }
    }
}