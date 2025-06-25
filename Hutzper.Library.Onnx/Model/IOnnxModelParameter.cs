using Hutzper.Library.Common.Data;

namespace Hutzper.Library.Onnx.Model;

/// <summary>
/// ONNXモデルパラメータインタフェース
/// </summary>
public interface IOnnxModelParameter : IControllerParameter
{
    /// <summary>
    /// 識別
    /// </summary>
    public Common.Drawing.Point Location { get; }

    /// <summary>
    /// デバイスID
    /// </summary>
    public int DeviceID { get; set; }

    /// <summary>
    /// onnxファイル名
    /// </summary>
    public string OnnxModelFullFileName { get; set; }

    /// <summary>
    /// モデルのアルゴリズム
    /// </summary>
    public OnnxModelAlgorithm Algorithm { get; set; }

    /// <summary>
    /// ExecutionProvider
    /// </summary>
    public OnnxModelExecutionProvider ExecutionProvider { get; set; }
}