namespace Hutzper.Library.DigitalIO.Device
{
    /// <summary>
    /// リモートデバイス
    /// </summary>
    public interface IDigitalIORemoteDevice : IDigitalIODevice
    {
        /// <summary>
        /// 接続イベント
        /// </summary>
        public event Action<object>? Connected;

        /// <summary>
        /// 切断イベント
        /// </summary>
        public event Action<object>? Disconnected;

        /// <summary>
        /// 再接続
        /// </summary>
        public bool IsReconnectable { get; set; }

        /// <summary>
        /// 再接続試行間隔
        /// </summary>
        public int ReconnectionAttemptsIntervalSec { get; set; }
    }
}