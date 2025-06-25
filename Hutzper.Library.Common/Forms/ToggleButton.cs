using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Runtime.Versioning;

namespace Hutzper.Library.Common.Forms
{
    /// <summary>
    /// トグルスイッチ外観のCheckBox
    /// </summary>
    public class ToggleButton : CheckBox
    {
        #region フィールド

        protected Color onBackColor = Color.MediumSlateBlue;
        protected Color onToggleColor = Color.WhiteSmoke;
        protected Color offBackColor = Color.Gray;
        protected Color offToggleColor = Color.Gainsboro;
        protected bool solidStyle = true;

        #endregion

        #region プロパティ

        /// <summary>
        /// トグルON時の背景色
        /// </summary>
        [Category("Toggle Appearance")]
        public Color OnBackColor { get => this.onBackColor; set { this.onBackColor = value; this.Invalidate(); } }

        /// <summary>
        /// トグルON字の前景色
        /// </summary>
        [Category("Toggle Appearance")]
        public Color OnToggleColor { get => this.onToggleColor; set { this.onToggleColor = value; this.Invalidate(); } }

        /// <summary>
        /// トグルOFF時の背景色
        /// </summary>
        [Category("Toggle Appearance")]
        public Color OffBackColor { get => this.offBackColor; set { this.offBackColor = value; this.Invalidate(); } }

        /// <summary>
        /// トグルOFF字の前景色
        /// </summary>
        [Category("Toggle Appearance")]
        public Color OffToggleColor { get => this.offToggleColor; set { this.offToggleColor = value; this.Invalidate(); } }

        /// <summary>
        /// 塗りつぶしスタイルを適用するかどうか
        /// </summary>
        [Category("Toggle Appearance")]
        [DefaultValue(true)]
        public bool SolidStyle { get => this.solidStyle; set { this.solidStyle = value; this.Invalidate(); } }

        public override string Text { get => base.Text; }

        #endregion

        #region コンストラクタ

        public ToggleButton()
        {
            this.MinimumSize = new Size(45, 22);
        }

        #endregion

        #region メソッド

        [SupportedOSPlatform("windows7.0")]
        protected virtual GraphicsPath GetGraphicsPath()
        {
            var arcSize = this.Height - 1;

            var leftArc = new Rectangle(0, 0, arcSize, arcSize);
            var rightArc = new Rectangle(this.Width - arcSize - 2, 0, arcSize, arcSize);

            var path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(leftArc, 90, 180);
            path.AddArc(rightArc, 270, 180);
            path.CloseFigure();

            return path;
        }

        #endregion

        #region イベント

        /// <summary>
        /// イベント:描画
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            try
            {
                var toggleSize = this.Height - 5;

                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                pevent.Graphics.Clear(this.Parent!.BackColor);

                using var backPath = this.GetGraphicsPath();

                if (this.Checked)
                {
                    if (this.solidStyle)
                    {
                        using var onBackBrush = new SolidBrush(this.onBackColor);
                        pevent.Graphics.FillPath(onBackBrush, backPath);
                    }
                    else
                    {
                        using var onBackPen = new Pen(this.onBackColor, 2);
                        pevent.Graphics.DrawPath(onBackPen, backPath);
                    }

                    using var toggleBrush = new SolidBrush(this.onToggleColor);
                    pevent.Graphics.FillEllipse(toggleBrush, new Rectangle(this.Width - this.Height + 1, 2, toggleSize, toggleSize));
                }
                else
                {
                    if (this.solidStyle)
                    {
                        using var offBackBrush = new SolidBrush(this.offBackColor);
                        pevent.Graphics.FillPath(offBackBrush, backPath);
                    }
                    else
                    {
                        using var offBackPen = new Pen(this.offBackColor, 2);
                        pevent.Graphics.DrawPath(offBackPen, backPath);
                    }

                    using var toggleBrush = new SolidBrush(this.offToggleColor);
                    pevent.Graphics.FillEllipse(toggleBrush, new Rectangle(2, 2, toggleSize, toggleSize));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }

        }

        #endregion
    }
}