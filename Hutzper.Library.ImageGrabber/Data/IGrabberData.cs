using Hutzper.Library.Common;
using System.Drawing;

namespace Hutzper.Library.ImageGrabber.Data
{
    /// <summary>
    /// 画像取得データ
    /// </summary>
    public interface IGrabberData : ISafelyDisposable
    {
        /// <summary>
        /// 識別
        /// </summary>
        public Common.Drawing.Point Location { get; init; }

        /// <summary>
        /// 汎用カウンタ値
        /// </summary>
        public ulong Counter { get; set; }

        /// <summary>
        /// Bitmap変換
        /// </summary>
        /// <returns></returns>
        public Bitmap? ToBitmap();

        /// <summary>
        /// ピクセルフォーマット
        /// </summary>
        public System.Drawing.Imaging.PixelFormat PixelFormat { get; set; }

        /// <summary>
        /// サイズ
        /// </summary>
        public Hutzper.Library.Common.Drawing.Size Size { get; set; }

        /// <summary>
        /// ファイル読み込み
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public bool Read(FileInfo fileInfo);

        /// <summary>
        /// ファイル書き込み
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public bool Write(FileInfo fileInfo);

        /// <summary>
        /// コピー(DeepCopy)
        /// </summary>
        /// <returns></returns>
        public IGrabberData? Clone();
    }
}