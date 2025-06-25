using System.Diagnostics;

namespace Hutzper.Library.Common.Drawing
{
    /// <summary>
    /// 2 次元平面に点を定義する、実数座標ペア (x 座標と y 座標) を表します。
    /// </summary>
    [Serializable]
    public class PointD
    {
        #region プロパティ

        /// <summary>
        /// X座標
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y座標
        /// </summary>        
        public double Y { get; set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PointD() : this(0, 0)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">コピー元</param>
        public PointD(PointD source) : this(source.X, source.Y)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">コピー元</param>
        public PointD(Point source) : this(source.X, source.Y)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">コピー元</param>
        public PointD(System.Drawing.Point source) : this(source.X, source.Y)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="source">コピー元</param>
        public PointD(System.Drawing.PointF source) : this(source.X, source.Y)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public PointD(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public PointD(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public PointD(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 自身を示す文字列を返す
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{" + string.Format("X={0:F}, Y={1:F}", this.X, this.Y) + "}";
        }

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
        public void Offset(SizeD offset)
        {
            this.Offset(offset.Width, offset.Height);
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
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        public void Offset(double offsetX, double offsetY)
        {
            this.X += offsetX;
            this.Y += offsetY;
        }

        /// <summary>
        /// 値の割り当て
        /// </summary>
        /// <param name="size"></param>
        public void Assign(PointD point)
        {
            this.X = point.X;
            this.Y = point.Y;
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
        public void Assign(System.Drawing.PointF point)
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
        public PointD DistanceFrom(PointD origin, out double distance)
        {
            double direction;
            return this.DistanceFrom(origin, out distance, out direction);
        }

        /// <summary>
        /// 指定座標からの距離を計測する
        /// </summary>
        /// <param name="origin">指定座標</param>
        /// <param name="distance">距離</param>
        /// <param name="direction">指定座標からの向き</param>
        /// <returns>指定座標からの相対座標</returns>
        public PointD DistanceFrom(PointD origin, out double distance, out double direction)
        {
            var relativePoint = new PointD();
            relativePoint.X = this.X - origin.X;
            relativePoint.Y = this.Y - origin.Y;

            distance = System.Math.Sqrt(relativePoint.X * relativePoint.X + relativePoint.Y * relativePoint.Y);
            direction = System.Math.Atan2(relativePoint.Y, relativePoint.X);

            return relativePoint;
        }

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public PointD Clone()
        {
            return PointD.New(this);
        }

        /// <summary>
        /// 変換
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Point ToPoint()
        {
            return new System.Drawing.Point((int)this.X, (int)this.Y);
        }

        /// <summary>
        /// 変換
        /// </summary>
        /// <returns></returns>
        public System.Drawing.PointF ToPointF()
        {
            return new System.Drawing.PointF((float)this.X, (float)this.Y);
        }

        /// <summary>
        /// 変換
        /// </summary>
        /// <returns></returns>
        public Point ToPointInt()
        {
            return new Point((int)this.X, (int)this.Y);
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
                var point = compare as PointD;

                if (null != point)
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

        public static PointD New()
        {
            return new PointD();
        }

        public static PointD New(double x, double y)
        {
            return new PointD(x, y);
        }

        public static PointD New(PointD source)
        {
            return new PointD(source);
        }

        public static PointD Convert(System.Drawing.PointF source)
        {
            return new PointD(source);
        }

        public static System.Drawing.PointF Convert(PointD source)
        {
            return source.ToPointF();
        }

        #endregion
    }
}