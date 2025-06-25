using System.Runtime.Serialization;
using Tomlyn.Model;

namespace Hutzper.Library.Common.Toml;

// ユーザーコントロールの種類
public enum UICType
{
    Default,
    LightBrightnessSlider,  // 照明用の明るさスライダー
    OnnxName,
    InsightProject,
    IsExistFile,  // ファイルの存在有無
    IPAddress,  // IPアドレス
    SelectFolder, // フォルダ選択
    SelectFile, // フォルダ選択
    IsAIConfig, // AI設定
}

public record UserEditVariable<T> : ITomlMetadataProvider
{
    // 値
    public T? Value { get; set; } = default;

    // コンストラクタ
    public UserEditVariable() { }

    public UserEditVariable(T Value, EditingData<T>? EditingData)
    {
        this.Value = Value;
        this.EditingData = EditingData ?? new EditingData<T>();
    }

    public UserEditVariable(T Value)
    {
        this.Value = Value;
        this.EditingData = null;
    }

    // 編集用のデータ
    [IgnoreDataMember]  // tomlには書き出さない
    public EditingData<T>? EditingData { get; init; } = new();

    // [IgnoreDataMember]を使うために必要な実装
    TomlPropertiesMetadata? ITomlMetadataProvider.PropertiesMetadata { get; set; }

    // UserEditVariable<float> name;
    // name.Value = 0.5f;   // 面倒なので
    // name = 0.5f;         // 直接参照出来るようにする
    public static implicit operator T?(UserEditVariable<T> v)
    {
        return v.Value;
    }
}

public record EditingData<T>(
    T? Default = default,
    string? DisplayName = null,
    string? Tooltip = null,
    T? Min = default,
    T? Max = default,
    bool IsHide = false,
    string IsVisibleWhenParameter = "", // 指定したパラメータがtrueになったときだけ表示する
    List<T>? PresetList = null,         // 指定した選択肢 (cuda/gpgu/
    UICType UICType = UICType.Default,
    Func<object?, object?, bool>? IsHideFunc = null,        // 指定した関数がtrueになったときだけ表示する
    Action? ClickAction = null,
    string? Extension = null
);

public static class Prototype
{
    public static UserEditVariable<bool> Bool { get; set; } = new(Value: false, EditingData: new(Default: false, Tooltip: "bool", IsHide: false));
    public static UserEditVariable<int> Int { get; set; } = new(Value: 5, EditingData: new(Default: 5, Tooltip: "int", Min: 0, Max: 10, IsHide: false));
    public static UserEditVariable<float> Float { get; set; } = new(Value: 0.0f, EditingData: new(Default: 0.0f, Tooltip: "", Min: 0.0f, Max: 1.0f, IsHide: false));
    public static UserEditVariable<string> String { get; set; } = new(Value: "", EditingData: new(Default: "", Tooltip: ""));
    public static UserEditVariable<string> ShowLang { get; set; } = new(Value: "");
}