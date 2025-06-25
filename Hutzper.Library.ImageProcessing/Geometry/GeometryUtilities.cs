using Hutzper.Library.Common.Drawing;
using OpenCvSharp;
using System.Drawing;
using Point = Hutzper.Library.Common.Drawing.Point;
using Rectangle = Hutzper.Library.Common.Drawing.Rectangle;
using Size = Hutzper.Library.Common.Drawing.Size;

namespace Hutzper.Library.ImageProcessing.Geometry
{
    /// <summary>
    /// 幾何学的な計算をサポートするユーティリティクラス
    /// </summary>
    /// <remarks>ランレングスデータの処理や、最小回転外接矩形の計算など、画像処理に関連する基本的な機能を提供します。</remarks>
    public static class GeometryUtilities
    {
        /// <summary>
        /// ランレングスデータから最小回転外接矩形を取得
        /// </summary>
        /// <param name="runLength">ランレングスデータ</param>
        /// <returns>最小回転外接矩形</returns>
        public static RotatedRectangleF GetSmallestRectangle2(List<RunLength> runLength)
        {
            // ランレングスデータをPoint2fに変換
            var cvPoints = new List<Point2f>();
            foreach (var run in runLength)
            {
                // 各ランレングスの始点と終点を追加
                cvPoints.Add(new Point2f(run.X, run.Y)); // 始点
                cvPoints.Add(new Point2f(run.X + run.Length, run.Y)); // 終点
                cvPoints.Add(new Point2f(run.X, run.Y + 1)); // 始点
                cvPoints.Add(new Point2f(run.X + run.Length, run.Y + 1)); // 終点
            }

            // 最小回転外接矩形を計算
            var rotatedRect = Cv2.MinAreaRect(cvPoints);

            // OpenCVの角度（度単位）をラジアンに変換
            float angleRadian = rotatedRect.Angle * (MathF.PI / 180);

            // 幅と高さを比較して角度を調整
            if (rotatedRect.Size.Width < rotatedRect.Size.Height)
            {
                // 幅が短辺の場合、+π/2を加える
                angleRadian += MathF.PI / 2;
            }

            // 角度を -π/2 < Phi <= π/2 に正規化
            if (angleRadian > MathF.PI / 2)
            {
                angleRadian -= MathF.PI;
            }
            else if (angleRadian <= -MathF.PI / 2)
            {
                angleRadian += MathF.PI;
            }

            // 結果を RotatedRectangleF に変換
            var rectangle = new RotatedRectangleF
            {
                Center = new PointF(rotatedRect.Center.X, rotatedRect.Center.Y),
                Length1 = MathF.Max(rotatedRect.Size.Width, rotatedRect.Size.Height) / 2f, // 長径の1/2
                Length2 = MathF.Min(rotatedRect.Size.Width, rotatedRect.Size.Height) / 2f, // 短径の1/2
                AngleRadian = angleRadian // 正規化された角度
            };

            // コーナー座標を算出(プロパティ更新)
            rectangle.CalculateCorners();

            return rectangle;
        }

        /// <summary>
        /// IGeometryResultをマージする
        /// </summary>
        /// <param name="results">元のIGeometryResultのリスト</param>
        /// <param name="distanceThreshold">この距離以内のものがマージされるしきい値</param>
        /// <returns>マージされた新しいIGeometryResultのリスト</returns>
        public static List<T> MergeCloseResults<T>(List<T> results, double distanceThreshold) where T : IGeometryResult
        {
            // 距離しきい値が0未満の場合はそのまま返す
            if (0 > distanceThreshold)
            {
                return results;
            }

            // IGeometryResultを距離しきい値に基づいてグループ化
            var groupedResults = GeometryUtilities.GroupResults(results, (float)distanceThreshold);

            // 個数が同じ(グループ化が行われなかった)場合はそのまま返す
            if (results.Count == groupedResults.Count)
            {
                return results;
            }

            // グループ化されたIGeometryResultをグループ単位でマージ
            var mergedResults = new List<T>();
            foreach (var group in groupedResults)
            {
                var newResult = group.First();

                if (1 >= group.Count)
                {
                    mergedResults.Add(newResult);
                    continue;
                }

                var minX = newResult.Rect.Left;
                var minY = newResult.Rect.Top;
                var maxX = newResult.Rect.Right;
                var maxY = newResult.Rect.Bottom;
                foreach (var r in group.Skip(1))
                {
                    newResult.Area += r.Area;

                    minX = Math.Min(minX, r.Rect.Left);
                    minY = Math.Min(minY, r.Rect.Top);
                    maxX = Math.Max(maxX, r.Rect.Right);
                    maxY = Math.Max(maxY, r.Rect.Bottom);

                    newResult.RunLength.AddRange(r.RunLength);
                }

                newResult.Rect = new Rectangle(new Point(minX, minY), new Size(maxX - minX + 1, maxY - minY + 1));
                newResult.RotatedRect = GeometryUtilities.GetSmallestRectangle2(newResult.RunLength);
                mergedResults.Add(newResult);
            }

            return mergedResults;
        }

        /// <summary>
        /// IGeometryResultオブジェクトを距離しきい値に基づいてグルーピングします。
        /// </summary>
        /// <param name="results">グルーピング対象のIGeometryResultリスト</param>
        /// <param name="distanceThreshold">距離のしきい値 (mm)</param>
        /// <returns>グループ化されたIGeometryResultリストのリスト</returns>
        public static List<List<T>> GroupResults<T>(List<T> results, float distanceThreshold) where T : IGeometryResult
        {
            // グループ化された結果を格納するリスト
            var groups = new List<List<T>>();

            // 訪問済みのIGeometryResultを管理するHashSet
            var visited = new HashSet<T>();

            // 各IGeometryResultについてグルーピングを実行
            foreach (var r in results)
            {
                // すでに訪問済みのIGeometryResultはスキップ
                if (visited.Contains(r)) continue;

                // 新しいグループを作成し、現在のIGeometryResultを追加
                var group = new List<T> { r };
                visited.Add(r);

                // 現在のIGeometryResultを基点に再帰的にグループ化
                GeometryUtilities.FindGroup(r, results, distanceThreshold, group, visited);

                // グループを結果リストに追加
                groups.Add(group);
            }

            return groups;
        }

        /// <summary>
        /// 指定したIGeometryResultを基点として、しきい値以内にあるIGeometryResultを再帰的に探索してグループ化します。
        /// </summary>
        /// <param name="result">基点となるIGeometryResult</param>
        /// <param name="results">すべてのIGeometryResultリスト</param>
        /// <param name="distanceThreshold">距離しきい値 (mm)</param>
        /// <param name="group">現在作成中のグループ</param>
        /// <param name="visited">訪問済みIGeometryResultのセット</param>
        public static void FindGroup<T>(
            T result,
            List<T> results,
            float distanceThreshold,
            List<T> group,
            HashSet<T> visited) where T : IGeometryResult
        {
            foreach (var other in results)
            {
                // 訪問済みのIGeometryResultはスキップ
                if (visited.Contains(other)) continue;

                // IGeometryResultのRotatedRect間の距離を計算し、しきい値以内の場合にグループに追加
                if (result.RotatedRect.DistanceFrom(other.RotatedRect) <= distanceThreshold)
                {
                    group.Add(other);
                    visited.Add(other);

                    // 追加したIGeometryResultを基点に再帰的に探索
                    GeometryUtilities.FindGroup(other, results, distanceThreshold, group, visited);
                }
            }
        }
    }
}
