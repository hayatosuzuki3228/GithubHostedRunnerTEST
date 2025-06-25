using Hutzper.Library.Common.Drawing;
using Hutzper.Library.ImageProcessing.Geometry;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Data.Inspection
{
    /// <summary>
    /// ラベル情報
    /// </summary>
    [Serializable]
    public class LabelInfo : IGeometryResult
    {
        public int ClassIndex { get; set; } // クラスインデックス

        public bool IsNg { get; set; }       // NGかどうか

        public int Area { get; set; }       // 面積（画素数）

        public double DiagonalLengthMm { get; set; } // 対角線長さ(mm)

        public double MajorAxisLengthMm { get; set; } // 長軸長さ(mm)

        public double MinorAxisLengthMm { get; set; } // 短軸長さ(mm)

        public Library.Common.Drawing.Rectangle Rect { get; set; } = new();    // 外接矩形

        public List<RunLength> RunLength { get; set; } = new(); // ランレングス

        public RotatedRectangleF RotatedRect { get; set; } = new();    // 回転矩形
    }
}
