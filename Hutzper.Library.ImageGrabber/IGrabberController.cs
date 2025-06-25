using Hutzper.Library.Common.Controller;
using Hutzper.Library.ImageGrabber.Data;
using Hutzper.Library.ImageGrabber.Device;

namespace Hutzper.Library.ImageGrabber
{
    /// <summary>
    /// 画像取得制御
    /// </summary>
    public interface IGrabberController : IController
    {
        /// <summary>
        /// 画像が取得可能な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public bool Enabled { get; }

        /// <summary>
        /// 画像取得タイプ
        /// </summary>
        public GrabberType GrabberType { get; init; }

        /// <summary>
        /// 画像取得インスタンス数
        /// </summary>
        public int NumberOfGrabber { get; }

        /// <summary>
        /// 画像取得インスタンスリスト
        /// </summary>
        public List<IGrabber> Grabbers { get; }

        /// <summary>
        /// エラーイベント
        /// </summary>
        public event Action<object, IGrabberError>? ErrorOccurred;

        /// <summary>
        /// データ取得イベント
        /// </summary>
        public event Action<object, IGrabberData>? DataGrabbed;

        /// <summary>
        /// グラバー無効
        /// </summary>
        public event Action<object, IGrabber>? GrabberDisabled;

        /// <summary>
        /// デバイス割り付け
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public bool Attach(params IGrabber[] grabbers);

        /// <summary>
        /// 画像取得
        /// </summary>
        /// <returns></returns>
        public bool Grab();

        /// <summary>
        /// 連続画像取得
        /// </summary>
        /// <returns></returns>
        public bool GrabContinuously(int number = -1);

        /// <summary>
        /// 連続撮影停止
        /// </summary>
        /// <returns></returns>
        public bool StopGrabbing();
    }
}