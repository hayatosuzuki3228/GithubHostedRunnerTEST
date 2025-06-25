using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.Drawing;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Point = Hutzper.Library.Common.Drawing.Point;
using Rectangle = Hutzper.Library.Common.Drawing.Rectangle;
using Size = Hutzper.Library.Common.Drawing.Size;

namespace Hutzper.Library.Forms.ImageProcessing
{
    /// <summary>
    /// ヒストグラムユーザーコントロール
    /// </summary>
    public partial class HistogramUserControl : HutzperUserControl
    {
        #region サブクラス

        /// <summary>
        /// チャネル
        /// </summary>
        [Serializable]
        public enum Channel
        {
            L,
            R,
            G,
            B,
        }

        /// <summary>
        /// グラフデータ
        /// </summary>
        [Serializable]
        public class Graph
        {
            #region プロパティ

            /// <summary>
            /// チャネルを使用するかどうか
            /// </summary>
            public bool Use
            {
                get => this.use && this.data != null;
                set
                {
                    var prev = this.use;

                    this.use = value;

                    if (prev != this.use)
                    {
                        this.Changed?.Invoke(this);
                    }
                }
            }

            /// <summary>
            /// チャネルを表示するかどうか
            /// </summary>
            public bool Visible
            {
                get => this.visible;
                set
                {
                    var prev = this.visible;

                    this.visible = value;

                    if (prev != this.visible)
                    {
                        this.Changed?.Invoke(this);
                    }
                }
            }

            /// <summary>
            /// 自身のチャネル
            /// </summary>
            public Channel Channel { get; init; }

            /// <summary>
            /// チャネル描画色
            /// </summary>
            public Color Color
            {
                get => this.color;
                set
                {
                    var prev = this.color;

                    this.color = value;

                    if (prev != this.color)
                    {
                        this.Changed?.Invoke(this);
                    }
                }
            }

            /// <summary>
            /// チャネルデータ
            /// </summary>
            public IHistogramData Data
            {
                get => this.data;
                set
                {
                    this.data = value;
                    this.Changed?.Invoke(this);
                }
            }

            #endregion

            #region イベント

            /// <summary>
            /// イベント:変更
            /// </summary>
            public event Action<object>? Changed;

            #endregion

            #region フィールド

            private bool use = false;
            private bool visible = false;
            private Color color;
            private IHistogramData data;

            #endregion

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="channel"></param>
            public Graph(Channel channel, Color color)
            {
                this.Channel = channel;
                this.color = color;
                this.data = new HistogramData();
                this.data.Histogram[0] = 320 * 240;
                this.data.Recalculate();
            }

            #endregion

            #region メソッド

            /// <summary>
            /// 更新
            /// </summary>
            public void Update()
            {
                this.Changed?.Invoke(this);
            }

            #endregion
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// グラフ設定
        /// </summary>
        [Browsable(false)]
        public Dictionary<Channel, Graph> Graphs
        {
            get
            {
                var items = new Dictionary<Channel, Graph>();

                foreach (var c in this.graphs.Values)
                {
                    items.Add(c.Channel, c);
                }

                return items;
            }
        }

        /// <summary>
        /// 背景色
        /// </summary>
        [Browsable(true)]
        [Category("ヒストグラム")]
        [Description("背景色です。")]
        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;

                if (null != this.renderer)
                {
                    this.renderer.BackColor = base.BackColor;
                    var backgroundRect = new Library.Common.Drawing.Rectangle(Point.New(), new Size(this.ClientRectangle.Size));
                    this.renderer.CopyFrom(backgroundRect, base.BackColor);

                    this.renderer.Draw();
                }
            }
        }

        /// <summary>
        /// カーソル色
        /// </summary>
        [Browsable(true)]
        [Category("ヒストグラム")]
        [Description("カーソル色です。")]
        public Color CursorColor
        {
            get => this.cursorColor;

            set
            {
                this.cursorColor = value;
                this.renderer?.Draw();
            }
        }

        /// <summary>
        /// グラフを疑似カラー表示するかどうか
        /// </summary>
        [Browsable(true)]
        [Category("ヒストグラム")]
        [Description("グラフを疑似カラー表示するかどうかを示します。")]
        public bool PseudColorEnabled
        {
            get => this.pseudColorEnabled;

            set
            {
                this.pseudColorEnabled = value;

                if (true == this.pseudColorEnabled)
                {
                    if (true == this.pseudColorInverted)
                    {
                        Array.Copy(HistogramUserControl.DefaultInvertedPseudColors, this.pseudColors, this.pseudColors.Length);
                    }
                    else
                    {
                        Array.Copy(HistogramUserControl.DefaultPseudColors, this.pseudColors, this.pseudColors.Length);
                    }
                }
                else
                {
                    this.pseudColors = Enumerable.Repeat(this.graphs[Channel.L].Color, this.pseudColors.Length).ToArray();
                }

                foreach (var blackValue in this.pseudColorInvalidValues)
                {
                    this.pseudColors[blackValue] = base.BackColor;
                }

                this.renderer?.Draw();
            }
        }

