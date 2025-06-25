using Microsoft.ML.OnnxRuntime;

namespace Hutzper.Library.Onnx.Data;

/// <summary>
/// ONNX出力データインタフェース
/// </summary>
public interface IOnnxDataOutput : IOnnxData
{
    /// <summary>
    /// モデル出力生データ
    /// </summary>
    public IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? RawCollection { get; }

    /// <summary>
    /// モデル出力データ設定
    /// </summary>
    /// <param name="resultOfRun"></param>
    public void CopyFrom(IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? resultOfRun);
}