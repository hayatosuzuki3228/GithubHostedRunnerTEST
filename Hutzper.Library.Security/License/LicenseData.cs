namespace Hutzper.Library.Security.License;

/// <summary>
/// license.sig の JSON 構造を表すデータクラス
/// </summary>
public class LicenseData
{
    public string? Key { get; set; } = "";
    public string? IV { get; set; } = "";
    public string? Signature { get; set; } = "";
}