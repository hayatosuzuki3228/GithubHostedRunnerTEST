using Hutzper.Library.Common;
using Hutzper.Library.Common.Drawing;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace Hutzper.Library.Forms
{
    /// <summary>
    /// ユーザーコントロールの基本クラスです。
    /// </summary>
    /// <remarks>ユーザーコントロールを作成する場合に継承します</remarks>
    public partial class HutzperUserControl : UserControl, ILoggable
    {
        #region プロパティ

        [Category("Custom Appearance")]
        public RoundedCorner RoundedCorner
        {
            get => this.roundedCorner;
            set
            {
                this.roundedCorner = value;
                this.Invalidate();
            }
        }

        [Category("Custom Appearance")]
        public double BorderRadius
        {
            get => this.borderRadius;
            set
            {
                this.borderRadius = (float)value;
                this.Invalidate();
            }
        }

        [Category("Custom Appearance")]
        public double BorderSize
        {
            get => this.borderSize;
            set
            {
                this.borderSize = (float)value;
                this.Invalidate();
            }
        }

        [Category("Custom Appearance")]
        public Color BorderColor
        {
            get => this.borderColor;
            set
            {
                this.borderColor = value;
                this.Invalidate();
            }
        }
        protected override bool DoubleBuffered { get => base.DoubleBuffered; set { } }

        /// <summary>
        /// 文字列全体を表示するフォントサイズに調整するか
        /// </summary>
        public virtual bool IsTextShrinkToFit
        {
            get => this.isTextShrinkToFit;

            set
            {
                this.isTextShrinkToFit = false;
                this.Invalidate();
            }
        }

        protected RoundedCorner roundedCorner;
        protected float borderRadius = 25f;
        protected float borderSize = 1f;
        protected Color borderColor = Color.Black;
        protected bool isTextShrinkToFit = false;
        protected float textSpecifiedFontSize;

        #endregion

        #region ILoggable

        /// <summary>
        /// インスタンスを識別する名前
        /// </summary>
        /// <remarks>NicknameとIndexプロパティから成る文字列</remarks>
        public string UniqueName
        {
            #region 取得
            get
            {
                if (0 <= this.Index)
                {
                    return string.Format("{0}{1:D2}", this.Nickname, this.Index + 1);
                }
                else
                {
                    return string.Format("{0}", this.Nickname);
                }
            }
            #endregion
        }

        /// <summary>
        /// 通称を取得、設定する
        /// </summary>
        public string Nickname
        {
            #region 取得
            get
            {
                var value = base.ToString() ?? string.Empty;

                if (false == string.IsNullOrEmpty(this.nickname))
                {
                    value = this.nickname;
                }

                return value;
            }
            #endregion

            #region 更新
            set
            {
                this.nickname = value;
            }
            #endregion
        }
        protected string nickname = String.Empty;

        /// <summary>
        /// 整数インデックス
        /// </summary>
        public int Index { get; set; } = -1;

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="logger"></param>
        public void Attach(string nickname, int index)
        {
            try
            {
                this.nickname = nickname;
                this.Index = index;
            }
            catch
            {
            }
        }

        #endregion

        #region リソースの破棄

        protected virtual void DisposeImplicit() { }

        #endregion

        public HutzperUserControl()
        {
            this.InitializeComponent();

            this.AutoScaleMode = AutoScaleMode.None;

            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.roundedCorner = RoundedCorner.All;
            this.Resize += this.Control_Resize;

            this.textSpecifiedFontSize = this.Font.Size;
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
                    using var penSurface = new Pen(this.Parent?.BackColor ?? this.BackColor, 2);
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

            if (this.Parent != null)
            {
                this.Parent.BackColorChanged += new EventHandler(this.Container_BackColorChanged);
            }
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