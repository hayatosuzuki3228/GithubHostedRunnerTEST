namespace Hutzper.Library.Common.Forms
{
    /// <summary>
    /// ダブルバッファリングが有効なパネル
    /// </summary>
    /// <remarks>表示のちらつきを低減します。</remarks>
    public class DoubleBufferedPanel : Panel
    {
        protected override bool DoubleBuffered { get => base.DoubleBuffered; set { } }

        public DoubleBufferedPanel()
        : base()
        {
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }
    }
}