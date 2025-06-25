using Microsoft.ML.OnnxRuntime;

namespace Hutzper.Library.Onnx.Model;

/// <summary>
/// ONNX モデル for QUDA
/// </summary>
[Serializable]
public class OnnxModelGpuCuda : OnnxModelBase
{
    #region IController

    /// <summary>
    /// オープン
    /// </summary>
    /// <returns></returns>
    public override bool Open()
    {
        try
        {
            if (this.Parameter is not null && this.InferenceSession is null)
            {
                var fileInfo = new FileInfo(this.Parameter.OnnxModelFullFileName);

                if (true == fileInfo.Exists && this.ExecutionProvider == OnnxModelExecutionProvider.Cuda)
                {
                    var so = new SessionOptions()
                    {
                        InterOpNumThreads = 1,
                        IntraOpNumThreads = 1,
                        GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_EXTENDED
                    };

                    so.AppendExecutionProvider_CUDA(System.Math.Max(0, this.Parameter.DeviceID));

                    this.InferenceSession = new InferenceSession(fileInfo.FullName, so);
                }
            }
        }
        catch (Exception ex)
        {
            this.InferenceSession = null;
            Serilog.Log.Warning(ex, ex.Message);
        }

        return this.Enabled;
    }

    #endregion

    #region コンストラクタ

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public OnnxModelGpuCuda() : this(typeof(OnnxModelGpuCuda).Name, Common.Drawing.Point.New())
    {

    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public OnnxModelGpuCuda(Common.Drawing.Point location) : this(typeof(OnnxModelGpuCuda).Name, location)
    {

    }
    public OnnxModelGpuCuda(string Identifier) : base(Identifier)
    {

    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="nickname"></param>
    /// <param name="location"></param>
    public OnnxModelGpuCuda(string nickname, Common.Drawing.Point location) : base(nickname, location)
    {
        this.ExecutionProvider = OnnxModelExecutionProvider.Cuda;
    }

    #endregion
}