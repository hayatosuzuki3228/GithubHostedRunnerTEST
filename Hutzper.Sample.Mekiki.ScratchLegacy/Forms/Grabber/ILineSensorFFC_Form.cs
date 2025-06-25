using Hutzper.Library.ImageGrabber.Device;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Grabber
{
    /// <summary>
    /// ラインセンサ FFC画面インタフェース
    /// </summary>
    public interface ILineSensorFFC_Form
    {
        /// <summary>
        /// カメラ状態を問い合わせるイベント
        /// </summary>
        public event EventHandler<GrabberStatusEventArgs> GrabberStatusRequested;

        /// <summary>
        /// 指定したデバイスのFFCをサポートしているかどうかを取得します
        /// </summary>
        /// <param name="device">チェック対象のデバイス</param>
        /// <returns>サポートしている場合 true</returns>
        public bool IsFFCSupported(ILineSensor device);

        /// <summary>
        /// FFCを実行するデバイスを登録して初期化します。
        /// </summary>
        /// <param name="device"></param>
        public void Initialize(params ILineSensor[] device);

        /// <summary>
        /// 画面の取得
        /// </summary>
        public Form? GetForm();
    }

    public class GrabberStatusEventArgs : EventArgs
    {
        public bool IsLive { get; set; }
    }
}
