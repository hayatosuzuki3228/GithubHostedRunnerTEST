using Hutzper.Library.Common.Laungage;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;

namespace Hutzper.Library.Common.Forms
{
    /// <summary>
    /// Hutzper基本フォーム
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>直接継承しないでください。HutzperFormを継承してください</remarks>
    public partial class HutzperBaseForm : Form
    {
        #region プロパティ

        [Category("Custom Appearance")]
        public int BorderRadius
        {
            get => this.borderRadius;
            set
            {
                this.borderRadius = System.Math.Min(this.Height / 2, value);

                this.Invalidate();
            }
        }

        [Category("Custom Appearance")]
        public int BorderSize
        {
            get => this.borderSize;
            set
            {
                this.borderSize = System.Math.Max(value, 2);
                this.Padding = new Padding(this.borderSize);

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
                this.BasePanelTitleBar.BackColor = this.borderColor;
                this.BaseLabelTitle.BackColor = this.borderColor;
                this.BaseButtonClose.BackColor = this.borderColor;
                this.BaseButtonMinimize.BackColor = this.borderColor;
                this.BaseButtonMaximize.BackColor = this.borderColor;
                base.BackColor = this.borderColor;

                this.Invalidate();
            }
        }

        [Category("Custom Appearance")]
        public bool TitleBarVisible
        {
            get => this.basePanelTitleBarVisible;
            set
            {
                this.BasePanelTitleBar.Visible = this.basePanelTitleBarVisible = value;
                this.Invalidate();
            }
        }

        [Category("Custom Appearance")]
        public Font TitleFont
        {
            get => this.BaseLabelTitle.Font;
            set
            {
                this.BaseLabelTitle.Font = value;

                var defaultSize = (Size?)this.BasePanelTitleBar.Tag;

                this.BaseLabelTitle.Size = new Size(defaultSize?.Width ?? 256, defaultSize?.Height ?? 256);

                if (this.BaseLabelTitle.Size.Height < this.BaseLabelTitle.Height)
                {
                    this.BaseLabelTitle.Height = this.BaseLabelTitle.Size.Height;
                }

                this.BaseLabelTitle.Top = (this.BasePanelTitleBar.Height - this.BaseLabelTitle.Height) / 2;

                this.Invalidate();
            }
        }

        [Category("Custom Appearance")]
        public override Color BackColor
        {
            get => this.BasePanelContainer.BackColor;
            set
            {
                this.BasePanelContainer.BackColor = value;
                this.Invalidate();
            }
        }

        [Category("Custom Appearance")]
        public Color TitleForeColor
        {
            get => this.BaseLabelTitle.ForeColor;
            set
            {
                this.BaseLabelTitle.ForeColor = value;
                this.Invalidate();
            }
        }


        [Category("Custom Appearance")]
        [AllowNull]
        public override string Text
        {
            get => base.Text;
            set
            {
                base.Text = value;
                this.BaseLabelTitle.Text = value;
            }
        }

        [Category("Custom Appearance")]
        [DefaultValue(true)]
        public bool MinimizeButtonVisible
        {
            get => this.baseButtonMinimizeVisible;
            set
            {
                this.BaseButtonMinimize.Visible = this.baseButtonMinimizeVisible = value;

                this.Invalidate();
            }
        }

        [Category("Custom Appearance")]
        [DefaultValue(true)]
        public bool MaximizeButtonVisible
        {
            get => this.baseButtonMaximizeVisible;
            set
            {
                this.BaseButtonMaximize.Visible = this.baseButtonMaximizeVisible = value;

                this.Invalidate();
            }
        }

        [Category("Custom Appearance")]
        [DefaultValue(true)]
        public bool CloseButtonVisible
        {
            get => this.baseButtonCloseVisible;
            set
            {
                this.BaseButtonClose.Visible = this.baseButtonCloseVisible = value;

                this.Invalidate();
            }
        }

