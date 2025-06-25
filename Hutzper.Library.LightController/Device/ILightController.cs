using Hutzper.Library.Common.Controller;

namespace Hutzper.Library.LightController.Device
{
    /// <summary>
    /// 照明制御インタフェース
    /// </summary>
    public interface ILightController : IController
    {
        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceID { get; }

        /// <summary>
        /// 識別
        /// </summary>
        public Common.Drawing.Point Location { get; protected set; }

        /// <summary>
        /// 制御が可能な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public bool Enabled { get; }

        /// <summary>
        /// チャンネル
        /// </summary>
        public int Channel { get; set; }

        /// <summary>
        /// 無効状態に変化した
        /// </summary>
        public event Action<object>? Disabled;

        /// <summary>
        /// 点灯
        /// </summary>
        /// <returns></returns>
        public bool TurnOn();
        public bool TurnOn(int channel);

        /// <summary>
        /// 消灯
        /// </summary>
        /// <returns></returns>
        public bool TurnOff();
        public bool TurnOff(int channel);

        /// <summary>
        /// 調光
        /// </summary>
        public bool Modulate(double value);
        public bool Modulate(int channel, double value);

        /// <summary>
        /// 調光
        /// </summary>
        public Task<double> ModulationAsync { get; }

        /// <summary>
        /// 外部トリガー点灯時間変更
        /// </summary>
        public bool ChangeExternalTriggerStrobeTimeUs(int channel, int strobeTimeUs = 0);
        public bool ChangeExternalTriggerStrobeTimeUs(int strobeTimeUs = 0);

        /// <summary>
        /// 外部トリガー点灯時間
        /// </summary>
        public Task<int> ExternalTriggerStrobeTimeUsAsync { get; }
    }
}