using Hutzper.Library.Onnx;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.PostProcessing
{
    /// <summary>
    /// IInferenceResultPostProcessing用パラメータインタフェース
    /// </summary>
    public interface IInferenceResultPostProcessingParameter
    {
        public OnnxModelAlgorithm OnnxModelAlgorithm { get; set; }
    }

    /// <summary>
    /// IInferenceResultPostProcessingParameter実装
    /// </summary>
    public class InferenceResultPostProcessingParameter : IInferenceResultPostProcessingParameter
    {
        public OnnxModelAlgorithm OnnxModelAlgorithm { get; set; }
    }
}
