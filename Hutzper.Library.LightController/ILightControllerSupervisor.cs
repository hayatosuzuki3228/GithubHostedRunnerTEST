using Hutzper.Library.Common.Controller;
using Hutzper.Library.LightController.Device;

namespace Hutzper.Library.LightController
{
    /// <summary>
    /// 照明制御統括
    /// </summary>
    public interface ILightControllerSupervisor : IController
    {
        #region プロパティ

        /// <summary>
        /// 有効な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public bool Enabled { get; }

        /// <summary>
        /// デバイス数
        /// </summary>
        public int NumberOfDevice { get; }

        /// <summary>
        /// 全デバイス
        /// </summary>
        public List<ILightController> Devices { get; init; }

        #endregion

        #region イベント

        /// <summary>
        /// 無効状態に変化した
        /// </summary>
        public event Action<object, ILightController>? Disabled;

        #endregion

        #region メソッド

        /// <summary>
        /// デバイス割り付け
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public bool Attach(params ILightController[] devices);

        /// <summary>
        /// 点灯
        /// </summary>
        /// <returns></returns>
        public bool TurnOn();

        /// <summary>
        /// 消灯
        /// </summary>
        /// <returns></returns>
        public bool TurnOff();

        #endregion
    }
}