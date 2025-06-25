using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO;
using Hutzper.Library.ImageProcessing.Controller;

namespace Hutzper.Library.Onnx.PostProcessing;

/// <summary>
/// ブロブ検出
/// </summary>
[Serializable]
public record IncrementalBlobDetectionParameter : ControllerParameterBaseRecord
{
    /// <summary>
    /// 最大並列処理数
    /// </summary>
    [IniKey(true, 4)]
    public int MaxDegreeOfParallelism { get; set; } = 4;

    /// <summary>
    /// 画像枠に接触したオブジェクトを無視（除外）するかどうか
    /// </summary>
    [IniKey(true, false)]
    public bool IgnoreObjectsTouchingBoundary { get; set; } = false;

    /// <summary>
    /// RLEパラメータ
    /// </summary>
    public IRleControllerParameter RleControllerParameter { get; set; }

    #region コンストラクタ

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public IncrementalBlobDetectionParameter() : this(typeof(IncrementalBlobDetectionParameter).Name)
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="fileNameWithoutExtension"></param>
    public IncrementalBlobDetectionParameter(string fileNameWithoutExtension) : base("Blob_Detection".ToUpper(), "BlobDetectionParameter", $"{fileNameWithoutExtension}.ini")
    {
        this.IsHierarchy = false;
        this.RleControllerParameter = new RleControllerParameter();

        this.RleControllerParameter.MaximumLineNumber = 2048;
        this.RleControllerParameter.MaximumLabelNumber = 512;
        this.RleControllerParameter.MaximumRleNumber = 65535;
    }

    #endregion

    /// <summary>
    /// 読み込み
    /// </summary>
    /// <param name="iniFile"></param>
    /// <returns></returns>
    public override bool Read(IniFileReaderWriter iniFile)
    {
        var isSuccess = base.Read(iniFile);

        if (this.RleControllerParameter is IIniFileCompatible iniP)
        {
            isSuccess &= iniP.Read(iniFile);
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
        var isSuccess = base.Read(iniFile);

        if (this.RleControllerParameter is IIniFileCompatible iniP)
        {
            isSuccess &= iniP.Write(iniFile);
        }

        return isSuccess;
    }
}