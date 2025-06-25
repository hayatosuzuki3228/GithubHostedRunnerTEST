using System.Diagnostics;

namespace Hutzper.Library.Common.Drawing
{
    /// <summary>
    /// 四角形の位置とサイズを表す 4 つの実数を格納します。
    /// </summary>
    [Serializable]
    public class RectangleD
    {
        #region プロパティ

        /// <summary>
        /// プロパティ:左上座標
        /// </summary>
        public PointD Location { get; private set; }

        /// <summary>
        /// プロパティ:サイズ
        /// </summary>
        public SizeD Size { get; private set; }

        /// <summary>
        /// プロパティ:領域矩形の左端
        /// </summary>
        public double Left
        {
            get
            {
                return this.Location.X;
            }
            set
            {
                this.Location.X = value;
            }
        }

        /// <summary>
        /// プロパティ:領域矩形の上端
        /// </summary>
        public double Top
        {
            get
            {
                return this.Location.Y;
            }
            set
            {
                this.Location.Y = value;
            }
        }

        /// <summary>
        /// プロパティ:領域矩形の右端
        /// </summary>
        public double Right
        {
            get
            {
                return this.Location.X + this.Size.Width;
            }
        }

        /// <summary>
        /// プロパティ:領域矩形の下端
        /// </summary>
        public double Bottom
        {
            get
            {
                return this.Location.Y + this.Size.Height;
            }
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RectangleD() : this(PointD.New(), SizeD.New())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">コピー元</param>
        public RectangleD(RectangleD source) : this(source.Location, source.Size)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">コピー元</param>
        public RectangleD(Rectangle source) : this(source.Location, source.Size)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RectangleD(Point location, Size size) : this(new PointD(location), new SizeD(size))
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RectangleD(System.Drawing.RectangleF rectangle) : this(rectangle.Location, rectangle.Size)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RectangleD(System.Drawing.Rectangle rectangle) : this(rectangle.Location, rectangle.Size)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RectangleD(System.Drawing.Point location, System.Drawing.Size size) : this(new PointD(location), new SizeD(size))
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RectangleD(System.Drawing.PointF location, System.Drawing.SizeF size) : this(new PointD(location), new SizeD(size))
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RectangleD(PointD location, SizeD size)
        {
            this.Location = new PointD(location);
            this.Size = new SizeD(size);
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 平行移動
        /// </summary>
        /// <param name="offset"></param>
        public void Offset(PointD offset)
        {
            this.Offset(offset.X, offset.Y);
        }

        /// <summary>
        /// 平行移動
        /// </summary>
        /// <param name="offset"></param>
        public void Offset(System.Drawing.PointF offset)
        {
            this.Offset(offset.X, offset.Y);
        }

        /// <summary>
        /// 平行移動
        /// </summary>
        /// <param name="offset"></param>
        public void Offset(SizeD offset)
        {
            this.Offset((int)offset.Width, (int)offset.Height);
        }

        /// <summary>
        /// 平行移動
        /// </summary>
        /// <param name="offset"></param>
        public void Offset(Size offset)
        {
            this.Offset(offset.Width, offset.Height);
        }

        /// <summary>
        /// 平行移動
        /// </summary>
        /// <param name="offset"></param>
        public void Offset(Point offset)
        {
            this.Offset(offset.X, offset.Y);
        }

        /// <summary>
        /// 平行移動
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        public void Offset(double offsetX, double offsetY)
        {
            this.Location.X += offsetX;
            this.Location.Y += offsetY;
        }

        /// <summary>
        /// この Rectangle を指定の量だけ膨らませます。
        /// </summary>
        /// <param name="size">この四角形の膨張量</param>
        public void Inflate(SizeD size)
        {
            this.Inflate(size.Width, size.Height);
        }

        /// <summary>
        /// この Rectangle を指定の量だけ膨らませます。
        /// </summary>
        /// <param name="size">この四角形の膨張量</param>
        public void Inflate(System.Drawing.SizeF size)
        {
            this.Inflate(size.Width, size.Height);
        }

        /// <summary>
        /// この Rectangle を指定の量だけ膨らませます。
        /// </summary>
        /// <param name="width">このRectangle の水平方向の膨張量</param>
        /// <param name="height">このRectangle の垂直方向の膨張量</param>
        public void Inflate(double width, double height)
        {
            this.Offset(-width, -height);
            this.Size.Width += width * 2;
            this.Size.Height += height * 2;
        }

        /// <summary>
        /// 値の割り当て
        /// </summary>
        /// <param name="rectangle"></param>
        public void Assign(RectangleD rectangle)
        {
            this.Assign(rectangle.Location, rectangle.Size);
        }

        /// <summary>
        /// 値の割り当て
        /// </summary>
        /// <param name="rectangle"></param>
        public void Assign(System.Drawing.RectangleF rectangle)
        {
            this.Assign(rectangle.Location, rectangle.Size);
        }

        /// <summary>
        /// 値の割り当て
        /// </summary>
        public void Assign(PointD location, SizeD size)
        {
            this.Location.Assign(location);
            this.Size.Assign(size);
        }

        /// <summary>
        /// 値の割り当て
        /// </summary>
        public void Assign(System.Drawing.PointF location, System.Drawing.SizeF size)
        {
            this.Location.Assign(location);
            this.Size.Assign(size);
        }

        /// <summary>
        /// 中心座標を取得する
        /// </summary>
        /// <param name="point">中心座標</param>
        public void GetCenterPoint(out PointD point)
        {
            point = new PointD();
            point.X = (this.Left + this.Right) / 2d;
            point.Y = (this.Top + this.Bottom) / 2d;
        }

        /// <summary>
        /// 中心座標を取得する
        /// </summary>
        /// <param name="point">中心座標</param>
        public void GetCenterPoint(out System.Drawing.PointF point)
        {
            point = new System.Drawing.PointF();
            point.X = System.Convert.ToSingle((this.Left + this.Right) / 2d);
            point.Y = System.Convert.ToSingle((this.Top + this.Bottom) / 2d);
        }

        /// <summary>
        /// 中心座標を取得する
        /// </summary>
        /// <param name="x">中心X座標</param>
        /// <param name="y">中心Y座標</param>
        public void GetCenterPoint(out double x, out double y)
        {
            x = (this.Left + this.Right) / 2d;
            y = (this.Top + this.Bottom) / 2d;
        }

        /// <summary>
        /// 中心座標を取得する
        /// </summary>
        /// <param name="x">中心X座標</param>
        /// <param name="y">中心Y座標</param>
        public void GetCenterPoint(out float x, out float y)
        {
            x = System.Convert.ToSingle((this.Left + this.Right) / 2d);
            y = System.Convert.ToSingle((this.Top + this.Bottom) / 2d);
        }

        /// <summary>
        /// 対角座標を取得する
        /// </summary>
        /// <param name="point">中心座標</param>
        public void GetDiagonalPoint(out Point point)
        {
            point = new Point((int)this.Right, (int)this.Bottom);
        }

        /// <summary>
        /// 対角座標を取得する
        /// </summary>
        /// <param name="point">中心座標</param>
        public void GetDiagonalPoint(out PointD point)
        {
            point = new PointD(this.Right, this.Bottom);
        }

        /// <summary>
        /// 対角座標を取得する
        /// </summary>
        /// <param name="point">中心座標</param>
        public void GetDiagonalPoint(out System.Drawing.Point point)
        {
            point = new System.Drawing.Point((int)this.Right, (int)this.Bottom);
        }

        /// <summary>
        /// 対角座標を取得する
        /// </summary>
        /// <param name="point">中心座標</param>
        public void GetDiagonalPoint(out System.Drawing.PointF point)
        {
            point = new System.Drawing.PointF((float)this.Right, (float)this.Bottom);
        }

        /// <summary>
        /// 対角座標を取得する
        /// </summary>
        /// <param name="x">中心X座標</param>
        /// <param name="y">中心Y座標</param>
        public void GetDiagonalPoint(out int x, out int y)
        {
            x = (int)this.Right;
            y = (int)this.Bottom;
        }

        /// <summary>
        /// 対角座標を取得する
        /// </summary>
        /// <param name="x">中心X座標</param>
        /// <param name="y">中心Y座標</param>
        public void GetDiagonalPoint(out double x, out double y)
        {
            x = this.Right;
            y = this.Bottom;
        }

        /// <summary>
        /// 対角座標を取得する
        /// </summary>
        /// <param name="x">中心X座標</param>
        /// <param name="y">中心Y座標</param>
        public void GetDiagonalPoint(out float x, out float y)
        {
            x = (float)this.Right;
            y = (float)this.Bottom;
        }

        /// <summary>
        /// 対角座標を取得する
        /// </summary>
        /// <returns></returns>
        public PointD GetDiagonalPoint()
        {
            return new PointD(this.Right, this.Bottom);
        }

        /// <summary>
        /// 4座標を取得する(時計周り)
        /// </summary>
        /// <param name="points"></param>
        public void ToPoints(out Point[] points)
        {
            points = Array.ConvertAll(this.ToPoints(), p => new Point(p));
        }

        /// <summary>
        /// 4座標を取得する(時計周り)
        /// </summary>
        /// <param name="points"></param>
        public void ToPoints(out PointF[] points)
        {
            points = Array.ConvertAll(this.ToPoints(), p => p.ToPointF());
        }

        /// <summary>
        /// 4座標を取得する(時計周り)
        /// </summary>
        /// <returns></returns>
        public PointD[] ToPoints()
        {
            var rectPoints = new PointD[4];
            foreach (var i in Enumerable.Range(0, rectPoints.Length))
            {
                rectPoints[i] = new PointD(this.Location);
            }

            rectPoints[0].X += 0;
            rectPoints[0].Y += 0;
            rectPoints[1].X += this.Size.Width;
            rectPoints[1].Y += 0;
            rectPoints[2].X += this.Size.Width;
            rectPoints[2].Y += this.Size.Height;
            rectPoints[3].X += 0;
            rectPoints[3].Y += this.Size.Height;

            return rectPoints;
        }

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public RectangleD Clone()
        {
            return RectangleD.New(this);
        }

        /// <summary>
        /// 変換
        /// </summary>
        /// <returns></returns>
        public Rectangle ToRectangleInt()
        {
            return new Rectangle(this.Location.ToPointInt(), this.Size.ToSizeInt());
        }

        /// <summary>
        /// 変換
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Rectangle ToRectangle()
        {
            return new System.Drawing.Rectangle((int)this.Location.X, (int)this.Location.Y, (int)this.Size.Width, (int)this.Size.Height);
        }

        /// <summary>
        /// 変換
        /// </summary>
        /// <returns></returns>
        public System.Drawing.RectangleF ToRectangleF()
        {
            return new System.Drawing.RectangleF(this.Location.ToPointF(), this.Size.ToSizeF());
        }

        /// <summary>
        /// 比較
        /// </summary>
        /// <param name="compare"></param>
        /// <returns></returns>
        public override bool Equals(object? compare)
        {
            bool isEqual = false;

            try
            {
                var rectangle = compare as RectangleD;

                if (null != rectangle)
                {
                    if ((true == this.Location.Equals(rectangle.Location))
                    && (true == this.Size.Equals(rectangle.Size)))
                    {
                        isEqual = true;
                    }
                }
            }
            catch (Exception ex)
            {
                isEqual = false;
                Debug.WriteLine(this, ex.Message);
            }

            return isEqual;
        }

        /// <summary>
        /// ハッシュコード
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 矩形が交差するかどうか
        /// </summary>
        /// <param name="compare">テスト対象の四角形</param>
        /// <returns>交差部分が存在する場合、このメソッドは true を返します。それ以外の場合は false を返します</returns>
        public bool IntersectsWith(RectangleD rect)
        {
            bool isIntersects = false;

            try
            {
                isIntersects = (this.Left <= rect.Right && rect.Left <= this.Right && this.Top <= rect.Bottom && rect.Top <= this.Bottom);
            }
            catch (Exception ex)
            {
                isIntersects = false;
                Debug.WriteLine(this, ex.Message);
            }

            return isIntersects;
        }

        /// <summary>
        /// 指定した点が Rectangle 構造体に含まれているかどうかを判断します
        /// </summary>
        /// <param name="point">テスト対象の Point</param>
        /// <returns>pt によって表される点がこの Rectangle 構造体に含まれている場合、このメソッドは true を返します。それ以外の場合は false を返します</returns>
        public bool Contains(PointD point)
        {
            return this.Contains(point.X, point.Y);
        }

        /// <summary>
        /// 指定した点が Rectangle 構造体に含まれているかどうかを判断します
        /// </summary>
        /// <param name="x">テストする点の x 座標</param>
        /// <param name="y">テストする点の y 座標</param>
        /// <returns>pt によって表される点がこの Rectangle 構造体に含まれている場合、このメソッドは true を返します。それ以外の場合は false を返します</returns>
        public bool Contains(double x, double y)
        {
            if (this.Left > x)
            {
                return false;
            }

            if (this.Right < x)
            {
                return false;
            }

            if (this.Top > y)
            {
                return false;
            }

            if (this.Bottom < y)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region スタティックメソッド

        public static RectangleD New()
        {
            return new RectangleD();
        }

        public static RectangleD New(RectangleD source)
        {
            return new RectangleD(source);
        }

        public static RectangleD New(PointD location, SizeD size)
        {
            return new RectangleD(location, size);
        }

        public static RectangleD Convert(System.Drawing.PointF location, System.Drawing.SizeF size)
        {
            return new RectangleD(location, size);
        }

        public static RectangleD Convert(System.Drawing.RectangleF source)
        {
            return new RectangleD(source);
        }

        public static System.Drawing.RectangleF Convert(RectangleD source)
        {
            return source.ToRectangleF();
        }

        #endregion
    }
}