        /// <summary>
        /// 疑似カラーを反転させるかどうか
        /// </summary>
        [Browsable(true)]
        [Category("ヒストグラム")]
        [Description("疑似カラーを反転させるかどうかを示します。")]
        public bool PseudColorInverted
        {
            get => this.pseudColorInverted;

            set
            {
                this.pseudColorInverted = value;

                if (true == this.pseudColorEnabled)
                {
                    if (true == this.pseudColorInverted)
                    {
                        Array.Copy(HistogramUserControl.DefaultInvertedPseudColors, this.pseudColors, this.pseudColors.Length);
                    }
                    else
                    {
                        Array.Copy(HistogramUserControl.DefaultPseudColors, this.pseudColors, this.pseudColors.Length);
                    }
                }

                foreach (var blackValue in this.pseudColorInvalidValues)
                {
                    this.pseudColors[blackValue] = base.BackColor;
                }

                this.renderer?.Draw();
            }
        }

        /// <summary>
        /// 疑似カラー表示で背景色表示とする無効値の配列を指定します。
        /// </summary>
        public int[] PseudColorInvalidValues
        {
            get
            {
                var inalidValues = new int[this.pseudColorInvalidValues.Length];

                Array.Copy(this.pseudColorInvalidValues, inalidValues, inalidValues.Length);

                return inalidValues;
            }

            set
            {
                if (null == value)
                {
                    this.pseudColorInvalidValues = Array.Empty<int>();
                }
                else
                {
                    this.pseudColorInvalidValues = new int[value.Length];

                    Array.Copy(value, this.pseudColorInvalidValues, this.pseudColorInvalidValues.Length);
                }

                if (true == this.pseudColorEnabled)
                {
                    if (true == this.pseudColorInverted)
                    {
                        Array.Copy(HistogramUserControl.DefaultInvertedPseudColors, this.pseudColors, this.pseudColors.Length);
                    }
                    else
                    {
                        Array.Copy(HistogramUserControl.DefaultPseudColors, this.pseudColors, this.pseudColors.Length);
                    }
                }

                foreach (var blackValue in this.pseudColorInvalidValues)
                {
                    this.pseudColors[blackValue] = base.BackColor;
                }

                this.renderer?.Draw();
            }
        }

        /// <summary>
        /// カーソルを表示するかどうか
        /// </summary>
        [Browsable(true)]
        [Category("ヒストグラム")]
        [Description("カーソルを表示するかどうかを示します。")]
        public bool CursorOverlayEnalbed
        {
            get => this.cursorOverlayEnalbed;

            set
            {
                this.cursorOverlayEnalbed = value;
                this.renderer?.Draw();
            }
        }

        /// <summary>
        /// 水平方向範囲の最小値
        /// </summary>
        [Browsable(true)]
        [Category("ヒストグラム")]
        [Description("水平方向範囲の最小値を示します。")]
        public int HorizontalRangeMinimum
        {
            get => this.scaleRangeMinimum.X;

            set
            {
                this.scaleRangeMinimum.X = System.Math.Min(value, this.scaleRangeMaximum.X);
                this.scaleRangeMaximum.X = System.Math.Max(this.scaleRangeMinimum.X, this.scaleRangeMaximum.X);

                if (this.scaleRangeMinimum.X == this.scaleRangeMaximum.X)
                {
                    this.scaleRangeMaximum.X = this.scaleRangeMinimum.X + 1;
                }

                this.renderer?.Draw();
            }
        }

        /// <summary>
        /// 水平方向範囲の最大値
        /// </summary>
        [Browsable(true)]
        [Category("ヒストグラム")]
        [Description("水平方向範囲の最大値を示します。")]
        public int HorizontalRangeMaximum
        {
            get => this.scaleRangeMaximum.X;

            set
            {
                this.scaleRangeMaximum.X = System.Math.Max(value, this.scaleRangeMinimum.X);
                this.scaleRangeMinimum.X = System.Math.Min(this.scaleRangeMinimum.X, this.scaleRangeMaximum.X);

                if (this.scaleRangeMinimum.X == this.scaleRangeMaximum.X)
                {
                    this.scaleRangeMinimum.X = this.scaleRangeMaximum.X - 1;
                }

                this.renderer?.Draw();
            }
        }

        #endregion

        #region イベント

        /// <summary>
        /// イベント:描画
        /// </summary>
        public event Action<object, Renderer, Surface, Rectangle>? Drawing;

        #endregion

        #region スタティックフィールド

        // 初期値データ
        protected static readonly HistogramData DefaultData;
        protected static readonly Color[] DefaultPseudColors;
        protected static readonly Color[] DefaultInvertedPseudColors;

        private static readonly double[] lutOfConvRgbToGrayFromR;
        private static readonly double[] lutOfConvRgbToGrayFromG;
        private static readonly double[] lutOfConvRgbToGrayFromB;

        #endregion

