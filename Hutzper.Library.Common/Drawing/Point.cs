using System.Diagnostics;

namespace Hutzper.Library.Common.Drawing
{
    /// <summary>
    /// 2 次元平面に点を定義する、整数座標ペア (x 座標と y 座標) を表します。
    /// </summary>
    [Serializable]
    public class Point
    {
        #region プロパティ

        /// <summary>
        /// X座標
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y座標
        /// </summary>        
        public int Y { get; set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Point() : this(0, 0)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">コピー元</param>
        public Point(Point source) : this(source.X, source.Y)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">コピー元</param>
        public Point(System.Drawing.Point source) : this(source.X, source.Y)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Point(PointD point)
        {
            this.X = (int)point.X;
            this.Y = (int)point.Y;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 自身を示す文字列を返す
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{" + string.Format("X={0:D}, Y={1:D}", this.X, this.Y) + "}";
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
        public void Offset(int offsetX, int offsetY)
        {
            this.X += offsetX;
            this.Y += offsetY;
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
        /// 値の割り当て
        /// </summary>
        /// <param name="size"></param>
        public void Assign(Point point)
        {
            this.X = point.X;
            this.Y = point.Y;
        }

        /// <summary>
        /// 値の割り当て
        /// </summary>
        /// <param name="size"></param>
        public void Assign(System.Drawing.Point point)
        {
            this.X = point.X;
            this.Y = point.Y;
        }

        /// <summary>
        /// 指定座標からの距離を計測する
        /// </summary>
        /// <param name="origin">指定座標</param>
        /// <param name="distance">距離</param>
        /// <returns>指定座標からの相対座標</returns>
        public Point DistanceFrom(Point origin, out double distance)
        {
            return this.DistanceFrom(origin, out distance, out _);
        }

        /// <summary>
        /// 指定座標からの距離を計測する
        /// </summary>
        /// <param name="origin">指定座標</param>
        /// <param name="distance">距離</param>
        /// <param name="direction">指定座標からの向き</param>
        /// <returns>指定座標からの相対座標</returns>
        public Point DistanceFrom(Point origin, out double distance, out double direction)
        {
            var relativePointD = new PointD
            {
                X = this.X - origin.X,
                Y = this.Y - origin.Y
            };

            distance = System.Math.Sqrt(relativePointD.X * relativePointD.X + relativePointD.Y * relativePointD.Y);
            direction = System.Math.Atan2(relativePointD.Y, relativePointD.X);

            return new Point((int)relativePointD.X, (int)relativePointD.Y);
        }

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public Point Clone()
        {
            return Point.New(this);
        }

        /// <summary>
        /// 変換
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Point ToPoint()
        {
            return new System.Drawing.Point(this.X, this.Y);
        }

        /// <summary>
        /// 変換
        /// </summary>
        /// <returns></returns>
        public System.Drawing.PointF ToPointF()
        {
            return new System.Drawing.PointF(this.X, this.Y);
        }

        /// <summary>
        /// 変換
        /// </summary>
        /// <returns></returns>
        public PointD ToPointD()
        {
            return new PointD(this.X, this.Y);
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
                if (compare is Point point)
                {
                    if ((true == this.X.Equals(point.X))
                    && (true == this.Y.Equals(point.Y)))
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

        #endregion

        #region スタティックメソッド

        public static Point New()
        {
            return new Point();
        }

        public static Point New(int x, int y)
        {
            return new Point(x, y);
        }

        public static Point New(Point source)
        {
            return new Point(source);
        }

        public static Point Convert(System.Drawing.Point source)
        {
            return new Point(source);
        }

        public static System.Drawing.Point Convert(Point source)
        {
            return source.ToPoint();
        }

        #endregion
    }
}