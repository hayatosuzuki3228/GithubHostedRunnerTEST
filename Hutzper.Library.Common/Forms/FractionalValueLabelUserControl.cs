namespace Hutzper.Library.Common.Forms
{
    using Hutzper.Library.Common.Drawing;

    /// <summary>
    /// 現状は手抜き
    /// </summary>
    public partial class FractionalValueLabelUserControl : UserControl
    {
        public string TitleText
        {
            get => this.titleText;
            set
            {
                this.titleText = value;
                this.Invalidate();
            }
        }

        public double NumeratorValue
        {
            get => this.numeratorValue;
            set
            {
                this.numeratorValue = value;
                this.Invalidate();
            }
        }

        public double DenominatorValue
        {
            get => this.denominatorValue;
            set
            {
                this.denominatorValue = value;
                this.Invalidate();
            }
        }

        public string NumeratorFormat
        {
            get => this.numeratorFormat;
            set
            {
                this.numeratorFormat = value ?? String.Empty;
                this.Invalidate();
            }
        }

        public string DenominatorFormat
        {
            get => this.denominatorFormat;
            set
            {
                this.denominatorFormat = value ?? String.Empty;
                this.Invalidate();
            }
        }

        public Font NumeratorFont
        {
            get => this.numeratorFont;
            set
            {
                if (null != value)
                {
                    this.numeratorFont?.Dispose();
                    this.numeratorFont = new Font(value.FontFamily, value.Size);
                }
                this.Invalidate();
            }
        }

        public Font DenominatorFont
        {
            get => this.denominatorFont;
            set
            {
                if (null != value)
                {
                    this.denominatorFont?.Dispose();
                    this.denominatorFont = new Font(value.FontFamily, value.Size);
                }
                this.Invalidate();
            }
        }

        private string titleText = "タイトル";

        private double numeratorValue;

        private double denominatorValue;

        private string numeratorFormat = "F1";

        private string denominatorFormat = "F1";

        private Font numeratorFont;
        private Font denominatorFont;

        public FractionalValueLabelUserControl()
        {
            InitializeComponent();

            this.numeratorFont = new Font(this.Font.FontFamily, this.Font.Size);
            this.denominatorFont = new Font(this.Font.FontFamily, this.Font.Size);
        }

        private void FractionalValueLabelUserControl_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                var uc = (UserControl)sender;

                var g = e.Graphics;

                // 背景色で塗りつぶす
                g.FillRectangle(new SolidBrush(uc.BackColor), e.ClipRectangle);

                // 表示テキスト
                var textNumeratorValue = $"{this.NumeratorValue.ToString(this.NumeratorFormat)}";
                var textDenominatorValue = $"{this.DenominatorValue.ToString(this.DenominatorFormat)}";

                // 表示テキストサイズ
                var sizeNumeratorValue = g.MeasureString(textNumeratorValue, this.numeratorFont);
                var sizeDenominatorValue = g.MeasureString(textDenominatorValue, this.denominatorFont);

                // タイトル文字サイズ
                var titleTop = string.IsNullOrEmpty(this.TitleText) ? "T" : this.TitleText.First().ToString();
                var sizeTitle = g.MeasureString(titleTop, this.Font);
                g.DrawString(this.TitleText, this.Font, new SolidBrush(this.ForeColor), new PointF());

                // タイトル文字サイズを除いた矩形
                var rectangle = new RectangleD(PointD.New(), new(e.ClipRectangle.Width - sizeTitle.Width * 0.5, e.ClipRectangle.Height - sizeTitle.Height * 1.0));
                rectangle.Offset(sizeTitle.Width * 0.5, sizeTitle.Height * 1.0);

                //// 確認用:矩形表示
                //g.DrawRectangle(Pens.Black, rectangle.ToRectangle());

                // 分数線表示矩形
                var lineRect = rectangle.Clone();

                lineRect.Inflate(new SizeD(rectangle.Size.Width * -0.3, rectangle.Size.Height * -0.3));
                lineRect.ToPoints(out PointF[] points);
                g.DrawLine(new Pen(this.ForeColor, 3), new PointF(points.Last().X, points.Last().Y), new PointF(points.Skip(1).First().X, points.Skip(1).First().Y));

                lineRect.GetCenterPoint(out PointF centerPoint);
                var textPoint = new PointF(centerPoint.X - sizeNumeratorValue.Width, centerPoint.Y - sizeNumeratorValue.Height);
                g.DrawString(textNumeratorValue, this.numeratorFont, new SolidBrush(this.ForeColor), textPoint);

                textPoint = centerPoint;
                g.DrawString(textDenominatorValue, this.denominatorFont, new SolidBrush(this.ForeColor), textPoint);
            }
            catch
            {

            }
        }
    }
}