namespace Hutzper.Library.LightController.Device.Net.Sockets
{
    /// <summary>
    /// TCP/IP通信 照明制御インタフェース
    /// </summary>
    public interface ITcpLightController : ILightController
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