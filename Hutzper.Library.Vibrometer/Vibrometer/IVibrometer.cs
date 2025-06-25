using Hutzper.Library.Common.Controller;
using Hutzper.Library.Vibrometer.Data;

namespace Hutzper.Library.Vibrometer.Vibrometer
{
    /// <summary>
    /// 振動計インタフェース
    /// </summary>
    public interface IVibrometer : IController
    {
        /// <summary>
        /// 有効かどうか
        /// </summary>
        public bool Enabled { get; }

        /// <summary>
        /// 装置停止フラグ
        /// </summary>
        public bool StopDeviceFlag { get; set; }

        /// <summary>
        /// 計測開始
        /// </summary>
        public void Start();

        /// <summary>
        /// 計測停止
        /// </summary>
        public void Stop();

        /// <summary>
        /// イベント:データ受信
        /// </summary>
        public event Action<object, IVibrometerResult> DataReceived;

        /// <summary>
        /// イベント:エラー
        /// </summary>
        public event Action<object, IVibrometerErrorInfo> Error;
    }
}