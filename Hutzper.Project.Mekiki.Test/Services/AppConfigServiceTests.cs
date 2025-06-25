using Hutzper.Project.Mekiki.Models.Config;
using Hutzper.Project.Mekiki.Services.AppConfigService;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Hutzper.Project.Mekiki.Test.Services;

/// <summary>
/// AppConfigService の PostgreSQL に関する動作テスト
/// </summary>
public class AppConfigServiceTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _pgContainer;
    private AppConfigDbContext _dbContext = null!;
    private AppConfigService _service = null!;

    public AppConfigServiceTests()
    {
        _pgContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15")
            .WithCleanUp(true)
            .WithDatabase("mekiki_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

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

        _service = new AppConfigService(_dbContext);
    }

    public async Task DisposeAsync()
    {
        await _pgContainer.DisposeAsync();
    }

    [Fact(DisplayName = "AppConfigが未登録時にデフォルトが取得されること（正常系）")]
    [Trait("Category", "Docker")]
    public void GetConfig_WhenNotExists_ShouldReturnDefault()
    {
        var result = _service.GetConfig();
        Assert.NotNull(result);
        Assert.Null(result.CreatedAt);
        Assert.Null(result.UpdatedAt);
    }

    [Fact(DisplayName = "AppConfigを新規保存できること（正常系）")]
    [Trait("Category", "Docker")]
    public void SaveOrUpdate_WhenNoExisting_ShouldInsert()
    {
        var config = new AppConfig();
        _service.SaveOrUpdate(config);

        var stored = _dbContext.AppConfigs.FirstOrDefault();
        Assert.NotNull(stored);
        Assert.NotNull(stored!.CreatedAt);
        Assert.NotNull(stored.UpdatedAt);
    }

    [Fact(DisplayName = "AppConfigが更新されること（正常系）")]
    [Trait("Category", "Docker")]
    public void SaveOrUpdate_WhenExists_ShouldUpdateTimestamp()
    {
        var config = new AppConfig
        {
            CreatedAt = DateTime.UtcNow.AddMinutes(-10),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-10)
        };
        _dbContext.AppConfigs.Add(config);
        _dbContext.SaveChanges();

        _service.SaveOrUpdate(new AppConfig()); // 新たな構成値は空でも可

        var updated = _dbContext.AppConfigs.First();
        Assert.True(updated.UpdatedAt > updated.CreatedAt);
    }
}
