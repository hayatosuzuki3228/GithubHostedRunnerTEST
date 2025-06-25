using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace Hutzper.Library.Onnx.Data;

/// <summary>
///  ONNN用出力データ:セグメンテーション用
/// </summary>
[Serializable]
public class OnnxDataOutputSegmentation : OnnxDataOutput, IOnnxDataOutputClassIndexed
{
    #region IOnnxDataOutput

    /// <summary>
    /// モデル出力データ設定
    /// </summary>
    /// <param name="resultOfRun"></param>
    public override void CopyFrom(IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? resultOfRun)
    {
        try
        {
            this.ClassIndexed.Clear();

            base.CopyFrom(resultOfRun);

            if (this.RawCollection is not null)
            {
                foreach (var data in this.RawCollection.Where(c => c.ElementType == TensorElementType.UInt8))
                {
                    if (data.AsTensor<byte>() is Tensor<byte> tensorData && 3 < tensorData.Dimensions.Length)
                    {
                        var allBytes = tensorData.ToArray();

                        var classNum = tensorData.Dimensions[1];
                        var frameHeight = tensorData.Dimensions[2];
                        var frameWidth = tensorData.Dimensions[3];

                        this.ClassFrameSize = new Common.Drawing.Size(frameWidth, frameHeight);

                        var frameLength = frameHeight * frameWidth;
                        foreach (var classId in Enumerable.Range(0, classNum))
                        {
                            var frameArray = new ArraySegment<byte>(allBytes, classId * frameLength, frameLength);

                            this.ClassIndexed.Add(frameArray.ToArray());
                        }

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

    #region IOnnxDataOutputClassIndexed

    /// <summary>
    /// フレームサイズ
    /// </summary>
    public Common.Drawing.Size ClassFrameSize { get; set; } = new();

    /// <summary>
    /// クラス別検出インデックス
    /// </summary>
    public List<byte[]> ClassIndexed { get; } = new();

    #endregion
}