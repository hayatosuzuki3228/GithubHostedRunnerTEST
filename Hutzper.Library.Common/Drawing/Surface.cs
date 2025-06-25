using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace Hutzper.Library.Common.Drawing
{
    /// <summary>
    /// 描画サーフェイス
    /// </summary>
    public class Surface : SafelyDisposable
    {
        #region サブクラス

        /// <summary>
        /// 描画モード
        /// </summary>
        public enum DrawingMode
        {
            Outline,
            Fill,
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 同期用オブジェクト
        /// </summary>
        public readonly object SyncRoot;

        /// <summary>
        /// 表示領域位置
        /// </summary>
        public readonly Point Location;

        /// <summary>
        /// 表示領域サイズ
        /// </summary>
        public readonly Size Size;

        /// <summary>
        /// 水平方向表示倍率
        /// </summary>
        public readonly double ScalingH;

        /// <summary>
        /// 垂直方向表示倍率
        /// </summary>
        public readonly double ScalingV;

        /// <summary>
        /// 描画モード
        /// </summary>
        public DrawingMode Mode { get; set; }

        /// <summary>
        /// ペン
        /// </summary>
        public System.Drawing.Pen Pen
        {
            #region 取得
            get
            {
                return this.pen;
            }
            #endregion

            #region 更新
            set
            {
                using var old = this.pen;
                this.pen = (System.Drawing.Pen)value.Clone();
            }
            #endregion
        }

        /// <summary>
        /// ブラシ
        /// </summary>
        public System.Drawing.Brush Brush
        {
            #region 取得
            get
            {
                return this.brush;
            }
            #endregion

            #region 更新
            set
            {
                using var old = this.brush;
                this.brush = (System.Drawing.Brush)value.Clone();
            }
            #endregion
        }

        /// <summary>
        /// フォント
        /// </summary>
        public System.Drawing.Font Font
        {
            #region 取得
            get
            {
                return this.font;
            }
            #endregion

            #region 更新
            set
            {
                using var old = this.font;
                this.font = (System.Drawing.Font)value.Clone();
            }
            #endregion
        }

        /// <summary>
        /// 座標変換行列
        /// </summary>
        /// <remarks>取得したMatrixインスタンスは使用後にDisposeしてください</remarks>
        public Matrix CodeTransformMatrix => this.codeTransformMatrix.Clone();

        #endregion

        #region フィールド

        /// <summary>
        /// 描画
        /// </summary>
        private readonly System.Drawing.Graphics graphics;

        /// <summary>
        /// ペン
        /// </summary>
        private System.Drawing.Pen pen;

        /// <summary>
        /// ブラシ
        /// </summary>
        private System.Drawing.Brush brush;

        /// <summary>
        /// フォント
        /// </summary>
        private System.Drawing.Font font;

        /// <summary>
        /// オフセット値
        /// </summary>
        private PointF offset;

        /// <summary>
        /// 座標変換行列
        /// </summary>
        private readonly Matrix codeTransformMatrix = new();

        #endregion

        #region コンストラクタ

        public Surface(Graphics graphics, Point location, Size size, double ScalingH, double ScalingV)
        : this(graphics, location, size, ScalingH, ScalingV, SystemFonts.DefaultFont)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="location"></param>
        /// <param name="size"></param>
        /// <param name="Sscaling"></param>
        public Surface(Graphics graphics, Point location, Size size, double scalingH, double scalingV, System.Drawing.Font font)
        {
            this.graphics = graphics;
            this.graphics.SmoothingMode = SmoothingMode.AntiAlias;

            this.SyncRoot = new object();
            this.Location = new Point(location);
            this.Size = new Size(size);
            this.ScalingH = scalingH;
            this.ScalingV = scalingV;

            this.pen = new Pen(Color.Black);
            this.brush = new SolidBrush(Color.Black);
            this.font = (Font)font.Clone();

            this.offset = new PointF(this.Location.X * -1, this.Location.Y * -1);

            this.codeTransformMatrix.Scale((float)this.ScalingH, (float)this.ScalingV);
            this.codeTransformMatrix.Translate(this.offset.X, this.offset.Y);

            this.graphics.Transform = this.codeTransformMatrix;
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
                this.DisposeSafely(this.graphics);
                this.DisposeSafely(this.pen);
                this.DisposeSafely(this.brush);
                this.DisposeSafely(this.font);
                this.DisposeSafely(this.codeTransformMatrix);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 真円を描画する
        /// </summary>
        /// <param name="x">中心座標x</param>
        /// <param name="y">中心座標y</param>
        /// <param name="radius">半径</param>
        public void DrawCircle(double x, double y, double radius)
        {
            var rectangleF = new RectangleF(new PointF((float)(x - radius), (float)(y - radius)), new SizeF((float)radius * 2f, (float)radius * 2f));

            if (DrawingMode.Outline == this.Mode)
            {
                var w = this.pen.Width;
                this.pen.Width = (float)(w / System.Math.Max(this.ScalingH, this.ScalingV));

                this.graphics.DrawEllipse(this.pen, rectangleF);

                this.pen.Width = w;
            }
            else
            {
                this.graphics.FillEllipse(this.brush, rectangleF);
            }
        }

        /// <summary>
        /// 楕円を描画する
        /// </summary>
        /// <param name="x">中心座標x</param>
        /// <param name="y">中心座標y</param>
        /// <param name="width">楕円を定義する外接する四角形の幅</param>
        /// <param name="height">楕円を定義する外接する四角形の高さ</param>
        public void DrawEllipse(double x, double y, double width, double height)
        {
            var rectangleF = new RectangleF(new PointF((float)(x - width / 2f), (float)(y - height / 2f)), new SizeF((float)width, (float)height));

            if (DrawingMode.Outline == this.Mode)
            {
                var w = this.pen.Width;
                this.pen.Width = (float)(w / System.Math.Max(this.ScalingH, this.ScalingV));

                this.graphics.DrawEllipse(this.pen, rectangleF);

                this.pen.Width = w;
            }
            else
            {
                this.graphics.FillEllipse(this.brush, rectangleF);
            }
        }

        /// <summary>
        /// 矩形を描画する
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        public void DrawRectangle1(Point location, Size size) => this.DrawRectangle1(location.X, location.Y, size.Width, size.Height);

        /// <summary>
        /// 矩形を描画する
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        public void DrawRectangle1(PointD location, SizeD size) => this.DrawRectangle1(location.X, location.Y, size.Width, size.Height);

        /// <summary>
        /// 矩形を描画する
        /// </summary>
        /// <param name="rectangle"></param>
        public void DrawRectangle1(Rectangle rectangle) => this.DrawRectangle1(rectangle.Left, rectangle.Top, rectangle.Size.Width, rectangle.Size.Height);

        /// <summary>
        /// 矩形を描画する
        /// </summary>
        /// <param name="rectangle"></param>
        public void DrawRectangle1(RectangleD rectangle) => this.DrawRectangle1(rectangle.Left, rectangle.Top, rectangle.Size.Width, rectangle.Size.Height);

        /// <summary>
        /// 矩形を描画する
        /// </summary>
        /// <param name="rectangle"></param>
        public void DrawRectangle1(System.Drawing.Rectangle rectangle) => this.DrawRectangle1(rectangle.Left, rectangle.Top, rectangle.Size.Width, rectangle.Size.Height);

        /// <summary>
        /// 矩形を描画する
        /// </summary>
        /// <param name="x">左上隅のX座標</param>
        /// <param name="y">左上隅のY座標</param>
        /// <param name="width">矩形の幅</param>
        /// <param name="height">矩形の高さ</param>
        public void DrawRectangle1(double x, double y, double width, double height)
        {
            var rectangleF = new RectangleF(new PointF((float)x, (float)y), new SizeF((float)width, (float)height));

            if (DrawingMode.Outline == this.Mode)
            {
                var w = this.pen.Width;
                this.pen.Width = (float)(w / System.Math.Max(this.ScalingH, this.ScalingV));

                this.graphics.DrawRectangle(this.pen, rectangleF.Left, rectangleF.Top, rectangleF.Width, rectangleF.Height);

                this.pen.Width = w;
            }
            else
            {
                this.graphics.FillRectangle(this.brush, rectangleF);
            }
        }

        /// <summary>
        /// 角度付き矩形を描画する
        /// </summary>
        /// <param name="x">中心座標x</param>
        /// <param name="y">中心座標y</param>
        /// <param name="deg">角度</param>
        /// <param name="length1">長辺の半分</param>
        /// <param name="length2">短辺の半分</param>
        public void DrawRectangle2(double x, double y, double deg, double length1, double length2)
        {
            var points = new PointF[4];

            points[0] = new PointF((float)(x - length1), (float)(y - length2));
            points[1] = new PointF((float)(x + length1), (float)(y - length2));
            points[2] = new PointF((float)(x + length1), (float)(y + length2));
            points[3] = new PointF((float)(x - length1), (float)(y + length2));

            using var matrix = new Matrix();
            matrix.RotateAt((float)deg, new PointF((float)x, (float)y));
            matrix.Translate(this.offset.X, this.offset.Y);
            matrix.Scale((float)this.ScalingH, (float)this.ScalingV);

            this.graphics.Transform = matrix;

            if (DrawingMode.Outline == this.Mode)
            {
                var w = this.pen.Width;
                this.pen.Width = (float)(w / System.Math.Max(this.ScalingH, this.ScalingV));

                this.graphics.DrawPolygon(this.pen, points);

                this.pen.Width = w;
            }
            else
            {
                this.graphics.FillPolygon(this.brush, points);
            }

            this.graphics.Transform = this.codeTransformMatrix;
        }

        /// <summary>
        /// 線分を描画する
        /// </summary>
        /// <param name="x1">線の開始点のX座標</param>
        /// <param name="y1">線の開始点のY座標</param>
        /// <param name="x2">線の終点のX座標</param>
        /// <param name="y2">線の終点のY座標</param>
        public void DrawLine(double x1, double y1, double x2, double y2)
        {
            var w = this.pen.Width;
            this.pen.Width = (float)(w / System.Math.Max(this.ScalingH, this.ScalingV));

            this.graphics.DrawLine(this.pen, new PointF((float)x1, (float)y1), new PointF((float)x2, (float)y2));

            this.pen.Width = w;
        }

        /// <summary>
        /// 線分を描画する
        /// </summary>
        /// <param name="pointS">線の開始点</param>
        /// <param name="pointE">線の終点</param>
        public void DrawLine(PointD pointS, PointD pointE)
        {
            var w = this.pen.Width;
            this.pen.Width = (float)(w / System.Math.Max(this.ScalingH, this.ScalingV));

            this.graphics.DrawLine(this.pen, pointS.ToPointF(), pointE.ToPointF());

            this.pen.Width = w;
        }

        /// <summary>
        /// 配列を接続する一連の線分を描画する
        /// </summary>
        /// <param name="points">座標の配列</param>
        public void DrawLines(PointF[] points)
        {
            var w = this.pen.Width;
            this.pen.Width = (float)(w / System.Math.Max(this.ScalingH, this.ScalingV));

            this.graphics.DrawLines(this.pen, points);

            this.pen.Width = w;
        }

        /// <summary>
        /// 配列を接続する一連の線分を描画する
        /// </summary>
        /// <param name="points">座標の配列</param>
        public void DrawLines(System.Drawing.Point[] points)
        {
            var w = this.pen.Width;
            this.pen.Width = (float)(w / System.Math.Max(this.ScalingH, this.ScalingV));

            this.graphics.DrawLines(this.pen, points);

            this.pen.Width = w;
        }

        /// <summary>
        /// 配列を接続する一連の線分を描画する
        /// </summary>
        /// <param name="x">X座標の配列</param>
        /// <param name="y">Y座標の配列</param>
        public void DrawLines(double[] x, double[] y)
        {
            var pointsF = x.Zip(y, (px, py) => new PointF((float)px, (float)py)).ToArray();

            var w = this.pen.Width;
            this.pen.Width = (float)(w / System.Math.Max(this.ScalingH, this.ScalingV));

            this.graphics.DrawLines(this.pen, pointsF);

            this.pen.Width = w;
        }

        /// <summary>
        /// 配列で定義された多角形を描画する
        /// </summary>
        /// <param name="points">座標の配列</param>
        public void DrawPolygon(PointF[] points)
        {
            if (DrawingMode.Outline == this.Mode)
            {
                var w = this.pen.Width;
                this.pen.Width = (float)(w / System.Math.Max(this.ScalingH, this.ScalingV));

                this.graphics.DrawPolygon(this.pen, points);

                this.pen.Width = w;
            }
            else
            {
                this.graphics.FillPolygon(this.brush, points);
            }
        }

        /// <summary>
        /// 配列で定義された多角形を描画する
        /// </summary>
        /// <param name="points">座標の配列</param>
        public void DrawPolygon(System.Drawing.Point[] points)
        {
            var pointsF = Array.ConvertAll(points, p => new PointF(p.X, p.Y));

            if (DrawingMode.Outline == this.Mode)
            {
                var w = this.pen.Width;
                this.pen.Width = (float)(w / System.Math.Max(this.ScalingH, this.ScalingV));

                this.graphics.DrawPolygon(this.pen, pointsF);

                this.pen.Width = w;
            }
            else
            {
                this.graphics.FillPolygon(this.brush, pointsF);
            }
        }

        /// <summary>
        /// 配列で定義された多角形を描画する
        /// </summary>
        /// <param name="x">X座標の配列</param>
        /// <param name="y">Y座標の配列</param>
        public void DrawPolygon(double[] x, double[] y)
        {
            var pointsF = x.Zip(y, (px, py) => new PointF((float)px, (float)py)).ToArray();

            if (DrawingMode.Outline == this.Mode)
            {
                var w = this.pen.Width;
                this.pen.Width = (float)(w / System.Math.Max(this.ScalingH, this.ScalingV));

                this.graphics.DrawPolygon(this.pen, pointsF);

                this.pen.Width = w;
            }
            else
            {
                this.graphics.FillPolygon(this.brush, pointsF);
            }
        }

        /// <summary>
        /// Region の内部を塗りつぶします
        /// </summary>
        /// <param name="region">塗りつぶす領域を表す</param>
        public void FillRegion(System.Drawing.Region region)
        {
            this.graphics.FillRegion(this.brush, region);
        }

        /// <summary>
        /// GraphicsPath描画
        /// </summary>
        /// <param name="path"></param>
        public void FillPath(GraphicsPath path)
        {
            this.graphics.FillPath(this.brush, path);
        }

        /// <summary>
        /// GraphicsPath描画
        /// </summary>
        /// <param name="path"></param>
        public void DrawPath(GraphicsPath path)
        {
            var w = this.pen.Width;
            this.pen.Width = (float)(w / System.Math.Max(this.ScalingH, this.ScalingV));

            this.graphics.DrawPath(this.pen, path);

            this.pen.Width = w;
        }

        /// <summary>
        /// 丸角GraphicsPath
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public GraphicsPath GetRoundedPath(RectangleD rect, float radius)
        {
            var path = new GraphicsPath();
            var curveSize = radius;

            var rectF = rect.ToRectangleF();

            path.StartFigure();
            path.AddArc(rectF.X, rectF.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rectF.Right - curveSize, rectF.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rectF.Right - curveSize, rectF.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rectF.X, rectF.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();

            return path;
        }


        /// <summary>
        /// 画像の一部を描画する
        /// </summary>
        /// <param name="image">描画画像</param>
        /// <param name="x">描画領域：左上隅のX座標</param>
        /// <param name="y">描画領域：左上隅のY座標</param>
        public void DrawImage(Image image, double x, double y)
        {
            this.graphics.DrawImage(image, new RectangleF((float)x, (float)y, image.Width, image.Height));
        }


        /// <summary>
        /// 画像の一部を描画する
        /// </summary>
        /// <param name="image">描画画像</param>
        /// <param name="x">描画領域：左上隅のX座標</param>
        /// <param name="y">描画領域：左上隅のY座標</param>
        /// <param name="width">描画領域：幅</param>
        /// <param name="height">描画領域：高さ</param>
        /// <param name="cutX">描画する image オブジェクトの部分：左上隅のX座標</param>
        /// <param name="cutY">描画する image オブジェクトの部分：左上隅のY座標</param>
        /// <param name="cutWidth">描画する image オブジェクトの部分：幅</param>
        /// <param name="cutHeight">描画する image オブジェクトの部分：高さ</param>
        public void DrawImage(Image image
                                    , double x, double y, double width, double height
                                    , double cutX, double cutY, double cutWidth, double cutHeight)
        {
            var rectangleDst = new RectangleF(new PointF((float)x, (float)y), new SizeF((float)width, (float)height));
            var rectangleSrc = new RectangleF(new PointF((float)cutX, (float)cutY), new SizeF((float)cutWidth, (float)cutHeight));

            this.graphics.DrawImage(image, rectangleDst, rectangleSrc, GraphicsUnit.Pixel);
        }

        /// <summary>
        /// 文字を描画する
        /// </summary>
        /// <param name="text">描画する文字列</param>
        /// <param name="p">描画するテキストの左上隅の座標を指定する</param>
        public void WriteString(string text, PointD p)
        {
            this.WriteString(text, p.X, p.Y);
        }

        /// <summary>
        /// 文字を描画する
        /// </summary>
        /// <param name="text">描画する文字列</param>
        /// <param name="p">描画するテキストの左上隅の座標を指定する</param>
        /// <param name="format">描画するテキストに適用する行間や配置などの書式属性を指定する</param>
        public void WriteString(string text, PointD p, StringFormat format)
        {
            this.WriteString(text, p.X, p.Y, format);
        }

        /// <summary>
        /// 文字を描画する
        /// </summary>
        /// <param name="text">描画する文字列</param>
        /// <param name="x">描画するテキストの左上隅のX座標を指定する</param>
        /// <param name="y">描画するテキストの左上隅のY座標を指定する</param>
        public void WriteString(string text, double x, double y)
        {
            var fh = this.font.Size;
            this.Font = new Font(this.font.FontFamily, (float)(fh / System.Math.Max(this.ScalingH, this.ScalingV)));

            this.graphics.DrawString(text, this.font, this.brush, new PointF((float)x, (float)y));

            this.Font = new Font(this.font.FontFamily, fh);
        }

        /// <summary>
        /// 文字を描画する
        /// </summary>
        /// <param name="text">描画する文字列</param>
        /// <param name="x">描画するテキストの左上隅のX座標を指定する</param>
        /// <param name="y">描画するテキストの左上隅のY座標を指定する</param>
        /// <param name="format">描画するテキストに適用する行間や配置などの書式属性を指定する</param>
        public void WriteString(string text, double x, double y, StringFormat format)
        {
            this.graphics.DrawString(text, this.font, this.brush, new PointF((float)x, (float)y), format);
        }

        /// <summary>
        /// 文字を回転して描画する
        /// </summary>
        /// <param name="text"></param>
        /// <param name="p"></param>
        /// <param name="angleDegree"></param>
        public void WriteRotatedString(string text, PointD p, double angleDegree)
        {
            this.WriteRotatedString(text, p.X, p.Y, angleDegree);
        }

        /// <summary>
        /// 文字を回転して描画する
        /// </summary>
        /// <param name="text"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="angleDegree"></param>
        public void WriteRotatedString(string text, double x, double y, double angleDegree)
        {
            using var matrix = new Matrix();
            matrix.RotateAt((float)angleDegree, new PointF((float)x, (float)y));
            matrix.Translate(this.offset.X, this.offset.Y);
            matrix.Scale((float)this.ScalingH, (float)this.ScalingV);

            this.graphics.Transform = matrix;

            this.graphics.DrawString(text, this.font, this.brush, 0, 0);

            this.graphics.Transform = this.codeTransformMatrix;
        }

        /// <summary>
        /// 縁取り文字列を表示します
        /// </summary>
        /// <param name="text"></param>
        /// <param name="location"></param>
        /// <param name="coreColor"></param>
        /// <param name="outlineColor"></param>
        /// <param name="outlineWidth"></param>
        public void WriteOutlineString(string text, Point location, System.Drawing.Color coreColor, System.Drawing.Color outlineColor, int outlineWidth)
        {
            this.WriteOutlineString(text, location.X, location.Y, coreColor, outlineColor, outlineWidth);
        }

        /// <summary>
        /// 縁取り文字列を表示します
        /// </summary>
        /// <param name="text"></param>
        /// <param name="location"></param>
        /// <param name="coreColor"></param>
        /// <param name="outlineColor"></param>
        /// <param name="outlineWidth"></param>
        public void WriteOutlineString(string text, PointD location, System.Drawing.Color coreColor, System.Drawing.Color outlineColor, int outlineWidth)
        {
            this.WriteOutlineString(text, location.X, location.Y, coreColor, outlineColor, outlineWidth);
        }

        /// <summary>
        /// 縁取り文字列を表示します
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="coreColor">文字列色</param>
        /// <param name="outlineColor">縁取り色</param>
        /// <param name="outlineWidth">縁取り幅</param>
        /// <remarks>縁取り幅は1以上を指定します</remarks>
        public void WriteOutlineString(string text, double x, double y, System.Drawing.Color coreColor, System.Drawing.Color outlineColor, int outlineWidth)
        {
            // 縁取り表示設定
            int outlineValue = outlineColor.ToArgb();
            int outlineRed = (outlineValue >> 16) & 0xFF;
            int outlineGreen = (outlineValue >> 8) & 0xFF;
            int outlineBlue = (outlineValue >> 0) & 0xFF;

            // 中心文字表示設定
            int coreValue = coreColor.ToArgb();
            int coreRed = (coreValue >> 16) & 0xFF;
            int coreGreen = (coreValue >> 8) & 0xFF;
            int coreBlue = (coreValue >> 0) & 0xFF;

            // 縁取り文字列を表示
            using var outlineBrush = new SolidBrush(Color.FromArgb(outlineRed, outlineGreen, outlineBlue));
            using var coreBrush = new SolidBrush(Color.FromArgb(coreRed, coreGreen, coreBlue));
            this.WriteOutlineString(text, x, y, coreBrush, outlineBrush, outlineWidth);
        }

        /// <summary>
        /// 縁取り文字列を表示します
        /// </summary>
        /// <param name="text">文字列</param>
        /// <param name="x">X座標</param>
        /// <param name="y">Y座標</param>
        /// <param name="coreColor">文字列色</param>
        /// <param name="outlineColor">縁取り色</param>
        /// <param name="outlineWidth">縁取り幅</param>
        /// <remarks>縁取り幅は1以上を指定します</remarks>
        public void WriteOutlineString(string text, double x, double y, Brush coreBrush, Brush outlineBrush, int outlineWidth)
        {
            var point = new PointF((float)x, (float)y);

            var fh = this.font.Size;
            this.Font = new Font(this.font.FontFamily, (float)(fh / System.Math.Max(this.ScalingH, this.ScalingV)));

            #region 縁取り文字列を表示する
            {
                // 表示設定
                this.Brush = outlineBrush;

                // 縁取り文字を表示する
                if (0 > outlineWidth)
                {
                    outlineWidth = 0;
                }
                var sequence = Enumerable.Range(outlineWidth * -1, outlineWidth * 2 + 1);
                foreach (var shiftRow in sequence)
                    foreach (var shiftCol in sequence)
                    {
                        var shiftedPoint = new PointF(point.X, point.Y);
                        shiftedPoint.X += (shiftCol / (float)this.ScalingH);
                        shiftedPoint.Y += (shiftRow / (float)this.ScalingV);
                        this.graphics.DrawString(text, this.font, this.brush, shiftedPoint);
                    }
            }
            #endregion

            #region 中心文字列を表示する
            {
                // 表示設定
                this.Brush = coreBrush;

                // 中心文字を表示する
                this.graphics.DrawString(text, this.font, this.brush, point);
            }
            #endregion

            this.Font = new Font(this.font.FontFamily, fh);
        }

        /// <summary>
        /// 矩形上に文字列を表示します。
        /// </summary>
        /// <param name="text"></param>
        /// <param name="location"></param>
        /// <param name="foreColor"></param>
        /// <param name="boxColor"></param>
        /// <param name="margin"></param>
        public void WriteBoxedString(string text, Point location, System.Drawing.Color foreColor, System.Drawing.Color boxColor, int margin)
        {
            this.WriteBoxedString(text, location.X, location.Y, foreColor, boxColor, margin);
        }

        /// <summary>
        /// 矩形上に文字列を表示します。
        /// </summary>
        /// <param name="text"></param>
        /// <param name="location"></param>
        /// <param name="foreColor"></param>
        /// <param name="boxColor"></param>
        /// <param name="margin"></param>
        public void WriteBoxedString(string text, PointD location, System.Drawing.Color foreColor, System.Drawing.Color boxColor, int margin)
        {
            this.WriteBoxedString(text, location.X, location.Y, foreColor, boxColor, margin);
        }

        /// <summary>
        /// 矩形上に文字列を表示します。
        /// </summary>
        public void WriteBoxedString(string text, double x, double y, System.Drawing.Color foreColor, System.Drawing.Color boxColor, int margin)
        {
            // 文字表示設定
            int foreValue = foreColor.ToArgb();
            int foreRed = (foreValue >> 16) & 0xFF;
            int foreGreen = (foreValue >> 8) & 0xFF;
            int foreBlue = (foreValue >> 0) & 0xFF;

            // 矩形表示設定
            int boxValue = boxColor.ToArgb();
            int boxRed = (boxValue >> 16) & 0xFF;
            int boxGreen = (boxValue >> 8) & 0xFF;
            int boxBlue = (boxValue >> 0) & 0xFF;

            // 縁取り文字列を表示
            using var foreBrush = new SolidBrush(Color.FromArgb(foreRed, foreGreen, foreBlue));
            using var boxBrush = new SolidBrush(Color.FromArgb(boxRed, boxGreen, boxBlue));
            this.WriteBoxedString(text, x, y, foreBrush, boxBrush, margin);
        }

        /// <summary>
        /// 矩形上に文字列を表示します。
        /// </summary>
        public void WriteBoxedString(string text, double x, double y, Brush foreBrush, Brush boxBrush, int margin)
        {
            // 文字サイズの算出
            var textSize = this.MeasureTextD(text);

            // 矩形サイズの算出
            var boxSize = new SizeD(textSize);
            boxSize.Width += margin * 2d / this.ScalingH;
            boxSize.Height += margin * 2d / this.ScalingV;

            // 矩形の算出
            var boxRectangle = new RectangleD(PointD.New(x - margin / this.ScalingH, y - margin / this.ScalingV), boxSize);

            #region 背景矩形を表示する
            {
                // 表示設定
                this.Brush = boxBrush;
                this.Mode = DrawingMode.Fill;
                this.DrawRectangle1(boxRectangle.Left, boxRectangle.Top, boxRectangle.Size.Width, boxRectangle.Size.Height);
            }
            #endregion

            #region 文字列を表示する
            {
                this.Brush = foreBrush;
                this.WriteString(text, x, y);
            }
            #endregion
        }

        /// <summary>
        /// 指定したテキストのサイズ (ピクセル単位) を取得する
        /// </summary>
        /// <param name="text">計測するテキスト。</param>
        /// <returns>現在のfontで描画される text の Size (ピクセル単位)</returns>
        public System.Drawing.Size MeasureText(string text)
        {
            using var testFont = new Font(this.font.FontFamily, (float)(this.font.Size / System.Math.Max(this.ScalingH, this.ScalingV)));

            return TextRenderer.MeasureText(this.graphics, text, testFont);
        }

        /// <summary>
        /// 指定したテキストのサイズ (ピクセル単位) を取得する
        /// </summary>
        /// <param name="text">計測するテキスト。</param>
        /// <param name="font">計測するテキストに適用される Font</param>
        /// <returns>指定した fontで描画される text の Size (ピクセル単位)</returns>
        public System.Drawing.Size MeasureText(string text, Font font)
        {
            using var testFont = new Font(font.FontFamily, (float)(font.Size / System.Math.Max(this.ScalingH, this.ScalingV)));

            return TextRenderer.MeasureText(this.graphics, text, testFont);
        }

        /// <summary>
        /// 指定したテキストのサイズ (ピクセル単位) を取得する
        /// </summary>
        /// <param name="text">計測するテキスト。</param>
        /// <param name="font">計測するテキストに適用される Font</param>
        /// <param name="proposedSize">初期の外接する四角形の Size</param>
        /// <returns>指定した font と書式で描画される text の Size (ピクセル単位)</returns>
        public System.Drawing.Size MeasureText(string text, Font font, System.Drawing.Size proposedSize)
        {
            using var testFont = new Font(font.FontFamily, (float)(font.Size / System.Math.Max(this.ScalingH, this.ScalingV)));

            return TextRenderer.MeasureText(this.graphics, text, testFont, proposedSize);
        }

        /// <summary>
        /// 指定したテキストのサイズ (ピクセル単位) を取得する
        /// </summary>
        /// <param name="text">計測するテキスト。</param>
        /// <param name="font">計測するテキストに適用される Font</param>
        /// <param name="proposedSize">初期の外接する四角形の Size</param>
        /// <param name="flags">計測するテキストに適用される書式指定</param>
        /// <returns>指定した font と書式で描画される text の Size (ピクセル単位)</returns>
        public System.Drawing.Size MeasureText(string text, Font font, System.Drawing.Size proposedSize, TextFormatFlags flags)
        {
            using var testFont = new Font(font.FontFamily, (float)(font.Size / System.Math.Max(this.ScalingH, this.ScalingV)));

            return TextRenderer.MeasureText(this.graphics, text, testFont, proposedSize, flags);
        }

        /// <summary>
        /// 指定したテキストのサイズ (ピクセル単位) を取得する
        /// </summary>
        /// <param name="text">計測するテキスト。</param>
        /// <returns>現在のfontで描画される text の Size (ピクセル単位)</returns>
        public System.Drawing.SizeF MeasureTextF(string text)
        {
            using var testFont = new Font(this.font.FontFamily, (float)(this.font.Size / System.Math.Max(this.ScalingH, this.ScalingV)));

            return this.graphics.MeasureString(text, testFont);
        }

        /// <summary>
        /// 指定したテキストのサイズ (ピクセル単位) を取得する
        /// </summary>
        /// <param name="text">計測するテキスト。</param>
        /// <param name="font">計測するテキストに適用される Font</param>
        /// <returns>指定した fontで描画される text の Size (ピクセル単位)</returns>
        public System.Drawing.SizeF MeasureTextF(string text, Font font)
        {
            using var testFont = new Font(font.FontFamily, (float)(font.Size / System.Math.Max(this.ScalingH, this.ScalingV)));

            return this.graphics.MeasureString(text, testFont);
        }

        /// <summary>
        /// 指定したテキストのサイズ (ピクセル単位) を取得する
        /// </summary>
        /// <param name="text">計測するテキスト。</param>
        /// <param name="font">計測するテキストに適用される Font</param>
        /// <param name="proposedSize">初期の外接する四角形の Size</param>
        /// <returns>指定した font と書式で描画される text の Size (ピクセル単位)</returns>
        public System.Drawing.SizeF MeasureTextF(string text, Font font, System.Drawing.SizeF proposedSize)
        {
            using var testFont = new Font(font.FontFamily, (float)(font.Size / System.Math.Max(this.ScalingH, this.ScalingV)));

            return this.graphics.MeasureString(text, testFont, proposedSize);
        }

        /// <summary>
        /// 指定したテキストのサイズ (ピクセル単位) を取得する
        /// </summary>
        /// <param name="text">計測するテキスト。</param>
        /// <returns>現在のfontで描画される text の Size (ピクセル単位)</returns>
        public SizeD MeasureTextD(string text)
        {
            return new SizeD(this.MeasureTextF(text));
        }

        /// <summary>
        /// 指定したテキストのサイズ (ピクセル単位) を取得する
        /// </summary>
        /// <param name="text">計測するテキスト。</param>
        /// <param name="font">計測するテキストに適用される Font</param>
        /// <returns>指定した fontで描画される text の Size (ピクセル単位)</returns>
        public SizeD MeasureTextD(string text, Font font)
        {
            return new SizeD(this.MeasureTextF(text, font));
        }

        /// <summary>
        /// 指定したテキストのサイズ (ピクセル単位) を取得する
        /// </summary>
        /// <param name="text">計測するテキスト。</param>
        /// <param name="font">計測するテキストに適用される Font</param>
        /// <param name="proposedSize">初期の外接する四角形の Size</param>
        /// <returns>指定した font と書式で描画される text の Size (ピクセル単位)</returns>
        public SizeD MeasureTextD(string text, Font font, SizeD proposedSize)
        {
            return new SizeD(this.MeasureTextF(text, font, proposedSize.ToSizeF()));
        }

        #endregion
    }
}