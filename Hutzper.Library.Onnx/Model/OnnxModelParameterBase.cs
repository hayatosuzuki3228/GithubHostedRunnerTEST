using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.Onnx.Model;

/// <summary>
/// ONNXモデルパラメータインタフェース実装 record
/// </summary>
[Serializable]
public record OnnxModelParameterBase : ControllerParameterBaseRecord, IOnnxModelParameter
{
    #region IOnnxModelParameter

    /// <summary>
    /// 識別
    /// </summary>
    public virtual Common.Drawing.Point Location
    {
        get => this.location.Clone();
    }

    /// <summary>
    /// デバイスID
    /// </summary>
    [IniKey(true, -1)]
    public virtual int DeviceID { get; set; } = -1;

    /// <summary>
    /// onnxファイル名
    /// </summary>
    [IniKey(true, "")]
    public virtual string OnnxModelFullFileName { get; set; } = string.Empty;

    /// <summary>
    /// モデルのアルゴリズム
    /// </summary>
    [IniKey(true, OnnxModelAlgorithm.Undefined)]
    public OnnxModelAlgorithm Algorithm { get; set; } = OnnxModelAlgorithm.Undefined;

    /// <summary>
    /// ExecutionProvider
    /// </summary>
    [IniKey(true, OnnxModelExecutionProvider.Cuda)]
    public OnnxModelExecutionProvider ExecutionProvider { get; set; } = OnnxModelExecutionProvider.Cuda;

    #endregion

    protected Common.Drawing.Point location = new();
    protected string fileNameWithoutExtension;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public OnnxModelParameterBase() : this(new Common.Drawing.Point())
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public OnnxModelParameterBase(Common.Drawing.Point location) : this(location, typeof(OnnxModelParameterBase).Name)
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="directoryName"></param>
    public OnnxModelParameterBase(Common.Drawing.Point location, string fileNameWithoutExtension) : base($"OnnxModel_Y{location.Y + 1:D2}_X{location.X + 1:D2}", "OnnxModelParameter", $"{fileNameWithoutExtension}_Y{location.Y + 1:D2}_X{location.X + 1:D2}.ini")
    {
        this.IsHierarchy = false;
        this.fileNameWithoutExtension = fileNameWithoutExtension;
        this.location = location.Clone();
    }
}