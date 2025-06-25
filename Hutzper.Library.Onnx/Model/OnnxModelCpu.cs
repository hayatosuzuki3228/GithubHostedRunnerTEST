using Microsoft.ML.OnnxRuntime;

namespace Hutzper.Library.Onnx.Model;

/// <summary>
/// ONNX モデル for QUDA
/// </summary>
[Serializable]
public class OnnxModelCpu : OnnxModelBase
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

                if (true == fileInfo.Exists && this.ExecutionProvider == OnnxModelExecutionProvider.Cpu)
                {
                    var so = new SessionOptions();
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
    public OnnxModelCpu() : this(typeof(OnnxModelCpu).Name, Common.Drawing.Point.New())
    {

    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public OnnxModelCpu(Common.Drawing.Point location) : this(typeof(OnnxModelCpu).Name, location)
    {

    }
    public OnnxModelCpu(string Identifier) : base(Identifier)
    {

    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="nickname"></param>
    /// <param name="location"></param>
    public OnnxModelCpu(string nickname, Common.Drawing.Point location) : base(nickname, location)
    {
        this.ExecutionProvider = OnnxModelExecutionProvider.Cpu;
    }

    #endregion
}