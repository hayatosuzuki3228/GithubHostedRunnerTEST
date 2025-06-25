using Hutzper.Library.Common.Drawing;

namespace Hutzper.Library.ImageProcessing.Geometry
{
    /// <summary>
    /// 幾何学的計算結果を提供するインターフェース
    /// </summary>
    public interface IGeometryResult
    {
        /// <summary>
        /// ランレングスデータ
        /// </summary>
        public List<RunLength> RunLength { get; set; }

        /// <summary>
        /// 面積
        /// </summary>
        /// <remarks>画素数</remarks>
        public int Area { get; set; }

        /// <summary>
        /// XY座標軸に平行な矩形
        /// </summary>
        public Rectangle Rect { get; set; }

        /// <summary>
        /// 角度付き最小外接矩形
        /// </summary>
        public RotatedRectangleF RotatedRect { get; set; }
    }
}
