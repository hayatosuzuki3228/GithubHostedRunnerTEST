namespace Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.PostProcessing
{
    /// <summary>
    /// セグメンテーション後処理パラメータ
    /// </summary>
    public class SegmentationPostProcessingParameter : InferenceResultPostProcessingParameter
    {
        /// <summary>
        /// 最大クラス数
        /// </summary>
        public int MaxNumberOfClasses { get; set; } = 8;

        /// <summary>
        /// 最大画像高さ
        /// </summary>
        public int MaximumImageHeight { get; set; } = 2048;

        /// <summary>
        /// 最大ラベル数
        /// </summary>
        public int MaximumLabelNumber { get; set; } = 100;

        /// <summary>
        /// 2値化領域数
        /// </summary>
        public int MaximumRegionNumber { get; set; } = 2048 * 20;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SegmentationPostProcessingParameter()
        {
            this.OnnxModelAlgorithm = Library.Onnx.OnnxModelAlgorithm.Segmentation_Image;
        }
    }
}
