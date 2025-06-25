namespace Hutzper.Library.ImageGrabber.Device
{
    /// <summary>
    /// 画像取得エラー
    /// </summary>
    public interface IGrabberError
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
        /// エラーメッセージ
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}