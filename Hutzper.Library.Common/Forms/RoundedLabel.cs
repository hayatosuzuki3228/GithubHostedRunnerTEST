using Hutzper.Library.Common.Drawing;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using Label = System.Windows.Forms.Label;

namespace Hutzper.Library.Common.Forms
{
    public class RoundedLabel : Label, IMarqueeText
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

        [Category("Custom Appearance")]
        public Color MouseOverEmphasizeBorderColor
        {
            get => this.mouseOverEmphasizeBorderColor;
            set => this.mouseOverEmphasizeBorderColor = value;
        }

        [Category("Custom Appearance")]
        public Color MouseOverEmphasizeBackgroundColor
        {
            get => this.mouseOverEmphasizeBackgroundColor;
            set => this.mouseOverEmphasizeBackgroundColor = value;
        }

        [Category("Custom Appearance")]
        public bool MouseOverEmphasize { get; set; }

        [Category("Custom Appearance")]
        public bool MarqueeTextEnabled { get; protected set; }

        #endregion

        #region フィールド

        private int borderSize = 2;
        private int borderRadius = 25;
        private Color borderColor = SystemColors.ControlDarkDark;

        private Color mouseOverEmphasizeBorderColor = Color.FromArgb(0, 120, 212);
        private Color mouseOverEmphasizeBackgroundColor = Color.FromArgb(224, 238, 249);

        private PointF MarqueeTextLocation;

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RoundedLabel()
        {
            this.FlatStyle = FlatStyle.Flat;
            this.BorderStyle = BorderStyle.None;
            this.BackColor = SystemColors.Control;
            this.ForeColor = SystemColors.ControlText;
            this.TextAlign = ContentAlignment.MiddleCenter;
            this.Resize += this.Control_Resize;
        }

        public void Attach(MarqueeTimingProvider provider)
        {
            this.MarqueeTextEnabled = true;
            this.MarqueeTextLocation = ControlUtilities.GetLocation(this.TextAlign, this.ClientRectangle, TextRenderer.MeasureText(this.Text ?? "dumyy", this.Font, this.ClientRectangle.Size, TextFormatFlags.NoPadding));

            provider.Attach(this);
        }

        public void Detach(MarqueeTimingProvider provider)
        {
            this.MarqueeTextEnabled = false;

            provider.Detach(this);

            this.Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            try
            {
                base.OnPaint(pevent);

                pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                var rectClient = new RectangleD(new PointD(), new SizeD(this.Size));

                rectClient.Inflate(-1, -1);
                var rectSurface = rectClient.ToRectangleF();

                rectClient.Size.Width -= 2.0;
                rectClient.Size.Height -= 2.0;
                var rectBorder = rectClient.ToRectangleF();

                using var pathBorder = GraphicsUtilities.CreateRound(rectBorder, RoundedCorner.All, this.borderRadius);

                var textRectangle = new Drawing.RectangleD(rectBorder);
                var textLocation = ControlUtilities.GetLocation(this.TextAlign, textRectangle.ToRectangleF(), pevent.Graphics.MeasureString(this.Text, this.Font));

                var isEmphasize = this.MouseOverEmphasize && this.ClientRectangle.Contains(this.PointToClient(Cursor.Position));

                #region MarqueeText位置調整
                if (true == this.MarqueeTextEnabled && false == string.IsNullOrEmpty(this.Text))
                {
                    var currentLocation = this.MarqueeTextLocation;

                    var sizeTotal = pevent.Graphics.MeasureString(this.Text, this.Font);
                    var sizeTop = pevent.Graphics.MeasureString(this.Text.First().ToString(), this.Font);

                    var x = currentLocation.X;
                    foreach (var c in this.Text.ToArray())
                    {
                        var sizeTemp = pevent.Graphics.MeasureString(c.ToString(), this.Font);
                        if (x + sizeTemp.Width > 0)
                        {
                            sizeTop = sizeTemp;
                            break;
                        }
                        x += sizeTemp.Width;
                    }

                    var alignmentLocation = ControlUtilities.GetLocation(this.TextAlign, textRectangle.ToRectangleF(), sizeTotal);

                    currentLocation.X -= sizeTop.Width;
                    currentLocation.Y = alignmentLocation.Y;

                    textLocation = currentLocation;

                    if (currentLocation.X + sizeTotal.Width < 0f)
                    {
                        currentLocation.X = pevent.ClipRectangle.Width - sizeTop.Width;
                    }

                    this.MarqueeTextLocation = currentLocation;
                }
                #endregion

                // raounded label
                if (this.borderRadius > 2)
                {
                    using var pathSurface = GraphicsUtilities.CreateRound(rectSurface, RoundedCorner.All, this.borderRadius);
                    using var penSurface = new Pen(this.Parent!.BackColor, 2);
                    using var penbBorder = new Pen(isEmphasize ? this.mouseOverEmphasizeBorderColor : this.borderColor, this.borderSize);

                    penbBorder.Alignment = PenAlignment.Inset;

                    this.Region = new Region(pathSurface);

                    if (this.Image is not null)
                    {
                        pevent.Graphics.DrawImage(this.Image, rectSurface);
                    }
                    else
                    {
                        using var innerBrush = new SolidBrush(isEmphasize ? this.mouseOverEmphasizeBackgroundColor : this.BackgroundColor);
                        pevent.Graphics.FillPath(innerBrush, pathBorder);
                    }

                    using var textBrush = new SolidBrush(this.ForeColor);
                    pevent.Graphics.DrawString(this.Text, this.Font, textBrush, textLocation);

                    pevent.Graphics.DrawPath(penSurface, pathSurface);

                    if (this.borderSize >= 1)
                    {
                        pevent.Graphics.DrawPath(penbBorder, pathBorder);
                    }
                }
                // normal label
                else
                {
                    this.Region = new Region(rectSurface);

                    if (this.Image is not null)
                    {
                        pevent.Graphics.DrawImage(this.Image, rectSurface);
                    }
                    else
                    {
                        using var innerBrush = new SolidBrush(isEmphasize ? this.mouseOverEmphasizeBackgroundColor : this.BackgroundColor);
                        pevent.Graphics.FillPath(innerBrush, pathBorder);
                    }

                    using var textBrush = new SolidBrush(this.ForeColor);
                    pevent.Graphics.DrawString(this.Text, this.Font, textBrush, textLocation);

                    if (this.borderSize >= 1)
                    {
                        using var penBorder = new Pen(isEmphasize ? this.mouseOverEmphasizeBorderColor : this.borderColor, this.borderSize);

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