        /// <summary>
        /// タイトルバーダブルクリック動作
        /// </summary>
        public TitleBarDoubleClickAction TitleBarDoubleClickAction { get; set; }

        /// <summary>
        /// タイトルバーダブルクリック時指定サイズ
        /// </summary>
        [Category("Custom Appearance")]
        public Size SpecifiedSize { get; set; }

        // ITranslator
        public ITranslator? Translator { get; protected set; }

        #endregion

        #region フィールド

        // IServiceCollection
        protected IServiceCollectionSharing? Services;

        // 翻訳ヘルパ
        protected TranslationHelper TranslationHelper;

        /// <summary>
        /// Enterイベントを登録されているコントロールのリスト
        /// </summary>
        protected List<Control> listEnterControl;

        protected int borderRadius = 30;
        protected int borderSize = 2;
        protected Color borderColor = Color.FromArgb(255, 192, 128);

        /// <summary>
        /// ドラッグ位置
        /// </summary>
        protected Hutzper.Library.Common.Drawing.Point? dragLocation;

        private readonly FormDragResizer dragResizer;

        protected bool baseButtonMinimizeVisible = true;
        protected bool baseButtonMaximizeVisible = true;
        protected bool baseButtonCloseVisible = true;
        protected bool basePanelTitleBarVisible = true;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HutzperBaseForm() : this(null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HutzperBaseForm(IServiceCollectionSharing? serviceCollectionSharing)
        {
            this.InitializeComponent();

            // DIサービスへの参照を保持
            this.Services = serviceCollectionSharing;

            // 翻訳への参照を取得
            this.Translator = this.Services?.GetTranslator();
            this.TranslationHelper = new TranslationHelper(this.Translator);

            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.AutoScaleMode = AutoScaleMode.None;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BorderSize = this.borderSize;
            this.BorderColor = this.borderColor;

            this.BaseButtonMinimize.Visible = this.baseButtonMinimizeVisible;
            this.BaseButtonMaximize.Visible = this.baseButtonMaximizeVisible;
            this.BaseButtonClose.Visible = this.baseButtonCloseVisible;
            this.BasePanelTitleBar.Visible = this.basePanelTitleBarVisible;

            this.BasePanelTitleBar.Tag = new Size(this.BasePanelTitleBar.Width, this.BasePanelTitleBar.Height);
            this.Font = new Font(this.Font.FontFamily, 16);
            this.TitleForeColor = SystemColors.ControlText;

            this.listEnterControl = new List<Control>();

            this.KeyPreview = true;

            this.dragResizer = new FormDragResizer(this, this.BasePanelContainer, ResizeDirection.Left | ResizeDirection.Right | ResizeDirection.Bottom);

            this.previousSize = this.Size;
        }

        #endregion

        #region GUIイベント

        /// <summary>
        /// フォーム表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HutzperBaseForm_Shown(object sender, EventArgs e)
        {
            try
            {
                this.TranslationHelper.TranslateControl(this);

                foreach (var control in Forms.ControlUtilities.GetAllControls(this))
                {
                    if (
                        (control is TextBox)
                    || (control is NumericUpDown)
                    )
                    {
                        this.listEnterControl.Add(control);

                        control.Enter += this.Control_Enter;
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// フォームクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HutzperBaseForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            try
            {
                foreach (var control in this.listEnterControl)
                {
                    control.Enter -= this.Control_Enter;
                }

                this.listEnterControl.Clear();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
        private void BaseButtonMaximize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            var availableRect = new Rectangle(Point.Empty, new Size(this.ClientRectangle.Width, this.BasePanelTitleBar.Height));

            if (availableRect.Contains(e.Location) && e.Button == MouseButtons.Left)
            {
                this.dragLocation = new Library.Common.Drawing.Point(e.Location);
                this.Invalidate();
            }
        }

        private void panelTitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (null != this.dragLocation)
            {
                this.Left += e.X - this.dragLocation.X;
                this.Top += e.Y - this.dragLocation.Y;
            }
        }
        private void panelTitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            this.dragLocation = null;
            this.Invalidate();
        }

        private void HutzperBaseForm_ResizeEnd(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void HutzperBaseForm_SizeChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void HutzperBaseForm_Activated(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void HutzperBaseForm_Paint(object sender, PaintEventArgs e)
        {
            this.FormRegionAndBorder(this, this.borderRadius, e.Graphics, this.borderColor, this.borderSize);
        }

        private void panelContainer_Paint(object sender, PaintEventArgs e)
        {
            this.ControlRegionAndBorder(this.BasePanelContainer, this.borderRadius - (this.borderSize / 2), e.Graphics, this.borderColor);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            if (this.dragResizer.IsResizing) return;

            // SMOOTH OUTER BORDER
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            var rectForm = this.ClientRectangle;
            var halfSize = new SizeF(rectForm.Width / 2f, rectForm.Height / 2f);
            var fbColors = this.GetFormBoundColors();

            // Top left
            this.DrawPath(rectForm, e.Graphics, fbColors.TopLeftColor);

            // Top Right
            var rectTopRight = new RectangleF(halfSize.Width, rectForm.Y, halfSize.Width, halfSize.Height);
            this.DrawPath(rectTopRight, e.Graphics, fbColors.TopRithgColor);

            // BottomLeft
            var rectBottomLeft = new RectangleF(rectForm.X, rectForm.Y + halfSize.Height, halfSize.Width, halfSize.Height);
            this.DrawPath(rectBottomLeft, e.Graphics, fbColors.BottomLeftColor);

            // BottomRight
            var rectBottomRight = new RectangleF(halfSize.Width, rectForm.Y + halfSize.Height, halfSize.Width, halfSize.Height);
            this.DrawPath(rectBottomRight, e.Graphics, fbColors.BottomRightColor);
        }

        /// <summary>
        /// アクティブコントロール取得
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        protected virtual Control GetActiveControl(Control control)
        {
            Control selectedControl = control;

            if (selectedControl is ContainerControl container)
            {
                selectedControl = this.GetActiveControl(container.ActiveControl!);
            }

            return selectedControl;
        }

        /// <summary>
        /// コントロールイベント:フォーカス入力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Control_Enter(object? sender, EventArgs e)
        {
            try
            {
                var control = sender as Control;

                if (control is TextBox)
                {
                    var textBox = control as TextBox;

                    textBox?.Select(0, textBox.Text.Length);
                }
                else if (control is NumericUpDown)
                {
                    var numericUpDown = control as NumericUpDown;

                    numericUpDown?.Select(0, numericUpDown.Value.ToString().Length);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 翻訳
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        protected virtual string Translate(params string[] sourceString) => this.Translator?.Translate(sourceString) ?? string.Join("", sourceString);

        /// <summary>
        /// 確認画面
        /// </summary>
        /// <returns></returns>
        protected virtual ConfirmationForm NewConfirmationForm() => this.Services?.ServiceProvider?.GetRequiredService<ConfirmationForm>() ?? new ConfirmationForm();

        protected virtual void DrawPath(RectangleF rect, Graphics graph, Color color)
        {
            using var roundPath = this.GetRoundedPath(rect, this.borderRadius);
            using var penBorder = new Pen(color, 3f);

            graph.DrawPath(penBorder, roundPath);
        }

        protected virtual GraphicsPath GetRoundedPath(RectangleF rect, float radius)
        {
            var path = new GraphicsPath();
            var curveSize = radius * 2f;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();

            return path;
        }

        protected virtual void FormRegionAndBorder(Form form, float radius, Graphics graph, Color borderColor, float bordesSize)
        {
            if (this.WindowState != FormWindowState.Minimized)
            {
                using var roundPath = this.GetRoundedPath(form.ClientRectangle, radius);
                using var penBorder = new Pen(borderColor, bordesSize);
                using var transform = new Matrix();

                graph.SmoothingMode = SmoothingMode.AntiAlias;
                form.Region = new Region(roundPath);
                if (borderSize >= 1)
                {
                    var rect = form.ClientRectangle;
                    var scaleX = 1.0f - ((borderSize + 1f) / (float)rect.Width);
                    var scaleY = 1.0f - ((borderSize + 1f) / (float)rect.Height);

                    transform.Scale(scaleX, scaleY);
                    transform.Translate(borderSize / 1.6f, borderSize / 1.6f);

                    graph.Transform = transform;
                    graph.DrawPath(penBorder, roundPath);
                }
            }
        }

        protected virtual void ControlRegionAndBorder(Control contorol, float radius, Graphics graph, Color borderColor)
        {
            using var roundPath = this.GetRoundedPath(contorol.ClientRectangle, radius);
            using var penBorder = new Pen(borderColor, 1f);

            graph.SmoothingMode = SmoothingMode.AntiAlias;
            contorol.Region = new Region(roundPath);
            graph.DrawPath(penBorder, roundPath);
        }

        protected struct FormBoundColors
        {
            public Color TopLeftColor;
            public Color TopRithgColor;
            public Color BottomLeftColor;
            public Color BottomRightColor;
        }

        protected virtual FormBoundColors GetFormBoundColors()
        {
            var fbColor = new FormBoundColors();

            using var bmp = new Bitmap(1, 1);
            using var graph = Graphics.FromImage(bmp);

            var rectBmp = new Rectangle(0, 0, 1, 1);

            // Top Left
            rectBmp.X = this.Bounds.X - 1;
            rectBmp.Y = this.Bounds.Y;
            graph.CopyFromScreen(rectBmp.Location, Point.Empty, rectBmp.Size);
            fbColor.TopLeftColor = bmp.GetPixel(0, 0);

            // Top Right
            rectBmp.X = this.Bounds.Right;
            rectBmp.Y = this.Bounds.Y - 1;
            graph.CopyFromScreen(rectBmp.Location, Point.Empty, rectBmp.Size);
            fbColor.TopRithgColor = bmp.GetPixel(0, 0);

            // Bottom Left
            rectBmp.X = this.Bounds.X - 1;
            rectBmp.Y = this.Bounds.Bottom;
            graph.CopyFromScreen(rectBmp.Location, Point.Empty, rectBmp.Size);
            fbColor.BottomLeftColor = bmp.GetPixel(0, 0);

            // Bottom Right
            rectBmp.X = this.Bounds.Right;
            rectBmp.Y = this.Bounds.Bottom;
            graph.CopyFromScreen(rectBmp.Location, Point.Empty, rectBmp.Size);
            fbColor.BottomRightColor = bmp.GetPixel(0, 0);

            return fbColor;
        }

        #endregion

        protected Size previousSize;

        private void panelTitleBar_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var availableRect = new Rectangle(Point.Empty, new Size(this.ClientRectangle.Width, this.BasePanelTitleBar.Height));

            if (availableRect.Contains(e.Location) && e.Button == MouseButtons.Left)
            {
                if (TitleBarDoubleClickAction.SpecifiedSize == this.TitleBarDoubleClickAction && !this.SpecifiedSize.Equals(Size.Empty))
                {
                    if (this.Size.Equals(this.SpecifiedSize))
                    {
                        this.Size = this.previousSize;
                    }
                    else
                    {
                        this.previousSize = this.Size;
                        this.Size = this.SpecifiedSize;
                    }

                    this.Left = (Screen.PrimaryScreen!.WorkingArea.Width - this.Size.Width) / 2;
                    this.Top = (Screen.PrimaryScreen!.WorkingArea.Height - this.Size.Height) / 2;
                }
                else
                {
                    if (FormWindowState.Maximized == this.WindowState)
                        this.WindowState = FormWindowState.Normal;
                    else
                        this.WindowState = FormWindowState.Maximized;
                }
            }
        }

    }

    [Serializable]
    public enum TitleBarDoubleClickAction
    {
        Maximization,
        SpecifiedSize,
    }
}