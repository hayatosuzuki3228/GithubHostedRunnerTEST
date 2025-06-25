using Hutzper.Library.Common.Drawing;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Data.Inspection
{
    /// <summary>
    /// セグメンテーション用データ
    /// </summary>
    [Serializable]
    public class SegmentationAdditionalData : AdditionalDataContainerBase
    {
        /// <summary>
        /// クラスごとのラベル情報
        /// </summary>
        /// <remarks>背景は含みません</remarks>
        public List<List<LabelInfo>> LabelsPerClass { get; set; } = new();

        /// <summary>
        /// 元画像サイズ
        /// </summary>
        public Hutzper.Library.Common.Drawing.Size RawImageSize { get; set; } = new();

        /// <summary>
        /// フレームサイズ
        /// </summary>
        public Hutzper.Library.Common.Drawing.Size ClassFrameSize { get; set; } = new();

        /// <summary>
        /// 元画像サイズにあわせるためのスケール
        /// </summary>
        public double FitToOriginalScale { get; set; } = 1d;

        /// <summary>
        /// 元画像サイズにあわせるためのパディング
        /// </summary>
        public SizeD FitToOriginalPadding { get; set; } = new(1, 1);
    }
}
