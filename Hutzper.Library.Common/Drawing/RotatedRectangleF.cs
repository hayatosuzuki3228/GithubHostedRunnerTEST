namespace Hutzper.Library.Common.Drawing
{
    /// <summary>
    /// 角度付きの矩形を表すクラス
    /// </summary>
    [Serializable]
    public class RotatedRectangleF
    {
        #region プロパティ

        /// <summary>
        /// 矩形の中心座標
        /// </summary>
        public PointF Center { get; set; }

        /// <summary>
        /// 矩形の長径1/2
        /// </summary>
        /// <remarks>回転後の最も長い辺の長さ1/2</remarks>
        public float Length1 { get; set; }

        /// <summary>
        /// 矩形の短径1/2
        /// </summary>
        /// <remarks>回転後の最も短い辺の長さ1/2</remarks>
        public float Length2 { get; set; }

        /// <summary>
        /// 矩形の方向（弧測度）
        /// </summary>
        /// <remarks>ラジアン単位、-π/2 < AngleRadian <= π/2 の範囲</remarks>
        public float AngleRadian { get; set; }

        /// <summary>
        /// コーナー座標
        /// </summary>
        /// <remarks>CalculateCornersメソッドを呼び出してから参照してください</remarks>
        public PointF[] Corners => (PointF[])this.corners.Clone();

        #endregion

        #region フィールド

        private PointF[] corners = Array.Empty<PointF>();   // コーナー座標配列

        #endregion

        #region staticメソッド

        /// <summary>
        /// 矩形リストをしきい値に基づいてグルーピングします。
        /// 距離しきい値以内の矩形を同じグループにまとめます。
        /// </summary>
        /// <param name="rectangles">グルーピング対象の矩形リスト</param>
        /// <param name="threshold">矩形間の距離のしきい値 (mm)</param>
        /// <returns>グループ化された矩形リストのリスト</returns>
        public static List<List<RotatedRectangleF>> GroupRectangles(List<RotatedRectangleF> rectangles, float threshold)
        {
            // グループ化された結果を格納するリスト
            var groups = new List<List<RotatedRectangleF>>();

            // 訪問済みの矩形を管理するHashSet
            var visited = new HashSet<RotatedRectangleF>();

            // 各矩形についてグルーピングを実行
            foreach (var rect in rectangles)
            {
                // すでに訪問済みの矩形はスキップ
                if (true == visited.Contains(rect)) continue;

                // 新しいグループを作成し、現在の矩形を追加
                var group = new List<RotatedRectangleF> { rect };
                visited.Add(rect);

                // 現在の矩形を基点に再帰的にグループ化
                RotatedRectangleF.FindGroup(rect, rectangles, threshold, group, visited);

                // グループを結果リストに追加
                groups.Add(group);
            }

            return groups;
        }

        /// <summary>
        /// 指定した矩形を基点として、しきい値以内にある矩形を再帰的に探索してグループ化します。
        /// </summary>
        /// <param name="rect">基点となる矩形</param>
        /// <param name="rectangles">すべての矩形リスト</param>
        /// <param name="threshold">距離しきい値 (mm)</param>
        /// <param name="group">現在作成中のグループ</param>
        /// <param name="visited">訪問済み矩形のセット</param>
        private static void FindGroup(
            RotatedRectangleF rect,
            List<RotatedRectangleF> rectangles,
            float threshold,
            List<RotatedRectangleF> group,
            HashSet<RotatedRectangleF> visited)
        {
            foreach (var other in rectangles)
            {
                // 訪問済みの矩形はスキップ
                if (visited.Contains(other)) continue;

                // 矩形間の距離がしきい値以内の場合、グループに追加
                if (rect.DistanceFrom(other) <= threshold)
                {
                    group.Add(other);
                    visited.Add(other);

                    // 追加した矩形を基点に再帰的に探索
                    RotatedRectangleF.FindGroup(other, rectangles, threshold, group, visited);
                }
            }
        }
        #endregion

        #region publicメソッド

        /// <summary>
        /// コーナー座標を算出し、プロパティを更新します
        /// </summary>
        public void CalculateCorners()
        {
            // 中心からの距離
            var halfLength1 = this.Length1;
            var halfLength2 = this.Length2;

            // 回転行列を計算
            var cosTheta = MathF.Cos(this.AngleRadian);
            var sinTheta = MathF.Sin(this.AngleRadian);

            // ローカル座標系のコーナー座標（回転前）
            var localCorners = new[]
            {
                new PointF(-halfLength1, -halfLength2), // 左下
                new PointF( halfLength1, -halfLength2), // 右下
                new PointF( halfLength1,  halfLength2), // 右上
                new PointF(-halfLength1,  halfLength2)  // 左上
            };

            // グローバル座標系に変換
            var globalCorners = new PointF[4];
            for (int i = 0; i < 4; i++)
            {
                globalCorners[i] = new PointF
                (
                    Center.X + localCorners[i].X * cosTheta - localCorners[i].Y * sinTheta,
                    Center.Y + localCorners[i].X * sinTheta + localCorners[i].Y * cosTheta
                );
            }

            // プロパティを更新
            this.corners = globalCorners;
        }

        /// <summary>
        /// 他の矩形との距離を計算する
        /// </summary>
        public float DistanceFrom(RotatedRectangleF other)
        {
            // 矩形が交差している場合、距離はゼロ
            if (this.IsRectangleOverlapping(other))
            {
                return 0;
            }

            // 一方の矩形が他方を完全に含む場合、距離はゼロ
            if (this.IsRectangleContained(other) || other.IsRectangleContained(this))
            {
                return 0;
            }

            // 通常の距離計算
            return this.CalculateDistanceBetweenRectangles(this.Corners, other.Corners);
        }

        #endregion

        #region privateメソッド

        /// <summary>
        /// 矩形が交差しているかを判定
        /// </summary>
        private bool IsRectangleOverlapping(RotatedRectangleF other)
        {
            var corners1 = this.Corners;
            var corners2 = other.Corners;

            return !this.HasSeparatingAxis(corners1, corners2) && !this.HasSeparatingAxis(corners2, corners1);
        }

        /// <summary>
        /// 矩形が他方を完全に含むかを判定
        /// </summary>
        private bool IsRectangleContained(RotatedRectangleF other)
        {
            var otherCorners = other.Corners;

            foreach (var corner in otherCorners)
            {
                if (!this.IsPointInsideRectangle(corner))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 点が矩形の内部にあるかを判定
        /// </summary>
        private bool IsPointInsideRectangle(PointF point)
        {
            var edges = this.GetEdges(this.Corners);

            foreach (var edge in edges)
            {
                var dx = edge.End.X - edge.Start.X;
                var dy = edge.End.Y - edge.Start.Y;

                var normalX = -dy;
                var normalY = dx;

                var dotProduct = (point.X - edge.Start.X) * normalX + (point.Y - edge.Start.Y) * normalY;
                if (dotProduct > 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 矩形間の最短距離を計算
        /// </summary>
        private float CalculateDistanceBetweenRectangles(PointF[] corners1, PointF[] corners2)
        {
            var edges1 = this.GetEdges(corners1);
            var edges2 = this.GetEdges(corners2);

            var minDistance = float.MaxValue;

            foreach (var edge1 in edges1)
            {
                foreach (var edge2 in edges2)
                {
                    var distance = this.SegmentToSegmentDistance(edge1.Start, edge1.End, edge2.Start, edge2.End);
                    minDistance = MathF.Min(minDistance, distance);
                }
            }

            return minDistance;
        }

        /// <summary>
        /// 線分間の最短距離を計算
        /// </summary>
        private float SegmentToSegmentDistance(PointF p1, PointF q1, PointF p2, PointF q2)
        {
            var d1 = this.PointToSegmentDistance(p1, p2, q2);
            var d2 = this.PointToSegmentDistance(q1, p2, q2);
            var d3 = this.PointToSegmentDistance(p2, p1, q1);
            var d4 = this.PointToSegmentDistance(q2, p1, q1);

            return MathF.Min(MathF.Min(d1, d2), MathF.Min(d3, d4));
        }

        /// <summary>
        /// 点と線分間の距離を計算
        /// </summary>
        private float PointToSegmentDistance(PointF point, PointF segmentStart, PointF segmentEnd)
        {
            var dx = segmentEnd.X - segmentStart.X;
            var dy = segmentEnd.Y - segmentStart.Y;

            if (dx == 0 && dy == 0)
            {
                return this.Distance(point, segmentStart);
            }

            var t = ((point.X - segmentStart.X) * dx + (point.Y - segmentStart.Y) * dy) / (dx * dx + dy * dy);

            if (t < 0)
            {
                return this.Distance(point, segmentStart);
            }
            else if (t > 1)
            {
                return this.Distance(point, segmentEnd);
            }
            else
            {
                var closestX = segmentStart.X + t * dx;
                var closestY = segmentStart.Y + t * dy;
                return this.Distance(point, new PointF(closestX, closestY));
            }
        }

        /// <summary>
        /// 2点間の距離を計算
        /// </summary>
        private float Distance(PointF p1, PointF p2)
        {
            return MathF.Sqrt(MathF.Pow(p1.X - p2.X, 2) + MathF.Pow(p1.Y - p2.Y, 2));
        }

        /// <summary>
        /// 矩形の辺リストを取得
        /// </summary>
        private List<(PointF Start, PointF End)> GetEdges(PointF[] corners)
        {
            var edges = new List<(PointF Start, PointF End)>();
            for (int i = 0; i < corners.Length; i++)
            {
                edges.Add((corners[i], corners[(i + 1) % corners.Length]));
            }
            return edges;
        }

        /// <summary>
        /// Separating Axis Theorem (SAT) による分離軸判定
        /// </summary>
        private bool HasSeparatingAxis(PointF[] rect1Corners, PointF[] rect2Corners)
        {
            var axes = this.GetAxes(rect1Corners);

            foreach (var axis in axes)
            {
                var projection1 = this.ProjectRectangle(rect1Corners, axis);
                var projection2 = this.ProjectRectangle(rect2Corners, axis);

                if (projection1.Max < projection2.Min || projection2.Max < projection1.Min)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 矩形を特定軸に投影して最小値と最大値を取得
        /// </summary>
        private (float Min, float Max) ProjectRectangle(PointF[] corners, PointF axis)
        {
            var min = float.MaxValue;
            var max = float.MinValue;

            foreach (var corner in corners)
            {
                var projection = (corner.X * axis.X + corner.Y * axis.Y) /
                                   MathF.Sqrt(axis.X * axis.X + axis.Y * axis.Y);

                min = MathF.Min(min, projection);
                max = MathF.Max(max, projection);
            }

            return (min, max);
        }

        /// <summary>
        /// 矩形の法線軸を取得
        /// </summary>
        private List<PointF> GetAxes(PointF[] corners)
        {
            var axes = new List<PointF>();

            for (int i = 0; i < corners.Length; i++)
            {
                var p1 = corners[i];
                var p2 = corners[(i + 1) % corners.Length];

                var dx = p2.X - p1.X;
                var dy = p2.Y - p1.Y;

                axes.Add(new PointF(-dy, dx));
            }

            return axes;
        }

        #endregion
    }
}
