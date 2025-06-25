using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace Hutzper.Library.Onnx.Data;

/// <summary>
/// IOnnxDataOutputClassProbability実装
/// </summary>
[Serializable]
public class OnnxDataOutputClassProbability : OnnxDataOutput, IOnnxDataOutputClassProbability
{
    #region IOnnxDataOutputClassProbability

    /// <summary>
    /// クラス確率値
    /// </summary>
    public float[] ClassProbability { get; protected set; } = Array.Empty<float>();

    #endregion

    #region IOnnxDataOutput

    /// <summary>
    /// モデル出力データ設定
    /// </summary>
    /// <param name="resultOfRun"></param>
    public override void CopyFrom(IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? resultOfRun)
    {
        try
        {
            this.ClassProbability = Array.Empty<float>();

            base.CopyFrom(resultOfRun);

            if (this.RawCollection is not null)
            {
                foreach (var data in this.RawCollection.Where(c => c.ElementType == TensorElementType.Float))
                {
                    if (data.AsTensor<float>() is Tensor<float> tensorData)
                    {
                        this.ClassProbability = tensorData.ToArray();
                        break;
                    }
                }
            }
        }
        catch
        {
            this.RawCollection = null;
        }
    }

    #endregion
}