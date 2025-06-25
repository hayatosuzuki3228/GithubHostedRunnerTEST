using System.Drawing.Drawing2D;

namespace Hutzper.Library.Common.Drawing
{
    #region サブクラス

    /// <summary>
    /// コーナー丸め指定
    /// </summary>
    [Serializable]
    [Flags]
    public enum RoundedCorner
    {
        Disable = 0,
        LeftTop = 1 << 0,
        RightTop = 1 << 1,
        RightBottom = 1 << 2,
        LeftBottom = 1 << 3,
        All = 15,
    }

    #endregion

    /// <summary>
    /// グラフィックスユーティリティ
    /// </summary>
    public static class GraphicsUtilities
    {
        public static GraphicsPath CreateRound(RectangleF baseRect, RoundedCorner roundedCorner, float diameter = 25f)
        {
            var halfR = diameter / 2f;

            var arcRect = new RectangleF(baseRect.Location, new SizeF(halfR, halfR));
            var srpRect = new RectangleF(baseRect.Location, new SizeF(0.1f, 0.1f));

            var path = new GraphicsPath();
            path.StartFigure();

            // handle top left corner
            path.AddArc(roundedCorner.Contains(RoundedCorner.LeftTop) ? arcRect : srpRect, 180, 90);

            // handle top right corner
            arcRect.X = baseRect.Width - arcRect.Width + baseRect.Left;
            srpRect.X = baseRect.Width - srpRect.Width + baseRect.Left;
            path.AddArc(roundedCorner.Contains(RoundedCorner.RightTop) ? arcRect : srpRect, 270, 90);

            // handle baseRect right corner
            arcRect.Y = baseRect.Height - arcRect.Height + baseRect.Top;
            srpRect.Y = baseRect.Height - srpRect.Height + baseRect.Top;
            path.AddArc(roundedCorner.Contains(RoundedCorner.RightBottom) ? arcRect : srpRect, 0, 90);

            // handle bottom left corner
            arcRect.X = baseRect.Left;
            srpRect.X = baseRect.Left;
            path.AddArc(roundedCorner.Contains(RoundedCorner.LeftBottom) ? arcRect : srpRect, 90, 90);

            path.CloseFigure();

            return path;
        }

        /// <summary>
        /// 矩形を4座標に分解する(時計回り)
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static PointD[] RectangleToPoints(RectangleD rect)
        {
            var rectPoints = Enumerable.Repeat(rect.Location.Clone(), 4).ToArray();
            rectPoints[0].X += 0;
            rectPoints[0].Y += 0;
            rectPoints[1].X += rect.Size.Width;
            rectPoints[1].Y += 0;
            rectPoints[2].X += rect.Size.Width;
            rectPoints[2].Y += rect.Size.Height;
            rectPoints[3].X += 0;
            rectPoints[3].Y += rect.Size.Height;

            return rectPoints;
        }

        /// <summary>
        /// 矩形を4座標に分解する(時計回り)
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static Point[] RectangleToPoints(Rectangle rect)
        {
            var rectPoints = Enumerable.Repeat(rect.Location.Clone(), 4).ToArray();
            rectPoints[0].X += 0;
            rectPoints[0].Y += 0;
            rectPoints[1].X += rect.Size.Width;
            rectPoints[1].Y += 0;
            rectPoints[2].X += rect.Size.Width;
            rectPoints[2].Y += rect.Size.Height;
            rectPoints[3].X += 0;
            rectPoints[3].Y += rect.Size.Height;

            return rectPoints;
        }

        /// <summary>
        /// 4座標から矩形を生成する
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        /// <remarks>点数チェックは行いません</remarks>
        public static RectangleD PointsToRectangle(PointD[] points)
        {
            var sortedPoints = Array.ConvertAll(points, p => p.Clone());

            // Y,X座標でソート
            Array.Sort(sortedPoints, (a, b) =>
            {
                var result = a.Y.CompareTo(b.Y);

                return result != 0 ? result : a.X.CompareTo(b.X);
            });

            // 再矩形化
            return new RectangleD(
                                     sortedPoints.First()
                                   , new SizeD(sortedPoints.Skip(1).First().X - sortedPoints.First().X, sortedPoints.Last().Y - sortedPoints.First().Y)
                                   );
        }

