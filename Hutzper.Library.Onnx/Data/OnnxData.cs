using Hutzper.Library.Common;
using Hutzper.Library.Onnx.Model;
using Microsoft.ML.OnnxRuntime;

namespace Hutzper.Library.Onnx.Data;

/// <summary>
/// ONNX用データコンテナ
/// </summary>
[Serializable]
public abstract class OnnxData : SafelyDisposable, IOnnxData
{
    #region IOnnxData

    /// <summary>
    /// 入力メタデータ
    /// </summary>
    public virtual IReadOnlyDictionary<string, NodeMetadata> InputMetadata { get; protected set; } = new Dictionary<string, NodeMetadata>();

    /// <summary>
    /// 出力メタデータ
    /// </summary>
    public virtual IReadOnlyDictionary<string, NodeMetadata> OutputMetadata { get; protected set; } = new Dictionary<string, NodeMetadata>();

    /// <summary>
    /// 初期化
    /// </summary>
    public virtual void Initialize(IOnnxModel model)
    {
        try
        {
            this.InputMetadata = model.GetInputMetadata();
            this.OutputMetadata = model.GetOutputMetadata();
        }
        catch
        {
        }
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
        }
        catch
        {
        }
    }

    #endregion
}