using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace Hutzper.Library.Onnx.Data;

/// <summary>
/// ONNX入力データインタフェース
/// </summary>
public interface IOnnxDataInput : IOnnxData
{
    /// <summary>
    /// Tensorデータ
    /// </summary>
    public Dictionary<string, Tuple<DenseTensor<byte>, byte[]>> InputList { get; }

    /// <summary>
    /// モデル入力データ作成
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public IReadOnlyCollection<NamedOnnxValue> ToReadOnlyCollection();

    /// <summary>
    /// モデル入力データ作成
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public IReadOnlyCollection<NamedOnnxValue> ToReadOnlyCollectionForDryFire();
}