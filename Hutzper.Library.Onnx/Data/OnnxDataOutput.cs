using Microsoft.ML.OnnxRuntime;

namespace Hutzper.Library.Onnx.Data;

/// <summary>
/// ONNN用出力データ基本クラス
/// </summary>
[Serializable]
public class OnnxDataOutput : OnnxData, IOnnxDataOutput
{
    #region IOnnxDataOutput

    /// <summary>
    /// モデル出力生データ
    /// </summary>
    public virtual IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? RawCollection { get; protected set; }

    /// <summary>
    /// モデル出力データ設定
    /// </summary>
    /// <param name="resultOfRun"></param>
    public virtual void CopyFrom(IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? resultOfRun)
    {
        try
        {
            this.DisposeSafely(this.RawCollection);

            this.RawCollection = resultOfRun;
        }
        catch
        {
            this.RawCollection = null;
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
            base.DisposeExplicit();
            this.DisposeSafely(this.RawCollection);
        }
        catch
        {
        }
        finally
        {
            this.RawCollection = null;
        }
    }

    #endregion
}