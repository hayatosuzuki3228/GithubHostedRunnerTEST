using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO;
using Hutzper.Library.ImageProcessing.Controller;

namespace Hutzper.Library.Onnx.PostProcessing;

/// <summary>
/// ブロブ検出
/// </summary>
[Serializable]
public record BlobDetectionParameter : ControllerParameterBaseRecord
{
    /// <summary>
    /// 最大並列処理数
    /// </summary>
    [IniKey(true, 4)]
    public int MaxDegreeOfParallelism { get; set; } = 4;

    /// <summary>
    /// 最大クラス数
    /// </summary>
    [IniKey(true, 4)]
    public int MaxNumberOfClasses { get; set; } = 4;

    /// <summary>
    /// 画像枠に接触したオブジェクトを無視（除外）するかどうか
    /// </summary>
    [IniKey(true, true)]
    public bool IgnoreObjectsTouchingBoundary { get; set; } = true;

    /// <summary>
    /// RLEパラメータ
    /// </summary>
    public IRleControllerParameter RleControllerParameter { get; set; }

    #region コンストラクタ

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public BlobDetectionParameter() : this(typeof(BlobDetectionParameter).Name)
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="fileNameWithoutExtension"></param>
    public BlobDetectionParameter(string fileNameWithoutExtension) : base("Blob_Detection".ToUpper(), "BlobDetectionParameter", $"{fileNameWithoutExtension}.ini")
    {
        this.IsHierarchy = false;
        this.RleControllerParameter = new RleControllerParameter();

        this.RleControllerParameter.MaximumLineNumber = 512;
        this.RleControllerParameter.MaximumLabelNumber = 1000;
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