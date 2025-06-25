namespace Hutzper.Project.Mekiki.Models;

/// <summary>
/// アプリの設定情報に関するスキーマ
/// </summary>
public class UserModel
{
    public string Name { get; set; }
    public int Age { get; set; }

    public UserModel(string name, int age)
    {
        this.Name = name;
        this.Age = age;
    }
}
