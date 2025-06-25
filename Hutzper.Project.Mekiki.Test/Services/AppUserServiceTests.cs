using Hutzper.Project.Mekiki.Models.Auth;
using Hutzper.Project.Mekiki.Services.AppConfigService;
using Hutzper.Project.Mekiki.Services.AppUserService;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Testcontainers.PostgreSql;

namespace Hutzper.Project.Mekiki.Test.Services;

/// <summary>
/// AooUserServiceのPostgreSQLに関する動作テスト
/// </summary>
public class AppUserServiceTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _pgContainer;
    private AppConfigDbContext _dbContext = null!;
    private AppUserService _service = null!;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public AppUserServiceTests()
    {
        _pgContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15")
            .WithCleanUp(true)
            .WithDatabase("mekiki_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    /// <summary>
    /// 初期データ生成用関数
    /// </summary>
    private static (AppUser, AppUser, AppUser) InitialData()
    {
        var password = "test";
        var salt = new byte[] { 1, 2, 3, 4 }; //  テスト用固定値。本来はBase64でランダムに生成
        var hash = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA512).GetBytes(32);

        var userDeveloper = new AppUser();
        userDeveloper.Username = "Developer";
        userDeveloper.PasswordHash = Convert.ToBase64String(hash);
        userDeveloper.Salt = Convert.ToBase64String(salt);
        userDeveloper.Role = 2;

        var userAdmin = new AppUser();
        userAdmin.Username = "Admin";
        userAdmin.PasswordHash = Convert.ToBase64String(hash);
        userAdmin.Salt = Convert.ToBase64String(salt);
        userAdmin.Role = 1;

        var userOperator = new AppUser();
        userOperator.Username = "Operator";
        userOperator.PasswordHash = string.Empty;
        userOperator.Salt = string.Empty;
        userOperator.Role = 0;

        return (userDeveloper, userAdmin, userOperator);
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public async Task InitializeAsync()
    {
        await _pgContainer.StartAsync();

        var options = new DbContextOptionsBuilder<AppConfigDbContext>()
            .UseNpgsql(_pgContainer.GetConnectionString())
            .Options;

        _dbContext = new AppConfigDbContext(options);

        // テストケース毎にDB初期化（必須）
        await _dbContext.Database.EnsureDeletedAsync();
        await _dbContext.Database.EnsureCreatedAsync();

        _dbContext = new AppConfigDbContext(options);
        _service = new AppUserService(_dbContext);

        var (userDeveloper, userAdmin, userOperator) = InitialData();

        // 初期データを挿入
        try
        {
            _service.SaveOrUpdate(userDeveloper);
            _service.SaveOrUpdate(userAdmin);
            _service.SaveOrUpdate(userOperator);
        }
        catch
        {
            // この時点でエラーが発生する可能性もあるので例外処理
            Assert.Fail();
        }
    }

    /// <summary>
    /// テスト終了後にコンテナを破棄する関数
    /// </summary>
    public async Task DisposeAsync()
    {
        await _pgContainer.DisposeAsync();
    }

    [Fact(DisplayName = "AppUserの保存に成功すること（正常系）")]
    [Trait("Category", "Docker")]
    public void SaveAppUser_WithValidData_ShouldSucceed()
    {
        var user = new AppUser();

        // テスト用データを作成
        var password = "test";
        var salt = new byte[] { 1, 2, 3, 4 }; //  テスト用固定値。本来はBase64でランダムに生成
        var hash = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA512).GetBytes(32);

        user.Username = "test";
        user.PasswordHash = Convert.ToBase64String(hash);
        user.Salt = Convert.ToBase64String(salt);
        user.Role = 2;

        // 保存処理
        try
        {
            _service.SaveOrUpdate(user);
        }
        catch
        {
            // 保存に失敗したらFail
            Assert.Fail();
        }

        AppUser result = new AppUser();

        // 読込処理
        try
        {
            result = _service.GetUserInformation(user.Username);
        }
        catch
        {
            // データの読込に失敗したらFail
            Assert.Fail();
        }

        Assert.NotNull(result);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.PasswordHash, result.PasswordHash);
        Assert.Equal(user.Salt, result.Salt);
        Assert.Equal(user.Role, result.Role);
    }

    [Fact(DisplayName = "AppUserを2回保存したときに既存データが更新されること（正常系）")]
    [Trait("Category", "Docker")]
    public void SaveAppUser_WithDuplicateUsername_ShouldThrowException()
    {
        var user1 = new AppUser();
        var user2 = new AppUser();

        // ユーザ名を重複させる
        user1.Username = "test";
        user2.Username = user1.Username;

        var password1 = "test";
        var salt1 = new byte[] { 1, 2, 3, 4 };  //  テスト用固定値。本来はBase64でランダムに生成
        var hash1 = new Rfc2898DeriveBytes(password1, salt1, 100000, HashAlgorithmName.SHA512).GetBytes(32);

        var password2 = "test";
        var salt2 = new byte[] { 1, 2, 3, 4 };  //  テスト用固定値。本来はBase64でランダムに生成
        var hash2 = new Rfc2898DeriveBytes(password2, salt2, 100000, HashAlgorithmName.SHA512).GetBytes(32);

        user1.PasswordHash = Convert.ToBase64String(hash1);
        user2.PasswordHash = Convert.ToBase64String(hash2);
        user1.Salt = Convert.ToBase64String(salt1);
        user2.Salt = Convert.ToBase64String(salt2);
        user1.Role = 2;
        user2.Role = 2;

        // 新規作成処理
        try
        {
            _service.SaveOrUpdate(user1);
        }
        catch
        {
            // 例外が発生したらFail
            Assert.Fail();
        }

        // 更新処理で例外が発生した時テストに成功する
        Assert.Throws<DbUpdateException>(() => _service.SaveOrUpdate(user2));
    }

    [Fact(DisplayName = "AppUserをOperatorとしてパスワードなしで保存できること（正常系）")]
    [Trait("Category", "Docker")]
    public void SaveAppUser_AsOperatorWithoutPassword_ShouldSucceed()
    {
        var user = new AppUser();

        user.Username = "test";
        user.PasswordHash = string.Empty;   // パスワード無し
        user.Salt = Convert.ToBase64String(new byte[32]);
        user.Role = 0;  // Operatorロール

        try
        {
            _service.SaveOrUpdate(user);
        }
        catch
        {
            // 保存に失敗したらFail
            Assert.Fail();
        }

        var result = _service.GetUserInformation(user.Username);

        Assert.NotNull(result);
        Assert.Equal(user.Username, result.Username);
        Assert.Equal(user.PasswordHash, result.PasswordHash);
        Assert.Equal(user.Salt, result.Salt);
        Assert.Equal(user.Role, result.Role);
    }

    [Fact(DisplayName = "AppUserの初期データにDeveloper / Admin / Operatorが含まれること（正常系）")]
    [Trait("Category", "Docker")]
    public void SeededAppUsers_ShouldContainDeveloperAdminOperator()
    {
        AppUser resultDeveloper = new AppUser();
        AppUser resultAdmin = new AppUser();
        AppUser resultOperator = new AppUser();

        // 読込処理
        try
        {
            resultDeveloper = _service.GetUserInformation("Developer");
            resultAdmin = _service.GetUserInformation("Admin");
            resultOperator = _service.GetUserInformation("Operator");
        }
        catch
        {
            // データの読込に失敗したらFail
            Assert.Fail();
        }

        // 初期データを呼び出し
        var (userDeveloper, userAdmin, userOperator) = InitialData();

        Assert.Equal(userDeveloper.Username, resultDeveloper.Username);
        Assert.Equal(userDeveloper.PasswordHash, resultDeveloper.PasswordHash);
        Assert.Equal(userDeveloper.Salt, resultDeveloper.Salt);
        Assert.Equal(userDeveloper.Role, resultDeveloper.Role);

        Assert.Equal(userAdmin.Username, resultAdmin.Username);
        Assert.Equal(userAdmin.PasswordHash, resultAdmin.PasswordHash);
        Assert.Equal(userAdmin.Salt, resultAdmin.Salt);
        Assert.Equal(userAdmin.Role, resultAdmin.Role);

        Assert.Equal(userOperator.Username, resultOperator.Username);
        Assert.Equal(userOperator.PasswordHash, resultOperator.PasswordHash);
        Assert.Equal(userOperator.Salt, resultOperator.Salt);
        Assert.Equal(userOperator.Role, resultOperator.Role);
    }

    [Fact(DisplayName = "AppUserを2回保存したときに既存データが更新されること（正常系）")]
    [Trait("Category", "Docker")]
    public void SaveAppUser_Twice_ShouldUpdateExistingUser()
    {
        var username = "testuser";  // ユーザー名は同一（ユニークキー制約）
        var password1 = "test1";
        var password2 = "test2";

        var salt1 = new byte[] { 1, 2, 3, 4 };
        var hash1 = new Rfc2898DeriveBytes(password1, salt1, 100000, HashAlgorithmName.SHA512).GetBytes(32);

        var salt2 = new byte[] { 10, 11, 12, 13 };
        var hash2 = new Rfc2898DeriveBytes(password2, salt1, 100000, HashAlgorithmName.SHA512).GetBytes(32);

        var user = new AppUser
        {
            Username = username,
            PasswordHash = Convert.ToBase64String(hash1),
            Salt = Convert.ToBase64String(salt1),
            Role = 1
        };

        _service.SaveOrUpdate(user); // 新規作成処理

        var createResult = _service.GetUserInformation(username);

        // 更新内容
        var role = 2;
        user.PasswordHash = Convert.ToBase64String(hash2);
        user.Salt = Convert.ToBase64String(salt2);
        user.Role = role;

        _service.SaveOrUpdate(user); // 更新処理

        var updateResult = _service.GetUserInformation(username);

        Assert.NotNull(updateResult);   // データの存在を確認
        Assert.Equal(Convert.ToBase64String(hash2), updateResult.PasswordHash);    // パスワードの更新を確認
        Assert.Equal(Convert.ToBase64String(salt2), updateResult.Salt);   // ロールの更新を確認
        Assert.Equal(createResult.CreatedAt, updateResult.CreatedAt); // 作成日時は更新されていないことを確認
    }
}
