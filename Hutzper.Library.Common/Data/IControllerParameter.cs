using Hutzper.Library.Common.IO;

namespace Hutzper.Library.Common.Data;

/// <summary>
/// 制御系パラメーター
/// </summary>
public interface IControllerParameter : IEquatable<IControllerParameter>
{
    /// <summary>
    /// データの保存に階層構造(サブディレクトリ)を用いるか
    /// </summary>
    public bool IsHierarchy { get; init; }

    /// <summary>
    /// ディレクトリ名
    /// </summary>
    /// <remarks>IPathManagerのアクセスに使用するキー</remarks>
    public string DirectoryKey { get; init; }

    /// <summary>
    /// 管理ファイル名
    /// </summary>
    public string[] FileNames { get; init; }

    /// <summary>
    /// IIniFileCompatibleリストを取得する
    /// </summary>
    /// <returns></returns>
    public List<IIniFileCompatible> GetItems();
}

/// <summary>
/// IControllerParameter 実装:class
/// </summary>
[Serializable]
public class ControllerParameterBase : IControllerParameter
{
    #region IControllerParameter

    /// <summary>
    /// データの保存に階層構造(サブディレクトリを用いるか)
    /// </summary>
    public virtual bool IsHierarchy { get; init; } = true;

    /// <summary>
    /// ディレクトリ名
    /// </summary>
    public virtual string DirectoryKey { get; init; }

    /// <summary>
    /// 管理ファイル名
    /// </summary>
    public virtual string[] FileNames { get; init; }

    /// <summary>
    /// IIniFileCompatibleリストを取得する
    /// </summary>
    /// <returns></returns>
    public virtual List<IIniFileCompatible> GetItems() => new();

    #endregion

    #region IEquatable

    public virtual bool Equals(IControllerParameter? compalison)
    {
        if (this is IIniFileCompatible iniOwn && compalison is IIniFileCompatible iniCom)
        {
            if (false == iniOwn.Equals(iniCom))
            {
                return false;
            }
        }
        var items1 = this.GetItems();
        var items2 = compalison?.GetItems() ?? new(); ;

        if (items1.Count != items2.Count)
        {
            return false;
        }

        foreach (var i in Enumerable.Range(0, items1.Count))
        {
            if (false == items1[i].Equals(items2[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        hashCode.Add(base.GetHashCode());

        this.GetItems().ForEach(item => hashCode.Add(item));

        return hashCode.ToHashCode();
    }

    #endregion

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="directoryName"></param>
    public ControllerParameterBase(string directoryName, params string[] fileNames)
    {
        this.DirectoryKey = directoryName;
        this.FileNames = fileNames;
    }
}

/// <summary>
/// IControllerParameter 実装:record
/// </summary>
[Serializable]
public record ControllerParameterBaseRecord : IniFileCompatible<ControllerParameterBaseRecord>, IControllerParameter
{
    #region IControllerParameter

    /// <summary>
    /// データの保存に階層構造(サブディレクトリを用いるか)
    /// </summary>
    public virtual bool IsHierarchy { get; init; } = true;

    /// <summary>
    /// ディレクトリ名
    /// </summary>
    public virtual string DirectoryKey { get; init; }

    /// <summary>
    /// 管理ファイル名
    /// </summary>
    public virtual string[] FileNames { get; init; }

    /// <summary>
    /// IIniFileCompatibleリストを取得する
    /// </summary>
    /// <returns></returns>
    public virtual List<IIniFileCompatible> GetItems() => new();

    #endregion

    #region IEquatable

    public virtual bool Equals(IControllerParameter? compalison)
    {
        var items1 = this.GetItems();
        var items2 = compalison?.GetItems() ?? new(); ;

        if (items1.Count != items2.Count)
        {
            return false;
        }

        foreach (var i in Enumerable.Range(0, items1.Count))
        {
            if (false == items1[i].Equals(items2[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        hashCode.Add(base.GetHashCode());

        this.GetItems().ForEach(item => hashCode.Add(item));

        return hashCode.ToHashCode();
    }

    #endregion

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="directoryName"></param>
    public ControllerParameterBaseRecord(string directoryName, params string[] fileNames) : this(directoryName, directoryName, fileNames)
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="directoryName"></param>
    public ControllerParameterBaseRecord(string iniFileSection, string directoryName, params string[] fileNames) : base(iniFileSection.ToUpper())
    {
        this.DirectoryKey = directoryName;
        this.FileNames = fileNames;
    }
}