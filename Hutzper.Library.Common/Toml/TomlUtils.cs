using System.IO;
using Tomlyn;

namespace Hutzper.Library.Common.Toml;

static public class TomlUtils
{
    public static T? LoadToml<T>(string path) where T : class, new()
    {
        if (File.Exists(path) == false) return null;
        string text = File.ReadAllText(path);
        try
        {
            return Tomlyn.Toml.ToModel<T>(text);
        }
        catch
        {
            return null;
        }
    }

    public static void SaveToml(string path, object value)
    {
        if (path is not "")
        {
            var text = Tomlyn.Toml.FromModel(value);
            File.WriteAllText(path, text);
        }
    }

    public static T? ToModel<T>(string text) where T : class, new() => Tomlyn.Toml.ToModel<T>(text);
    public static string FromModel(object obj) => Tomlyn.Toml.FromModel(obj);
}