using System.Drawing.Drawing2D;

namespace Hutzper.Library.Common.Drawing
{
    /// <summary>
    /// 描画座標変換
    /// </summary>
    /// <remarks>Rendererクラスを使用した描画を行う場合に使用できます</remarks>
    [Serializable]
    public class RenderingCoordConverter
    {
        #region プロパティ

        /// <summary>
        /// 描画インスタンスへの参照
        /// </summary>
        public Renderer Renderer { get; protected set; }

        /// <summary>
        /// コンテナ
        /// </summary>
        public Control Container { get; set; }

        /// <summary>
        /// 描画域サイズ
        /// </summary>
        public SizeD SurfaceSize { get; protected set; }

        /// <summary>
        /// 描画域倍率
        /// </summary>
        public PointD SurfaceReciprocal { get; protected set; } = new PointD(1, 1);

        /// <summary>
        /// 変換比率
        /// </summary>
        public PointD ConversionRatio { get; protected set; } = new PointD(1, 1);

        /// <summary>
        /// 描画範囲
        /// </summary>
        public RectangleD ClipRectangle { get; protected set; }

        /// <summary>
        /// 座標軸の方向
        /// </summary>
        public Dictionary<CoordSystem.Axis, CoordSystem.AxisOrientation> AxisOrientation
        {
            get => new Dictionary<CoordSystem.Axis, CoordSystem.AxisOrientation>()
            {
                {CoordSystem.Axis.X, this.xAxisOrientation}
            ,   {CoordSystem.Axis.Y, this.yAxisOrientation}
            };
        }

        /// <summary>
        /// 変換行列
        /// </summary>
        public Matrix ConversionMatrix { get; protected set; } = new Matrix();

        /// <summary>
        /// 逆変換行列
        /// </summary>
        public Matrix ConversionMatrixInverted { get; protected set; } = new Matrix();

        /// <summary>
        /// X軸方向係数
        /// </summary>
        public double XAxisOrientationCoef { get; protected set; } = 1;

        /// <summary>
        /// Y軸方向係数
        /// </summary>
        public double YAxisOrientationCoef { get; protected set; } = 1;

        /// <summary>
        /// タグ
        /// </summary>
        public object? Tag { get; set; }

        #endregion

        #region フィールド

