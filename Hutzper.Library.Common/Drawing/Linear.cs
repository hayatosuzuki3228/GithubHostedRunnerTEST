using System.Diagnostics;

namespace Hutzper.Library.Common.Drawing
{
    /// <summary>
    /// 直線式を表します
    /// </summary>
    /// <remarks>ax + by + c = 0</remarks>
    [Serializable]
    public class Linear
    {
        #region プロパティ

        /// <summary>
        /// [a]x + by + c = 0
        /// </summary>
        public double A { get; protected set; }

        /// <summary>
        /// ax + [b]y + c = 0
        /// </summary>
        public double B { get; protected set; }

        /// <summary>
        /// ax + by + [c] = 0
        /// </summary>
        public double C { get; protected set; }

        /// <summary>
        /// Sqrt(A * A + B * B)
        /// </summary>
        public double D { get; protected set; }

        /// <summary>
        /// 有効な直線式かどうか
        /// </summary>
        public bool Enabled
        {
            #region 取得
            get
            {
                return !((0 == this.A) && (0 == this.B));
            }
            #endregion
        }

        /// <summary>
        /// X座標に平行かどうか
        /// </summary>
        public bool IsHorizontal
        {
            #region 取得
            get
            {
                return 0 == this.A;
            }
            #endregion
        }

        /// <summary>
        /// Y座標に平行かどうか
        /// </summary>
        public bool IsVertical
        {
            #region 取得
            get
            {
                return 0 == this.B;
            }
            #endregion
        }