        /// <summary>
        /// 4座標から矩形を生成する
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        /// <remarks>点数チェックは行いません</remarks>
        public static Rectangle PointsToRectangle(Point[] points)
        {
            var sortedPoints = Array.ConvertAll(points, p => p.Clone());

            // Y,X座標でソート
            Array.Sort(sortedPoints, (a, b) =>
            {
                var result = a.Y.CompareTo(b.Y);

                return result != 0 ? result : a.X.CompareTo(b.X);
            });

            // 再矩形化
            return new Rectangle(
                                  sortedPoints.First()
                                , new Size(sortedPoints.Skip(1).First().X - sortedPoints.First().X, sortedPoints.Last().Y - sortedPoints.First().Y)
                                );
        }

        /// <summary>
        /// 座標を指定角度方向へ指定距離移動させた座標に変換する
        /// </summary>
        /// <param name="source">元座標</param>
        /// <param name="radian">方向角度</param>
        /// <param name="length">距離</param>
        /// <param name="destination">変換座標</param>
        public static void GetDestination(PointD source, double radian, double length, out PointD destination)
        {

            GraphicsUtilities.GetDestination(source.X, source.Y, radian, length, out double destinationX, out double destinationY);

            destination = new PointD(destinationX, destinationY);
        }

        /// <summary>
        /// 座標を指定角度方向へ指定距離移動させた座標に変換する
        /// </summary>
        /// <param name="sourceX">元座標X</param>
        /// <param name="sourceY">元座標Y</param>
        /// <param name="radian">方向角度</param>
        /// <param name="length">距離</param>
        /// <param name="destinationX">変換座標X</param>
        /// <param name="destinationY">変換座標Y</param>
        public static void GetDestination(double sourceX, double sourceY, double radian, double length, out double destinationX, out double destinationY)
        {
            destinationX = length * System.Math.Cos(radian) + sourceX;
            destinationY = length * System.Math.Sin(radian) + sourceY;
        }

        /// <summary>
        /// 座標を指定座標を中心に指定角度回転させた座標に変換する
        /// </summary>
        /// <param name="source">元座標</param>
        /// <param name="radian">回転角度</param>
        /// <param name="center">回転中心座標</param>
        /// <param name="destination">変換座標</param>
        public static void GetRotation(PointD source, double radian, PointD center, out PointD destination)
        {
            GraphicsUtilities.GetRotation(source.X, source.Y, radian, center.X, center.Y, out double destinationX, out double destinationY);

            destination = new PointD(destinationX, destinationY);
        }

        /// <summary>
        /// 座標を指定座標を中心に指定角度回転させた座標に変換する
        /// </summary>
        /// <param name="source">元座標</param>
        /// <param name="radian">回転角度</param>
        /// <param name="center">回転中心座標</param>
        /// <param name="destination">変換座標</param>
        public static void GetRotation(PointD[] source, double radian, PointD center, out PointD[] destination)
        {
            destination = new PointD[source.Length];

            foreach (var i in Enumerable.Range(0, source.Length))
            {
                GraphicsUtilities.GetRotation(source[i].X, source[i].Y, radian, center.X, center.Y, out double destinationX, out double destinationY);

                destination[i] = new PointD(destinationX, destinationY);
            }
        }

        /// <summary>
        /// 座標を指定座標を中心に指定角度回転させた座標に変換する
        /// </summary>
        /// <param name="source">元座標</param>
        /// <param name="radian">回転角度</param>
        /// <param name="center">回転中心座標</param>
        /// <param name="destination">変換座標</param>
        public static void GetRotation(List<PointD> source, double radian, PointD center, out List<PointD> destination)
        {
            destination = new List<PointD>();

            foreach (var i in Enumerable.Range(0, source.Count))
            {
                GraphicsUtilities.GetRotation(source[i].X, source[i].Y, radian, center.X, center.Y, out double destinationX, out double destinationY);

                destination.Add(new PointD(destinationX, destinationY));
            }
        }

        /// <summary>
        /// 座標を指定座標を中心に指定角度回転させた座標に変換する
        /// </summary>
        /// <param name="sourceX">元座標X</param>
        /// <param name="sourceY">元座標Y</param>
        /// <param name="radian">回転角度</param>
        /// <param name="centerX">回転中心座標X</param>
        /// <param name="centerY">回転中心座標Y座標</param>
        /// <param name="destinationX">変換座標X</param>
        /// <param name="destinationY">変換座標Y</param>
        public static void GetRotation(double sourceX, double sourceY, double radian, double centerX, double centerY, out double destinationX, out double destinationY)
        {
            double offsetX = sourceX - centerX;
            double offsetY = sourceY - centerY;

            double rotationX = offsetX * System.Math.Cos(radian) - offsetY * System.Math.Sin(radian);
            double rotationY = offsetX * System.Math.Sin(radian) + offsetY * System.Math.Cos(radian);

            destinationX = rotationX + centerX;
            destinationY = rotationY + centerY;
        }