        CoordSystem.AxisOrientation xAxisOrientation = CoordSystem.AxisOrientation.Positive;
        CoordSystem.AxisOrientation yAxisOrientation = CoordSystem.AxisOrientation.Positive;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="renderer"></param>
        public RenderingCoordConverter(Renderer renderer, Control container)
        {
            this.Renderer = renderer;
            this.Container = container;
            this.SurfaceSize = new SizeD(this.Container.Size);
            this.ClipRectangle = new RectangleD(PointD.New(), this.SurfaceSize);
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="offsetCoordPix">原点座標</param>
        /// <param name="conversionRatio">変換比率</param>
        /// <param name="XAxisOrientation">X軸方向</param>
        /// <param name="YAxisOrientation">Y軸方向</param>
        public void Initialize(
                                Point offsetCoordPix
                              , Size clipRightBottom
                              , CoordSystem.AxisOrientation xAxisOrientation
                              , CoordSystem.AxisOrientation yAxisOrientation
                              , Size dataRangeSize
                              , Size surfaceSize
                              )
        {
            // データ描画比率
            var conversionRatio = new PointD(surfaceSize.Width - offsetCoordPix.X - clipRightBottom.Width, surfaceSize.Height - offsetCoordPix.Y - clipRightBottom.Height);
            conversionRatio.X /= dataRangeSize.Width;
            conversionRatio.Y /= dataRangeSize.Height;

            // 初期化
            this.Initialize(offsetCoordPix, clipRightBottom, xAxisOrientation, yAxisOrientation, surfaceSize, conversionRatio);
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="offsetCoordPix">原点座標</param>
        /// <param name="conversionRatio">変換比率</param>
        /// <param name="XAxisOrientation">X軸方向</param>
        /// <param name="YAxisOrientation">Y軸方向</param>
        public void Initialize(
                              Point offsetCoordPix
                            , Size clipRightBottom
                            , CoordSystem.AxisOrientation xAxisOrientation
                            , CoordSystem.AxisOrientation yAxisOrientation
                            , Size surfaceSize
                            , PointD conversionRatio
                            )
        {
            // 軸の向き
            this.xAxisOrientation = xAxisOrientation;
            this.yAxisOrientation = yAxisOrientation;

            this.XAxisOrientationCoef = (CoordSystem.AxisOrientation.Positive == this.xAxisOrientation) ? 1d : -1d;
            this.YAxisOrientationCoef = (CoordSystem.AxisOrientation.Positive == this.yAxisOrientation) ? 1d : -1d;

            // 描画原点と描画範囲
            this.ClipRectangle.Left = (CoordSystem.AxisOrientation.Positive == this.xAxisOrientation) ? offsetCoordPix.X : surfaceSize.Width - offsetCoordPix.X;
            this.ClipRectangle.Top = (CoordSystem.AxisOrientation.Positive == this.yAxisOrientation) ? offsetCoordPix.Y : surfaceSize.Height - offsetCoordPix.Y;
            this.ClipRectangle.Size.Width = (surfaceSize.Width - offsetCoordPix.X - clipRightBottom.Width) * this.XAxisOrientationCoef;
            this.ClipRectangle.Size.Height = (surfaceSize.Height - offsetCoordPix.Y - clipRightBottom.Height) * this.YAxisOrientationCoef;

            // データ描画比率
            this.ConversionRatio = conversionRatio;

            // 描画情報更新
            this.UpdateRenderingInfo();
        }

        /// <summary>
        /// 描画情報の更新
        /// </summary>
        /// <remarks>座標変換メソッドを使用する前にこのメソッドを呼び出してください</remarks>
        public void UpdateRenderingInfo()
        {
            // 描画域サイズを取得する
            this.SurfaceSize = new SizeD(this.Container.Size);

            // 現在の描画倍率(分母)を取得する
            var rrx = 0d;
            var rry = 0d;
            this.Renderer.GetSurfaceReciprocal(out rrx, out rry);
            this.SurfaceReciprocal = new PointD(rrx, rry);

            // 座標変換行列の作成
            this.ConversionMatrix.Reset();
            this.ConversionMatrix.Translate((float)this.ClipRectangle.Left, (float)this.ClipRectangle.Top);
            this.ConversionMatrix.Scale((float)(this.ConversionRatio.X / this.SurfaceReciprocal.X * this.XAxisOrientationCoef), (float)(this.ConversionRatio.Y / this.SurfaceReciprocal.Y * this.YAxisOrientationCoef));

            // 座標変換逆行列の作成
            this.ConversionMatrixInverted = this.ConversionMatrix.Clone();
            this.ConversionMatrixInverted.Invert();
        }

        #region 実座標を描画座標に変換

        public PointD ConvertToDrawing(double x, double y)
        {
            return this.Convert(new PointD(x, y), this.ConversionMatrix);
        }

        public double ConvertToDrawingX(double x)
        {
            return this.Convert(new PointD(x, 0), this.ConversionMatrix).X;
        }

        public double ConvertToDrawingY(double y)
        {
            return this.Convert(new PointD(0, y), this.ConversionMatrix).Y;
        }

        /// <summary>
        /// 描画座標系におけるX長さに変換
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double ConvertToDrawingLengthX(double x) => this.ConvertToDrawing(new SizeD(x, 0)).Width;

        /// <summary>
        /// 描画座標系におけるY長さに変換
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public double ConvertToDrawingLengthY(double y) => this.ConvertToDrawing(new SizeD(0, y)).Height;

        /// <summary>
        /// 実座標を描画座標に変換 PointD
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public PointD ConvertToDrawing(PointD location) => this.Convert(location, this.ConversionMatrix);

        /// <summary>
        /// 実座標を描画座標に変換 PointD []
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public PointD[] ConvertToDrawing(PointD[] location) => this.Convert(location, this.ConversionMatrix);

        /// <summary>
        /// 実座標を描画座標に変換 Point
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Point ConvertToDrawing(Point location)
        {
            var transformed = this.Convert(new PointD(location), this.ConversionMatrix);

            return new Point((int)transformed.X, (int)transformed.Y);
        }

        /// <summary>
        /// 実座標を描画座標に変換 Point[]
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Point[] ConvertToDrawing(Point[] location)
        {
            var transformed = this.ConvertToDrawing(Array.ConvertAll(location, p => new PointD(p)));

            return Array.ConvertAll(transformed, p => new Point((int)p.X, (int)p.Y));
        }

        /// <summary>
        /// 実サイズを描画サイズに変換
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public SizeD ConvertToDrawing(SizeD size)
        {
            var transformed = size.Clone();
            transformed.Width *= this.ConversionRatio.X / this.SurfaceReciprocal.X;
            transformed.Height *= this.ConversionRatio.Y / this.SurfaceReciprocal.Y;

            return transformed;
        }

        /// <summary>
        /// 実サイズを描画サイズに変換
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public SizeD[] ConvertToDrawing(SizeD[] size)
        {
            var transformed = Array.ConvertAll(size, s => new SizeD(s.Width * this.ConversionRatio.X / this.SurfaceReciprocal.X, s.Height * this.ConversionRatio.Y / this.SurfaceReciprocal.Y));

            return transformed;
        }

        /// <summary>
        /// 実サイズを描画サイズに変換
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public Size ConvertToDrawing(Size size)
        {
            var transformed = size.Clone();
            transformed.Width = (int)(transformed.Width * this.ConversionRatio.X / this.SurfaceReciprocal.X);
            transformed.Height = (int)(transformed.Height * this.ConversionRatio.Y / this.SurfaceReciprocal.Y);

            return transformed;
        }

        /// <summary>
        /// 実サイズを描画サイズに変換
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public Size[] ConvertToDrawing(Size[] size)
        {
            var transformed = Array.ConvertAll(size, s => new Size((int)(s.Width * this.ConversionRatio.X / this.SurfaceReciprocal.X), (int)(s.Height * this.ConversionRatio.Y / this.SurfaceReciprocal.Y)));

            return transformed;
        }

        /// <summary>
        /// 実座標を描画座標に変換 RectangleD
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public RectangleD ConvertToDrawing(RectangleD rect)
        {
            var transformed = this.ConvertToDrawing(new[] { rect });

            return transformed.First();
        }

        /// <summary>
        /// 実座標を描画座標に変換 RectangleD[]
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public RectangleD[] ConvertToDrawing(RectangleD[] rect) => this.Convert(rect, this.ConversionMatrix);

        /// <summary>
        /// 実座標を描画座標に変換 Rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Rectangle ConvertToDrawing(Rectangle rect)
        {
            var transformed = this.ConvertToDrawing(new[] { rect });

            return transformed.First();
        }

        /// <summary>
        /// 実座標を描画座標に変換 Rectangle[]
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Rectangle[] ConvertToDrawing(Rectangle[] rect)
        {
            var transformed = this.ConvertToDrawing(Array.ConvertAll(rect, r => new RectangleD(r)));

            return Array.ConvertAll(transformed, r => new Rectangle(new Point((int)r.Left, (int)r.Top), new Size((int)r.Size.Width, (int)r.Size.Height)));
        }

        #endregion

        #region 描画座標を実座標に変換

        /// <summary>
        /// 描画座標を実座標に変換
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public PointD ConvertToReal(double x, double y)
        {
            return this.Convert(new PointD(x, y), this.ConversionMatrixInverted);
        }

        public double ConvertToRealX(double x)
        {
            return this.Convert(new PointD(x, 0), this.ConversionMatrixInverted).X;
        }

        public double ConvertToRealY(double y)
        {
            return this.Convert(new PointD(0, y), this.ConversionMatrixInverted).Y;
        }

        /// <summary>
        /// 実座標系における長さXに変換
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double ConvertToRealLengthX(double x) => this.ConvertToReal(new SizeD(x, 0)).Width;

        /// <summary>
        /// 実座標系における長さYに変換
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public double ConvertToRealLengthY(double y) => this.ConvertToReal(new SizeD(0, y)).Height;

        /// <summary>
        /// 描画座標を実座標に変換
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public PointD ConvertToReal(PointD location) => this.Convert(location, this.ConversionMatrixInverted);

        /// <summary>
        /// 描画座標を実座標に変換
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public PointD[] ConvertToReal(PointD[] location) => this.Convert(location, this.ConversionMatrixInverted);

        /// <summary>
        /// 実座標を描画座標に変換 Point
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Point ConvertToReal(Point location)
        {
            var transformed = this.Convert(new PointD(location), this.ConversionMatrixInverted);

            return new Point((int)transformed.X, (int)transformed.Y);
        }

        /// <summary>
        /// 実座標を描画座標に変換 Point[]
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Point[] ConvertToReal(Point[] location)
        {
            var transformed = this.ConvertToReal(Array.ConvertAll(location, p => new PointD(p)));

            return Array.ConvertAll(transformed, p => new Point((int)p.X, (int)p.Y));
        }

        /// <summary>
        /// 描画サイズを実サイズに変換
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public SizeD ConvertToReal(SizeD size)
        {
            var transformed = new SizeD();
            transformed.Width = size.Width / this.ConversionRatio.X * this.SurfaceReciprocal.X;
            transformed.Height = size.Height / this.ConversionRatio.Y * this.SurfaceReciprocal.Y;

            return transformed;
        }

        /// <summary>
        /// 描画サイズを実サイズに変換
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public SizeD[] ConvertToReal(SizeD[] size)
        {
            var transformed = Array.ConvertAll(size, s => new SizeD(s.Width / this.ConversionRatio.X * this.SurfaceReciprocal.X, s.Height / this.ConversionRatio.Y * this.SurfaceReciprocal.Y));

            return transformed;
        }

        /// <summary>
        /// 描画サイズを実サイズに変換
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public Size ConvertToReal(Size size)
        {
            var transformed = new Size();
            transformed.Width = (int)(size.Width / this.ConversionRatio.X * this.SurfaceReciprocal.X);
            transformed.Height = (int)(size.Height / this.ConversionRatio.Y * this.SurfaceReciprocal.Y);

            return transformed;
        }

        /// <summary>
        /// 描画サイズを実サイズに変換
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public Size[] ConvertToReal(Size[] size)
        {
            var transformed = Array.ConvertAll(size, s => new Size((int)(s.Width / this.ConversionRatio.X * this.SurfaceReciprocal.X), (int)(s.Height / this.ConversionRatio.Y * this.SurfaceReciprocal.Y)));

            return transformed;
        }

        /// <summary>
        /// 描画座標を実座標に変換 RectangleD
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public RectangleD ConvertToReal(RectangleD rect)
        {
            var transformed = this.ConvertToReal(new[] { rect });

            return transformed.First();
        }

        /// <summary>
        /// 描画座標を実座標に変換 RectangleD[]
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public RectangleD[] ConvertToReal(RectangleD[] rect) => this.Convert(rect, this.ConversionMatrixInverted);

        /// <summary>
        /// 描画座標を実座標に変換 Rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Rectangle ConvertToReal(Rectangle rect)
        {
            var transformed = this.ConvertToReal(new[] { rect });

            return transformed.First();
        }

        /// <summary>
        /// 描画座標を実座標に変換 Rectangle[]
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Rectangle[] ConvertToReal(Rectangle[] rect)
        {
            var transformed = this.ConvertToReal(Array.ConvertAll(rect, r => new RectangleD(r)));

            return Array.ConvertAll(transformed, r => new Rectangle(new Point((int)r.Left, (int)r.Top), new Size((int)r.Size.Width, (int)r.Size.Height)));
        }

        #endregion

        #region 固定値に変換(描画倍率によらない画素数を得る)

        public double ConvertToFixedX(double x, bool withOrientation)
        {
            return x * this.SurfaceReciprocal.X * (withOrientation ? this.XAxisOrientationCoef : 1);
        }

        public double ConvertToFixedY(double y, bool withOrientation)
        {
            return y * this.SurfaceReciprocal.Y * (withOrientation ? this.YAxisOrientationCoef : 1);
        }

        public SizeD ConvertToFixed(SizeD size, bool withOrientation)
        {
            var coefX = 1d;
            var coefY = 1d;
            if (true == withOrientation)
            {
                coefX = this.XAxisOrientationCoef;
                coefY = this.YAxisOrientationCoef;
            }

            return new SizeD(size.Width * this.SurfaceReciprocal.X * coefX, size.Height * this.SurfaceReciprocal.Y * coefY);
        }

        public Size ConvertToFixed(Size size, bool withOrientation)
        {
            var coefX = 1d;
            var coefY = 1d;
            if (true == withOrientation)
            {
                coefX = this.XAxisOrientationCoef;
                coefY = this.YAxisOrientationCoef;
            }

            return new Size((int)(size.Width * this.SurfaceReciprocal.X * coefX), (int)(size.Height * this.SurfaceReciprocal.Y * coefY));
        }

        #endregion

        #region 汎用メソッド

        /// <summary>
        /// 指定された行列で座標をアフィン変換 PointD
        /// </summary>
        /// <param name="location"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public PointD Convert(PointD location, Matrix matrix)
        {
            var transformed = new[] { location.ToPointF() };
            matrix.TransformPoints(transformed);

            return new PointD(transformed[0]);
        }

        /// <summary>
        /// 指定された行列で座標をアフィン変換 PointD[]
        /// </summary>
        /// <param name="location"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public PointD[] Convert(PointD[] location, Matrix matrix)
        {
            var transformed = Array.ConvertAll(location, p => p.ToPointF());
            matrix.TransformPoints(transformed);

            return Array.ConvertAll(transformed, p => new PointD(p));
        }

        /// <summary>
        /// 指定された行列で座標をアフィン変換 PointF
        /// </summary>
        /// <param name="location"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public PointF Convert(PointF location, Matrix matrix)
        {
            var transformed = new[] { location };
            matrix.TransformPoints(transformed);

            return transformed[0];
        }

        /// <summary>
        /// 指定された行列で座標をアフィン変換 PointF[]
        /// </summary>
        /// <param name="location"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public PointF[] Convert(PointF[] location, Matrix matrix)
        {
            var transformed = new PointF[location.Length];
            Array.Copy(location, transformed, transformed.Length);

            matrix.TransformPoints(transformed);

            return transformed;
        }

        /// <summary>
        /// 指定された行列で座標をアフィン変換 RectangleD[]
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public RectangleD[] Convert(RectangleD[] rect, Matrix matrix)
        {
            var transformed = new List<RectangleD>();

            foreach (var r in rect)
            {
                // 4座標化して変換
                var rectPointsTransformed = this.Convert(GraphicsUtilities.RectangleToPoints(r), matrix);

                // 再矩形化
                transformed.Add(GraphicsUtilities.PointsToRectangle(rectPointsTransformed));
            }

            return transformed.ToArray();
        }

        #endregion

        #endregion
    }
}