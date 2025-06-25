namespace Hutzper.Library.ImageGrabber.Device
{
    public class GrabberErrorBase : IGrabberError
    {
        #region IGrabberError

        /// <summary>
        /// 識別
        /// </summary>
        public Common.Drawing.Point Location { get; init; }

        /// <summary>
        /// 汎用カウンタ値
        /// </summary>
        public virtual ulong Counter { get; set; }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public virtual string ErrorMessage { get; set; } = string.Empty;

        #endregion

        public GrabberErrorBase(Common.Drawing.Point location)
        {
            this.Location = location.Clone();
        }
    }
}