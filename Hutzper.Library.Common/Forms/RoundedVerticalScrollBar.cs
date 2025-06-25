using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using Point = Hutzper.Library.Common.Drawing.Point;

namespace Hutzper.Library.Common.Forms
{
    /// <summary>
    /// 水平スクロールバー
    /// </summary>
    public class RoundedVerticalScrollBar : DoubleBufferedPanel
    {
        #region プロパティ

        [Category("Vertical ScrollBar")]
        public int Minimum
        {
            get => this.minimumValue;
            set
            {
                this.minimumValue = Math.Min(this.maximumValue, value);

                if (this.currentValue < this.minimumValue)
                {
                    this.currentValue = this.minimumValue;
                    this.OnValueChanged();
                }

                this.UdpateScrollInfo();
            }
        }

        [Category("Vertical ScrollBar")]
        public int Maximum
        {
            get => this.maximumValue;
            set
            {
                this.maximumValue = Math.Max(this.minimumValue, value);

                if (this.currentValue > this.maximumValue)
                {
                    this.currentValue = this.maximumValue;
                    this.OnValueChanged();
                }

                this.UdpateScrollInfo();
            }
        }

        [Category("Vertical ScrollBar")]
        public int Value
        {
            get => this.currentValue;
            set
            {
                var previousValue = this.currentValue;

                this.currentValue = Math.Max(this.minimumValue, Math.Min(this.maximumValue - this.largeChange + 1, value));

                if (false == previousValue.Equals(this.currentValue))
                {
                    this.OnValueChanged();
                }

                this.UdpateScrollInfo();
            }
        }

        [Category("Vertical ScrollBar")]
        public int LargeChange
        {
            get => this.largeChange;
            set
            {
                this.largeChange = Math.Min(this.maximumValue - this.minimumValue, Math.Max(this.smallChange, value));
                this.UdpateScrollInfo();
            }
        }

        [Category("Vertical ScrollBar")]
        public int SmallChange
        {
            get => this.smallChange;
            set
            {
                this.smallChange = Math.Max(0, Math.Min(this.largeChange, value));
                this.UdpateScrollInfo();
            }
        }

        [Category("Vertical ScrollBar")]
        public Color SliderInnerColor
        {
            get => this.sliderInnerColor;
            set
            {
                this.sliderInnerColor = value;
                this.Invalidate();
            }
        }

        [Category("Vertical ScrollBar")]
        public Color SliderBorderColor
        {
            get => this.sliderBorderColor;
            set
            {
                this.sliderBorderColor = value;
                this.Invalidate();
            }
        }