        #region スタティックコンストラクタ

        /// <summary>
        /// スタティックコンストラクタ
        /// </summary>
        static HistogramUserControl()
        {
            #region 初期データ生成
            {
                HistogramUserControl.DefaultData = new();

                var rng = new GaussianDistributionRng();
                rng.Initialize(new GaussianDistributionRnp() { Mean = 128, StandardDeviation = 24 });

                foreach (var _ in Enumerable.Range(0, 160 * 120))
                {
                    var index = Convert.ToInt32(rng.NextDouble());

                    if ((0 <= index)
                    && (HistogramUserControl.DefaultData.Histogram.Length > index))
                    {
                        HistogramUserControl.DefaultData.Histogram[index]++;

                        HistogramUserControl.DefaultData.Sum += index;
                        HistogramUserControl.DefaultData.Num++;
                    }
                }

                HistogramUserControl.DefaultData.Recalculate();
            }
            #endregion

            #region 擬似カラーLUT作成
            {
                var palette = new Color[byte.MaxValue + 1];

                int step = palette.Length / 4;
                int slope = Convert.ToInt32((double)palette.Length / (double)step);
                int index = 0;

                for (int i = 0; i < step; i++)
                {
                    palette[index++] = Color.FromArgb(0, i * slope, 255);
                }
                for (int i = 0; i < step; i++)
                {
                    palette[index++] = Color.FromArgb(0, 255, 256 - (i + 1) * slope);
                }
                for (int i = 0; i < step; i++)
                {
                    palette[index++] = Color.FromArgb(i * slope, 255, 0);
                }
                for (int i = 0; i < step; i++)
                {
                    palette[index++] = Color.FromArgb(255, 256 - (i + 1) * slope, 0);
                }

                HistogramUserControl.DefaultPseudColors = palette;

                var invertedPallete = new Color[palette.Length];
                Array.Copy(palette, invertedPallete, invertedPallete.Length);
                Array.Reverse(invertedPallete);
                HistogramUserControl.DefaultInvertedPseudColors = invertedPallete;
            }
            #endregion

            #region RGB画像をグレイスケール画像変換
            {
                var lenghtOfLut = byte.MaxValue + 1;
                HistogramUserControl.lutOfConvRgbToGrayFromR = new double[lenghtOfLut];
                HistogramUserControl.lutOfConvRgbToGrayFromG = new double[lenghtOfLut];
                HistogramUserControl.lutOfConvRgbToGrayFromB = new double[lenghtOfLut];
                foreach (var i in Enumerable.Range(0, lenghtOfLut))
                {
                    HistogramUserControl.lutOfConvRgbToGrayFromR[i] = i * 0.299;
                    HistogramUserControl.lutOfConvRgbToGrayFromG[i] = i * 0.587;
                    HistogramUserControl.lutOfConvRgbToGrayFromB[i] = i * 0.114;
                }
            }
            #endregion
        }

        #endregion

        #region フィールド

        protected readonly Renderer renderer;

        protected readonly Dictionary<Channel, Graph> graphs = new();

        /// <summary>
        /// 描画色
        /// </summary>
        protected Color scaleColor;
        protected Color cursorColor;
        protected Color[] pseudColors;
        protected int[] pseudColorInvalidValues;

        /// <summary>
        /// 機能切替
        /// </summary>
        protected bool pseudColorEnabled;
        protected bool pseudColorInverted;
        protected bool cursorOverlayEnalbed;

        /// <summary>
        /// 表示範囲
        /// </summary>
        protected Point scaleRangeMinimum;
        protected Point scaleRangeMaximum;

        protected double verticalZoomRatio = 1.0;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HistogramUserControl()
        {
            this.InitializeComponent();

            this.graphs.Add(Channel.L, new Graph(Channel.L, Color.Lavender));
            this.graphs.Add(Channel.R, new Graph(Channel.R, Color.Red));
            this.graphs.Add(Channel.G, new Graph(Channel.G, Color.Green));
            this.graphs.Add(Channel.B, new Graph(Channel.B, Color.Blue));

            foreach (var g in this.graphs.Values)
            {
                g.Data = new HistogramData(HistogramUserControl.DefaultData);
                g.Changed += this.Channel_Changed;
            }
            this.graphs[Channel.L].Use = true;
            this.graphs[Channel.L].Visible = true;

            // 描画色を初期化する                        
            base.BackColor = Color.Black;
            this.scaleColor = Color.White;
            this.cursorColor = Color.LightGoldenrodYellow;

            // 疑似カラーテーブルを初期化する
            this.pseudColors = Enumerable.Repeat(this.graphs[Channel.L].Color, HistogramUserControl.DefaultPseudColors.Length).ToArray();
            this.pseudColorEnabled = false;
            this.pseudColorInvalidValues = Array.Empty<int>();

            // スケール範囲を初期化する
            this.scaleRangeMinimum = new Point(0, 0);
            this.scaleRangeMaximum = new Point(HistogramUserControl.DefaultData.Histogram.Length - 1, ushort.MaxValue);
            this.cursorOverlayEnalbed = true;

            this.channelOfGray.Tag = Channel.L;
            this.channelOfRed.Tag = Channel.R;
            this.channelOfGreen.Tag = Channel.G;
            this.channelOfBlue.Tag = Channel.B;

            this.renderer = new(this, new Size(this.ClientRectangle.Size))
            {
                MouseDragEnabled = false,
                MouseZoomEnabled = false,
            };
            this.renderer.Drawing += this.Renderer_Rendering;
            this.renderer.Resize += this.Renderer_Resize;
            this.renderer.MouseMove += this.Renderer_MouseMove;
            this.renderer.MouseDoubleClick += this.Renderer_MouseDoubleClick;
            this.renderer.MouseClick += this.Renderer_MouseClick;

            var backgroundRect = new Rectangle(Point.New(), new Size(this.ClientRectangle.Size));
            this.renderer.CopyFrom(backgroundRect, base.BackColor);

            this.renderer.Draw();

            this.MouseWheel += this.HistogramUserControl_MouseWheel;
        }

