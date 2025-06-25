using Hutzper.Project.Mekiki.Models.Config;
using Microsoft.EntityFrameworkCore;

namespace Hutzper.Project.Mekiki.Services.AppConfigService;

/// <summary>
/// AppConfig の取得・保存・更新を担当するサービス
/// </summary>
public class AppConfigService
{
    private readonly AppConfigDbContext _dbContext;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public AppConfigService(AppConfigDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// AppConfigを1件取得する。未登録の場合は空のインスタンスを返す。
    /// </summary>
    public AppConfig GetConfig()
    {
        return _dbContext.AppConfigs.FirstOrDefault() ?? new AppConfig();
    }

    /// <summary>
    /// AppConfigを保存または更新する。存在すればUpdatedAtを更新し、存在しなければ新規作成。
    /// </summary>
    public void SaveOrUpdate(AppConfig config)
    {
        try
        {
            var now = DateTime.UtcNow;
            var existing = _dbContext.AppConfigs.FirstOrDefault();

            if (existing != null)
            {
                existing.UpdatedAt = now;
            }
            else
            {
                config.CreatedAt = now;
                config.UpdatedAt = now;
                _dbContext.AppConfigs.Add(config);
            }

            _dbContext.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            Serilog.Log.Error(ex, "AppConfig の保存に失敗しました（DBエラー）");
            throw new InvalidOperationException("DB更新時にエラーが発生しました", ex);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "AppConfig の保存に失敗しました（一般エラー）");
            throw;
        }
    }
}
