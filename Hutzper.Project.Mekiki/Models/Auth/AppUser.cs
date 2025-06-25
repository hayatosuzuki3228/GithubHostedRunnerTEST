namespace Hutzper.Project.Mekiki.Models.Auth;

/// <summary>
/// ユーザデータに関するスキーマ
/// </summary>
public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public int Role { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
