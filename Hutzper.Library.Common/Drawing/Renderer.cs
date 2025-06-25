using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Hutzper.Library.Common.Drawing
{
    /// <summary>
    /// 描画クラス
    /// </summary>
    public class Renderer : SafelyDisposable, ILoggable
    {
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

        #region スタティックメソッド

        /// <summary>
        /// 画像ファイルを読み込んでBitmapを作成する
        /// </summary>
        /// <param name="fullFileName">画像ファイル名</param>
        /// <returns>作成されたビットマップ</returns>
        public static Bitmap LoadBitmapFromFile(string fullFileName)
        {
            Bitmap? bitmap = null;

            try
            {
                using var stream = new System.IO.FileStream(fullFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                byte[] sourceArray;

                using var bitmapOnStream = new Bitmap(stream);
                BitmapData sourceData = bitmapOnStream.LockBits(new System.Drawing.Rectangle(0, 0, bitmapOnStream.Width, bitmapOnStream.Height), ImageLockMode.ReadOnly, bitmapOnStream.PixelFormat);
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

                BitmapData destinationData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                {
                    Marshal.Copy(sourceArray, 0, destinationData.Scan0, sourceArray.Length);
                }
                bitmap.UnlockBits(destinationData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Renderer.CreateBitmap Bitmap作成 エラー,{ex.Message}");
            }

            return bitmap;
        }

        #endregion

        #region パブリックサブクラス

        /// <summary>
        /// 描画領域構造体
        /// </summary>
        public class DrawingArea
        {
            #region プロパティ

            /// <summary>
            /// プロパティ:矩形領域 表示範囲を示します。※画像範囲ではないことに注意してください。（負の値や画像サイズを超えた座標になりえます。）
            /// </summary>
            public Rectangle Rectangle { get; private set; }

            /// <summary>
            /// プロパティ:倍率逆数 ※画像表示内部計算用（計算や描画にはRendererクラスのGetSurfaceReciprocalメソッドから得られる値を使用してください。）
            /// </summary>
            public double Reciprocal { get; set; }

            /// <summary>
            /// プロパティ:倍率 ※画像表示内部計算用（計算や描画にはRendererクラスのGeturfaceMagnificationメソッドから得られる値を使用してください。）
            /// </summary>
            public double Magnification
            {
                get
                {
                    return 1 / this.Reciprocal;
                }
            }

            #endregion

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public DrawingArea()
            {
                this.Rectangle = new Rectangle();
                this.Reciprocal = 1;
            }

            /// <summary>
            /// コピーコンストラクタ
            /// </summary>
            /// <param name="source"></param>
            public DrawingArea(DrawingArea source)
            {
                this.Rectangle = new Rectangle(source.Rectangle);
                this.Reciprocal = source.Reciprocal;
            }

            #endregion

            #region メソッド

            /// <summary>
            /// 値の割り当て
            /// </summary>
            /// <param name="source"></param>
            public void Assign(DrawingArea source)
            {
                this.Rectangle.Assign(source.Rectangle);
                this.Reciprocal = source.Reciprocal;
            }

            /// <summary>
            /// 画像表示領域を取得します。
            /// </summary>
            /// <param name="imageSize">画像サイズ</param>
            /// <returns>画像表示領域</returns>
            public Rectangle GetImageArea(Size imageSize)
            {
                // 領域の座標を取得する
                var imageLeft = this.Rectangle.Left;
                var imageTop = this.Rectangle.Top;
                var imageRight = this.Rectangle.Right;
                var imageBottom = this.Rectangle.Bottom;

                #region 左端を画像サイズ範囲内にする
                if (imageLeft < 0)
                {
                    imageLeft = 0;
                }
                else if (imageLeft >= imageSize.Width)
                {
                    imageLeft = imageSize.Width - 1;
                }
                #endregion

                #region 上端を画像サイズ範囲内にする
                if (imageTop < 0)
                {
                    imageTop = 0;
                }
                else if (imageTop >= imageSize.Height)
                {
                    imageTop = imageSize.Height - 1;
                }
                #endregion

                #region 右端を画像サイズ範囲内にする
                if (imageRight < 0)
                {
                    imageRight = 0;
                }
                else if (imageRight >= imageSize.Width)
                {
                    imageRight = imageSize.Width - 1;
                }
                #endregion

                #region 下端を画像サイズ範囲内にする
                if (imageBottom < 0)
                {
                    imageBottom = 0;
                }
                else if (imageBottom >= imageSize.Height)
                {
                    imageBottom = imageSize.Height - 1;
                }
                #endregion

                var cropLocation = Point.New(imageLeft, imageTop);
                var cropSize = Size.New();

                if (imageLeft <= imageRight)
                {
                    cropSize.Width = imageRight - imageLeft + 1;
                }

                if (imageTop <= imageBottom)
                {
                    cropSize.Height = imageBottom - imageTop + 1;
                }

                return new Rectangle(cropLocation, cropSize);
            }

            /// <summary>
            /// 複製
            /// </summary>
            /// <returns></returns>
            public DrawingArea Clone()
            {
                return new DrawingArea(this);
            }

            #endregion
        }

        /// <summary>
        /// マウスホイールズーム
        /// </summary>
        public enum ZoomDelta
        {
            Positive,
            Negative,
        }

        /// <summary>
        /// 高速スクロール
        /// </summary>
        public enum HighSpeedScrollingSetting
        {
            Disabled,
            Enabled,
            EnabledWithControlKey,
        }

        /// <summary>
        /// 交差線描画パラメータクラスです。
        /// </summary>
        public class CrossLines
        {
            #region プロパティ

            /// <summary>
            /// 描画有効化フラグ
            /// </summary>
            public bool Enabled { get; set; }

            /// <summary>
            /// 水平線描画有効化フラグ
            /// </summary>
            public bool HorizontalLineEnabled { get; set; }

            /// <summary>
            /// 垂直線描画有効化フラグ
            /// </summary>
            public bool VerticalLineEnabled { get; set; }

            /// <summary>
            /// 描画色
            /// </summary>
            public Color Color { get; set; }

            /// <summary>
            /// 描画線幅
            /// </summary>
            public int LineWidth { get; set; }

            /// <summary>
            /// 描画を有効にする表示倍率
            /// </summary>
            public double UsefulMagnification { get; set; }

            #endregion

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public CrossLines()
            {
                this.Enabled = false;
                this.HorizontalLineEnabled = true;
                this.VerticalLineEnabled = true;
                this.Color = Color.DarkGray;
                this.LineWidth = 1;
                this.UsefulMagnification = -1;
            }

            #endregion
        }

        /// <summary>
        /// 画像描画モード
        /// </summary>
        public enum ImageDrawingMode
        {
            /// <summary>
            /// 画像描画にGraphics.DrawImageを使用するモード(最近傍補間)
            /// </summary>
            /// <remarks>画像サイズが大きくなるにしたがって描画速度が低下します。</remarks>
            Normal,

            /// <summary>
            /// 画像描画にGraphics.FillRectangle(TextureBrush)を使用するモード
            /// </summary>
            /// <remarks>Normalに比べて画像サイズが大きくなっても描画速度が低下しません。TextureBrushによる画像補間が常に適用されます。</remarks>
            SpeedOriented,

            /// <summary>
            /// 画像描画にGraphics.DrawImageとGraphics.FillRectangle(TextureBrush)を描画倍率によって使い分けるモード
            /// </summary>
            /// <remarks>Normalに比べて画像サイズが大きくなっても描画速度が低下しません。縮小時はTextureBrushによる画像補間、拡大時は最近傍補間が適用されます。</remarks>
            Balanced,
        }

        #endregion

        #region プライベートサブクラス

        /// <summary>
        /// 拡大/縮小
        /// </summary>
        private enum ZoomDirection
        {
            In,         //拡大
            Out,        //縮小
        }

        /// <summary>
        /// 画像矩形
        /// </summary>
        /// <remarks>画像の代わりに用いる矩形</remarks>
        private class SubstituteImageRectangle : Rectangle
        {
            #region プロパティ

            /// <summary>
            /// 単一色
            /// </summary>
            public Color Color { get; set; }

            #endregion

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public SubstituteImageRectangle() : this(Color.Black)
            {
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public SubstituteImageRectangle(Color color)
            {
                this.Color = color;
            }

            /// <summary>
            /// コピーコンストラクタ
            /// </summary>
            public SubstituteImageRectangle(SubstituteImageRectangle source) : base(source)
            {
                this.Color = source.Color;
            }

            #endregion
        }

        /// <summary>
        /// 描画情報クラスです。
        /// </summary>
        private class DrawingInfo : SafelyDisposable
        {
            #region プロパティ

            /// <summary>
            /// 有効かどうか
            /// </summary>
            public bool Enabled
            {
                #region 取得
                get
                {
                    bool isEnabled = false;

                    if (false == this.UseSubstituteImage)
                    {
                        if (null != this.image)
                        {
                            isEnabled = true;
                        }
                    }
                    else if (null != this.substituteImage)
                    {
                        if ((0 < this.substituteImage.Size.Width)
                        && (0 < this.substituteImage.Size.Height))
                        {
                            isEnabled = true;
                        }
                    }

                    return isEnabled;
                }
                #endregion
            }

            /// <summary>
            /// プロパティ:ウィンドウ
            /// </summary>
            public PictureBox PictureBox { get; private set; }

            /// <summary>
            /// 親フォーム
            /// </summary>
            public Form? ParentForm
            {
                #region 取得
                get
                {
                    Control? parentControl = this.PictureBox.Parent;
                    while (null != parentControl?.Parent)
                    {
                        parentControl = parentControl.Parent;
                    }

                    return parentControl as Form;
                }
                #endregion
            }

            /// <summary>
            /// プロパティ:画像
            /// </summary>
            public Image? Image
            {
                #region 取得
                get
                {
                    var currentImage = (Image?)null;

                    if (false == this.UseSubstituteImage)
                    {
                        currentImage = this.image;
                    }

                    return currentImage;
                }
                #endregion

                #region 更新
                set
                {
                    this.DisposeSafely(this.image);
                    this.image = value;

                    base.DisposeSafely(this.TextureBrush);
                    this.TextureBrush = null;

                    if (null != this.image)
                    {
                        try
                        {
                            if (ImageDrawingMode.Normal != this.drawingMode)
                            {
                                this.TextureBrush = new TextureBrush(this.image, WrapMode.Clamp);
                            }

                            this.imageSize = new Size(this.image.Size);
                        }
                        catch (Exception ex)
                        {
                            this.imageSize = new Size();
                            Debug.WriteLine(this, $"画像サイズ取得 エラー, [{MethodBase.GetCurrentMethod()?.Name ?? String.Empty}]{ex.Message}");
                        }
                    }

                    this.UseSubstituteImage = false;
                }
                #endregion
            }

            /// <summary>
            /// プロパティ:画像矩形
            /// </summary>
            public SubstituteImageRectangle SubstituteImage
            {
                #region 取得
                get
                {
                    return this.substituteImage;
                }
                #endregion

                #region 更新
                set
                {
                    this.DisposeSafely(this.image);
                    this.image = null;

                    base.DisposeSafely(this.TextureBrush);
                    this.TextureBrush = null;

                    this.substituteImage = value;

                    if (null != this.substituteImage)
                    {
                        try
                        {
                            this.imageSize = this.substituteImage.Size.Clone();
                            this.UseSubstituteImage = true;
                        }
                        catch (Exception ex)
                        {
                            this.imageSize = new Size();
                            this.UseSubstituteImage = false;
                            Debug.WriteLine(this, $"画像矩形設定 エラー, [{MethodBase.GetCurrentMethod()?.Name ?? String.Empty}]{ex.Message}");
                        }
                    }
                }
                #endregion
            }

            /// <summary>
            /// プロパティ:画像矩形を使用するかどうか
            /// </summary>
            public bool UseSubstituteImage { get; private set; }

            /// <summary>
            /// プロパティ:表示領域
            /// </summary>
            public DrawingArea DrawingArea { get; set; }

            /// <summary>
            /// プロパティ:領域リスト
            /// </summary>
            public List<DrawingArea> DrawingAreaList { get; private set; }

            /// <summary>
            /// 選択領域インデックス
            /// </summary>
            public int SelectedDrawingAreaIndex { get; set; }

            /// <summary>
            /// 画像描画モード
            /// </summary>
            public ImageDrawingMode DrawingMode
            {
                #region 取得
                get
                {
                    return this.drawingMode;
                }
                #endregion

                #region 更新
                set
                {
                    try
                    {
                        if (value != this.drawingMode)
                        {
                            this.drawingMode = value;

                            base.DisposeSafely(this.TextureBrush);
                            this.TextureBrush = null;

                            if ((ImageDrawingMode.Normal != this.drawingMode) && (null != this.Image))
                            {
                                this.TextureBrush = new TextureBrush(this.Image, WrapMode.Clamp);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.imageSize = new Size();
                        Debug.WriteLine(this, ex.Message);
                    }
                }
                #endregion
            }

            /// <summary>
            /// テクスチャーブラシ
            /// </summary>		
            public TextureBrush? TextureBrush { get; protected set; }

            /// <summary>
            /// プロパティ:画像サイズ
            /// </summary>
            public Size ImageSize
            {
                #region 取得
                get
                {
                    var currentSize = Size.New();

                    try
                    {
                        if (false == this.UseSubstituteImage)
                        {
                            currentSize = this.imageSize.Clone();
                        }
                        else
                        {
                            currentSize = this.SubstituteImage.Size.Clone();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(this, ex.Message);
                    }

                    return currentSize;
                }
                #endregion
            }

            /// <summary>
            /// プロパティ:描画領域サイズ
            /// </summary>
            public Size SurfaceSize
            {
                #region 取得
                get
                {
                    return Size.New(this.surfaceSize);
                }
                #endregion
            }

            #endregion

            #region フィールド

            /// <summary>
            /// 画像データ
            /// </summary>
            private Image? image;

            /// <summary>
            /// 画像矩形
            /// </summary>
            private SubstituteImageRectangle substituteImage;

            /// <summary>
            /// 画像サイズ
            /// </summary>
            private Size imageSize;

            /// <summary>
            /// 描画域サイズ
            /// </summary>
            private Size surfaceSize;

            /// <summary>
            /// 画像描画モード
            /// </summary>
            private ImageDrawingMode drawingMode;

            #endregion

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="windowParent">親ウィンドウのコントロール</param>
            public DrawingInfo(System.Windows.Forms.Control windowParent)
            {
                this.PictureBox = new PictureBox
                {
                    Parent = windowParent,
                    Dock = DockStyle.Fill
                };

                this.substituteImage = new SubstituteImageRectangle();
                this.UseSubstituteImage = false;

                this.imageSize = new Size();
                this.surfaceSize = new Size(this.PictureBox.Width, this.PictureBox.Height);

                this.DrawingArea = new DrawingArea
                {
                    Reciprocal = 0
                };
                this.DrawingArea.Rectangle.Location.X = 0;
                this.DrawingArea.Rectangle.Location.Y = 0;
                this.DrawingArea.Rectangle.Size.Width = this.SurfaceSize.Width;
                this.DrawingArea.Rectangle.Size.Height = this.SurfaceSize.Height;

                this.DrawingAreaList = new List<DrawingArea>();
            }

            #endregion

            #region リソースの解放

            /// <summary>
            /// リソースの解放
            /// </summary>
            protected override void DisposeExplicit()
            {
                try
                {
                    this.DisposeSafely(this.PictureBox);
                    this.DisposeSafely(this.TextureBrush);
                    this.DisposeSafely(this.image);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(this, $"リソース解放 エラー, [{MethodBase.GetCurrentMethod()?.Name ?? String.Empty}]{ex.Message}");
                }
            }

            #endregion

            #region メソッド

            /// <summary>
            /// サイズ変更通知
            /// </summary>
            public void NotifyResize()
            {
                this.surfaceSize = new Size(this.PictureBox.Width, this.PictureBox.Height);
            }

            /// <summary>
            /// 表示倍率逆数取得
            /// </summary>
            /// <param name="horizontal"></param>
            /// <param name="vertical"></param>
            public void GetSurfaceReciprocal(out double horizontal, out double vertical)
            {
                horizontal = (double)this.DrawingArea.Rectangle.Size.Width / this.surfaceSize.Width;
                vertical = (double)this.DrawingArea.Rectangle.Size.Height / this.surfaceSize.Height;
            }

            /// <summary>
            /// 表示倍率取得
            /// </summary>
            /// <param name="horizontal"></param>
            /// <param name="vertical"></param>
            public void GetSurfaceMagnification(out double horizontal, out double vertical)
            {
                horizontal = (double)this.surfaceSize.Width / this.DrawingArea.Rectangle.Size.Width;
                vertical = (double)this.surfaceSize.Height / this.DrawingArea.Rectangle.Size.Height;
            }

            #endregion
        }

        /// <summary>
        /// ドラッグクラスです。
        /// </summary>
        private class DragInfo
        {
            #region プロパティ

            /// <summary>
            /// プロパティ:ドラッグ可否フラグ
            /// </summary>
            public bool Executable { get; private set; }

            /// <summary>
            /// プロパティ:座標
            /// </summary>
            public Point Point { get; private set; }

            /// <summary>
            /// プロパティ:残移動量
            /// </summary>
            public PointD Remaining { get; private set; }

            /// <summary>
            /// プロパティ:ドラッグ有無
            /// </summary>
            public bool IsDragged { get; set; }

            #endregion

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public DragInfo()
            {
                this.Point = Point.New();
                this.Remaining = PointD.New();
            }

            #endregion

            #region パブリックメソッド

            /// <summary>
            /// ドラッグを開始する
            /// </summary>
            /// <param name="location"></param>
            public void StartDrag(System.Drawing.Point location)
            {
                // 現在位置を保持する
                this.Point.Assign(location);

                // 残移動量を初期化する
                this.Remaining = PointD.New();

                // ドラッグ中フラグON
                this.Executable = true;

                // ドラッグ有無OFF
                this.IsDragged = false;
            }

            /// <summary>
            /// ドラッグを終了する
            /// </summary>
            public void EndDrag()
            {
                // ドラッグ中フラグOFF
                this.Executable = false;

                // ドラッグ有無OFF
                this.IsDragged = false;
            }

            #endregion
        }

        /// <summary>
        /// 描画色クラスです。
        /// </summary>
        private class DrawingColors
        {
            #region プロパティ

            /// <summary>
            /// プロパティ:背景色
            /// </summary>
            public Color BackColor { get; set; }

            #endregion

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public DrawingColors()
            {
                this.BackColor = Color.Blue;
            }

            #endregion
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// プロパティ:汎用オブジェクト
        /// </summary>
        public object? Tag { get; set; }

        /// <summary>
        /// プロパティ:描画領域
        /// </summary>
        public DrawingArea Area
        {
            #region 取得
            get
            {
                return this.drawingInfo.DrawingArea.Clone();
            }
            #endregion
        }

        /// <summary>
        /// プロパティ:描画色
        /// </summary>
        public Color BackColor
        {
            #region 取得
            get
            {
                return this.drawingColors.BackColor;
            }
            #endregion

            #region 更新
            set
            {
                this.drawingColors.BackColor = value;
            }
            #endregion
        }

        /// <summary>
        /// プロパティ:描画更新有効無効フラグ
        /// </summary>
        public bool IsLocked { get; protected set; }

        /// <summary>
        /// プロパティ:ドラッグ可否フラグ
        /// </summary>
        public bool MouseDragEnabled { get; set; }

        /// <summary>
        /// プロパティ:ズーム可否フラグ
        /// </summary>
        public bool MouseZoomEnabled { get; set; }

        /// <summary>
        /// マウスエンターイベントで描画コントロールへフォーカスを移すかどうか
        /// </summary>
        public bool IsFocusOnMouseEnter { get; set; }

        /// <summary>
        /// プロパティ:マウスホイールズーム
        /// </summary>
        public ZoomDelta MouseZoomDelta { get; set; }

        /// <summary>
        /// プロパティ:中心線描画設定
        /// </summary>
        public CrossLines CenterLines { get; set; }

        /// <summary>
        /// プロパティ:カーソル線描画設定
        /// </summary>
        public CrossLines CursorLines { get; set; }

        /// <summary>
        /// プロパティ:グリッド線描画設定
        /// </summary>
        public CrossLines GridLines { get; set; }

        /// <summary>
        /// プロパティ:画像サイズ
        /// </summary>
        public Size ImageSize
        {
            #region 取得
            get
            {
                return Size.New(this.drawingInfo.ImageSize);
            }
            #endregion
        }

        /// <summary>
        /// プロパティ:リサイズ有効無効フラグ
        /// </summary
        public bool ResizeEventEnabled
        {
            #region 取得
            get
            {
                return this.resizeEventEnabled;
            }
            #endregion

            #region 更新
            set
            {
                this.resizeEventEnabled = value;

                if (true == this.resizeEventEnabled)
                {
                    this.drawingInfo.PictureBox.Resize += this.resizeEventHandler;

                    if (false == this.drawingInfo.SurfaceSize.Equals(this.drawingInfo.PictureBox.Size))
                    {
                        this.pictureBoxOnResize(this.drawingInfo.PictureBox, new EventArgs());
                    }
                }
                else
                {
                    this.drawingInfo.PictureBox.Resize -= this.resizeEventHandler;
                }
            }
            #endregion
        }

        /// <summary>
        /// 高速スクロール設定
        /// </summary>
        public HighSpeedScrollingSetting HighSpeedScrolling { get; set; }

        /// <summary>
        /// 高速スクロールが有効な場合のズーム回数
        /// </summary>
        /// <remarks>2以上の値を指定します</remarks>
        public int HighSpeedScrollingNumber { get; set; }

        /// <summary>
        /// 画像描画モード
        /// </summary>
        public ImageDrawingMode DrawingMode
        {
            #region 取得
            get
            {
                return this.drawingInfo.DrawingMode;
            }
            #endregion

            #region 更新
            set
            {
                this.drawingInfo.DrawingMode = value;
            }
            #endregion
        }

        #endregion

        #region イベント

        /// <summary>
        /// イベント:マウスムーヴ
        /// </summary>
        public event MouseEventHandler? MouseMove;

        /// <summary>
        /// イベント:マウスクリック
        /// </summary>
        public event MouseEventHandler? MouseClick;

        /// <summary>
        /// イベント:マウスダブルクリック
        /// </summary>
        public event MouseEventHandler? MouseDoubleClick;

        /// <summary>
        /// イベント:マウスアップ
        /// </summary>
        public event MouseEventHandler? MouseUp;

        /// <summary>
        /// イベント:マウスダウン
        /// </summary>
        public event MouseEventHandler? MouseDown;

        /// <summary>
        /// イベント:マウスホイール
        /// </summary>
        public event MouseEventHandler? MouseWheel;

        /// <summary>
        /// イベント:ウィンドウリサイズ
        /// </summary>
        public event EventHandler? Resize;

        /// <summary>
        /// イベント:描画
        /// </summary>
        public event Action<object, Surface>? Drawing;

        /// <summary>
        /// 全体表示割り込みイベントデリゲート
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        public delegate void DrawToFitInterruptEventHandler(object sender, ref Point offset, ref Size size);

        /// <summary>
        /// 全体表示割り込みイベント
        /// </summary>
        public event DrawToFitInterruptEventHandler? DrawToFitInterrupt;

        #endregion

        #region フィールド

        /// <summary>
        /// 描画情報
        /// </summary>
        private readonly DrawingInfo drawingInfo = default!;

        /// <summary>
        /// 描画色
        /// </summary>
        private readonly DrawingColors drawingColors;

        /// <summary>
        /// ドラッグ情報
        /// </summary>
        private readonly DragInfo dragInfo;

        /// <summary>
        /// 同期用オブジェクト
        /// </summary>
        protected readonly object syncRoot = new();

        /// <summary>
        /// リサイズイベントハンドラ
        /// </summary>s
        private readonly EventHandler resizeEventHandler;

        /// <summary>
        /// リサイズイベント有効無効フラグ
        /// </summary>
        private bool resizeEventEnabled;


        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="container">親ウィンドウのコントロール</param>
        /// <param name="imageSize">画像サイズ</param>
        public Renderer(System.Windows.Forms.Control container, Size imageSize)
        {
            // 初期化:描画情報
            this.drawingInfo = new DrawingInfo(container);
            this.drawingColors = new DrawingColors();
            this.dragInfo = new DragInfo();

            // 初期化:画像
            this.drawingInfo.Image = new Bitmap(imageSize.Width, imageSize.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // イベント設定
            this.resizeEventHandler = new System.EventHandler(this.pictureBoxOnResize);
            this.drawingInfo.PictureBox.MouseEnter += new System.EventHandler(this.pictureBoxOnMouseEnter);
            this.drawingInfo.PictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxOnPaint);
            this.drawingInfo.PictureBox.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBoxOnMouseWheel);
            this.drawingInfo.PictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxOnMouseUp);
            this.drawingInfo.PictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxOnMouseDown);
            this.drawingInfo.PictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxOnMouseMove);
            this.drawingInfo.PictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxOnMouseClick);
            this.drawingInfo.PictureBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictureBoxOnMouseDoubleClick);

            // 初期化:プロパティ
            this.DrawingMode = ImageDrawingMode.Balanced;
            this.MouseDragEnabled = true;
            this.MouseZoomEnabled = true;
            this.IsFocusOnMouseEnter = true;
            this.MouseZoomDelta = Renderer.ZoomDelta.Positive;
            this.BackColor = Color.DarkGray;
            this.ResizeEventEnabled = true;
            this.CenterLines = new CrossLines();
            this.CursorLines = new CrossLines();
            this.GridLines = new CrossLines
            {
                UsefulMagnification = 6.0
            };

            this.HighSpeedScrolling = HighSpeedScrollingSetting.Disabled;
            this.HighSpeedScrollingNumber = 2;

            // 表示情報初期化
            initializePart(this.drawingInfo);

            // 画像表示
            this.drawingInfo?.PictureBox.Invalidate();
        }

        #endregion

        #region リソースの解放

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
                this.DisposeSafely(this.drawingInfo);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 画像データを設定します。
        /// </summary>
        /// <param name="image">画像データ</param>
        /// <returns>true:成功 false:失敗</returns>
        public bool CopyFrom(System.Drawing.Bitmap bitmap)
        {
            bool result = false;

            try
            {
                lock (this.syncRoot)
                {
                    // 新しい画像をコピー
                    this.drawingInfo.Image = (System.Drawing.Bitmap)bitmap.Clone();

                    // 描画情報設定
                    initializePart(this.drawingInfo);
                }

                result = true;
            }
            catch (Exception ex)
            {
                // エラー出力
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 画像矩形を設定します。
        /// </summary>
        public bool CopyFrom(Rectangle rectangle, Color fillColor)
        {
            bool result = false;

            try
            {
                lock (this.syncRoot)
                {
                    this.drawingInfo.SubstituteImage = new SubstituteImageRectangle(fillColor);
                    this.drawingInfo.SubstituteImage.Assign(rectangle);

                    // 描画情報設定
                    initializePart(this.drawingInfo);
                }

                result = true;
            }
            catch (Exception ex)
            {
                // エラー出力
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 画像データを取得します。
        /// </summary>
        /// <param name="image">画像データ</param>
        /// <returns>true:成功 false:失敗</returns>
        public bool CopyTo(out System.Drawing.Bitmap? bitmap)
        {
            bitmap = null;

            try
            {
                lock (this.syncRoot)
                {
                    try
                    {
                        if (true == this.drawingInfo.Enabled)
                        {
                            if (false == this.drawingInfo.UseSubstituteImage)
                            {
                                bitmap = (System.Drawing.Bitmap?)this.drawingInfo.Image?.Clone();
                            }
                            else
                            {
                                var substituteImage = this.drawingInfo.SubstituteImage;

                                bitmap = new Bitmap(substituteImage.Size.Width, substituteImage.Size.Height, PixelFormat.Format24bppRgb);
                                using var g = Graphics.FromImage(bitmap);
                                g.FillRectangle(new SolidBrush(substituteImage.Color), substituteImage.ToRectangle());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        bitmap = null;
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                bitmap = null;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return (null != bitmap);
        }

        /// <summary>
        /// 画像を置換します。
        /// </summary>
        /// <param name="image">画像データ</param>
        /// <returns>true:成功 false:失敗</returns>
        public bool Replace(System.Drawing.Bitmap bitmap)
        {
            return this.Replace(bitmap, false);
        }

        /// <summary>
        /// 画像を置換します。
        /// </summary>
        /// <param name="image">画像データ</param>
        /// <returns>true:成功 false:失敗</returns>
        public bool Replace(System.Drawing.Bitmap bitmap, bool keepArea)
        {
            bool result = false;

            try
            {
                lock (this.syncRoot)
                {
                    // 画像サイズ取得
                    var imageSize = new Size(bitmap.Size);

                    #region 現在の画像とサイズが一致している場合
                    if (true == this.drawingInfo.ImageSize.Equals(imageSize))
                    {
                        // 画像設定
                        this.drawingInfo.Image = (System.Drawing.Bitmap)bitmap.Clone();
                    }
                    #endregion

                    #region 表示領域保持する場合
                    else if (true == keepArea)
                    {
                        // 描画条件退避
                        DrawingArea bufArea = this.drawingInfo.DrawingArea.Clone();

                        // 画像設定
                        this.drawingInfo.Image = (System.Drawing.Bitmap)bitmap.Clone();

                        // 描画条件初期化
                        initializePart(this.drawingInfo);

                        // 描画条件変更
                        assignPart(this.drawingInfo, ref bufArea);
                    }
                    #endregion

                    #region 表示領域をリセットする場合
                    else
                    {
                        // 画像設定
                        this.drawingInfo.Image = (System.Drawing.Bitmap)bitmap.Clone();

                        // 描画条件初期化
                        initializePart(this.drawingInfo);
                    }
                    #endregion
                }

                result = true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 画像矩形を置換します。
        /// </summary>
        public bool Replace(Rectangle rectangle, Color fillColor)
        {
            return this.Replace(rectangle, fillColor, false);
        }

        /// <summary>
        /// 画像矩形を置換します。
        /// </summary>
        public bool Replace(Rectangle rectangle, Color fillColor, bool keepArea)
        {
            bool result = false;

            try
            {
                lock (this.syncRoot)
                {
                    // 画像サイズ取得
                    var imageSize = new Size(rectangle.Size);

                    #region 現在の画像とサイズが一致している場合
                    if (true == this.drawingInfo.ImageSize.Equals(imageSize))
                    {
                        // 画像矩形設定
                        this.drawingInfo.SubstituteImage = new SubstituteImageRectangle(fillColor);
                        this.drawingInfo.SubstituteImage.Assign(rectangle);
                    }
                    #endregion

                    #region 表示領域保持する場合
                    else if (true == keepArea)
                    {
                        // 描画条件退避
                        DrawingArea bufArea = this.drawingInfo.DrawingArea.Clone();

                        // 画像矩形設定
                        this.drawingInfo.SubstituteImage = new SubstituteImageRectangle(fillColor);
                        this.drawingInfo.SubstituteImage.Assign(rectangle);

                        // 描画条件初期化
                        initializePart(this.drawingInfo);

                        // 描画条件変更
                        assignPart(this.drawingInfo, ref bufArea);
                    }
                    #endregion

                    #region 表示領域をリセットする場合
                    else
                    {
                        // 画像矩形設定
                        this.drawingInfo.SubstituteImage = new SubstituteImageRectangle(fillColor);
                        this.drawingInfo.SubstituteImage.Assign(rectangle);

                        // 描画条件初期化
                        initializePart(this.drawingInfo);
                    }
                    #endregion
                }

                result = true;
            }
            catch (Exception ex)
            {
                // エラー出力
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 描画します。
        /// </summary>
        public void Draw()
        {
            try
            {
                lock (this.syncRoot)
                {
                    // 全体表示割り込みイベントが設定されている場合
                    if (null != this.DrawToFitInterrupt)
                    {
                        // 初期表示倍率の場合
                        if (0 == this.drawingInfo.SelectedDrawingAreaIndex)
                        {
                            // 表示領域サイズ取得
                            var regionSize = this.drawingInfo.DrawingArea.Rectangle.Size;

                            // 画像サイズを取得
                            var imageSize = this.drawingInfo.ImageSize;

                            // 初期表示領域オフセットを取得
                            var regionOffset = new Point
                            {
                                X = 0 - (regionSize.Width - imageSize.Width) / 2,
                                Y = 0 - (regionSize.Height - imageSize.Height) / 2
                            };

                            // 初期表示領域オフセットと同じ場合
                            if (true == this.drawingInfo.DrawingArea.Rectangle.Location.Equals(regionOffset))
                            {
                                // 全体表示割り込み処理
                                this.interruptDrawToFit(this.drawingInfo);
                            }
                        }
                    }
                }

                this.drawingInfo.PictureBox.Invalidate();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }


        /// <summary>
        /// 描画します。
        /// </summary>
        public void DrawToFit()
        {
            try
            {
                lock (this.syncRoot)
                {
                    // 全体表示割り込みイベントが設定されていない場合
                    if (null == this.DrawToFitInterrupt)
                    {
                        // 全体表示
                        fitPart(this.drawingInfo);
                    }
                    else
                    {
                        // 全体表示割り込み処理
                        this.interruptDrawToFit(this.drawingInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.drawingInfo.PictureBox.Invalidate();
            }
        }

        /// <summary>
        /// 描画情報を設定します。
        /// </summary>
        /// <param name="area">描画領域</param>
        public void DrawWith(ref DrawingArea area)
        {
            try
            {
                lock (this.syncRoot)
                {
                    assignPart(this.drawingInfo, ref area);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.drawingInfo.PictureBox.Invalidate();
            }
        }

        /// <summary>
        /// 描画情報を設定します。
        /// </summary>
        /// <param name="offset">描画領域の始点</param>
        /// <param name="size">描画領域の幅と高さ</param>
        public void DrawWith(ref System.Drawing.Point location, ref System.Drawing.Size size)
        {
            try
            {
                var locationTemp = new Point(location);
                var sizeTemp = new Size(size);
                this.DrawWith(ref locationTemp, ref sizeTemp);

                location.X = locationTemp.X;
                location.Y = locationTemp.Y;

                size.Width = sizeTemp.Width;
                size.Height = sizeTemp.Height;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 描画情報を設定します。
        /// </summary>
        /// <param name="offset">描画領域の始点</param>
        /// <param name="size">描画領域の幅と高さ</param>
        public void DrawWith(ref Point location, ref Size size)
        {
            try
            {
                lock (this.syncRoot)
                {
                    assignPart(this.drawingInfo, ref location, ref size);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.drawingInfo.PictureBox.Invalidate();
            }
        }

        /// <summary>
        /// 初期表示領域情報取得
        /// </summary>
        /// <returns></returns>
        public DrawingArea GetInitialDrawingArea()
        {
            var initialArea = this.Area;

            try
            {
                initialArea = this.drawingInfo.DrawingAreaList[0].Clone();

                /// 表示領域サイズを取得
                var regionSize = initialArea.Rectangle.Size;

                /// 画像サイズを取得
                var imageSize = this.drawingInfo.ImageSize;

                /// 表示領域オフセットを計算
                var regionOffset = new Point
                {
                    X = 0 - (regionSize.Width - imageSize.Width) / 2,
                    Y = 0 - (regionSize.Height - imageSize.Height) / 2
                };

                initialArea.Rectangle.Assign(regionOffset, regionSize);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return initialArea;
        }

        /// <summary>
        /// 描画更新を無効にします。
        /// </summary>
        public void Lock()
        {
            this.IsLocked = true;

        }

        /// <summary>
        /// 描画更新を有効にします。
        /// </summary>
        public void UnLock()
        {
            this.IsLocked = false;

            // ドラッグ終了
            this.dragInfo.EndDrag();
        }

        /// <summary>
        /// 画素値取得
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <returns></returns>
        public bool GetImagePixel(int x, int y, out int red, out int green, out int blue)
        {
            var result = this.GetImagePixel(x, y, out Color color);

            int value = color.ToArgb();
            red = (value >> 16) & 0xFF;
            green = (value >> 8) & 0xFF;
            blue = (value >> 0) & 0xFF;

            return result;
        }

        /// <summary>
        /// 画素値取得
        /// </summary>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <returns>画像輝度</returns>
        public bool GetImagePixel(int x, int y, out System.Drawing.Color color)
        {
            bool result = false;

            color = System.Drawing.Color.Black;

            try
            {
                lock (this.syncRoot)
                {
                    if (false == this.drawingInfo.UseSubstituteImage)
                    {
                        if (this.drawingInfo.Image is Bitmap bitmap)
                        {
                            color = bitmap.GetPixel(x, y);
                            result = true;
                        }
                    }
                    else
                    {
                        color = this.drawingInfo.SubstituteImage.Color;
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// マウス座標における画像座標を取得します。
        /// </summary>
        /// <param name="x">マウスのX座標</param>
        /// <param name="y">マウスのY座標</param>
        /// <returns>true:成功 false:失敗</returns>
        public bool GetMousePosition(int mouseX, int mouseY, out int locationX, out int locationY)
        {
            var result = this.GetMousePosition(mouseX, mouseY, out double locationXTemp, out double locationYTemp);

            locationX = (int)locationXTemp;
            locationY = (int)locationYTemp;

            return result;
        }

        /// <summary>
        /// マウス座標における画像座標を取得します。
        /// </summary>
        /// <param name="x">マウスのX座標</param>
        /// <param name="y">マウスのY座標</param>
        /// <returns>true:成功 false:失敗</returns>
        public bool GetMousePosition(int mouseX, int mouseY, out double locationX, out double locationY)
        {
            bool result = false;

            locationX = 0;
            locationY = 0;

            try
            {
                var currentArea = this.drawingInfo.DrawingArea.Clone();

                this.drawingInfo.GetSurfaceReciprocal(out double reciprocalH, out double reciprocalV);
                locationX = currentArea.Rectangle.Location.X + mouseX * reciprocalH;
                locationY = currentArea.Rectangle.Location.Y + mouseY * reciprocalV;

                result = true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// 表示倍率逆数取得
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        public void GetSurfaceReciprocal(out double horizontal, out double vertical)
        {
            this.drawingInfo.GetSurfaceReciprocal(out horizontal, out vertical);
        }

        /// <summary>
        /// 表示倍率取得
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        public void GetSurfaceMagnification(out double horizontal, out double vertical)
        {
            this.drawingInfo.GetSurfaceMagnification(out horizontal, out vertical);
        }

        #endregion

        #region プライベートメソッド

        /// <summary>
        /// 描画情報を初期化します。
        /// </summary>
        /// <param name="setup">描画情報</param>
        private static void initializePart(DrawingInfo? setup)
        {
            // 画像データが存在しない場合(無効情報)
            if (null == setup || false == setup.Enabled)
            {
                // 初期化:描画情報
                setup?.DrawingAreaList.Clear();

                // 例外を投げる
                throw new Exception("no image");
            }

            // 画像サイズを取得
            var imageSize = Size.New(setup.ImageSize);

            // ウィンドウサイズを取得
            var windowSize = Size.New(setup.SurfaceSize);

            // ウィンドウアスペクト比を取得
            var ratioWindowWidth = (double)windowSize.Width / windowSize.Height;
            var ratioWindowHeight = (double)windowSize.Height / windowSize.Width;

            // ウィンドウと画像サイズ比を取得
            var ratioImageWidth = (double)imageSize.Width / windowSize.Width;
            var ratioImageHeight = (double)imageSize.Height / windowSize.Height;

            // 初期倍率を設定
            var defaultReciprocal = (ratioImageWidth > ratioImageHeight) ? ratioImageWidth : ratioImageHeight;

            // 最大倍率を設定
            var minimumReciprocal = (ratioWindowWidth >= 1) ? (1.0 / windowSize.Height) : (1.0 / windowSize.Width);

            // 表示領域登録(初期倍率から最大倍率まで、段階的に拡大しながら表示領域を登録)
            var tempArea = new DrawingArea
            {
                Reciprocal = defaultReciprocal
            };
            setup.DrawingAreaList.Clear();
            do
            {
                // (横 ≧ 縦)の場合
                if (ratioWindowWidth >= 1)
                {
                    // 表示領域サイズを設定
                    tempArea.Rectangle.Size.Height = (int)(imageSize.Height + (windowSize.Height * tempArea.Reciprocal - imageSize.Height));
                    tempArea.Rectangle.Size.Width = (int)(tempArea.Rectangle.Size.Height * ratioWindowWidth);
                }
                // (縦 > 横)の場合
                else
                {
                    // 表示領域サイズを設定
                    tempArea.Rectangle.Size.Width = (int)(imageSize.Width + (windowSize.Width * tempArea.Reciprocal - imageSize.Width));
                    tempArea.Rectangle.Size.Height = (int)(tempArea.Rectangle.Size.Width * ratioWindowHeight);
                }

                // 表示領域を登録
                setup.DrawingAreaList.Add(tempArea.Clone());

                // 次に登録すべき倍率に変更
                tempArea.Reciprocal *= 0.9;
            }
            while (tempArea.Reciprocal > minimumReciprocal);

            // 初期表示領域を選択
            setup.SelectedDrawingAreaIndex = 0;
            setup.DrawingArea = setup.DrawingAreaList[setup.SelectedDrawingAreaIndex].Clone();

            // 表示領域サイズ取得
            var regionSize = setup.DrawingArea.Rectangle.Size;

            // 表示領域オフセットを設定
            var regionOffset = new Point
            {
                X = 0 - (regionSize.Width - imageSize.Width) / 2,
                Y = 0 - (regionSize.Height - imageSize.Height) / 2
            };

            // 表示領域を設定
            setup.DrawingArea.Rectangle.Location.Assign(regionOffset);
        }

        /// <summary>
        /// 描画設定を行います。
        /// </summary>
        /// <param name="setup">描画情報</param>
        /// <param name="point">座標</param>
        /// <param name="zoom">拡大/縮小</param>
        /// <returns>true:成功 false:失敗</returns>
        private static bool zoomPart(DrawingInfo setup, Point point, ZoomDirection zoom)
        {
            // 表示領域を退避
            var currentArea = new DrawingArea(setup.DrawingArea);

            // マウスカーソルの画像上の座標を取得
            var imagePoint = new Point
            {
                X = (int)(currentArea.Rectangle.Location.X + point.X * currentArea.Reciprocal),
                Y = (int)(currentArea.Rectangle.Location.Y + point.Y * currentArea.Reciprocal)
            };

            // 縮小の場合
            if (zoom == ZoomDirection.Out)
            {
                // 初期倍率の場合
                if (setup.SelectedDrawingAreaIndex - 1 < 0)
                {
                    // 倍率を更新しないでreturn
                    return true;
                }

                // 倍率を更新する
                setup.SelectedDrawingAreaIndex--;
            }
            // 拡大の場合
            else
            {
                // 最大倍率の場合
                if (setup.SelectedDrawingAreaIndex + 1 >= setup.DrawingAreaList.Count)
                {
                    // 倍率を更新しないでreturn
                    return false;
                }

                //// 倍率を更新する
                setup.SelectedDrawingAreaIndex++;
            }

            // 新しい表示領域を取得
            setup.DrawingArea = new DrawingArea(setup.DrawingAreaList[setup.SelectedDrawingAreaIndex]);

            // 新しい表示領域を計算
            var regionOffset = new Point
            {
                X = (int)System.Math.Ceiling(imagePoint.X - point.X * setup.DrawingArea.Reciprocal),
                Y = (int)System.Math.Ceiling(imagePoint.Y - point.Y * setup.DrawingArea.Reciprocal)
            };

            // 表示領域を設定
            setup.DrawingArea.Rectangle.Location.Assign(regionOffset);
            return true;
        }

        /// <summary>
        /// 描画設定を行います。
        /// </summary>
        /// <param name="setup">描画情報</param>
        /// <param name="amount">座標移動量</param>
        /// <param name="remaining">座標移動量の残</param>
        /// <returns>true:成功 false:失敗</returns>
        private static bool movePart(DrawingInfo setup, Point amount, ref PointD remaining)
        {
            // 表示領域を取得
            var currentArea = new DrawingArea(setup.DrawingArea);

            // 表示倍率を取得
            setup.GetSurfaceReciprocal(out double reciprocalH, out double reciprocalV);

            // 画像上の移動量を取得
            var imageMove = new PointD(amount);
            imageMove.X *= reciprocalH;
            imageMove.Y *= reciprocalV;
            imageMove.Offset(remaining);

            // 移動量が0でないかチェック
            if ((System.Math.Abs(imageMove.X) <= 0)
            && (System.Math.Abs(imageMove.Y) <= 0))
            {
                return false;
            }

            // 表示領域を計算
            var regionOffset = new Point(currentArea.Rectangle.Location);
            regionOffset.X -= (int)imageMove.X;
            regionOffset.Y -= (int)imageMove.Y;

            // 残り量を更新
            remaining.X = imageMove.X - (int)imageMove.X;
            remaining.Y = imageMove.Y - (int)imageMove.Y;

            // 表示領域を設定
            setup.DrawingArea.Rectangle.Location.Assign(regionOffset);
            return true;
        }

        /// <summary>
        /// 描画条件を全体表示に変更します。
        /// </summary>
        /// <param name="setup">描画情報</param>
        private static void fitPart(DrawingInfo setup)
        {
            // 初期表示領域インデックスを選択
            setup.SelectedDrawingAreaIndex = 0;

            // 新しい表示領域を取得
            setup.DrawingArea = new DrawingArea(setup.DrawingAreaList[setup.SelectedDrawingAreaIndex]);

            // 表示領域サイズ取得
            var regionSize = setup.DrawingArea.Rectangle.Size;

            // 画像サイズを取得
            var imageSize = setup.ImageSize;

            // 表示領域オフセットを設定
            var regionOffset = new Point
            {
                X = 0 - (regionSize.Width - imageSize.Width) / 2,
                Y = 0 - (regionSize.Height - imageSize.Height) / 2
            };

            // 表示領域を設定
            setup.DrawingArea.Rectangle.Location.Assign(regionOffset);
        }

        /// <summary>
        /// 描画条件を指定領域に変更します。
        /// </summary>
        /// <param name="setup">描画情報</param>
        /// <param name="area">表示領域</param>
        private static void assignPart(DrawingInfo setup, ref DrawingArea area)
        {
            // 適正倍率を検索
            int matchIndex;
            for (matchIndex = 0; matchIndex < setup.DrawingAreaList.Count; matchIndex++)
            {
                //// 前回の倍率以上の倍率の中で最小の倍率を検索結果とする
                if (setup.DrawingAreaList[matchIndex].Reciprocal <= area.Reciprocal)
                {
                    break;
                }
            }

            // 適正倍率が見つからなかった場合
            if (matchIndex >= setup.DrawingAreaList.Count)
            {
                //// 最大倍率とする
                matchIndex = setup.DrawingAreaList.Count - 1;
            }

            // 選択領域インデックスを変更
            setup.SelectedDrawingAreaIndex = matchIndex;

            // 新しい表示領域を取得
            setup.DrawingArea = new DrawingArea(setup.DrawingAreaList[setup.SelectedDrawingAreaIndex]);

            // 指定領域の中心座標を取得
            var centerPoint = new Point(area.Rectangle.Location);
            centerPoint.X += area.Rectangle.Size.Width / 2;
            centerPoint.Y += area.Rectangle.Size.Height / 2;

            // 表示領域を計算
            setup.DrawingArea.Rectangle.Location.Assign(centerPoint);
            setup.DrawingArea.Rectangle.Location.X -= area.Rectangle.Size.Width / 2;
            setup.DrawingArea.Rectangle.Location.Y -= area.Rectangle.Size.Height / 2;

            // 表示領域を設定
            area.Assign(setup.DrawingArea);
        }

        /// <summary>
        /// 描画条件を指定領域に変更します。
        /// </summary>
        /// <param name="setup">描画情報</param>
        /// <param name="offset">表示領域の左上座標</param>
        /// <param name="size">表示領域の幅と高さ</param>
        private static void assignPart(DrawingInfo setup, ref Point location, ref Size size)
        {
            // 適正倍率検索
            int matchIndex;
            for (matchIndex = setup.DrawingAreaList.Count - 1; matchIndex >= 0; matchIndex--)
            {
                // 領域が含まれるものを検索結果とする
                if ((size.Width <= setup.DrawingAreaList[matchIndex].Rectangle.Size.Width)
                && (size.Height <= setup.DrawingAreaList[matchIndex].Rectangle.Size.Height))
                {
                    break;
                }
            }

            // 適正倍率が見つからなかった場合
            if (matchIndex < 0)
            {
                // 初期倍率とする
                matchIndex = 0;
            }

            // 指定領域の中心座標を取得
            var centerPoint = new Point(location);
            centerPoint.X += size.Width / 2;
            centerPoint.Y += size.Height / 2;

            // 選択領域インデックスを変更
            setup.SelectedDrawingAreaIndex = matchIndex;

            // 新しい表示領域を取得
            setup.DrawingArea = new DrawingArea(setup.DrawingAreaList[setup.SelectedDrawingAreaIndex]);

            // 表示領域を計算
            setup.DrawingArea.Rectangle.Location.Assign(centerPoint);
            setup.DrawingArea.Rectangle.Location.X -= size.Width / 2;
            setup.DrawingArea.Rectangle.Location.Y -= size.Height / 2;

            // 表示領域を設定
            location.Assign(setup.DrawingArea.Rectangle.Location);
            size.Assign(setup.DrawingArea.Rectangle.Size);
        }

        /// <summary>
        /// 全体表示割り込み処理
        /// </summary>
        private void interruptDrawToFit(DrawingInfo setup)
        {
            // 表示領域サイズ取得
            var regionSize = setup.DrawingAreaList[0].Rectangle.Size.Clone();

            // 画像サイズを取得
            var imageSize = setup.ImageSize;

            // 表示領域オフセットを設定
            var regionOffset = new Point
            {
                X = 0 - (regionSize.Width - imageSize.Width) / 2,
                Y = 0 - (regionSize.Height - imageSize.Height) / 2
            };

            try
            {
                // イベント通知
                this.DrawToFitInterrupt?.Invoke(this, ref regionOffset, ref regionSize);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            // 領域指定
            assignPart(setup, ref regionOffset, ref regionSize);
        }

        /// <summary>
        /// 描画イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBoxOnPaint(object? sender, System.Windows.Forms.PaintEventArgs e)
        {
            var currentArea = (DrawingArea?)null;
            var surface = (Surface?)null;

            lock (this.syncRoot)
            {
                try
                {
                    // 現在の描画領域を取得する
                    currentArea = this.drawingInfo.DrawingArea.Clone();

                    // 表示倍率を取得する                  
                    this.drawingInfo.GetSurfaceMagnification(out double magnificationH, out double magnificationV);

                    // 描画設定
                    e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

                    // 背景色で塗りつぶし
                    e.Graphics.Clear(this.drawingColors.BackColor);

                    if (true == this.drawingInfo.UseSubstituteImage)
                    {
                        #region 矩形の描画

                        // 転送元領域
                        var srcRect = new System.Drawing.Rectangle(
                                                                    currentArea.Rectangle.Location.X
                                                                  , currentArea.Rectangle.Location.Y
                                                                  , currentArea.Rectangle.Size.Width
                                                                  , currentArea.Rectangle.Size.Height
                                                                  );

                        // 画像領域を作成する
                        var imgRect = new Rectangle(new Point(), this.drawingInfo.ImageSize);

                        // 描画範囲に画像が存在する場合
                        if (true == imgRect.IntersectsWith(currentArea.Rectangle))
                        {
                            // 画像切り出し領域を作成する        
                            var crpRect = currentArea.GetImageArea(this.drawingInfo.ImageSize);

                            // 転送先領域を再設定する
                            var dstRect = new System.Drawing.Rectangle
                            {
                                Location = new System.Drawing.Point(Convert.ToInt32((crpRect.Location.X - srcRect.Location.X) * magnificationH), Convert.ToInt32((crpRect.Location.Y - srcRect.Location.Y) * magnificationV)),
                                Size = new System.Drawing.Size(Convert.ToInt32(crpRect.Size.Width * magnificationH), Convert.ToInt32(crpRect.Size.Height * magnificationV))
                            };

                            // 転送
                            e.Graphics.FillRectangle(new SolidBrush(this.drawingInfo.SubstituteImage.Color), dstRect);
                        }

                        #endregion
                    }
                    else if (null != this.drawingInfo.Image)
                    {
                        #region 画像の描画

                        // 転送先領域
                        var dstRect = new System.Drawing.Rectangle(
                                                                    0
                                                                  , 0
                                                                  , this.drawingInfo.SurfaceSize.Width
                                                                  , this.drawingInfo.SurfaceSize.Height
                                                                   );

                        // 転送元領域
                        var srcRect = new System.Drawing.Rectangle(
                                                                    currentArea.Rectangle.Location.X
                                                                  , currentArea.Rectangle.Location.Y
                                                                  , currentArea.Rectangle.Size.Width
                                                                  , currentArea.Rectangle.Size.Height
                                                                  );

                        // 画像描画がNormalモードの場合
                        if (ImageDrawingMode.Normal == this.drawingInfo.DrawingMode)
                        {
                            // 転送
                            e.Graphics.DrawImage(this.drawingInfo.Image, dstRect, srcRect, System.Drawing.GraphicsUnit.Pixel);
                        }
                        // 画像描画がSpeedOrientedモードの場合
                        else if (ImageDrawingMode.SpeedOriented == this.drawingInfo.DrawingMode)
                        {
                            var textureBrush = this.drawingInfo.TextureBrush;

                            if (null != textureBrush)
                            {
                                textureBrush.ResetTransform();
                                textureBrush.ScaleTransform((float)magnificationH, (float)magnificationV);
                                textureBrush.TranslateTransform((float)(-srcRect.X), (float)(-srcRect.Y));

                                e.Graphics.FillRectangle(textureBrush, dstRect);
                            }
                        }
                        // 画像描画がBalancedモードの場合
                        else if (ImageDrawingMode.Balanced == this.drawingInfo.DrawingMode)
                        {
                            // 縮小表示の場合
                            if ((1.0 > magnificationH)
                            && (1.0 > magnificationV))
                            {
                                var textureBrush = this.drawingInfo.TextureBrush;

                                if (null != textureBrush)
                                {
                                    textureBrush.ResetTransform();
                                    textureBrush.ScaleTransform((float)magnificationH, (float)magnificationV);
                                    textureBrush.TranslateTransform((float)(-srcRect.X), (float)(-srcRect.Y));

                                    e.Graphics.FillRectangle(textureBrush, dstRect);
                                }
                            }
                            // 等倍以上の場合
                            else
                            {
                                // 画像領域を作成する
                                var imgRect = new Rectangle(new Point(), this.drawingInfo.ImageSize);

                                // 描画範囲に画像が存在する場合
                                if (true == imgRect.IntersectsWith(currentArea.Rectangle) && (this.drawingInfo.Image is Bitmap srcBitmap))
                                {
                                    // 画像切り出し領域を作成する        
                                    var crpRect = currentArea.GetImageArea(this.drawingInfo.ImageSize);

                                    // 描画範囲のビットマップを切り出す
                                    using var cropImage = srcBitmap.Clone(crpRect.ToRectangle(), srcBitmap.PixelFormat);
                                    // 転送先領域を再設定する
                                    dstRect.Location = new System.Drawing.Point(Convert.ToInt32((crpRect.Location.X - srcRect.Location.X) * magnificationH), Convert.ToInt32((crpRect.Location.Y - srcRect.Location.Y) * magnificationV));
                                    dstRect.Size = new System.Drawing.Size(Convert.ToInt32(crpRect.Size.Width * magnificationH), Convert.ToInt32(crpRect.Size.Height * magnificationV));

                                    // 転送元領域を再設定する
                                    srcRect.Location = new System.Drawing.Point();
                                    srcRect.Size = crpRect.Size.ToSize();

                                    // 転送
                                    e.Graphics.DrawImage(cropImage, dstRect, srcRect, System.Drawing.GraphicsUnit.Pixel);
                                }
                            }
                        }

                        #endregion
                    }

                    // 描画サーフェイスを取得する                  
                    surface = new Surface(e.Graphics, currentArea.Rectangle.Location, currentArea.Rectangle.Size, magnificationH, magnificationV);

                    #region オプションの描画
                    {
                        var imageSize = this.drawingInfo.ImageSize;

                        var magnification = System.Math.Max(magnificationH, magnificationV);

                        #region グリッド線描画
                        if (
                            (null != this.GridLines)
                         && (true == this.GridLines.Enabled)
                         && (magnification >= this.GridLines.UsefulMagnification)
                        )
                        {
                            double horizontalBegin = 0;
                            double horizontalEnd = imageSize.Width;
                            double verticalBegin = 0;
                            double verticalEnd = imageSize.Height;

                            surface.Pen = new Pen(this.GridLines.Color, this.GridLines.LineWidth);

                            // 水平線描画が有効な場合
                            if (true == this.GridLines.HorizontalLineEnabled)
                            {
                                for (int y = 0; y < verticalEnd; y++)
                                {
                                    // 水平線を描画
                                    surface.DrawLine(horizontalBegin, y, horizontalEnd, y);
                                }
                            }
                            // 垂直線描画が有効な場合
                            if (true == this.GridLines.VerticalLineEnabled)
                            {
                                for (int x = 0; x < horizontalEnd; x++)
                                {
                                    // 垂直線を描画
                                    surface.DrawLine(x, verticalBegin, x, verticalEnd);
                                }
                            }
                        }
                        #endregion

                        #region 中心線描画
                        if (
                            (null != this.CenterLines)
                        && (true == this.CenterLines.Enabled)
                        )
                        {
                            double horizontalBegin = 0;
                            double horizontalEnd = imageSize.Width;
                            double verticalBegin = 0;
                            double verticalEnd = imageSize.Height;

                            surface.Pen = new Pen(this.CenterLines.Color, this.CenterLines.LineWidth);
                            surface.Pen.DashStyle = DashStyle.DashDot;
                            //surface.Pen.DashPattern = new[] { 4.0F, 4.0F };

                            // 水平線描画が有効な場合
                            if (true == this.CenterLines.HorizontalLineEnabled)
                            {
                                // 水平線を描画
                                surface.DrawLine(horizontalBegin, imageSize.Height / 2.0, horizontalEnd, imageSize.Height / 2.0);
                            }
                            // 垂直線描画が有効な場合
                            if (true == this.CenterLines.VerticalLineEnabled)
                            {
                                // 垂直線を描画
                                surface.DrawLine(imageSize.Width / 2.0, verticalBegin, imageSize.Width / 2.0, verticalEnd);
                            }
                        }
                        #endregion

                        #region カーソル線描画
                        if (
                            (null != this.CursorLines)
                        && (true == this.CursorLines.Enabled)
                        )
                        {
                            // マウスカーソル位置取得
                            var cursorPoint = this.drawingInfo.PictureBox.PointToClient(System.Windows.Forms.Control.MousePosition);

                            if (true == this.drawingInfo.PictureBox.ClientRectangle.Contains(cursorPoint))
                            {
                                this.GetMousePosition(cursorPoint.X, cursorPoint.Y, out double locationX, out double locationY);

                                double horizontalBegin = surface.Location.X;
                                double horizontalEnd = horizontalBegin + surface.Size.Width;
                                double verticalBegin = surface.Location.Y;
                                double verticalEnd = verticalBegin + surface.Size.Height;

                                surface.Pen = new Pen(this.CursorLines.Color, this.CursorLines.LineWidth);
                                surface.Pen.DashStyle = DashStyle.Solid;

                                // 水平線描画が有効な場合
                                if (true == this.CursorLines.HorizontalLineEnabled)
                                {
                                    // 水平線を描画
                                    surface.DrawLine(horizontalBegin, locationY, horizontalEnd, locationY);
                                }

                                // 垂直線描画が有効な場合
                                if (true == this.CursorLines.VerticalLineEnabled)
                                {
                                    // 垂直線を描画
                                    surface.DrawLine(locationX, verticalBegin, locationX, verticalEnd);
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
                finally
                {
                    if (null != surface)
                    {
                        surface.Pen.DashStyle = DashStyle.Solid;
                    }
                }
            }

            #region イベント通知
            try
            {
                if (null != surface)
                {
                    this.Drawing?.Invoke(this, surface);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            #endregion
        }

        /// <summary>
        /// コントロールリサイズのイベントです。
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベントデータ</param>
        private void pictureBoxOnResize(object? sender, System.EventArgs e)
        {
            #region 表示リセット
            try
            {
                lock (this.syncRoot)
                {
                    // リサイズ通知
                    this.drawingInfo.NotifyResize();

                    // 表示情報初期化
                    initializePart(this.drawingInfo);

                    // 全体表示割り込みイベントが設定されている場合
                    if (null != this.DrawToFitInterrupt)
                    {
                        this.interruptDrawToFit(this.drawingInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.drawingInfo.PictureBox.Invalidate();
            }
            #endregion

            #region イベント通知
            try
            {
                this.Resize?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            #endregion
        }

        /// <summary>
        /// マウスダウンのイベントです。
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベントデータ</param>
        private void pictureBoxOnMouseDown(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            #region ドラッグ開始
            if (
                (false == this.IsLocked)
            && (true == this.MouseDragEnabled)
            && (MouseButtons.Left == e.Button)
            )
            {
                // ドラッグ開始
                this.dragInfo.StartDrag(e.Location);

                // カーソルアイコン変更
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
            }
            #endregion

            #region イベント通知
            try
            {
                this.MouseDown?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            #endregion
        }

        /// <summary>
        /// マウスアップのイベントです。
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベントデータ</param>
        private void pictureBoxOnMouseUp(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            #region ドラッグ終了
            {
                // ドラッグ終了
                this.dragInfo.EndDrag();

                // カーソルアイコン変更
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
            #endregion

            #region イベント通知
            try
            {
                this.MouseUp?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            #endregion
        }

        /// <summary>
        /// マウスムーヴのイベントです。
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベントデータ</param>
        private void pictureBoxOnMouseMove(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            #region ドラッグ中の場合は表示範囲変更
            if (
                (true == this.drawingInfo.Enabled)
            && (true == this.dragInfo.Executable)
            )
            {
                var changedPart = false;
                try
                {
                    lock (this.syncRoot)
                    {
                        // カーソル移動量
                        var moveAmount = new Point(e.Location);
                        moveAmount.X -= this.dragInfo.Point.X;
                        moveAmount.Y -= this.dragInfo.Point.Y;

                        // 移動量の残りを取得する
                        var remaining = this.dragInfo.Remaining.Clone();

                        // 領域の移動												
                        changedPart = movePart(this.drawingInfo, moveAmount, ref remaining);

                        // 移動量の残りを更新する
                        this.dragInfo.Remaining.Assign(remaining);
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
                finally
                {
                    // 表示領域の変更ありの場合
                    if (true == changedPart)
                    {
                        // ドラッグ座標更新
                        this.dragInfo.Point.Assign(e.Location);
                        this.dragInfo.IsDragged = true;

                        // 更新通知
                        this.drawingInfo.PictureBox.Invalidate();
                    }
                }
            }
            #endregion

            #region カーソル線描画
            if (
                (null != this.CursorLines)
            && (true == this.CursorLines.Enabled)
            )
            {
                this.Draw();
            }
            #endregion

            #region イベント通知
            try
            {
                this.MouseMove?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            #endregion
        }

        /// <summary>
        /// マウスエンターのイベントです。
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベントデータ</param>
        private void pictureBoxOnMouseEnter(object? sender, System.EventArgs e)
        {
            #region フォーカスを設定
            try
            {
                if (true == this.IsFocusOnMouseEnter)
                {
                    var activeForm = Form.ActiveForm;

                    if (null != activeForm)
                    {
                        if (true == activeForm.Equals(this.drawingInfo.ParentForm))
                        {
                            this.drawingInfo.PictureBox.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            #endregion
        }

        /// <summary>
        /// マウスホイールのイベントです。
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベントデータ</param>
        private void pictureBoxOnMouseWheel(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            #region 拡大/縮小
            if (
                (true == this.drawingInfo.Enabled)
            && (false == this.IsLocked)
            && (true == this.MouseZoomEnabled)
            && (true == this.drawingInfo.PictureBox.ClientRectangle.Contains(e.Location))
            )
            {
                var changedPart = false;

                try
                {
                    lock (this.syncRoot)
                    {
                        ZoomDirection zoom;
                        if (Renderer.ZoomDelta.Negative == this.MouseZoomDelta)
                        {
                            zoom = (e.Delta > 0) ? ZoomDirection.Out : ZoomDirection.In;
                        }
                        else
                        {
                            zoom = (e.Delta < 0) ? ZoomDirection.Out : ZoomDirection.In;
                        }

                        int zoomNumber = 1;

                        if (HighSpeedScrollingSetting.Enabled == this.HighSpeedScrolling)
                        {
                            zoomNumber = this.HighSpeedScrollingNumber;
                        }
                        else if (HighSpeedScrollingSetting.EnabledWithControlKey == this.HighSpeedScrolling)
                        {
                            if (Keys.Control == (Control.ModifierKeys & Keys.Control))
                            {
                                zoomNumber = this.HighSpeedScrollingNumber;
                            }
                        }

                        for (int i = 0; i < zoomNumber; i++)
                        {
                            changedPart = zoomPart(this.drawingInfo, new Point(e.Location), zoom);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
                finally
                {
                    if (true == changedPart)
                    {
                        this.drawingInfo.PictureBox.Invalidate();
                    }
                }
            }
            #endregion

            #region イベント通知
            try
            {
                this.MouseWheel?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            #endregion
        }

        /// <summary>
        /// マウスクリックのイベントです。
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベントデータ</param>
        private void pictureBoxOnMouseClick(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            #region イベント通知
            try
            {
                this.drawingInfo.PictureBox.Focus();

                if (false == this.dragInfo.IsDragged)
                {
                    this.MouseClick?.Invoke(this, e);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            #endregion
        }

        /// <summary>
        /// マウスダブルクリックのイベントです。
        /// </summary>
        /// <param name="sender">イベント送信元</param>
        /// <param name="e">イベントデータ</param>
        private void pictureBoxOnMouseDoubleClick(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            #region イベント通知
            try
            {
                this.drawingInfo.PictureBox.Focus();

                this.MouseDoubleClick?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            #endregion
        }

        #endregion
    }
}