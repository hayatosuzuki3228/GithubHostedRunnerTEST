using Hutzper.Project.Mekiki.Models.Auth;
using Hutzper.Project.Mekiki.Services.AppConfigService;
using Microsoft.EntityFrameworkCore;

namespace Hutzper.Project.Mekiki.Services.AppUserService;

/// <summary>
/// AppUserの取得・保存・更新を担当するサービス
/// </summary>
public class AppUserService
{
    private readonly AppConfigDbContext _dbContext;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public AppUserService(AppConfigDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// AppUserの情報を一件取得する。未登録の場合は空のインスタンスを返す。
    /// </summary>
    public AppUser GetUserInformation(string username)
    {
        return _dbContext.AppUsers.FirstOrDefault(u => u.Username == username) ?? new AppUser();
    }

    /// <summary>
    /// AppUserを保存または更新する。存在しなければ新規作成。
    /// </summary>
    public void SaveOrUpdate(AppUser user)
    {
        try
        {
            var now = DateTime.UtcNow;
            var existing = _dbContext.AppUsers.SingleOrDefault(u => u.Username == user.Username);

            if (existing == null)
            {
                // 新規作成
                user.UpdatedAt = now;
                user.CreatedAt = now;

                _dbContext.AppUsers.Add(user);
            }
            else
            {
                // 更新
                user.UpdatedAt = now;
                _dbContext.AppUsers.Update(user);
            }
            _dbContext.SaveChanges();   // 変更を反映
        }
        catch (DbUpdateException ex)
        {
            Serilog.Log.Error(ex, "AppUser の保存または更新に失敗しました（DBエラー）");
            throw new DbUpdateException("DB更新時にエラーが発生しました", ex);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "AppUser の保存または更新に失敗しました（一般エラー）");
            throw;
        }
    }
}
