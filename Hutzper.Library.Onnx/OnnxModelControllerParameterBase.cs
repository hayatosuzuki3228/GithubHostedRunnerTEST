using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO;
using Hutzper.Library.Onnx.Model;

namespace Hutzper.Library.Onnx;

/// <summary>
/// ONNXモデル制御パラメータベース
/// </summary>
[Serializable]
public record OnnxModelControllerParameterBase : ControllerParameterBaseRecord, IOnnxModelControllerParameter
{
    #region IControllerParameter

    /// <summary>
    /// IIniFileCompatibleリストを取得する
    /// </summary>
    /// <returns></returns>
    public override List<IIniFileCompatible> GetItems() => this.ModelParameters.ConvertAll(p => (IIniFileCompatible)p);

    #endregion

    #region IOnnxModelControllerParameter

    /// <summary>
    /// 画像取得パラメータ
    /// </summary>
    public List<IOnnxModelParameter> ModelParameters { get; } = new();

    #endregion

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="directoryName"></param>
    public OnnxModelControllerParameterBase() : this("OnnxModelController")
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="directoryName"></param>
    public OnnxModelControllerParameterBase(string fileNameWithoutExtension) : base("OnnxModel_Control", "OnnxModelControllerParameter", $"{fileNameWithoutExtension}.ini")
    {
        this.IsHierarchy = false;
    }

    /// <summary>
    /// 読み込み
    /// </summary>
    /// <param name="iniFile"></param>
    /// <returns></returns>
    public override bool Read(IniFileReaderWriter iniFile)
    {
        var isSuccess = base.Read(iniFile);

        foreach (var m in this.ModelParameters)
        {
            if (m is IIniFileCompatible i)
            {
                isSuccess &= i.Read(iniFile);
            }
        }

        return isSuccess;
    }

    /// <summary>
    /// 書き込み
    /// </summary>
    /// <param name="iniFile"></param>
    /// <returns></returns>
    public override bool Write(IniFileReaderWriter iniFile)
    {
        var isSuccess = base.Write(iniFile);

        foreach (var m in this.ModelParameters)
        {
            if (m is IIniFileCompatible i)
            {
                isSuccess &= i.Write(iniFile);
            }
        }

        return isSuccess;
    }
}