        /// <summary>
        /// コントロールの角を丸める
        /// </summary>
        /// <param name="control"></param>
        /// <param name="radius"></param>
        public static void RoundRect(System.Windows.Forms.Control control, double radius)
        {
            using var gp = new System.Drawing.Drawing2D.GraphicsPath();
            var size = control.Size;
            var diameterF = Convert.ToSingle(radius * 2);
            var radiusF = Convert.ToSingle(radius);

            gp.AddLine(radiusF, 0, size.Width - diameterF, 0);
            gp.AddArc(size.Width - diameterF, 0, diameterF, diameterF, 270, 90);

            gp.AddLine(size.Width, radiusF, size.Width, size.Height - diameterF);
            gp.AddArc(size.Width - diameterF, size.Height - diameterF, diameterF, diameterF, 0, 90);

            gp.AddLine(size.Width - diameterF, size.Height, radiusF, size.Height);
            gp.AddArc(0, size.Height - diameterF, diameterF, diameterF, 90, 90);

            gp.AddLine(0, size.Height - diameterF, 0, radiusF);
            gp.AddArc(0, 0, diameterF, diameterF, 180, 90);

            gp.CloseFigure();
            control.Region = new Region(gp);
        }

        /// <summary>
        /// アフィン変換(System.Drawing.Drawing2D.Matrix)
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="sourceX"></param>
        /// <param name="sourceY"></param>
        /// <param name="transformedX"></param>
        /// <param name="transformedY"></param>
        /// <remarks>System.Drawing.Drawing2D.Matrixが表すジオメトリック変換を、指定した点に適用し、double型で返します</remarks>
        public static void TransformPoint2F(System.Drawing.Drawing2D.Matrix mat, double sourceX, double sourceY, out double transformedX, out double transformedY)
        {
            var transformedF = GraphicsUtilities.TransformPoint2F(mat, sourceX, sourceY);

            transformedX = transformedF.X;
            transformedY = transformedF.Y;
        }

        /// <summary>
        /// アフィン変換(System.Drawing.Drawing2D.Matrix)
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="sourceX"></param>
        /// <param name="sourceY"></param>
        /// <returns></returns>
        /// <remarks>System.Drawing.Drawing2D.Matrixが表すジオメトリック変換を、指定した点に適用し、PointF型で返します</remarks>
        public static PointF TransformPoint2F(System.Drawing.Drawing2D.Matrix mat, double sourceX, double sourceY)
        {
            return GraphicsUtilities.TransformPointD2F(mat, new PointD(sourceX, sourceY));
        }

        /// <summary>
        /// アフィン変換(System.Drawing.Drawing2D.Matrix)
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="sourceX"></param>
        /// <param name="sourceY"></param>
        /// <returns></returns>
        /// <remarks>System.Drawing.Drawing2D.Matrixが表すジオメトリック変換を、指定した点に適用し、PointD型で返します</remarks>
        public static PointD TransformPoint2D(System.Drawing.Drawing2D.Matrix mat, double sourceX, double sourceY)
        {
            return GraphicsUtilities.TransformPointD2D(mat, new PointD(sourceX, sourceY));
        }

        /// <summary>
        /// アフィン変換(System.Drawing.Drawing2D.Matrix)
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <remarks>System.Drawing.Drawing2D.Matrixが表すジオメトリック変換を、指定した座標に適用し、PointD型で返します</remarks>
        public static PointD TransformPointD2D(System.Drawing.Drawing2D.Matrix mat, PointD source)
        {
            var transformedF = GraphicsUtilities.TransformPointD2F(mat, source);

            return new PointD(transformedF);
        }

        /// <summary>
        /// アフィン変換(System.Drawing.Drawing2D.Matrix)
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <remarks>System.Drawing.Drawing2D.Matrixが表すジオメトリック変換を、指定した座標に適用し、PointF型で返します</remarks>
        public static PointF TransformPointD2F(System.Drawing.Drawing2D.Matrix mat, PointD source)
        {
            var transformed = new PointF[] { source.ToPointF() };
            mat.TransformPoints(transformed);

            return transformed.First();
        }

