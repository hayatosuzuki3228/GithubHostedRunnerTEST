using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace Hutzper.Library.Common.Forms
{
    public class RichRadioButton : RadioButton
    {
        #region プロパティ

        [Category("Custom Appearance")]
        public Color CheckedColor
        {
            get => this.checkedColor;
            set
            {
                this.checkedColor = value;
                this.Invalidate();
            }
        }

        [Category("Custom Appearance")]
        public Color CheckedBorderColor
        {
            get => this.checkedBorderColor;
            set
            {
                this.checkedBorderColor = value;
                this.Invalidate();
            }
        }

        [Category("Custom Appearance")]
        public Color UnCheckedColor
        {
            get => this.unCheckedColor;
            set
            {
                this.unCheckedColor = value;
                this.Invalidate();
            }
        }

        [Category("Custom Appearance")]
        public Color UnCheckedBorderColor
        {
            get => this.unCheckedBorderColor;
            set
            {
                this.unCheckedBorderColor = value;
                this.Invalidate();
            }
        }

        #endregion

        #region フィールド

        private Color checkedColor = Color.MediumSlateBlue;
        private Color checkedBorderColor = Color.MediumSlateBlue;
        private Color unCheckedBorderColor = Color.Gray;
        private Color unCheckedColor = Color.Transparent;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RichRadioButton()
        {
            this.MinimumSize = new Size(8, 8);
        }

        #endregion

        #region GUIイベント

        protected override void OnPaint(PaintEventArgs pevent)
        {
            try
            {
                var graphics = pevent.Graphics;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                var rbBorderSize = Math.Max(20f, this.ClientRectangle.Height - 8f);
                var rbCheckSize = Math.Min(rbBorderSize - 8f, rbBorderSize * 0.85f);

                var rectRbBorder = new RectangleF()
                {
                    X = 0.0f,
                    Y = (this.Height - rbBorderSize) / 2f,
                    Width = rbBorderSize,
                    Height = rbBorderSize
                };

                var rectRbCheck = new RectangleF()
                {
                    X = rectRbBorder.X + ((rectRbBorder.Width - rbCheckSize) / 2f),
                    Y = (this.Height - rbCheckSize) / 2f,
                    Width = rbCheckSize,
                    Height = rbCheckSize,
                };

                // draw surface
                graphics.Clear(this.BackColor);

                var penBorderWidth = 1.6f;

                // draw radio button
                if (this.Checked)
                {
                    using var penBorder = new Pen(this.checkedBorderColor, penBorderWidth);
                    using var brushRbCheck = new SolidBrush(this.checkedColor);
                    graphics.DrawEllipse(penBorder, rectRbBorder);
                    graphics.FillEllipse(brushRbCheck, rectRbCheck);
                }
                else
                {
                    using var penBorder = new Pen(this.unCheckedBorderColor, penBorderWidth);
                    using var brushRbInner = new SolidBrush(this.unCheckedColor);
                    graphics.FillEllipse(brushRbInner, rectRbBorder);
                    graphics.DrawEllipse(penBorder, rectRbBorder);
                }

                // draw text
                using var brushText = new SolidBrush(this.ForeColor);

                graphics.DrawString(this.Text, this.Font, brushText,
                    rbBorderSize + 4f, (this.Height - TextRenderer.MeasureText(this.Text, this.Font).Height) / 2f);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            try
            {
                base.OnResize(e);
                if (this.AutoSize)
                {
                    var rbBorderSize = Math.Max(20f, this.ClientRectangle.Height - 8f);

                    this.Width = Convert.ToInt32(TextRenderer.MeasureText(this.Text, this.Font).Width + rbBorderSize + 8f);
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