namespace Hutzper.Library.ImageGrabber
{
    /// <summary>
    /// ラインセンサ制御インタフェース
    /// </summary>
    public interface ILineSensorController : IGrabberController
    {
        /// <summary>
        /// 連結画像取得
        /// </summary>
        /// <returns></returns>
        /// <remarks>指定長の連結画像を取得します</remarks>
        public bool ConcatGrab(int totalHeight);

        /// <summary>
        /// 連結画像取得
        /// </summary>
        /// <remarks>画像枚数, 1枚の画像サイズ</remarks>
        /// <returns></returns>
        public bool ConcatGrabContinuously(int number = -1, int totalHeight = -1);

    }
}