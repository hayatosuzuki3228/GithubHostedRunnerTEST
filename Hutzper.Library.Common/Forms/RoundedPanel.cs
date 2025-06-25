using Hutzper.Library.Common.Drawing;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace Hutzper.Library.Common.Forms
{
    public class RoundedPanel : DoubleBufferedPanel
    {
        public RoundedCorner RoundedCorner
        {
            get => this.roundedCorner;
            set
            {
                this.roundedCorner = value;
                this.Invalidate();
            }
        }

        public double BorderRadius
        {
            get => this.borderRadius;
            set
            {
                this.borderRadius = (float)value;
                this.Invalidate();
            }
        }

        public double BorderSize
        {
            get => this.borderSize;
            set
            {
                this.borderSize = (float)value;
                this.Invalidate();
            }
        }

        public Color BorderColor
        {
            get => this.borderColor;
            set
            {
                this.borderColor = value;
                this.Invalidate();
            }
        }

        protected RoundedCorner roundedCorner;
        protected float borderRadius = 25f;
        protected float borderSize = 1f;
        protected Color borderColor = Color.Black;

        public RoundedPanel()
        {
            this.roundedCorner = RoundedCorner.All;
            base.BackColor = Color.Transparent;
            this.Resize += this.Control_Resize;

            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            try
            {
                base.OnPaint(pevent);

                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                var rectClient = new RectangleD(new PointD(), new SizeD(this.Width, this.Height));

                var rectSurface = rectClient.ToRectangleF();

                rectClient.Inflate(-1, -1);
                var rectBorder = rectClient.ToRectangleF();

                // raounded button
                if (this.borderRadius > 2)
                {
                    using var pathSurface = GraphicsUtilities.CreateRound(rectSurface, this.roundedCorner, this.borderRadius);
                    using var pathBorder = GraphicsUtilities.CreateRound(rectBorder, this.roundedCorner, this.borderRadius);
                    using var penSurface = new Pen(this.Parent!.BackColor, 2);
                    using var penBorder = new Pen(this.borderColor, this.borderSize);

                    penBorder.Alignment = PenAlignment.Inset;

                    this.Region = new Region(pathSurface);
                    pevent.Graphics.DrawPath(penSurface, pathSurface);

                    if (this.borderSize >= 1)
                    {
                        pevent.Graphics.DrawPath(penBorder, pathBorder);
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