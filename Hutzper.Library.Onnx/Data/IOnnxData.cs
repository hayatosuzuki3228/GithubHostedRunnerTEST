using Hutzper.Library.Onnx.Model;
using Microsoft.ML.OnnxRuntime;

namespace Hutzper.Library.Onnx.Data;

/// <summary>
/// ONNXデータインタフェース
/// </summary>
public interface IOnnxData : IDisposable
{
    /// <summary>
    /// 入力メタデータ
    /// </summary>
    public IReadOnlyDictionary<string, NodeMetadata> InputMetadata { get; }

    /// <summary>
    /// 出力メタデータ
    /// </summary>
    public IReadOnlyDictionary<string, NodeMetadata> OutputMetadata { get; }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize(IOnnxModel model);
}