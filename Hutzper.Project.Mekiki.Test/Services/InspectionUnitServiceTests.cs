using Hutzper.Project.Mekiki.Models.Config;
using Hutzper.Project.Mekiki.Services.AppConfigService;
using Hutzper.Project.Mekiki.Services.InspectionUnitService;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Hutzper.Project.Mekiki.Test.Services;

public class InspectionUnitServiceTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _pgContainer;
    private AppConfigDbContext _dbContext = null!;
    private AppConfigService _appConfigService = null!;
    private InspectionUnitService _service = null!;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public InspectionUnitServiceTests()
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

        _appConfigService = new AppConfigService(_dbContext);
        _service = new InspectionUnitService(_dbContext);
    }

    /// <summary>
    /// テスト終了後にコンテナを破棄する関数
    /// </summary>
    public async Task DisposeAsync()
    {
        await _pgContainer.DisposeAsync();
    }

    private AppConfig? CreateAndGetAppConfigDb()
    {
        // テスト用のAppCondigの作成
        var testConfig = new AppConfig();
        _appConfigService.SaveOrUpdate(testConfig);
        var storedConfig = _dbContext.AppConfigs.FirstOrDefault();
        return storedConfig;
    }

    [Fact(DisplayName = "InspectionUnitの保存に成功すること（正常系）")]
    public void SaveInspectionUnit_WithValidData_ShouldSucceed()
    {
        var storedConfig = this.CreateAndGetAppConfigDb();
        Assert.NotNull(storedConfig);

        var testUnit = new InspectionUnit
        {
            AppConfigId = storedConfig.Id,  // 作成したConfigの値を外部キーとして登録
            Name = "test",
            Description = "test",
            DisplayOrder = 1,
        };

        // 新規作成
        _service.SaveInspectionUnit(testUnit);
        var storedUnit = _service.GetInspectionUnit(testUnit.Name);

        // テストの実施
        Assert.NotNull(storedUnit);
        Assert.Equal(testUnit.Name, storedUnit.Name);
        Assert.Equal(testUnit.Description, storedUnit.Description);
        Assert.Equal(testUnit.DisplayOrder, storedUnit.DisplayOrder);
    }

    [Fact(DisplayName = "nameを省略してInspectionUnitの保存に失敗すること（異常系）")]
    public void SaveInspectionUnit_WithoutName_ShouldThrowException()
    {
        var storedConfig = this.CreateAndGetAppConfigDb();
        Assert.NotNull(storedConfig);

        var testUnit = new InspectionUnit
        {
            AppConfigId = storedConfig.Id,
            Description = "test",
            DisplayOrder = 1,
        };

        // NameがNULLであることを確認
        Assert.Null(testUnit.Name);

        // 新規作成においてエラーが発生すれば成功
        Assert.Throws<DbUpdateException>(() => _service.SaveInspectionUnit(testUnit));
    }

    [Fact(DisplayName = "app_config_idに対して無効なIDを指定するとInspectionUnitの保存に失敗すること（異常系）")]
    public void SaveInspectionUnit_WithhoutAppConfigId_ShouldThrowException()
    {
        var testId = -1;    // IDとして取りうることがない値を設定

        var testUnit = new InspectionUnit
        {
            AppConfigId = testId,
            Name = "test",
            Description = "test",
            DisplayOrder = 1,
        };

        Assert.True(testId < 0);

        // 新規作成においてエラーが発生すれば成功
        Assert.Throws<DbUpdateException>(() => _service.SaveInspectionUnit(testUnit));
    }


    [Fact(DisplayName = "AppConfigからInspectionUnitをリレーション付きで取得できること（正常系）")]
    public void LoadInspectionUnits_FromAppConfig_ShouldSucceed()
    {
        var storedConfig = this.CreateAndGetAppConfigDb();
        Assert.NotNull(storedConfig);

        var testUnit = new InspectionUnit
        {
            AppConfigId = storedConfig.Id,
            Name = "test",
            Description = "test",
            DisplayOrder = 1,
        };

        // 新規作成
        _service.SaveInspectionUnit(testUnit);
        var storedUnit = _service.GetInspectionUnit(testUnit.Name);

        // includeを使用してAppconfigからInspectionUnitを取得
        var includeConfigResult = _dbContext.AppConfigs.Include(n => n.inspectionUnits).ToList();

        // 外部キーとしてAppConfig内にInspectionUnitのIDが存在することをテスト
        Assert.Equal(includeConfigResult[0].inspectionUnits[0].Id, storedUnit.Id);
        Assert.Equal(includeConfigResult[0].inspectionUnits[0].Name, storedUnit.Name);
        Assert.Equal(includeConfigResult[0].inspectionUnits[0].Description, storedUnit.Description);
        Assert.Equal(includeConfigResult[0].inspectionUnits[0].DisplayOrder, storedUnit.DisplayOrder);
    }

    [Fact(DisplayName = "InspectionUnitのnameを更新できること（正常系）")]
    public void UpdateInspectionUnit_Name_ShouldReflectUpdatedValue()
    {
        var storedConfig = this.CreateAndGetAppConfigDb();
        Assert.NotNull(storedConfig);

        var prevName = "test1";
        var testUnit = new InspectionUnit
        {
            AppConfigId = storedConfig.Id,
            Name = prevName,
            Description = "test",
            DisplayOrder = 1,
        };

        // 新規作成
        _service.SaveInspectionUnit(testUnit);
        var storedUnitBeforeUpdate = _service.GetInspectionUnit(prevName);
        var storedUnitBeforeUpdateId = storedUnitBeforeUpdate.Id;   // 更新前のIDを保存
        var storedUnitBeforeUpdateName = storedUnitBeforeUpdate.Name;   // 更新前の名前を保存

        var updateName = "test2";

        // 更新
        storedUnitBeforeUpdate.Name = updateName;   // 名前を更新
        _service.UpdateInspectionUnitById(storedUnitBeforeUpdate.Id, storedUnitBeforeUpdate);  // DBに反映

        var storedUnitAfterUpdate = _service.GetInspectionUnit(updateName);
        var storedUnitAfterUpdateId = storedUnitAfterUpdate.Id; // 更新後のIDを保存
        var storedUnitAfterUpdateName = storedUnitAfterUpdate.Name; // 更新後の名前を保存

        Assert.NotEqual(prevName, updateName);
        Assert.NotEqual(storedUnitBeforeUpdateName, storedUnitAfterUpdateName);
        Assert.Equal(storedUnitBeforeUpdateId, storedUnitAfterUpdateId);
        Assert.Equal(prevName, storedUnitBeforeUpdateName);
        Assert.Equal(updateName, storedUnitAfterUpdateName);
    }

    [Fact(DisplayName = "InspectionUnit削除時にAppConfigが残ること（正常系）")]
    public void DeleteInspectionUnit_ShouldNotAffectAppConfig()
    {
        var storedConfig = this.CreateAndGetAppConfigDb();
        Assert.NotNull(storedConfig);

        var testUnit = new InspectionUnit
        {
            AppConfigId = storedConfig.Id,
            Name = "test",
            Description = "test",
            DisplayOrder = 1,
        };

        // 新規作成
        _service.SaveInspectionUnit(testUnit);

        // 削除
        _service.DeleteInspectionunitById(testUnit.Id);

        // 親テーブルに影響が無いこと確認
        var storedConfigCheckForeignKey = _dbContext.AppConfigs.FirstOrDefault();
        Assert.NotNull(storedConfigCheckForeignKey);
        var foreignKey = storedConfigCheckForeignKey.inspectionUnits.FindAll(n => n.Id == testUnit.Id);
        Assert.NotNull(foreignKey);
    }

    [Fact(DisplayName = "複数InspectionUnitがdisplay_order順に取得できること（正常系）")]
    public void SaveInspectionUnit_WithMultipleUnits_ShouldRespectOrder()
    {
        var storedConfig = this.CreateAndGetAppConfigDb();
        Assert.NotNull(storedConfig);

        var displayOrder1 = 1;
        var displayOrder2 = 2;

        var testUnit1 = new InspectionUnit
        {
            AppConfigId = storedConfig.Id,
            Name = "A",
            DisplayOrder = displayOrder1
        };

        var testUnit2 = new InspectionUnit
        {
            AppConfigId = storedConfig.Id,
            Name = "B",
            DisplayOrder = displayOrder2
        };

        // テストデータを新規作成
        _service.SaveInspectionUnit(testUnit1);
        _service.SaveInspectionUnit(testUnit2);

        var storedUnits = _service.GetInspectionUnitAll();

        Assert.NotNull(storedUnits);

        // 順序を保ったまま保存できていることを確認
        Assert.True(displayOrder2 > displayOrder1);
        Assert.Equal(testUnit1.Name, storedUnits[0].Name);
        Assert.Equal(testUnit2.Name, storedUnits[1].Name);
    }

    [Fact(DisplayName = "DisplayOrderの重複で保存に失敗すること（異常系）")]
    public void SaveInspectionUnit_WithDuplicationDisplayOrder_ShouldThrow()
    {
        var config = this.CreateAndGetAppConfigDb();
        Assert.NotNull(config);

        var testUnit1 = new InspectionUnit
        {
            AppConfigId = config.Id,
            Name = "orderDuplication1",
            DisplayOrder = 0    // DisplayOrderを重複させる
        };

        var testUnit2 = new InspectionUnit
        {
            AppConfigId = config.Id,
            Name = "orderDuplication2",
            DisplayOrder = 0    // DisplayOrderを重複させる
        };

        Assert.Equal(testUnit1.DisplayOrder, testUnit2.DisplayOrder);     // DisplayOrderが重複していることを確認

        _service.SaveInspectionUnit(testUnit1);
        Assert.Throws<DbUpdateException>(() => _service.SaveInspectionUnit(testUnit2));
    }


    [Fact(DisplayName = "DisplayOrderに負の値を指定すると保存に失敗すること（異常系）")]
    public void SaveInspectionUnit_WithNegativeDisplayOrder_ShouldThrow()
    {
        var config = this.CreateAndGetAppConfigDb();
        Assert.NotNull(config);
        var testUnit = new InspectionUnit
        {
            AppConfigId = config.Id,
            Name = "orderNegative",
            DisplayOrder = -1
        };

        Assert.True(testUnit.DisplayOrder < 0);     // DisplayOrderが負であることを確認

        Assert.Throws<DbUpdateException>(() => _service.SaveInspectionUnit(testUnit));
    }

    [Fact(DisplayName = "DisplayOrderを指定しないと保存に失敗すること（異常系）")]
    public void SaveInspectionUnit_WithoutDisplayOrder_ShouldThrow()
    {
        var config = this.CreateAndGetAppConfigDb();
        Assert.NotNull(config);
        var testUnit = new InspectionUnit
        {
            AppConfigId = config.Id,
            Name = "noOrder"
            // DisplayOrder を明示的に指定しない（null）
        };

        Assert.Null(testUnit.DisplayOrder);     // DisplayOrderがNULLであることを確認

        Assert.Throws<DbUpdateException>(() => _service.SaveInspectionUnit(testUnit));
    }


    [Fact(DisplayName = "nullのNameで取得しようとするとArgumentNullExceptionが発生すること（異常系）")]
    public void GetInspectionUnit_WithNullName_ShouldThrowArgumentNull()
    {
        string? name = null;
        Assert.Null(name);      // nameがNULLであることを確認
        Assert.Throws<ArgumentNullException>(() => _service.GetInspectionUnit(name));
    }

    [Fact(DisplayName = "空文字のNameで取得しようとするとArgumentNullExceptionが発生すること（異常系）")]
    public void GetInspectionUnit_WithEmptyName_ShouldThrowArgumentNull()
    {
        var name = string.Empty;
        Assert.Equal(string.Empty, name);   // nameが空文字であることを確認
        Assert.Throws<ArgumentNullException>(() => _service.GetInspectionUnit(name));
    }

    [Fact(DisplayName = "存在しないNameで取得しようとするとInvalidOperationExceptionが発生すること（異常系）")]
    public void GetInspectionUnit_WithUnknownName_ShouldThrowInvalidOperation()
    {
        var config = this.CreateAndGetAppConfigDb();
        Assert.NotNull(config);
        var testUnit = new InspectionUnit
        {
            AppConfigId = config.Id,
            Name = "A",
            DisplayOrder = 1
        };
        _service.SaveInspectionUnit(testUnit);

        var keyName = "B";      // 存在しないnameを指定
        Assert.NotEqual(testUnit.Name, keyName);    // 指定したnameとキーとなるnameが等しくないことを確認

        Assert.Throws<InvalidOperationException>(() => _service.GetInspectionUnit(keyName));
    }

    [Fact(DisplayName = "InspectionUnitをIdで取得できること（正常系）")]
    public void GetInspectionUnitById_WithValidId_ShouldSucceed()
    {
        var config = this.CreateAndGetAppConfigDb();
        Assert.NotNull(config);

        var testUnit = new InspectionUnit
        {
            AppConfigId = config.Id,
            Name = "test",
            Description = "test",
            DisplayOrder = 1,
        };

        _service.SaveInspectionUnit(testUnit);

        var keyId = testUnit.Id;
        var getUnit = _service.GetInspectionUnitById(keyId);
        Assert.NotNull(getUnit);
        Assert.Equal(testUnit, getUnit);
    }

    [Fact(DisplayName = "存在しないIdでInspectionUnit取得に失敗すること（異常系）")]
    public void GetInspectionUnitById_WithInvalidId_ShouldThrow()
    {
        var keyId = -1; // 存在しないIDを指定
        Assert.Throws<InvalidOperationException>(() => (_service.GetInspectionUnitById(keyId)));    // 存在しないIDで取得二失敗することを確認
    }

    [Fact(DisplayName = "Idで指定されたInspectionUnitを更新できること（正常系）")]
    public void UpdateInspectionUnit_ById_ShouldUpdateCorrectly()
    {
        var storedConfig = this.CreateAndGetAppConfigDb();
        Assert.NotNull(storedConfig);

        var prevName = "test1";
        var testUnit = new InspectionUnit
        {
            AppConfigId = storedConfig.Id,
            Name = prevName,
            Description = "test",
            DisplayOrder = 1,
        };

        // 新規作成
        _service.SaveInspectionUnit(testUnit);
        var storedUnitBeforeUpdate = _service.GetInspectionUnit(prevName);
        var storedUnitBeforeUpdateId = storedUnitBeforeUpdate.Id;   // 更新前のIDを保存
        var storedUnitBeforeUpdateName = storedUnitBeforeUpdate.Name;   // 更新前の名前を保存

        var updateName = "test2";

        // 更新
        storedUnitBeforeUpdate.Name = updateName;   // 名前を更新
        var keyId = storedUnitBeforeUpdate.Id;  // 新規作成したレコードのIDをキーとして保存
        _service.UpdateInspectionUnitById(keyId, storedUnitBeforeUpdate);   //DBをIDで更新

        var storedUnitAfterUpdate = _service.GetInspectionUnit(updateName);
        var storedUnitAfterUpdateId = storedUnitAfterUpdate.Id; // 更新後のIDを保存
        var storedUnitAfterUpdateName = storedUnitAfterUpdate.Name; // 更新後の名前を保存

        Assert.NotEqual(prevName, updateName);
        Assert.NotEqual(storedUnitBeforeUpdateName, storedUnitAfterUpdateName);
        Assert.Equal(storedUnitBeforeUpdateId, storedUnitAfterUpdateId);
        Assert.Equal(prevName, storedUnitBeforeUpdateName);
        Assert.Equal(updateName, storedUnitAfterUpdateName);
    }

    [Fact(DisplayName = "IdでInspectionUnitを削除できること（正常系）")]
    public void DeleteInspectionUnit_ById_ShouldRemove()
    {
        var storedConfig = this.CreateAndGetAppConfigDb();
        Assert.NotNull(storedConfig);

        var testUnit = new InspectionUnit
        {
            AppConfigId = storedConfig.Id,
            Name = "test",
            Description = "test",
            DisplayOrder = 1,
        };

        // 新規作成
        _service.SaveInspectionUnit(testUnit);

        // 削除
        var keyId = testUnit.Id;    // 新規作成したIDをキーとして設定
        _service.DeleteInspectionunitById(keyId);   // IDをキーとして削除

        // 削除したレコードを読み込むとエラーが発生することを確認
        Assert.Throws<InvalidOperationException>(() => _service.GetInspectionUnit(testUnit.Name));
    }

    [Fact(DisplayName = "存在しないIdでInspectionUnit削除に失敗すること（異常系）")]
    public void DeleteInspectionUnitById_WithInvalidId_ShouldThrow()
    {
        var keyId = -1;    // 存在しないIDをキーとして設定

        Assert.Throws<InvalidOperationException>(() => _service.GetInspectionUnitById(keyId));    // キーとして指定したIDが存在しないことを確認
        Assert.Throws<InvalidOperationException>(() => _service.DeleteInspectionunitById(keyId));   // 削除に失敗することを確認
    }

    [Fact(DisplayName = "InspectionUnitをNameで取得できること（正常系）")]
    public void GetInspectionUnit_WithValidName_ShouldGetUnit()
    {

        var config = this.CreateAndGetAppConfigDb();
        Assert.NotNull(config);

        var testUnit = new InspectionUnit
        {
            AppConfigId = config.Id,
            Name = "test",
            Description = "test",
            DisplayOrder = 1,
        };

        _service.SaveInspectionUnit(testUnit);  // testUnitを保存

        var getUnit = _service.GetInspectionUnit(testUnit.Name);    // InspectionUnitをNameで取得
        Assert.NotNull(getUnit);
        Assert.Equal(testUnit, getUnit);
    }
}
