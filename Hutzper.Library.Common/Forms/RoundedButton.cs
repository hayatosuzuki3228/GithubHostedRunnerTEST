using Hutzper.Library.Common.Drawing;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace Hutzper.Library.Common.Forms
{
    public class RoundedButton : Button
    {
        #region プロパティ

        [Category("Custom Appearance")]
        public int BorderSize { get => this.borderSize; set { this.borderSize = value; this.Invalidate(); } }

        [Category("Custom Appearance")]
        public int BorderRadius
        {
            get => borderRadius;
            set
            {
                if (value <= this.Height)
                    this.borderRadius = value;
                else
                    this.borderRadius = this.Height;

                this.Invalidate();
            }
        }

        [Category("Custom Appearance")]
        public Color BorderColor { get => this.borderColor; set { this.borderColor = value; this.Invalidate(); } }

        [Category("Custom Appearance")]
        public Color BackgroundColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }
        [Category("Custom Appearance")]

        public Color TextColor
        {
            get => base.ForeColor;
            set => base.ForeColor = value;
        }

        #endregion

        #region フィールド

        private int borderSize = 0;
        private int borderRadius = 25;
        private Color borderColor = Color.PaleVioletRed;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RoundedButton()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Size = new System.Drawing.Size(150, 40);
            this.BackColor = Color.MediumSlateBlue;
            this.ForeColor = Color.White;
            this.Resize += this.Control_Resize;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            try
            {
                base.OnPaint(pevent);

                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                var rectSurface = new RectangleF(0, 0, this.Width, this.Height);
                var rectBorder = new RectangleF(1, 1, this.Width - 0.8f, this.Height - 1);

                rectBorder.Inflate(-1, -1);

                // raounded button
                if (this.borderRadius > 2)
                {
                    using var pathSurface = GraphicsUtilities.CreateRound(rectSurface, RoundedCorner.All, this.borderRadius);
                    using var pathBorder = GraphicsUtilities.CreateRound(rectBorder, RoundedCorner.All, this.borderRadius);
                    using var penSurface = new Pen(this.Parent!.BackColor, 2);
                    using var penbBorder = new Pen(this.borderColor, this.borderSize);

                    penbBorder.Alignment = PenAlignment.Inset;

                    this.Region = new Region(pathSurface);
                    pevent.Graphics.DrawPath(penSurface, pathSurface);

                    if (this.borderSize >= 1)
                    {
                        pevent.Graphics.DrawPath(penbBorder, pathBorder);
                    }
                }
                // normal button
                else
                {
                    this.Region = new Region(rectSurface);

                    if (this.borderSize >= 1)
                    {
                        using var penBorder = new Pen(this.borderColor, this.borderSize);

                        penBorder.Alignment = PenAlignment.Inset;
                        pevent.Graphics.DrawRectangle(penBorder, 0, 0, this.Width - 1, this.Height - 1);
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }

        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.Parent!.BackColorChanged += new EventHandler(this.Container_BackColorChanged);
        }

        private void Container_BackColorChanged(object? sender, EventArgs e)
        {
            if (this.DesignMode)
                this.Invalidate();
        }

        private void Control_Resize(object? sender, EventArgs e)
        {
            if (this.borderRadius > this.Height)
            {
                this.BorderRadius = this.Height;
            }
        }
    }
}