        private void Renderer_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.contextMenuStrip1.Show(this, e.Location);
            }
        }

        private void HistogramUserControl_MouseWheel(object? sender, MouseEventArgs e)
        {
            this.verticalZoomRatio += (e.Delta > 0) ? +1.0 : -1.0;

            this.verticalZoomRatio = Math.Max(this.verticalZoomRatio, 1.0);
            this.verticalZoomRatio = Math.Min(this.verticalZoomRatio, 100.1);

            this.renderer?.Draw();
        }


        #endregion

        #region リソースの破棄

        /// <summary>
        /// リソース破棄
        /// </summary>
        protected override void DisposeImplicit()
        {
            try
            {
                this.renderer?.Dispose();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// クリア
        /// </summary>
        public void Clear()
        {
            foreach (var g in this.graphs.Values)
            {
                if (null != g.Data)
                {
                    Array.Clear(g.Data.Histogram);

                    g.Data.Recalculate();
                }
            }

            this.renderer.Draw();
        }

        /// <summary>
        /// 表示更新
        /// </summary>
        public void Draw() => this.renderer.Draw();

        /// <summary>
        /// ヒストグラム設定
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="histogramData"></param>
        /// <remarks>指定されたヒストグラムデータをグラフデータとして設定します</remarks>
        public void Set(Channel channel, IHistogramData histogramData)
        {
            if (this.graphs.ContainsKey(channel))
            {
                var selectedGraph = this.graphs[channel];
                selectedGraph.Data.CopyFrom(histogramData);
                selectedGraph.Use = true;
            }
        }


        /// <summary>
        /// 画像設定
        /// </summary>
        /// <param name="bitmap"></param>
        /// <remarks>指定されたBitmapからヒストグラムデータを作成してグラフデータとして設定します</remarks>
        public void Set(Bitmap bitmap)
        {
            try
            {
                switch (bitmap.PixelFormat)
                {
                    case PixelFormat.Format8bppIndexed:
                        {
                            var histogram = this.CalcHistograms(bitmap);

                            this.graphs[Channel.L].Data = histogram[0];
                            this.graphs[Channel.L].Use = true;
                        }
                        break;

                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppRgb:
                    case PixelFormat.Format32bppArgb:
                        {
                            var histogram = this.CalcHistograms(bitmap);

                            var chIndex = new Dictionary<Channel, int>();
                            chIndex.Add(Channel.L, chIndex.Count);
                            chIndex.Add(Channel.R, chIndex.Count);
                            chIndex.Add(Channel.G, chIndex.Count);
                            chIndex.Add(Channel.B, chIndex.Count);

                            foreach (var chi in chIndex)
                            {
                                var i = chi.Value;
                                this.graphs[chi.Key].Data = histogram[i];
                                this.graphs[chi.Key].Use = true;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
            finally
            {
                this.renderer?.Draw();
            }
        }

        /// <summary>
        /// 描画用に調整された座標配列を取得する
        /// </summary>
        /// <param name="drawBeginPoint"></param>
        /// <param name="drawEndPoint"></param>
        /// <param name="number"></param>
        /// <param name="drawInterval"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        protected virtual int[] GetOptimizedDrawPoints(int drawBeginPoint, int drawEndPoint, int number, out int[] drawInterval, out double step)
        {
            var drawPoints = new int[number];
            var drawRange = Math.Abs(drawEndPoint - drawBeginPoint) + 1d;
            step = drawRange / number * ((drawBeginPoint < drawEndPoint) ? 1d : -1d);

            if (0 < drawPoints.Length)
            {
                drawPoints[0] = drawBeginPoint;
                drawInterval = new int[drawPoints.Length];

                var summary = drawPoints[0];
                var remain = 0d;
                double widthD;
                int widthI;

                for (int i = 1; i < drawPoints.Length; i++)
                {
                    widthD = step + remain;
                    widthI = (int)System.Math.Floor(widthD + 0.5);

                    remain = widthD - widthI;
                    summary += widthI;

                    drawPoints[i] = summary;
                    drawInterval[i - 1] = drawPoints[i] - drawPoints[i - 1];
                }

                widthD = step + remain;
                widthI = (int)System.Math.Floor(widthD + 0.5);

                summary += widthI;
                drawInterval[^1] = summary - drawPoints[^1];
            }
            else
            {
                drawInterval = Array.Empty<int>();
            }

            return drawPoints;
        }

        /// <summary>
        /// グレースケールおよびカラー画像のヒストグラムを計算します。
        /// </summary>
        /// <param name="bitmap">入力画像</param>
        /// <returns>ヒストグラムデータ(Gray, R, G, B の4チャンネル分または Gray のみ)</returns>
        public HistogramData[] CalcHistograms(Bitmap bitmap)
        {
            var histograms = Array.Empty<HistogramData>();

            using var src = BitmapConverter.ToMat(bitmap);

            // グレースケール画像の場合
            if (1 == src.Channels())
            {
                histograms = new HistogramData[1];
                histograms[0] = this.CalcHistogram(src);
            }
            // カラー画像の場合
            else if (3 <= src.Channels())
            {
                histograms = new HistogramData[4];

                using var gray = new Mat();
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

                var channels = Array.Empty<Mat>();
                Cv2.Split(src, out channels);

                histograms[0] = this.CalcHistogram(gray);
                histograms[1] = this.CalcHistogram(channels[2]);   // R チャンネル
                histograms[2] = this.CalcHistogram(channels[1]);   // G チャンネル
                histograms[3] = this.CalcHistogram(channels[0]);   // B チャンネル

                channels[0].Dispose();
                channels[1].Dispose();
                channels[2].Dispose();
            }

            return histograms;
        }

        /// <summary>
        /// 単一チャンネル画像のヒストグラムを計算します。
        /// </summary>
        /// <param name="image">単一チャンネルの Mat オブジェクト</param>
        /// <returns>ヒストグラムデータ（256ビン）</returns>
        private HistogramData CalcHistogram(Mat image)
        {
            var size = byte.MaxValue + 1;

            using var hist = new Mat();
            var histSize = new int[] { size }; // ヒストグラムのビン数
            Rangef[] ranges = { new Rangef(0, size) }; // ピクセル値の範囲

            // ヒストグラムを計算
            Cv2.CalcHist(new Mat[] { image }, new int[] { 0 }, null, hist, 1, histSize, ranges);

            var data = new HistogramData(size);
            for (var i = 0; i < size; i++)
            {
                data.Histogram[i] = (int)hist.Get<float>(i);
            }

            data.Recalculate(true);

            return data;
        }

        #endregion

        #region Rendererイベント

        /// <summary>
        /// Rendererイベント:Rendering
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="s"></param>
        private void Renderer_Rendering(object sender, Surface s)
        {
            var rectangle = new Rectangle();

            try
            {
                // 表示対象の情報への参照を取得する
                var drawGraphCount = this.graphs.Values.Count(g => g.Use && g.Visible);
                var selectedGraph = this.graphs.Values.ToList().Find(g => g.Use && g.Visible);

                // 書式
                s.Font = new Font(this.Font.Name, 10);
                var format = "{0:#,0}";

                // 最頻値取得
                var listMode = new List<int>();
                foreach (var g in this.graphs.Values)
                {
                    if (true == g.Use)
                    {
                        listMode.Add((int)g.Data.Histogram[(int)g.Data.Mode]);
                    }
                }
                if (0 > listMode.Count)
                {
                    listMode.Add(this.scaleRangeMaximum.Y);
                }
                listMode.Add(100);

                var modeMax = System.Math.Max(100, (int)(listMode.Max() / this.verticalZoomRatio));
                var modeDigit = modeMax.ToString().Length;
                var modeString = string.Format(format, modeMax);
                var modeSize = s.MeasureText(modeString);

                // 描画範囲設定
                var beginPoint = new Point(60, this.Height - 35);
                var endPoint = new Point(this.Width - 20, 20);
                if (modeSize.Width > beginPoint.X)
                {
                    beginPoint.X = modeSize.Width + 5;
                }

                var range = new PointD(endPoint.X - beginPoint.X + 1, beginPoint.Y - endPoint.Y + 1);
                rectangle = new Rectangle(new Point(beginPoint.X, endPoint.Y), new Size(Convert.ToInt32(range.X), Convert.ToInt32(range.Y)));

                // 枠表示
                s.Pen.Width = 1;
                s.Pen.Color = this.scaleColor;
                s.Pen.DashStyle = DashStyle.Solid;
                s.DrawLine(beginPoint.X, beginPoint.Y, beginPoint.X, endPoint.Y - 1);
                s.DrawLine(beginPoint.X, beginPoint.Y, endPoint.X + 1, beginPoint.Y);

                // 階調取得
                var listTone = new List<int>();
                var toneMin = 0;
                foreach (var g in this.graphs.Values)
                {
                    if (true == g.Use)
                    {
                        listTone.Add(g.Data.Histogram.Length);
                    }
                }
                if (0 == listTone.Count)
                {
                    listTone.Add(this.scaleRangeMaximum.X + 1);
                    toneMin = this.scaleRangeMinimum.X;
                }
                var toneMax = listTone.Max() - 1;
                var toneRange = toneMax - toneMin + 1;
                var toneString = toneMax.ToString();
                var toneSize = s.MeasureText(toneString);

                var scalePoint = new PointD();

                #region 垂直目盛
                {
                    var verticalUnit = System.Math.Pow(10, modeDigit - 1);
                    var verticalNumberI = (int)(modeMax / (int)verticalUnit);

                    if ((1 == verticalNumberI) && (2 < modeDigit))
                    {
                        verticalUnit = System.Math.Pow(10, modeDigit - 2);
                        verticalNumberI = (int)(modeMax / (int)verticalUnit);
                    }

                    var verticalNumberD = verticalNumberI + (modeMax % (int)verticalUnit) / verticalUnit;
                    var verticalStep = range.Y / verticalNumberD;

                    scalePoint.Y = beginPoint.Y - verticalStep * verticalNumberD;

                    var detailNumber = 0;
                    if (30 < verticalStep)
                    {
                        detailNumber = 10;
                    }
                    else if (10 < verticalStep)
                    {
                        detailNumber = 5;
                    }
                    var detailStep = verticalStep / detailNumber;

                    verticalNumberI = (int)System.Math.Floor(verticalNumberD);

                    s.Brush = new SolidBrush(this.scaleColor);
                    for (int i = 0; i <= verticalNumberI; i++)
                    {
                        var point = beginPoint.Y - verticalStep * i;
                        s.DrawLine(beginPoint.X, point, beginPoint.X - 5, point);

                        for (int j = 1; j < detailNumber; j++)
                        {
                            var detailPoint = point - detailStep * j;

                            if (beginPoint.Y - verticalStep * verticalNumberD >= detailPoint)
                            {
                                break;
                            }
                            s.DrawLine(beginPoint.X, detailPoint, beginPoint.X - 3, detailPoint);
                        }

                        var text = string.Format(format, verticalUnit * i);
                        var size = s.MeasureText(text);
                        if (point - size.Height / 2 > scalePoint.Y + modeSize.Height / 2)
                        {
                            s.WriteString(text, beginPoint.X - size.Width, point - size.Height / 2);
                        }
                    }
                    s.DrawLine(beginPoint.X, scalePoint.Y, beginPoint.X - 5, scalePoint.Y);
                    s.WriteString(modeString, beginPoint.X - modeSize.Width, scalePoint.Y - modeSize.Height / 2);
                }
                #endregion

                var horizontalUnit = 100;
                var horizontalNumberI = toneRange / horizontalUnit;

                var horizontalPoints = this.GetOptimizedDrawPoints(beginPoint.X, endPoint.X, toneRange, out int[] horizontalInterval, out double horizontalStep);

                #region 水平目盛
                {
                    scalePoint.X = horizontalPoints[^1];

                    var detailNumber = 0;
                    if (30 < horizontalStep * horizontalUnit)
                    {
                        detailNumber = 10;
                    }
                    else if (10 < horizontalStep * horizontalUnit)
                    {
                        detailNumber = 5;
                    }
                    var detailStep = horizontalStep * horizontalUnit / detailNumber;

                    s.Brush = new SolidBrush(this.scaleColor);
                    int drawIndex = 0;
                    for (int i = 0; i <= horizontalNumberI; i++)
                    {
                        var selectedPoint = horizontalPoints[drawIndex];

                        s.DrawLine(selectedPoint, beginPoint.Y, selectedPoint, beginPoint.Y + 5);

                        var datailPoint = selectedPoint + detailStep;
                        for (int j = 1; j < detailNumber; j++)
                        {
                            if (horizontalPoints[^1] <= datailPoint)
                            {
                                break;
                            }

                            s.DrawLine(datailPoint, beginPoint.Y, datailPoint, beginPoint.Y + 3);

                            datailPoint += detailStep;
                        }

                        if (0 < i)
                        {
                            var text = (horizontalUnit * i + toneMin).ToString();
                            var size = s.MeasureText(text);

                            if (selectedPoint + size.Width / 2 < scalePoint.X - toneSize.Width + 10)
                            {
                                s.WriteString(text, selectedPoint - size.Width / 2, beginPoint.Y + 5);
                            }
                        }

                        drawIndex += horizontalUnit;
                    }

                    if (0 != toneMin)
                    {
                        s.WriteString(toneMin.ToString(), horizontalPoints[0], beginPoint.Y + 5);
                    }

                    s.DrawLine(scalePoint.X, beginPoint.Y, scalePoint.X, beginPoint.Y + 5);
                    s.WriteString(toneString, scalePoint.X - toneSize.Width + 10, beginPoint.Y + 5);
                }
                #endregion

                #region ヒストグラム
                if (null != selectedGraph)
                {
                    var toneShift = horizontalInterval.Min();
                    if (2 <= toneShift)
                    {
                        toneShift /= 2;

                        horizontalInterval[0] -= toneShift;
                    }

                    if (1 == drawGraphCount)
                    {
                        var toneStart = System.Math.Max(toneMin, 0);
                        var toneNumber = selectedGraph.Data.Histogram.Length;

                        s.Mode = Surface.DrawingMode.Fill;
                        var shift = 0;

                        if (true == this.pseudColorEnabled)
                        {
                            for (int i = toneStart; i < toneNumber; i++)
                            {
                                var index = i - toneMin;
                                var left = horizontalPoints[index] - shift;
                                var width = horizontalInterval[index];
                                var height = selectedGraph.Data.Histogram[i] * range.Y / modeMax;

                                if (0 < height)
                                {
                                    s.Brush = new SolidBrush(this.pseudColors[i]);
                                    s.DrawRectangle1(left, beginPoint.Y - height - 1, width, height);
                                }

                                shift = toneShift;
                            }
                        }
                        else
                        {
                            s.Brush = new SolidBrush(selectedGraph.Color);

                            for (int i = toneStart; i < toneNumber; i++)
                            {
                                var index = i - toneMin;
                                var left = horizontalPoints[index] - shift;
                                var width = horizontalInterval[index];
                                var height = selectedGraph.Data.Histogram[i] * range.Y / modeMax;

                                if (0 < height)
                                {
                                    s.DrawRectangle1(left, beginPoint.Y - height - 1, width, height);
                                }

                                shift = toneShift;
                            }
                        }
                    }
                    else
                    {
                        foreach (var g in this.graphs.Values)
                        {
                            if (g.Visible == false)
                            {
                                continue;
                            }

                            var toneStart = System.Math.Max(toneMin, 0);
                            var toneNumber = g.Data.Histogram.Length;

                            var shift = 0;
                            var outlines = new List<PointF>();
                            for (int i = toneStart; i < toneNumber; i++)
                            {
                                var index = i - toneMin;
                                var left = horizontalPoints[index] - shift;
                                var width = horizontalInterval[index];
                                var height = g.Data.Histogram[i] * range.Y / modeMax;

                                outlines.Add(new PointF(left + width / 2, (float)beginPoint.Y - (float)height - 1));

                                shift = toneShift;
                            }
                            s.Mode = Surface.DrawingMode.Outline;
                            s.Pen = new Pen(g.Color, 2);
                            s.DrawLines(outlines.ToArray());
                        }
                    }
                }
                #endregion

                #region カーソル表示
                if (true == this.cursorOverlayEnalbed)
                {
                    // コントロール上のマウス座標を取得する
                    var mousePoint = this.PointToClient(System.Windows.Forms.Cursor.Position);

                    // 座標が有効範囲内の場合
                    if ((beginPoint.X < mousePoint.X)
                    && (beginPoint.Y > mousePoint.Y)
                    && (endPoint.X > mousePoint.X)
                    && (endPoint.Y < mousePoint.Y))
                    {
                        // 十字線を描画する
                        s.Pen.Width = 2;
                        s.Pen.DashStyle = DashStyle.Solid;
                        s.Pen.Color = this.cursorColor;
                        s.DrawLine(mousePoint.X - 10, mousePoint.Y, mousePoint.X + 10, mousePoint.Y);
                        s.DrawLine(mousePoint.X, mousePoint.Y - 10, mousePoint.X, mousePoint.Y + 10);

                        // 水平・垂直線を描画する
                        s.Pen.Width = 1;
                        s.Pen.DashStyle = DashStyle.Dot;
                        s.Pen.Color = this.cursorColor;
                        s.DrawLine(beginPoint.X - 5, mousePoint.Y, endPoint.X, mousePoint.Y);
                        s.DrawLine(mousePoint.X, beginPoint.Y + 5, mousePoint.X, endPoint.Y);

                        #region カーソル位置の頻度値を表示する
                        {
                            var textV = string.Format(format, Convert.ToInt32((beginPoint.Y - mousePoint.Y) * (modeMax / range.Y))).PadLeft(modeString.Length);
                            var sizeV = s.MeasureText(textV);
                            var plotV = new Point(beginPoint.X - sizeV.Width - 1, mousePoint.Y - sizeV.Height / 2 - 1);

                            var modeRect = new RectangleD(new PointD(plotV.X, plotV.Y), new SizeD((beginPoint.X - 5 - plotV.X), sizeV.Height + 2));
                            using var modePath = GraphicsUtilities.CreateRound(modeRect.ToRectangleF(), RoundedCorner.All, 5);

                            s.Brush = new SolidBrush(this.BackColor);
                            s.FillPath(modePath);

                            s.Brush = new SolidBrush(this.cursorColor);
                            s.Pen.DashStyle = DashStyle.Solid;
                            s.DrawPath(modePath);

                            s.Brush = new SolidBrush(this.cursorColor);
                            s.WriteString(textV, plotV.X, plotV.Y);
                        }
                        #endregion                                                                                                                        

                        #region カーソル位置の輝度値を表示する
                        {
                            var lumiH = (int)((mousePoint.X - beginPoint.X) * (toneRange / range.X) + toneMin + 0.5);
                            var textH = lumiH.ToString().PadLeft(toneString.Length);
                            var sizeH = s.MeasureText(textH);
                            var plotH = new Point(mousePoint.X - sizeH.Width / 2 + 1, beginPoint.Y + 5);

                            var lumiRect = new RectangleD(new PointD(plotH.X, plotH.Y), new SizeD(sizeH.Width - 4, sizeH.Height + 2));
                            using var lumiPath = GraphicsUtilities.CreateRound(lumiRect.ToRectangleF(), RoundedCorner.All, 5);

                            s.Brush = new SolidBrush(this.BackColor);
                            s.FillPath(lumiPath);

                            s.Brush = new SolidBrush(this.cursorColor);
                            s.Pen.DashStyle = DashStyle.Solid;
                            s.DrawPath(lumiPath);

                            s.Brush = new SolidBrush(this.cursorColor);
                            s.WriteString(textH, plotH.X, plotH.Y);
                        }
                        #endregion                                                                                                                        
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
            finally
            {
                #region イベント通知
                try
                {
                    this.Drawing?.Invoke(this, this.renderer, s, rectangle);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(this, ex.Message);
                }
                #endregion
            }
        }
        private void Renderer_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            this.verticalZoomRatio = 1.0;
            this.renderer?.DrawToFit();
        }

        private void Renderer_MouseMove(object? sender, MouseEventArgs e)
        {
            if (true == this.cursorOverlayEnalbed)
            {
                this.renderer?.Draw();
            }
        }

        private void Renderer_Resize(object? sender, EventArgs e)
        {
            var backgroundRect = new Rectangle(Point.New(), new Size(this.ClientRectangle.Size));
            this.renderer?.CopyFrom(backgroundRect, base.BackColor);
            this.Invalidate();
        }

        #endregion

        #region GUIイベント

        private void HistogramUserControl_Load(object sender, EventArgs e)
        {
            if (false == this.DesignMode)
            {
                this.Clear();
            }
        }

        private void Channel_Changed(object sender)
        {
            var selectedGraph = (Graph)sender;

            foreach (var item in new[] { this.channelOfGray, this.channelOfRed, this.channelOfGreen, this.channelOfBlue })
            {
                if (item.Tag is Channel ch)
                {
                    if (ch == selectedGraph.Channel)
                    {
                        item.CheckStateChanged -= this.channel_CheckStateChanged;
                        item.Checked = selectedGraph.Visible;
                        item.CheckStateChanged += this.channel_CheckStateChanged;
                    }
                }
            }

            this.renderer?.Draw();
        }

        private void HistogramUserControl_Resize(object sender, EventArgs e)
        {
        }

        private void color_CheckStateChanged(object? sender, EventArgs e)
        {
            foreach (var item in new[] { this.graphColorNormal, this.graphColorPseudocolor, this.graphColorPseudocolorInverted })
            {
                if (false == item.Equals(sender))
                {
                    item.CheckStateChanged -= this.color_CheckStateChanged;
                    item.Checked = false;
                    item.CheckStateChanged += this.color_CheckStateChanged;
                }
            }

            this.PseudColorEnabled = this.graphColorPseudocolor.Checked | this.graphColorPseudocolorInverted.Checked;
            this.PseudColorInverted = this.graphColorPseudocolorInverted.Checked;

            this.renderer?.Draw();
        }

        private void channelOfAllOnOff_Click(object sender, EventArgs e)
        {
            var onOff = this.channelOfAllOn.Equals(sender);

            foreach (var item in new[] { this.channelOfGray, this.channelOfRed, this.channelOfGreen, this.channelOfBlue })
            {
                if (false == item.Equals(sender))
                {
                    item.CheckStateChanged -= this.channel_CheckStateChanged;
                    item.Checked = onOff;
                    item.CheckStateChanged += this.channel_CheckStateChanged;
                }
            }

            foreach (var g in this.graphs.Values)
            {
                g.Visible = onOff;
            }

            this.renderer?.Draw();
        }

        private void channel_CheckStateChanged(object? sender, EventArgs e)
        {
            if (null == sender) return;

            try
            {
                var selectedItem = (ToolStripMenuItem)sender;
                if (selectedItem.Tag is Channel channel)
                {
                    var selectedGraph = this.graphs[channel];

                    selectedGraph.Visible = selectedItem.Checked;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            this.renderer?.Draw();
        }

        private void option_CheckStateChanged(object sender, EventArgs e)
        {
            this.CursorOverlayEnalbed = this.optionCursorOverlayEnalbed.Checked;
        }

        #endregion
    }
}
