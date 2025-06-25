using Hutzper.Library.Common;
using Hutzper.Library.Common.Drawing;
using Hutzper.Library.Common.Forms;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Hutzper.Library.Forms.ImageView
{
    /// <summary>
    /// 画像表示用ユーザーコントロール
    /// </summary>
    public partial class ImageViewControl : HutzperUserControl, IDrawingUpdateTimingInjectable
    {
        #region サブクラス

        /// <summary>
        /// 距離計測情報
        /// </summary>
        public class DistanceMeasurement
        {
            public PointD? PointBegin { get; set; }

            public PointD? PointEnd { get; set; }

            public Library.Common.Drawing.Point PointTemp { get; set; } = new();

            public bool Enabled { get; set; }

            public bool Distance(out double length, out double angleRadian)
            {
                length = 0d;
                angleRadian = 0d;

                if (this.PointEnd is not null && this.PointBegin is not null)
                {
                    this.PointEnd.DistanceFrom(this.PointBegin, out length, out angleRadian);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 画像配列データ
        /// </summary>
        public record ImageData
        {
            public byte[] Bytes { get; set; } = Array.Empty<byte>();

            public PixelFormat Format { get; set; } = PixelFormat.Format8bppIndexed;

            public int Stride { get; set; }

            public int Width { get; set; }

            public int Height { get; set; }
        }

        /// <summary>
        /// Projection描画情報
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class DrawProjectionInfo
        {
            #region プロパティ

            /// <summary>
            /// 描画有効化フラグ
            /// </summary>
            public bool Enabled { get; set; }

            /// <summary>
            /// 描画色
            /// </summary>
            public Color Color { get; set; }

            /// <summary>
            /// 描画線幅
            /// </summary>
            [Browsable(false)]
            public int LineWidth { get; set; }

            #endregion

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public DrawProjectionInfo()
            {
                this.Enabled = false;
                this.Color = Color.Yellow;
                this.LineWidth = 1;
            }

            #endregion

            public override string ToString() => $"{this.Enabled}";
        }

        #endregion

        #region プロパティ

        #region 描画に関する設定

        /// <summary>
        /// 疑似カラー表示するかどうか
        /// </summary>
        /// <remarks>1ch画像に対して有効です</remarks>
        [Browsable(true)]
        [Category("描画設定")]
        [Description("疑似カラー表示するかどうか")]
        public bool PseudoColorEnabled
        {
            get => this.pseudoColorEnabled;
            set
            {
                if (this.pseudoColorEnabled != value)
                {
                    this.pseudoColorEnabled = value;

                    lock (this.DisplayBitmapSync)
                    {
                        this.Renderer.CopyTo(out var bitmap);

                        if (bitmap is not null)
                        {
                            if (false == this.pseudoColorEnabled)
                            {
                                this.AssignColorPaletteOfDefault(bitmap);
                            }

                            this.SetImage(bitmap, this.CurrentImageData);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 画像の背景色
        /// </summary>
        [Browsable(true)]
        [Category("描画設定")]
        [Description("画像の背景色")]
        public Color ImageBackColor
        {
            get => this.Renderer.BackColor;
            set => this.Renderer.BackColor = value;
        }

        /// <summary>
        /// 中心線描画設定
        /// </summary>
        [Browsable(true)]
        [Category("描画設定")]
        [Description("中心線")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Renderer.CrossLines CenterLines => this.Renderer.CenterLines;

        /// <summary>
        /// カーソル線描画設定
        /// </summary>
        [Browsable(true)]
        [Category("描画設定")]
        [Description("カーソル線")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Renderer.CrossLines CursorLines => this.Renderer.CursorLines;

        /// <summary>
        /// グリッド線描画設定
        /// </summary>
        [Browsable(true)]
        [Category("描画設定")]
        [Description("グリッド線")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Renderer.CrossLines GridLines => this.Renderer.GridLines;

        /// <summary>
        /// 水平プロジェクション描画設定
        /// </summary>
        [Browsable(true)]
        [Category("描画設定")]
        [Description("水平プロジェクション")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DrawProjectionInfo HProjection => this.hProjection;

        /// <summary>
        ///画像の内容を説明する文字列
        /// </summary>
        [Browsable(true)]
        [Category("描画設定")]
        [Description("画像の内容を説明する文字列を設定します")]
        public string ImageDescription { get => this.Text; set => this.Text = value; }

        /// <summary>
        /// 画素情報を表示するかどうか
        /// </summary>
        [Browsable(true)]
        [Category("描画設定")]
        [Description("ImageDescriptionプロパティの文字列を画像左上に表示するかどうか")]
        public bool DisplayImageDescription
        {
            get => this.displayImageDescription;
            set => this.displayImageDescription = value;
        }

        /// <summary>
        /// 画素情報を表示するかどうか
        /// </summary>
        [Browsable(true)]
        [Category("描画設定")]
        [Description("画素情報を表示するかどうか")]
        public bool DisplayPixelInfoEnabled
        {
            get => this.displayPixelInfoEnabled;
            set => this.displayPixelInfoEnabled = value;
        }

        /// <summary>
        /// FPSを表示するどうか
        /// </summary>
        /// <remarks>Fpsプロパティに最新値を設定してください</remarks>
        [Browsable(true)]
        [Category("描画設定")]
        [Description("FPSを表示するどうか")]
        public bool DisplayFpsEnabled
        {
            get => this.displayFpsEnabled;
            set => this.displayFpsEnabled = value;
        }

        #endregion

        /// <summary>
        /// 画像ファイルのドロップを受け入れるかどうか
        /// </summary>
        [Browsable(true)]
        [Category("描画設定")]
        [Description("画像ファイルのドロップを受け入れるかどうか")]
        public bool AcceptImageFileDrop { get => this.AllowDrop; set => this.AllowDrop = value; }

        /// <summary>
        /// 計測を使用するかどうか
        /// </summary>
        /// <remarks>左クリックで開始点、右クリックで終了点を指定しその距離を計測します</remarks>
        [Browsable(true)]
        [Category("描画設定")]
        [Description("計測を使用するかどうか")]
        public bool UseMeasurement
        {
            get => this.ImagePointMeasurement.Enabled;
            set => this.ImagePointMeasurement.Enabled = value;
        }

        /// <summary>
        /// 分解能 mm/pixel
        /// </summary>
        /// <remarks>計測に使用されます</remarks>
        [Browsable(true)]
        [Category("描画設定")]
        [Description("分解能mm/pixel値を設定します")]
        public double ResolutionMmPerPixel
        {
            get => this.resolutionMmPerPix;
            set => this.resolutionMmPerPix = value;
        }

        /// <summary>
        /// 現在のFPS
        /// </summary>
        /// <remarks>表示したい値を設定します</remarks>
        [Browsable(true)]
        [Category("描画設定")]
        [Description("表示するFPSを設定します")]
        public double CurrentFps
        {
            get => this.fpsValue;
            set => this.fpsValue = value;
        }

        /// <summary>
        /// 描画更新間隔ミリ秒
        /// </summary>
        /// <remarks>UseInjectedTimingプロパティがfalseの場合に有効です</remarks>
        [Browsable(true)]
        [Category("描画設定")]
        [Description("描画更新間隔をミリ秒で設定します。(通常は変更しません)")]
        public int DrawingUpdateIntervalMs
        {
            get => this.ImageDrawingTimer.Interval;
            set => this.ImageDrawingTimer.Interval = value;
        }

        /// <summary>
        /// 描画更新のタイミングを外部から注入するかどうか
        /// </summary>
        /// <remarks>trueの場合は内部タイマーを使用せず、InjectUpdateTimingメソッドが呼び出されたときに表示の更新を行います。ユーザーコントロールを複数使う場合に同一のタイマを用いることができます</remarks>
        public bool UseInjectedTiming
        {
            get => this.useInjectedTiming;
            set
            {
                this.useInjectedTiming = value;
                this.ImageDrawingTimer.Enabled = !this.useInjectedTiming;
            }
        }

        /// <summary>
        /// 描画インスタンス
        /// </summary>
        public Renderer Renderer { get; protected set; }

        #endregion

        #region イベント

        /// <summary>
        /// イベント:画像更新
        /// </summary>
        /// <remarks>実際に表示画像が更新されたタイミングで発生します。表示中の画像が必要な場合はGetImageメソッドで取得してください</remarks>
        public Action<object>? ImageUpdated;

        #endregion

        #region リソースの破棄

        /// <summary>
        /// リソース破棄
        /// </summary>
        protected override void DisposeImplicit()
        {
            try
            {
                this.Renderer?.Dispose();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region フィールド

        protected Tuple<PointD, Tuple<int, int, int>>? InterestedLocation;
        protected Tuple<PointD, Tuple<int, int, int>>? DisplayLocation;

        protected DistanceMeasurement ImagePointMeasurement = new();

        protected Bitmap? LatestDisplayBitmap;
        protected Bitmap? LatestGrabbedBitmap;
        protected ImageData CurrentImageData = new();
        protected object DisplayBitmapSync = new();

        protected bool pseudoColorEnabled;
        protected double resolutionMmPerPix = 1d;
        protected double fpsValue;

        protected bool useInjectedTiming;
        protected bool displayImageDescription;
        protected bool displayPixelInfoEnabled = true;
        protected bool displayFpsEnabled;
        protected bool useMeasurement;

        protected DrawProjectionInfo hProjection = new();

        #endregion

        #region staticフィールド

        private static readonly double[] lutOfConvRgbToGrayFromR;
        private static readonly double[] lutOfConvRgbToGrayFromG;
        private static readonly double[] lutOfConvRgbToGrayFromB;

        #endregion

        #region スタティックコンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static ImageViewControl()
        {
            #region RGB画像をグレイスケール画像変換
            {
                var lenghtOfLut = byte.MaxValue + 1;
                ImageViewControl.lutOfConvRgbToGrayFromR = new double[lenghtOfLut];
                ImageViewControl.lutOfConvRgbToGrayFromG = new double[lenghtOfLut];
                ImageViewControl.lutOfConvRgbToGrayFromB = new double[lenghtOfLut];
                foreach (var i in Enumerable.Range(0, lenghtOfLut))
                {
                    ImageViewControl.lutOfConvRgbToGrayFromR[i] = i * 0.299;
                    ImageViewControl.lutOfConvRgbToGrayFromG[i] = i * 0.587;
                    ImageViewControl.lutOfConvRgbToGrayFromB[i] = i * 0.114;
                }
            }
            #endregion
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImageViewControl()
        {
            this.InitializeComponent();

            #region Renderer
            {
                this.Renderer = new Renderer(this, new Common.Drawing.Size(this.ClientSize.Width, this.ClientSize.Height));

                // マウス移動
                this.Renderer.MouseMove += ((sender, e) =>
                {
                    try
                    {
                        // カーソル位置が画像範囲の場合は注目画素値を退避する
                        var temp = (Tuple<PointD, Tuple<int, int, int>>?)null;
                        if (this.Renderer.GetMousePosition(e.X, e.Y, out double x, out double y))
                        {
                            if (0 <= x && 0 <= y && x < this.Renderer.ImageSize.Width && y < this.Renderer.ImageSize.Height)
                            {
                                if (this.Renderer.GetImagePixel((int)x, (int)y, out int r, out int g, out int b))
                                {
                                    temp = Tuple.Create(new PointD(x, y), Tuple.Create(r, g, b));
                                }
                            }
                        }

                        this.InterestedLocation = temp;
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                });

                // 画像マウスダウン
                this.Renderer.MouseDown += ((sender, e) =>
                {
                    this.Renderer.GetMousePosition(e.Location.X, e.Location.Y, out int locationX, out int locationY);
                    this.ImagePointMeasurement.PointTemp = new Library.Common.Drawing.Point(e.Location);
                });

                //　画像マウスアップ
                this.Renderer.MouseUp += ((sender, e) =>
                {
                    try
                    {
                        if (true == this.ImagePointMeasurement.Enabled)
                        {
                            this.Renderer.GetMousePosition(e.Location.X, e.Location.Y, out double locationX, out double locationY);

                            if (true == this.ImagePointMeasurement.PointTemp.Equals(new Library.Common.Drawing.Point(e.Location)))
                            {
                                if (e.Button == MouseButtons.Left)
                                {
                                    this.ImagePointMeasurement.PointBegin = new PointD(locationX, locationY);
                                }
                                else
                                {
                                    this.ImagePointMeasurement.PointEnd = new PointD(locationX, locationY);
                                }

                                this.Renderer.Draw();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                });

                // マウスダブルクリック
                this.Renderer.MouseDoubleClick += (sender, e) =>
                {
                    this.Renderer.DrawToFit();
                };

                // 描画
                this.Renderer.Drawing += (sender, s) =>
                {
                    try
                    {
                        // Textプロパティ表示
                        if (true == this.displayImageDescription)
                        {
                            s.Font = new Font(s.Font.FontFamily, this.Font.Size);

                            s.WriteOutlineString(this.Text, s.Location, Color.Black, Color.White, 2);
                        }

                        // 注目画素情報表示
                        if (true == this.displayPixelInfoEnabled)
                        {
                            if (this.InterestedLocation is not null)
                            {
                                var text = $"(X={this.InterestedLocation.Item1.X:F1}, Y={this.InterestedLocation.Item1.Y:F1}, R={this.InterestedLocation.Item2.Item1}, G={this.InterestedLocation.Item2.Item2}, B={this.InterestedLocation.Item2.Item3})";

                                s.Font = new Font(s.Font.FontFamily, this.Font.Size);
                                s.Brush = Brushes.DodgerBlue;

                                var textPosition = new PointD(s.Location.X, s.Location.Y + s.Size.Height - s.MeasureTextF(text).Height);

                                s.WriteOutlineString(text, textPosition, Color.Black, Color.White, 2);
                            }
                        }

                        // FPS表示
                        if (true == this.displayFpsEnabled)
                        {
                            var text = $"fps = {this.fpsValue:F1}";

                            s.Font = new Font(s.Font.FontFamily, this.Font.Size);

                            var textSize = s.MeasureTextF(text);
                            var textPosition = new PointD(s.Location.X + s.Size.Width - textSize.Width, s.Location.Y + s.Size.Height - textSize.Height);

                            s.WriteOutlineString(text, textPosition, Color.Black, Color.White, 2);
                        }

                        // 水平プロジェクション描画
                        if (true == this.hProjection.Enabled)
                        {
                            var mouseLocation = this.PointToClient(Cursor.Position);
                            this.Renderer.GetMousePosition(mouseLocation.X, mouseLocation.Y, out int locationX, out int locationY);
                            this.DrawHorizontalProjection(s, this.CurrentImageData, new Library.Common.Drawing.Point(locationX, locationY));
                        }

                        // 距離計測
                        if (true == this.ImagePointMeasurement.Enabled)
                        {
                            var begin = new PointD();
                            var end = new PointD();
                            if (this.ImagePointMeasurement.PointBegin is not null)
                            {
                                begin = this.ImagePointMeasurement.PointBegin.Clone();
                                s.Mode = Surface.DrawingMode.Fill;
                                s.Brush = Brushes.Cyan;
                                s.DrawCircle(begin.X, begin.Y, 5 / s.ScalingH);
                            }

                            if (this.ImagePointMeasurement.PointEnd is not null)
                            {
                                end = this.ImagePointMeasurement.PointEnd.Clone();
                                s.Mode = Surface.DrawingMode.Fill;
                                s.Brush = Brushes.Cyan;
                                s.DrawCircle(end.X, end.Y, 5 / s.ScalingH);
                            }

                            if (true == this.ImagePointMeasurement.Distance(out double length, out double angleRadian))
                            {
                                var angleDegree = Mathematics.RadianToDegree(angleRadian);

                                s.Pen = new Pen(Color.Cyan);
                                s.DrawLine(begin, end);

                                s.Font = new Font(s.Font.FontFamily, this.Font.Size);
                                s.Brush = Brushes.Cyan;

                                var distanceText = $"{this.resolutionMmPerPix * length:F3}mm ({length:F2} pix)\n    {angleDegree:F3} deg";
                                var distancePosition = new PointD(end.X, end.Y);

                                s.WriteString(distanceText, distancePosition);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                };
            }
            #endregion

            #region プロパティの初期値設定
            {
                this.Renderer.CenterLines.Color = Color.Fuchsia;
            }
            #endregion

            // 描画更新タイマ設定
            this.ImageDrawingTimer.Interval = 1000 / 20;    // 20fps程度
            this.ImageDrawingTimer.Enabled = !this.useInjectedTiming;
        }

        #endregion

        #region publicメソッド

        /// <summary>
        /// 表示する画像を設定する
        /// </summary>
        /// <param name="bitmap">表示する画像(内部でCloneされます。不要となったbitmapは呼び出し側でDisposeしてください)</param>
        /// <param name="data">カメラ撮像データ</param>
        /// <remarks>カメラライブ表示等に使用します</remarks>
        public void SetImage(Bitmap bitmap, ImageData? data)
        {
            try
            {
                if (bitmap?.Clone() is Bitmap clonedBitmap)
                {
                    var tempData = new ImageData();

                    if (data is not null)
                    {
                        tempData = data;
                    }

                    if (this.pseudoColorEnabled)
                    {
                        this.AssignColorPaletteOfPseudo(clonedBitmap);
                    }

                    // 保持している最新画像を更新する
                    lock (this.DisplayBitmapSync)
                    {
                        this.CurrentImageData = tempData;

                        this.LatestGrabbedBitmap?.Dispose();
                        this.LatestGrabbedBitmap = clonedBitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 表示する画像を設定する
        /// </summary>
        /// <param name="bitmap">表示する画像(内部でCloneされます。不要となったbitmapは呼び出し側でDisposeしてください)</param>
        /// <param name="isGenBytes">バイト配列を生成するかどうか</param>
        /// <remarks>検査結果表示等に使用します。撮像調整等に使用する場合はImageData引数版を推奨</remarks>
        public void SetImage(Bitmap? bitmap)
        {
            try
            {
                if (bitmap?.Clone() is Bitmap clonedBitmap)
                {
                    if (this.pseudoColorEnabled)
                    {
                        this.AssignColorPaletteOfPseudo(clonedBitmap);
                    }

                    lock (this.DisplayBitmapSync)
                    {
                        this.CurrentImageData = new ImageData(); ;

                        this.LatestGrabbedBitmap?.Dispose();
                        this.LatestGrabbedBitmap = clonedBitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 表示中の画像を取得する
        /// </summary>
        /// <returns></returns>
        public Bitmap? GetImage()
        {
            var bitmap = (Bitmap?)null;

            try
            {
                this.Renderer.CopyTo(out bitmap);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return bitmap;
        }

        /// <summary>
        /// 再描画を行う
        /// </summary>
        /// <remarks>プロパティ変更時に呼び出して設定の変更を描画に反映させます</remarks>
        public void Redraw() => this.Renderer.Draw();

        /// <summary>
        /// 更新タイミングを注入する
        /// </summary>
        /// <remarks>UseInjectedTimingプロパティがtrueの場合に、このメソッドを呼び出すことで画像の更新が行われます</remarks>
        public void InjectUpdateTiming() => this.UpdateDrawing();

        #endregion

        #region protectedメソッド

        /// <summary>
        /// 表示更新
        /// </summary>
        protected void UpdateDrawing()
        {
            this.InvokeSafely(() =>
            {
                var bitmap = (Bitmap?)null;

                // 最新画像を取得する
                lock (this.DisplayBitmapSync)
                {
                    if (this.LatestGrabbedBitmap != this.LatestDisplayBitmap)
                    {
                        bitmap = this.LatestGrabbedBitmap;
                        this.LatestGrabbedBitmap = null;
                    }
                }

                var isNeedToDraw = false;

                // 最新画像がある場合
                if (bitmap is not null)
                {
                    this.LatestDisplayBitmap?.Dispose();
                    this.LatestDisplayBitmap = bitmap;

                    this.Renderer.Replace(bitmap);

                    this.OnImageUpdated();

                    isNeedToDraw = true;
                }

                if (this.InterestedLocation is not null)
                {
                    // カーソル位置の情報を更新する
                    var x = (int)this.InterestedLocation.Item1.X;
                    var y = (int)this.InterestedLocation.Item1.Y;

                    if (0 <= x && 0 <= y && x < this.Renderer.ImageSize.Width && y < this.Renderer.ImageSize.Height)
                    {
                        if (this.Renderer.GetImagePixel((int)x, (int)y, out int r, out int g, out int b))
                        {
                            var newLocation = Tuple.Create(this.InterestedLocation.Item1, Tuple.Create(r, g, b));

                            if (false == this.InterestedLocation.Item2.Equals(newLocation.Item2) || this.DisplayLocation != this.InterestedLocation)
                            {
                                this.InterestedLocation = newLocation;
                                this.DisplayLocation = this.InterestedLocation;
                                isNeedToDraw = true;
                            }
                        }
                    }
                }
                else if (this.DisplayLocation is not null)
                {
                    this.DisplayLocation = null;
                    isNeedToDraw = true;
                }

                // 画像表示更新
                if (true == isNeedToDraw)
                {
                    this.Renderer.Draw();
                }
            });
        }

        /// <summary>
        /// 画像更新イベント通知
        /// </summary>
        protected void OnImageUpdated()
        {
            try
            {
                this.ImageUpdated?.Invoke(this);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 水平プロジェクション描画
        /// </summary>
        /// <param name="s">描画サーフェス</param>
        /// <param name="imageData">画像配列データ</param>
        /// <param name="location">注目位置</param>
        protected void DrawHorizontalProjection(Surface s, ImageData imageData, Library.Common.Drawing.Point location)
        {
            if (0 < imageData.Bytes.Length)
            {
                // 画像の矩形サイズ
                var imageRect = new Library.Common.Drawing.Rectangle(Library.Common.Drawing.Point.New(), new Library.Common.Drawing.Size(imageData.Width, imageData.Height));

                // 注目位置が画像範囲内の場合
                if (true == imageRect.Contains(location))
                {
                    var imageArray = imageData.Bytes;

                    // 有効な画像データがある場合
                    if (0 < imageArray.Length)
                    {
                        // 現在の描画領域を取得する
                        var drawingArea = this.Renderer.Area;

                        // 注目位置の画像データ(水平方向1ライン分)を取得する
                        var selectedLine = new ArraySegment<byte>(imageArray, imageData.Stride * location.Y, imageData.Stride);

                        // 描画用の座標を生成する
                        PointF[] drawPoints;
                        var fMagnification = (float)drawingArea.Magnification;
                        var fOffsetY = 64f;
                        if (PixelFormat.Format8bppIndexed == imageData.Format)
                        {
                            drawPoints = new PointF[selectedLine.Count];
                            foreach (var x in Enumerable.Range(0, drawPoints.Length))
                            {
                                drawPoints[x] = new PointF(x, drawingArea.Rectangle.Bottom - (fOffsetY + (float)selectedLine[x]) / fMagnification);
                            }
                        }
                        else
                        {
                            // カラー画像の場合はグレースケールに変換する
                            var grayArray = Array.Empty<float>();
                            if (PixelFormat.Format32bppRgb == imageData.Format || PixelFormat.Format32bppArgb == imageData.Format)
                            {
                                this.ConvertBgraToGray(selectedLine, out grayArray);
                            }
                            else
                            {
                                this.ConvertBgrToGray(selectedLine, out grayArray);
                            }

                            drawPoints = new PointF[grayArray.Length];
                            foreach (var x in Enumerable.Range(0, drawPoints.Length))
                            {
                                drawPoints[x] = new PointF(x, drawingArea.Rectangle.Bottom - ((fOffsetY + grayArray[x]) / fMagnification));
                            }
                        }

                        // ガイド線色
                        var guideColor = Color.FromArgb(128, this.hProjection.Color);

                        // ガイド線:注目座標の水平線を描画する
                        s.Pen = new Pen(guideColor, this.hProjection.LineWidth);
                        s.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                        s.DrawLine(0, location.Y, imageData.Width - 1, location.Y);

                        // ガイド線:輝度スケールを描画する
                        var scaleMin = drawingArea.Rectangle.Bottom - ((fOffsetY + byte.MinValue) / fMagnification);    // 輝度0
                        var scaleMax = drawingArea.Rectangle.Bottom - ((fOffsetY + byte.MaxValue) / fMagnification);    // 輝度255
                        var scaleMid = drawingArea.Rectangle.Bottom - ((fOffsetY + (byte.MaxValue + byte.MinValue) / 2) / fMagnification);  //輝度中間値
                        s.Pen = new Pen(guideColor, this.hProjection.LineWidth);

                        s.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                        s.DrawLine(0, scaleMin, imageData.Width - 1, scaleMin);
                        s.DrawLine(0, scaleMax, imageData.Width - 1, scaleMax);

                        s.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
                        s.DrawLine(0, scaleMid, imageData.Width - 1, scaleMid);

                        // 水平プロジェクションを描画する
                        s.Pen = new Pen(this.hProjection.Color, this.hProjection.LineWidth);
                        s.Pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                        s.DrawLines(drawPoints);
                    }
                }
            }
        }

        /// <summary>
        /// BGRの順で格納された配列をグレースケール配列に変換する
        /// </summary>
        /// <param name="source"></param>
        /// <param name="converted"></param>
        protected void ConvertBgrToGray(ArraySegment<byte> source, out float[] converted)
        {
            converted = new float[source.Count / 3];

            var index = 0;
            for (int i = 0; i < converted.Length; i++)
            {
                var grayVal = ImageViewControl.lutOfConvRgbToGrayFromB[source[index++]];
                grayVal += ImageViewControl.lutOfConvRgbToGrayFromG[source[index++]];
                grayVal += ImageViewControl.lutOfConvRgbToGrayFromR[source[index++]];
                converted[i] = (float)grayVal;
            }
        }

        /// <summary>
        /// BGRAの順で格納された配列をグレースケール配列に変換する
        /// </summary>
        /// <param name="source"></param>
        /// <param name="converted"></param>
        protected void ConvertBgraToGray(ArraySegment<byte> source, out float[] converted)
        {
            converted = new float[source.Count / 4];

            var index = 0;
            for (int i = 0; i < converted.Length; i++)
            {
                var grayVal = ImageViewControl.lutOfConvRgbToGrayFromB[source[index++]];
                grayVal += ImageViewControl.lutOfConvRgbToGrayFromG[source[index++]];
                grayVal += ImageViewControl.lutOfConvRgbToGrayFromR[source[index++]];
                converted[i] = (float)grayVal;

                index++;
            }
        }
        /// <summary>
        /// カラーパレットをリセットする
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        protected void AssignColorPaletteOfDefault(Bitmap bitmap)
        {
            var palette = this.GetColorPaletteOfDefault(bitmap);

            if ((null != palette) && (0 < palette.Entries.Length))
            {
                bitmap.Palette = palette;
            }
        }

        /// <summary>
        /// リセットしたカラーパレットを取得する
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        /// <returns>疑似カラー表示用のカラーパレット</returns>
        protected ColorPalette GetColorPaletteOfDefault(Bitmap bitmap)
        {
            var palette = bitmap.Palette;

            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    {
                        for (int i = 0; i < palette.Entries.Length; i++)
                        {
                            palette.Entries[i] = Color.FromArgb(i, i, i);
                        }
                    }
                    break;
            }

            return palette;
        }

        /// <summary>
        /// 疑似カラー表示用のカラーパレットを設定する
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        protected void AssignColorPaletteOfPseudo(Bitmap bitmap)
        {
            var palette = this.GetColorPaletteOfPseudo(bitmap);

            if ((null != palette) && (0 < palette.Entries.Length))
            {
                bitmap.Palette = palette;
            }
        }

        /// <summary>
        /// 疑似カラー表示用のカラーパレットを取得する
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        protected ColorPalette GetColorPaletteOfPseudo(Bitmap bitmap)
        {
            var palette = bitmap.Palette;

            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    {
                        #region カラーパレット作成

                        int tone = byte.MaxValue + 1;
                        int step = tone / 4;
                        int slope = Convert.ToInt32((double)tone / (double)step);
                        int index = 0;

                        var indexLut = Enumerable.Range(0, tone).ToArray();

                        for (int i = 0; i < step; i++)
                        {
                            palette.Entries[indexLut[index++]] = Color.FromArgb(0, i * slope, byte.MaxValue);
                        }
                        for (int i = 0; i < step; i++)
                        {
                            palette.Entries[indexLut[index++]] = Color.FromArgb(0, byte.MaxValue, byte.MaxValue + 1 - (i + 1) * slope);
                        }
                        for (int i = 0; i < step; i++)
                        {
                            palette.Entries[indexLut[index++]] = Color.FromArgb(i * slope, byte.MaxValue, 0);
                        }
                        for (int i = 0; i < step; i++)
                        {
                            palette.Entries[indexLut[index++]] = Color.FromArgb(byte.MaxValue, byte.MaxValue + 1 - (i + 1) * slope, 0);
                        }

                        #endregion
                    }
                    break;
            }

            return palette;
        }

        /// <summary>
        /// 画像ファイルを読み込んでBitmapを作成する
        /// </summary>
        /// <param name="fileInfo">画像ファイル</param>
        /// <param name="stride">Bitmapストライド</param>
        /// <param name="bytes"></param>画像バイト配列</param>
        /// <returns>作成されたビットマップ</returns>
        protected Bitmap? ReadImageFrom(FileInfo fileInfo, out int stride, out byte[] bytes)
        {
            Bitmap? bitmap = null;
            bytes = Array.Empty<byte>();
            stride = 0;

            try
            {
                using var stream = new System.IO.FileStream(fileInfo.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                byte[] sourceArray;

                using var bitmapOnStream = new Bitmap(stream);
                var sourceData = bitmapOnStream.LockBits(new System.Drawing.Rectangle(0, 0, bitmapOnStream.Width, bitmapOnStream.Height), ImageLockMode.ReadOnly, bitmapOnStream.PixelFormat);
                {
                    sourceArray = new byte[sourceData.Stride * sourceData.Height];

                    Marshal.Copy(sourceData.Scan0, sourceArray, 0, sourceArray.Length);
                }
                bitmapOnStream.UnlockBits(sourceData);

                bitmap = new Bitmap(bitmapOnStream.Width, bitmapOnStream.Height, bitmapOnStream.PixelFormat);

                if (0 < bitmapOnStream.Palette.Entries.Length)
                {
                    bitmap.Palette = bitmapOnStream.Palette;
                }

                var destinationData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                {
                    Marshal.Copy(sourceArray, 0, destinationData.Scan0, sourceArray.Length);
                }

                stride = destinationData.Stride;
                bytes = sourceArray;

                bitmap.UnlockBits(destinationData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return bitmap;
        }

        #endregion

        #region GUIイベント

        /// <summary>
        /// 描画更新タイマイベント
        /// </summary>
        private void ImageDrawingTimer_Tick(object sender, EventArgs e) => this.UpdateDrawing();

        /// <summary>
        /// DragEnterイベント
        /// </summary>
        private void SingleImageViewUserControl_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data?.GetDataPresent(DataFormats.FileDrop) ?? false)
                {
                    e.Effect = DragDropEffects.All;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// DragDropイベント
        /// </summary>
        /// <remarks>ドロップされた画像ファイルを読み込んで表示ます</remarks>
        private void SingleImageViewUserControl_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data?.GetData(DataFormats.FileDrop, false) is string[] files)
                {
                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);

                        var bitmap = this.ReadImageFrom(fileInfo, out int stride, out byte[] bytes);

                        if (bitmap is not null)
                        {
                            var tempData = new ImageData
                            {
                                Format = bitmap.PixelFormat,
                                Width = bitmap.Width,
                                Height = bitmap.Height,
                                Stride = stride,
                                Bytes = bytes,
                            };

                            if (this.pseudoColorEnabled)
                            {
                                this.AssignColorPaletteOfPseudo(bitmap);
                            }

                            lock (this.DisplayBitmapSync)
                            {
                                this.LatestGrabbedBitmap?.Dispose();
                                this.LatestGrabbedBitmap = bitmap;

                                this.CurrentImageData = tempData;
                            }

                            this.UpdateDrawing();

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion
    }
}
