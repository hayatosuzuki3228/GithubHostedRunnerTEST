namespace Hutzper.Library.Common.Forms
{
    public class WatermarkTextbox : TextBox
    {
        /// <summary>ウォーターマーク内部値</summary>
        private string _watermarkText = string.Empty;

        /// <summary>ウォーターマーク</summary>
        public string WatermarkText
        {
            get
            {
                return _watermarkText;
            }
            set
            {
                _watermarkText = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// ウォーターマーク設定ありの場合に描画します
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            const int WM_PAINT = 0x000F;
            const int WM_LBUTTONDOWN = 0x0201;

            if (
                (m.Msg == WM_PAINT || m.Msg == WM_LBUTTONDOWN)
            && this.Enabled
            && string.IsNullOrEmpty(this.Text)
            && !string.IsNullOrEmpty(this.WatermarkText)
            )
            {
                using var g = Graphics.FromHwnd(this.Handle);

                var rect = this.ClientRectangle;
                rect.Offset(1, 1);

                TextRenderer.DrawText(g, this.WatermarkText, this.Font, rect, Color.LightGray, TextFormatFlags.Top | TextFormatFlags.Left);
            }
        }
    }
}