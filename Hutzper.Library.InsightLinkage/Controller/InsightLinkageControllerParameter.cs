using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO;
using Hutzper.Library.InsightLinkage.Connection;

namespace Hutzper.Library.InsightLinkage.Controller;

/// <summary>
/// IInsightLinkageControllerParameter実装
/// </summary>
[Serializable]

public record InsightLinkageControllerParameter : ControllerParameterBaseRecord, IInsightLinkageControllerParameter
{
    #region IControllerParameter

    /// <summary>
    /// IIniFileCompatibleリストを取得する
    /// </summary>
    /// <returns></returns>
    public override List<IIniFileCompatible> GetItems() => this.ConnectionParameters.ConvertAll(p => (IIniFileCompatible)p);

    #endregion

    #region IInsightLinkageControllerParameter

    /// <summary>
    /// 使用するかどうか
    /// </summary>
    [IniKey(true, true)]
    public bool IsUse { get; set; } = true;

    /// <summary>
    /// 再接続
    /// </summary>
    public bool IsReconnectable { get; set; } = false;

    /// <summary>
    /// 再接続試行間隔
    /// </summary>
    public int ReconnectionAttemptsIntervalSec { get; set; } = 5;

    /// <summary>
    /// 管理するコネクション毎のパラメータリスト
    /// </summary>
    public List<IConnectionParameter> ConnectionParameters { get; set; } = new();

    public List<IFileUploaderParameter> FileUploaderParameters => this.ConnectionParameters.Where(p => p is IFileUploaderParameter).ToList().ConvertAll(p => (IFileUploaderParameter)p).ToList();

    public List<ITextMessengerParameter> TextMessengerParameter => this.ConnectionParameters.Where(p => p is ITextMessengerParameter).ToList().ConvertAll(p => (ITextMessengerParameter)p).ToList();

    #endregion

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="directoryName"></param>
    public InsightLinkageControllerParameter() : this(-1, typeof(InsightLinkageControllerParameter).Name)
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="directoryName"></param>
    public InsightLinkageControllerParameter(int index, string fileNameWithoutExtension) : base($"Insight_Linkage_{((index < 0) ? 0 : index) + 1:D2}", "InsightLinkageControllerParameter", $"{fileNameWithoutExtension}_{((index < 0) ? 0 : index) + 1:D2}.ini")
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

        foreach (var p in this.ConnectionParameters)
        {
            if (p is IIniFileCompatible i)
            {
                isSuccess &= i.Read(iniFile);
            }
        }

        return isSuccess;
    }

    /// <summary>
    /// 読み込み
    /// </summary>
    /// <param name="iniFile"></param>
    /// <returns></returns>
    public override bool Write(IniFileReaderWriter iniFile)
    {
        var isSuccess = base.Write(iniFile);

        foreach (var p in this.ConnectionParameters)
        {
            if (p is IIniFileCompatible i)
            {
                isSuccess &= i.Write(iniFile);
            }
        }

        return isSuccess;
    }
}