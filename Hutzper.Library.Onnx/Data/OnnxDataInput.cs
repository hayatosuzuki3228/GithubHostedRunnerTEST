using Hutzper.Library.Common;
using Hutzper.Library.Onnx.Model;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Hutzper.Library.Onnx.Data;

/// <summary>
/// ONNX用入力データ基本クラス
/// </summary>
[Serializable]
public class OnnxDataInput : OnnxData, IOnnxDataInput
{
    #region IOnnxDataInput

    /// <summary>
    /// バッチサイズ
    /// </summary>
    public int BatchSize { get; set; } = 1;

    /// <summary>
    /// Tensorデータ
    /// </summary>
    public virtual Dictionary<string, Tuple<DenseTensor<byte>, byte[]>> InputList { get; protected set; } = new();

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="model"></param>
    /// <returns>Tensorデータ作成</returns>
    public override void Initialize(IOnnxModel model)
    {
        try
        {
            base.Initialize(model);

            this.InputList = new();

            if (this.InputMetadata is not null)
            {
                foreach (var data in this.InputMetadata)
                {
                    var dim = data.Value.Dimensions;
                    if (data.Key == "input_img") this.BatchSize = (dim.Length == 4) ? dim[0] : 1;

                    var tempTensor = new DenseTensor<byte>(dim);

                    var holdingBytes = tempTensor.ToArray();
                    var holdingTensor = new DenseTensor<byte>(new Memory<byte>(holdingBytes), tempTensor.Dimensions);

                    this.InputList.Add(data.Key, Tuple.Create(holdingTensor, holdingBytes));
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    /// <summary>
    /// モデル入力データ作成
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public virtual IReadOnlyCollection<NamedOnnxValue> ToReadOnlyCollection()
    {
        return this.ToReadOnlyCollectionForDryFire();
    }

    /// <summary>
    /// モデル入力データ作成
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public IReadOnlyCollection<NamedOnnxValue> ToReadOnlyCollectionForDryFire()
    {
        var collection = new List<NamedOnnxValue>();

        try
        {
            if (this.InputMetadata is not null)
            {
                var randomNumberGenerator = new GaussianDistributionRng();
                randomNumberGenerator.Initialize(new GaussianDistributionRnp() { Mean = 128, StandardDeviation = 64 });

                foreach (var data in this.InputMetadata)
                {
                    if (this.InputList.ContainsKey(data.Key))
                    {
                        var byteArray = this.InputList[data.Key].Item2;
                        foreach (var i in Enumerable.Range(0, byteArray.Length))
                        {
                            byteArray[i] = (byte)randomNumberGenerator.NextDouble();
                        }
                        var tensorData = this.InputList[data.Key].Item1;

                        collection.Add(NamedOnnxValue.CreateFromTensor(data.Key, tensorData));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

        return new ReadOnlyCollection<NamedOnnxValue>(collection);
    }

    #endregion

    #region SafelyDisposable

    /// <summary>
    /// リソースの解放
    /// </summary>
    protected override void DisposeExplicit()
    {
        try
        {
            base.DisposeExplicit();
        }
        catch
        {
        }
    }

    #endregion
}