        /// <summary>
        /// -π/2 ≤θ≤π/2 の、ラジアンで示した角度 θ
        /// </summary>
        public double Angle
        {
            #region 取得
            get
            {
                if (this.IsVertical)
                {
                    return System.Math.Asin(-1);
                }

                return System.Math.Atan(-this.A / this.B);
            }
            #endregion
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Linear() : this(0d, 0d, 0d, 0d)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Linear(double a, double b, double c) : this(a, b, c, System.Math.Sqrt(a * a + b * b))
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Linear(double a, double b, double c, double d)
        {
            this.Assign(a, b, c, d);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Linear(Linear source)
        {
            this.Assign(source);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Linear(Point p1, Point p2) : this(p1.ToPointD(), p2.ToPointD())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Linear(PointD p1, PointD p2)
        {
            this.Assign(p1, p2);
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 自身を示す文字列を返す
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "{" + string.Format("A={0:F}, B={1:F}, C={2:F}", this.A, this.B, this.C) + "}";
        }

        /// <summary>
        /// 平行移動
        /// </summary>
        /// <param name="offset">平行移動量</param>
        /// <remarks>直線式を指定された移動量で平行移動します。</remarks>
        public void Offset(PointD offset)
        {
            this.Offset(offset.X, offset.Y);
        }

        /// <summary>
        /// 平行移動
        /// </summary>
        /// <param name="offsetX">X軸方向の平行移動量</param>
        /// <param name="offsetY">Y軸方向の平行移動量</param>
        /// <remarks>直線式を指定された移動量で平行移動します。</remarks>
        public void Offset(double offsetX, double offsetY)
        {
            this.C = -1d * (this.A * offsetX + this.B * offsetY) + this.C;
            this.D = System.Math.Sqrt(this.A * this.A + this.B * this.B);
        }

        /// <summary>
        /// 値の割り当て
        /// </summary>
        /// <remarks>直線式パラメータを直接指定します。</remarks>
        public void Assign(double a, double b, double c)
        {
            this.Assign(a, b, c, System.Math.Sqrt(a * a + b * b));
        }

        /// <summary>
        /// 値の割り当て
        /// </summary>
        /// <remarks>直線式パラメータを直接指定します。</remarks>
        public void Assign(double a, double b, double c, double d)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.D = d;
        }

        /// <summary>
        /// 値の割り当て
        /// </summary>
        /// <param name="source">コピーされる直線式</param>
        /// <remarks>与えられた直線式のパラメータを設定します。</remarks>
        public void Assign(Linear source)
        {
            this.A = source.A;
            this.B = source.B;
            this.C = source.C;
            this.D = source.D;
        }

        /// <summary>
        /// 値の割り当て
        /// </summary>
        /// <param name="p1">直線を通る座標1</param>
        /// <param name="p2">直線を通る座標2</param>
        /// <remarks>与えられた2点を通る直線式パラメータを設定します。</remarks>
        public void Assign(Point p1, Point p2)
        {
            this.Assign(p1.ToPointD(), p2.ToPointD());
        }

        /// <summary>
        /// 値の割り当て
        /// </summary>
        /// <param name="p1">直線を通る座標1</param>
        /// <param name="p2">直線を通る座標2</param>
        /// <remarks>与えられた2点を通る直線式パラメータを設定します。</remarks>
        public void Assign(PointD p1, PointD p2)
        {
            var valX = p2.X - p1.X;
            var valY = p2.Y - p1.Y;

            this.A = valY;
            this.B = valX * -1d;
            this.C = p1.Y * valX - p1.X * valY;
            this.D = System.Math.Sqrt(this.A * this.A + this.B * this.B);
        }

        /// <summary>
        /// 平行かどうか
        /// </summary>
        /// <param name="compare">対象の直線式</param>
        /// <returns>平行の場合はtrue</returns>
        public bool IsParallel(Linear compare)
        {
            var result = this.A * compare.B - compare.A * this.B;

            return (0 == result);
        }

        /// <summary>
        /// 直角かどうか
        /// </summary>
        /// <param name="compare">対象の直線式</param>
        /// <returns>垂直の場合はtrue</returns>
        public bool IsPerpendicular(Linear compare)
        {
            var result = this.A * compare.A + this.B * compare.B;

            return (0 == result);
        }

        /// <summary>
        /// 交点を求める
        /// </summary>
        /// <param name="compare">対象の直線式</param>
        /// <param name="cross">交点</param>
        /// <returns>交点が求まる場合はtrue</returns>
        public bool Intersection(Linear compare, out PointD cross)
        {
            var result = !this.IsParallel(compare);

            cross = new PointD();

            if (true == result)
            {
                var denominator = this.A * compare.B - compare.A * this.B;

                cross.X = (this.B * compare.C - compare.B * this.C) / denominator;
                cross.Y = (compare.A * this.C - this.A * compare.C) / denominator;
            }

            return result;
        }

        /// <summary>
        /// 指定されたY座標に対応するX座標を求める
        /// </summary>
        /// <param name="selectedY">指定Y座標</param>
        /// <param name="pairedX">対応するX座標</param>
        /// <returns>対応座標が求まった場合はtrue</returns>
        public bool GetPointX(double selectedY, out double pairedX)
        {
            var result = !this.IsHorizontal;

            pairedX = 0;

            if (true == result)
            {
                pairedX = (this.B * selectedY + this.C) / this.A * -1d;
            }

            return result;
        }

        /// <summary>
        /// 指定されたY座標に対応するX座標を求める
        /// </summary>
        /// <param name="selectedY">指定Y座標</param>
        /// <param name="paired">対応するX座標と指定Y座標</param>
        /// <returns>対応座標が求まった場合はtrue</returns>
        public bool GetPointX(double selectedY, out PointD paired)
        {
            var result = this.GetPointX(selectedY, out double pairedX);

            paired = new PointD(pairedX, selectedY);

            return result;
        }

        /// <summary>
        /// 指定されたX座標に対応するY座標を求める
        /// </summary>
        /// <param name="selectedX">指定X座標</param>
        /// <param name="pairedY">対応するY座標</param>
        /// <returns>対応座標が求まった場合はtrue</returns>
        public bool GetPointY(double selectedX, out double pairedY)
        {
            var result = !this.IsVertical;

            pairedY = 0;

            if (true == result)
            {
                pairedY = (this.A * selectedX + this.C) / this.B * -1d;
            }

            return result;
        }

        /// <summary>
        /// 指定されたX座標に対応するY座標を求める
        /// </summary>
        /// <param name="selectedX">指定X座標</param>
        /// <param name="paired">指定X座標と対応するY座標</param>
        /// <returns>対応座標が求まった場合はtrue</returns>
        public bool GetPointY(double selectedX, out PointD paired)
        {
            var result = this.GetPointY(selectedX, out double pairedY);

            paired = new PointD(selectedX, pairedY);

            return result;
        }

        /// <summary>
        /// 平行移動した直線式を求める
        /// </summary>
        /// <param name="offset">平行移動量</param>
        /// <param name="linear">平行移動した直線式</param>
        /// <returns>直線式が求まった場合はtrue</returns>
        public bool GetOffsetLinear(PointD offset, out Linear linear)
        {
            return this.GetOffsetLinear(offset.X, offset.Y, out linear);
        }

        /// <summary>
        /// 平行移動した直線式を求める
        /// </summary>
        /// <param name="offsetX">X軸方向の平行移動量</param>
        /// <param name="offsetY">Y軸方向の平行移動量</param>
        /// <param name="linear">平行移動した直線式</param>
        /// <returns>直線式が求まった場合はtrue</returns>
        public bool GetOffsetLinear(double offsetX, double offsetY, out Linear linear)
        {
            var result = this.Enabled;

            linear = new Linear();

            if (true == result)
            {
                linear.A = this.A;
                linear.B = this.B;
                linear.C = -1d * (linear.A * offsetX + linear.B * offsetY) + this.C;
                linear.D = System.Math.Sqrt(linear.A * linear.A + linear.B * linear.B);
            }

            return result;
        }

        /// <summary>
        /// 指定された座標を通る平行な直線式を求める
        /// </summary>
        /// <param name="selectedPoint">指定座標</param>
        /// <param name="linear">指定座標を通る平行線</param>
        /// <returns>直線式が求まった場合はtrue</returns>
        public bool GetParallelLinear(PointD selectedPoint, out Linear linear)
        {
            return this.GetParallelLinear(selectedPoint.X, selectedPoint.Y, out linear);
        }

        /// <summary>
        /// 指定された座標を通る平行な直線式を求める
        /// </summary>
        /// <param name="selectedPointX">指定X座標</param>
        /// <param name="selectedPointY">指定Y座標</param>
        /// <param name="linear">指定座標を通る直線式</param>
        /// <returns>直線式が求まった場合はtrue</returns>
        public bool GetParallelLinear(double selectedPointX, double selectedPointY, out Linear linear)
        {
            var result = this.Enabled;

            linear = new Linear();

            if (true == result)
            {
                linear.A = this.A;
                linear.B = this.B;
                linear.C = -1d * this.A * selectedPointX - 1d * this.B * selectedPointY;
                linear.D = System.Math.Sqrt(linear.A * linear.A + linear.B * linear.B);
            }

            return result;
        }

        /// <summary>
        /// 指定された座標を通り垂直な直線式を求める
        /// </summary>
        /// <param name="selectedPoint">指定座標</param>
        /// <param name="linear">指定座標を通る直線式</param>
        /// <returns>直線式が求まった場合はtrue</returns>
        public bool GetPerpendicularLinear(PointD selectedPoint, out Linear linear)
        {
            return this.GetPerpendicularLinear(selectedPoint.X, selectedPoint.Y, out linear);
        }

        /// <summary>
        /// 指定された座標を通り垂直な直線式を求める
        /// </summary>
        /// <param name="selectedPointX">指定X座標</param>
        /// <param name="selectedPointY">指定Y座標</param>
        /// <param name="linear">指定座標を通る直線式</param>
        /// <returns>直線式が求まった場合はtrue</returns>
        public bool GetPerpendicularLinear(double selectedPointX, double selectedPointY, out Linear linear)
        {
            var result = this.Enabled;

            linear = new Linear();

            if (true == result)
            {
                linear.A = this.B;
                linear.B = this.A * -1d;
                linear.C = this.A * selectedPointY - this.B * selectedPointX;
                linear.D = System.Math.Sqrt(linear.A * linear.A + linear.B * linear.B);
            }

            return result;
        }

        /// <summary>
        /// 指定座標からの直線に下ろした垂線の長さを取得する
        /// </summary>
        /// <param name="selectedPoint">指定座標</param>
        /// <param name="distance">距離</param>
        /// <returns>垂線の長さが求まった場合はtrue</returns>
        public bool DistanceFrom(PointD selectedPoint, out double distance)
        {
            return this.DistanceFrom(selectedPoint.X, selectedPoint.Y, out distance);
        }

        /// <summary>
        /// 指定座標からの直線に下ろした垂線の長さを取得する
        /// </summary>
        /// <param name="selectedPointX">指定X座標</param>
        /// <param name="selectedPointY">指定Y座標</param>
        /// <param name="distance">距離</param>
        /// <returns>垂線の長さが求まった場合はtrue</returns>
        public bool DistanceFrom(double selectedPointX, double selectedPointY, out double distance)
        {
            var result = this.Enabled;

            distance = 0;

            if (true == result)
            {
                distance = System.Math.Abs(this.A * selectedPointX + this.B * selectedPointY + this.C) / this.D;
            }

            return result;
        }

        /// <summary>
        /// 指定座標からの直線に下ろした垂線の長さと交点を取得する
        /// </summary>
        /// <param name="selectedPoint">指定座標</param>
        /// <param name="distance">距離</param>
        /// <param name="intersection">交点</param>
        /// <returns>垂線の長さが求まった場合はtrue</returns>
        public bool DistanceFrom(PointD selectedPoint, out double distance, out PointD intersection)
        {
            var result = this.DistanceFrom(selectedPoint.X, selectedPoint.Y, out distance, out double intersectionX, out double intersectionY);

            intersection = new PointD(intersectionX, intersectionY);

            return result;
        }

        /// <summary>
        /// 指定座標からの直線に下ろした垂線の長さと交点を取得する
        /// </summary>
        /// <param name="selectedPoint">指定座標</param>
        /// <param name="distance">距離</param>
        /// <param name="intersectionX">交点X座標</param>
        /// <param name="intersectionY">交点Y座標</param>
        /// <returns>垂線の長さが求まった場合はtrue</returns>
        public bool DistanceFrom(double selectedPointX, double selectedPointY, out double distance, out double intersectionX, out double intersectionY)
        {
            // 垂線を求める
            var result = this.GetPerpendicularLinear(selectedPointX, selectedPointY, out Linear linear);

            distance = 0;
            intersectionX = 0;
            intersectionY = 0;

            // 垂線が求まった場合
            if (true == result)
            {
                // 長さを算出する
                distance = System.Math.Abs(this.A * selectedPointX + this.B * selectedPointY + this.C) / this.D;

                // 交点を求める
                var denominator = this.A * linear.B - linear.A * this.B;
                intersectionX = (this.B * linear.C - linear.B * this.C) / denominator;
                intersectionY = (linear.A * this.C - this.A * linear.C) / denominator;
            }

            return result;
        }

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public Linear Clone()
        {
            return Linear.New(this);
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
                if (compare is Linear point)
                {
                    if ((true == this.A.Equals(point.A))
                    && (true == this.B.Equals(point.B))
                    && (true == this.C.Equals(point.C))
                    && (true == this.D.Equals(point.D)))
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

        /// <summary>
        /// 直線式を生成します。
        /// </summary>
        /// <returns>生成された直線式</returns>
        public static Linear New()
        {
            return new Linear();
        }

        /// <summary>
        /// 指定されたパラメータで直線式を生成します。
        /// </summary>
        /// <param name="a">直線式パラメータA</param>
        /// <param name="b">直線式パラメータB</param>
        /// <param name="c">直線式パラメータC</param>
        /// <returns>生成された直線式</returns>
        public static Linear New(double a, double b, double c)
        {
            return Linear.New(a, b, c, System.Math.Sqrt(a * a + b * b));
        }

        /// <summary>
        /// 指定されたパラメータで直線式を生成します。
        /// </summary>
        /// <param name="a">直線式パラメータA</param>
        /// <param name="b">直線式パラメータB</param>
        /// <param name="c">直線式パラメータC</param>
        /// <param name="d">直線式パラメータD</param>
        /// <returns>生成された直線式</returns>
        public static Linear New(double a, double b, double c, double d)
        {
            return new Linear(a, b, c, d);
        }

        /// <summary>
        /// 直線式にコピーを生成します。
        /// </summary>
        /// <param name="source">コピーされる直線式</param>
        /// <returns>生成された直線式</returns>
        public static Linear New(Linear source)
        {
            return new Linear(source);
        }

        /// <summary>
        /// 与えられた2点を通る直線式を生成します。
        /// </summary>
        /// <param name="p1">直線を通る座標1</param>
        /// <param name="p2">直線を通る座標2</param>
        /// <returns>生成された直線式</returns>
        public static Linear New(Point p1, Point p2)
        {
            return new Linear(p1.ToPointD(), p2.ToPointD());
        }

        /// <summary>
        /// 与えられた2点を通る直線式を生成します。
        /// </summary>
        /// <param name="p1">直線を通る座標1</param>
        /// <param name="p2">直線を通る座標2</param>
        /// <returns>生成された直線式</returns>
        public static Linear New(PointD p1, PointD p2)
        {
            return new Linear(p1, p2);
        }

        /// <summary>
        /// 与えられたY座標を通り、X軸に平行な直線式を生成します。
        /// </summary>
        /// <param name="selectedY">Y座標</param>
        /// <remarks>与えられたY座標を通るX軸に平行な直線式を生成します。</remarks>
        public static Linear GetHorizontalLinear(double selectedY)
        {
            return new Linear(0d, 1d, -1d * selectedY);
        }

        /// <summary>
        /// 与えられたX座標を通り、Y軸に平行な直線式を生成します。
        /// </summary>
        /// <param name="selectedX">X座標</param>
        /// <remarks>与えられたX座標を通るY軸に平行な直線式を生成します。</remarks>
        public static Linear GetVerticalLinear(double selectedX)
        {
            return new Linear(1d, 0d, -1d * selectedX);
        }

        #endregion
    }
}