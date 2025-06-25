using Hutzper.Library.Common.Controller;
using Hutzper.Library.DigitalIO.Device;

namespace Hutzper.Library.DigitalIO
{
    /// <summary>
    /// デジタルIOデバイス制御
    /// </summary>
    public interface IDigitalIODeviceController : IController
    {
        /// <summary>
        /// 有効な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public bool Enabled { get; }

        /// <summary>
        /// 入力デバイス
        /// </summary>
        public int NumberOfInputDevice { get; }

        /// <summary>
        /// 出力デバイス
        /// </summary>
        public int NumberOfOutputDevice { get; }

        /// <summary>
        /// 監視中かどうか
        /// </summary>
        public bool IsMonitoring { get; }

        /// <summary>
        /// 全デバイス
        /// </summary>
        public List<IDigitalIODevice> AllDevices { get; init; }

        /// <summary>
        /// 入力デバイス
        /// </summary>
        public List<IDigitalIOInputDevice> InputDevices { get; }

        /// <summary>
        /// 出力デバイス
        /// </summary>
        public List<IDigitalIOOutputDevice> OutputDevices { get; }

        /// <summary>
        /// 無効状態に変化した
        /// </summary>
        public event Action<object, IDigitalIODevice>? Disabled;

        /// <summary>
        /// 入力変化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="device"></param>
        /// <param name="rising edge index"></param>
        /// <param name="falling edge index"></param>
        /// <param name="values"></param>
        public event Action<object, IDigitalIOInputDevice, int[], int[], bool[]>? InputChanged;

        /// <summary>
        /// 入力値を取得する
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public bool[] GetVolatileInputValues(IDigitalIOInputDevice device);

        /// <summary>
        /// 出力値を取得する
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public bool[] GetVolatileOutputValues(IDigitalIOInputDevice device);

        /// <summary>
        /// デバイス割り付け
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public bool Attach(params IDigitalIODevice[] devices);

        /// <summary>
        /// 監視を開始する
        /// </summary>
        /// <returns></returns>
        public bool StartMonitoring();

        /// <summary>
        /// 監視を終了する
        /// </summary>
        public void StopMonitoring();
    }
}