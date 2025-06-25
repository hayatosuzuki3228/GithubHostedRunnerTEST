using System.Runtime.Serialization;
using static Hutzper.Library.Common.Toml.TomlUtils;

namespace Hutzper.Library.Common.Toml;

// 共通データ
[Serializable]
public abstract record Common<T> where T : Common<T>, new()
{
    public DateTime DateTimeOfCreation { get; set; } = DateTime.Now;
    public DateTime DateTimeOfUpdating { get; set; } = DateTime.Now;
    public const string Extension = ".toml";

    [IgnoreDataMember]  // tomlには書き出さない
    public string FullPath { get; set; } = string.Empty;

    public static T? Deserialize(string fullPath)
    {
        Serilog.Log.Information($"Deserialize: {fullPath}");
        var result = LoadToml<T>(fullPath);
        if (result != null)
        {
            result.FullPath = fullPath;
        }
        return result;
    }
    public void Serialize(string fullPath)
    {
        DateTimeOfUpdating = DateTime.Now;
        Serilog.Log.Information($"Serialize: {fullPath}");
        var folder = Path.GetDirectoryName(fullPath);
        Directory.CreateDirectory(folder!);
        SaveToml(fullPath, this);
        FullPath = fullPath;
    }
    public void Serialize()
    {
        SaveToml(FullPath, this);
    }
    public T? DeepClone() => ToModel<T>(FromModel(this));
}