        /// <summary>
        /// アフィン変換(System.Drawing.Drawing2D.Matrix)
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <remarks>System.Drawing.Drawing2D.Matrixが表すジオメトリック変換を、指定した点の配列に適用し、PointD型の配列で返します</remarks>
        public static PointD[] TransformPointArrayD2D(System.Drawing.Drawing2D.Matrix mat, PointD[] source)
        {
            var transformedF = GraphicsUtilities.TransformPointArrayD2F(mat, source);

            return Array.ConvertAll<PointF, PointD>(transformedF, delegate (PointF f) { return new PointD(f); });
        }

        /// <summary>
        /// アフィン変換(System.Drawing.Drawing2D.Matrix)
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <remarks>System.Drawing.Drawing2D.Matrixが表すジオメトリック変換を、指定した点の配列に適用し、PointF型の配列で返します</remarks>
        public static PointF[] TransformPointArrayD2F(System.Drawing.Drawing2D.Matrix mat, PointD[] source)
        {
            var transformed = Array.ConvertAll<PointD, PointF>(source, delegate (PointD d) { return d.ToPointF(); });

            mat.TransformPoints(transformed);

            return transformed;
        }

        /// <summary>
        /// 座標軸に平行な外接矩形の座標を求める
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rect1Begin"></param>
        /// <param name="rect1End"></param>
        /// <remarks>大量のデータには向きません。回転したコーナー4点などを処理対象として想定</remarks>
        public static void FindRectangle1(PointD[] source, out PointF rect1Lt, out PointF rect1Rb)
        {
            GraphicsUtilities.FindRectangle1(source, out PointD? tempLt, out PointD? tempRb);

            rect1Lt = tempLt.ToPointF();
            rect1Rb = tempRb.ToPointF();
        }

        /// <summary>
        /// 座標軸に平行な外接矩形の座標を求める
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rect1Begin"></param>
        /// <param name="rect1End"></param>
        /// <remarks>大量のデータには向きません。回転したコーナー4点などを処理対象として想定</remarks>
        public static void FindRectangle1(PointD[] source, out PointD rect1Lt, out PointD rect1Rb)
        {
            var rectT = double.MaxValue;
            var rectL = double.MaxValue;
            var rectB = double.MinValue;
            var rectR = double.MinValue;
            Array.ForEach(source, delegate (PointD p) { rectT = System.Math.Min(rectT, p.Y); });
            Array.ForEach(source, delegate (PointD p) { rectL = System.Math.Min(rectL, p.X); });
            Array.ForEach(source, delegate (PointD p) { rectB = System.Math.Max(rectB, p.Y); });
            Array.ForEach(source, delegate (PointD p) { rectR = System.Math.Max(rectR, p.X); });

            rect1Lt = new PointD(rectL, rectT);
            rect1Rb = new PointD(rectR, rectB);
        }

        /// <summary>
        /// 座標軸に平行な外接矩形の座標を求める
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rect1Begin"></param>
        /// <param name="rect1End"></param>
        /// <remarks>大量のデータには向きません。回転したコーナー4点などを処理対象として想定</remarks>
        public static void FindRectangle1(PointF[] source, out PointD rect1Lt, out PointD rect1Rb)
        {
            GraphicsUtilities.FindRectangle1(source, out PointF tempLt, out PointF tempRb);

            rect1Lt = new PointD(tempLt);
            rect1Rb = new PointD(tempRb);
        }

        /// <summary>
        /// 座標軸に平行な外接矩形の座標を求める
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rect1Begin"></param>
        /// <param name="rect1End"></param>
        /// <remarks>大量のデータには向きません。回転したコーナー4点などを処理対象として想定</remarks>
        public static void FindRectangle1(PointF[] source, out PointF rect1Lt, out PointF rect1Rb)
        {
            var rectT = float.MaxValue;
            var rectL = float.MaxValue;
            var rectB = float.MinValue;
            var rectR = float.MinValue;
            Array.ForEach(source, delegate (PointF p) { rectT = System.Math.Min(rectT, p.Y); });
            Array.ForEach(source, delegate (PointF p) { rectL = System.Math.Min(rectL, p.X); });
            Array.ForEach(source, delegate (PointF p) { rectB = System.Math.Max(rectB, p.Y); });
            Array.ForEach(source, delegate (PointF p) { rectR = System.Math.Max(rectR, p.X); });

            rect1Lt = new PointF(rectL, rectT);
            rect1Rb = new PointF(rectR, rectB);
        }
    }
}