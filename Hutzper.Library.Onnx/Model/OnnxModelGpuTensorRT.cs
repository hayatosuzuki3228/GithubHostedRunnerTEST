using Microsoft.ML.OnnxRuntime;

namespace Hutzper.Library.Onnx.Model;

/// <summary>
/// ONNX モデル for QUDA
/// </summary>
[Serializable]
public class OnnxModelGpuTensorRT : OnnxModelBase
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

                if (true == fileInfo.Exists && this.ExecutionProvider == OnnxModelExecutionProvider.TensorRT)
                {
                    var so = new SessionOptions();

                    var trtOptions = new OrtTensorRTProviderOptions();
                    trtOptions.UpdateOptions(new()
                    {
                        { "device_id", System.Math.Max(0, this.Parameter.DeviceID).ToString() },
                        { "trt_fp16_enable", "True" },
                        { "trt_engine_cache_enable", "True" },
                        { "trt_engine_cache_path", fileInfo.DirectoryName },
                    });
                    so.AppendExecutionProvider_Tensorrt(trtOptions);
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
    public OnnxModelGpuTensorRT() : this(typeof(OnnxModelGpuTensorRT).Name, Common.Drawing.Point.New())
    {

    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public OnnxModelGpuTensorRT(Common.Drawing.Point location) : this(typeof(OnnxModelGpuTensorRT).Name, location)
    {

    }
    public OnnxModelGpuTensorRT(string Identifier) : base(Identifier)
    {

    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="nickname"></param>
    /// <param name="location"></param>
    public OnnxModelGpuTensorRT(string nickname, Common.Drawing.Point location) : base(nickname, location)
    {
        this.ExecutionProvider = OnnxModelExecutionProvider.TensorRT;
    }

    #endregion
}