        [Category("Vertical ScrollBar")]
        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                this.Invalidate();
            }
        }

        #endregion

        #region イベント

        /// <summary>
        /// 値変化
        /// </summary>
        public event EventHandler? ValueChanged;

        #endregion

        #region フィールド

        protected int minimumValue = 0;
        protected int maximumValue = 100;
        protected int currentValue = 0;
        protected int largeChange = 10;
        protected int smallChange = 1;
        protected Point? sliderDragPoint;
        protected int sliderPosition;
        protected int sliderHeight;
        protected Color sliderInnerColor = Color.Gray;
        protected Color sliderBorderColor = Color.Gainsboro;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RoundedVerticalScrollBar() : base()
        {
            this.MinimumSize = new System.Drawing.Size(24, 80);
            this.Size = new System.Drawing.Size(32, 128);

            this.MouseDown += this.RoundedVerticalScrollBar_MouseDown;
            this.MouseUp += this.RoundedVerticalScrollBar_MouseUp;
            this.MouseMove += this.RoundedVerticalScrollBar_MouseMove;
            this.Resize += this.RoundedVerticalScrollBar_Resize;

            this.UdpateScrollInfo(false);

            base.BackColor = Color.Gray;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// スクロール情報更新
        /// </summary>
        /// <param name="isInvalidate"></param>
        protected void UdpateScrollInfo(bool isInvalidate = true)
        {
            try
            {
                var backRect = new RectangleF(this.ClientRectangle.Location, this.ClientRectangle.Size);

                var valuePerDot = (this.maximumValue - this.minimumValue + 1) / backRect.Height;

                this.sliderHeight = Convert.ToInt32(Math.Max(backRect.Width, this.largeChange / valuePerDot));
                this.sliderPosition = Convert.ToInt32(Math.Min(backRect.Bottom - this.sliderHeight, backRect.Top + this.currentValue / valuePerDot));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }

            if (true == isInvalidate)
            {
                this.Invalidate();
            }
        }

        /// <summary>
        /// GraphicsPath取得
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="arcSize"></param>
        /// <returns></returns>
        protected virtual GraphicsPath GetGraphicsPath(RectangleF rect, float arcSize)
        {
            var arcTop = new RectangleF(rect.X, rect.Y, arcSize, arcSize);
            var arcBottom = new RectangleF(rect.X, rect.Y + rect.Height - arcSize, arcSize, arcSize);

            var path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(arcTop, 180, 180);
            path.AddArc(arcBottom, 0, 180);
            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// ValueChanged通知
        /// </summary>
        protected virtual void OnValueChanged()
        {
            try
            {
                this.ValueChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
        }

        #endregion

        #region GUIイベント

        private void RoundedVerticalScrollBar_Resize(object? sender, EventArgs e)
        {
            this.UdpateScrollInfo();
        }

        private void RoundedVerticalScrollBar_MouseMove(object? sender, MouseEventArgs e)
        {
            // スライダードラッグ中
            if (e.Button == MouseButtons.Left && null != this.sliderDragPoint)
            {
                // 移動量の算出
                var movePoint = new Point(e.Location);
                movePoint.X -= this.sliderDragPoint.X;
                movePoint.Y -= this.sliderDragPoint.Y;

                // スライダー位置算出
                var backRect = new RectangleF(this.ClientRectangle.Location, this.ClientRectangle.Size);
                this.sliderPosition = Convert.ToInt32(Math.Max(backRect.Top, Math.Min(backRect.Bottom - this.sliderHeight, this.sliderPosition + movePoint.Y)));

                // ドラッグ座標更新
                this.sliderDragPoint = new Point(e.Location);

                var previousValue = this.currentValue;

                // スクロールバー値の更新
                if (this.sliderPosition == backRect.Top)
                {
                    this.currentValue = this.minimumValue;
                }
                else if (this.sliderPosition == (backRect.Bottom - this.sliderHeight))
                {
                    this.currentValue = this.maximumValue - this.largeChange + 1;
                }
                else
                {
                    var valuePerDot = (this.maximumValue - this.minimumValue + 1) / backRect.Height;
                    var valueTemp = (this.sliderPosition - backRect.Top) * valuePerDot;
                    this.currentValue = Convert.ToInt32(Math.Max(this.minimumValue, Math.Min(this.maximumValue - this.largeChange + 1, valueTemp)));
                }

                // イベント通知
                if (false == previousValue.Equals(this.currentValue))
                {
                    this.OnValueChanged();
                }

                this.Invalidate();
            }
        }

        private void RoundedVerticalScrollBar_MouseUp(object? sender, MouseEventArgs e)
        {
            // スライダードラッグ終了
            this.sliderDragPoint = null;
            this.Invalidate();
        }

        private void RoundedVerticalScrollBar_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.Enabled)
            {
                var backRect = new RectangleF(this.ClientRectangle.Location, this.ClientRectangle.Size);
                var sliderRect = new RectangleF(backRect.Left, (float)this.sliderPosition + backRect.Top, backRect.Width, this.sliderHeight);

                // スライダードラッグ
                if (sliderRect.Contains(e.Location))
                {
                    this.sliderDragPoint = new Point(e.Location);

                    this.Invalidate();
                }
                // LargeChange動作
                else
                {
                    var previousValue = this.currentValue;

                    if (sliderRect.Top > e.Location.Y)
                    {
                        this.currentValue = Math.Max(this.minimumValue, this.currentValue - this.largeChange);
                    }
                    else
                    {
                        this.currentValue = Math.Min(this.maximumValue - this.largeChange + 1, this.currentValue + this.largeChange);
                    }

                    // イベント通知
                    if (false == previousValue.Equals(this.currentValue))
                    {
                        this.OnValueChanged();
                    }

                    this.UdpateScrollInfo();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            try
            {
                base.OnPaint(pevent);

                // 背景クリア
                pevent.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                pevent.Graphics.Clear(this.Parent!.BackColor);

                // スライダー枠描画
                var backRect = new RectangleF(pevent.ClipRectangle.Location, pevent.ClipRectangle.Size);
                using var backPath = this.GetGraphicsPath(backRect, backRect.Width);
                using var backBrush = new SolidBrush(this.BackColor);
                pevent.Graphics.FillPath(backBrush, backPath);

                // スライダー描画
                var sliderRect = new RectangleF(backRect.Left, backRect.Top + (float)this.sliderPosition, backRect.Width - 1, this.sliderHeight - 1);
                using var sliderPath = this.GetGraphicsPath(sliderRect, sliderRect.Width);
                using var sliderBrush = new SolidBrush(this.sliderInnerColor);
                using var sliderPen = new Pen(this.sliderBorderColor, 2f);
                pevent.Graphics.FillPath(sliderBrush, sliderPath);
                pevent.Graphics.DrawPath(sliderPen, sliderPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
        }
    }

    #endregion
}