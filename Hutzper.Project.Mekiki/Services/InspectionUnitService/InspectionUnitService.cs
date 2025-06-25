using Hutzper.Project.Mekiki.Models.Config;
using Hutzper.Project.Mekiki.Services.AppConfigService;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Hutzper.Project.Mekiki.Services.InspectionUnitService;

public class InspectionUnitService
{
    private readonly AppConfigDbContext _dbContext;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public InspectionUnitService(AppConfigDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Nameをキーとしてレコードを取得する
    /// </summary>
    public InspectionUnit GetInspectionUnit(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Serilog.Log.Error("指定された設定名がNULLか空文字です");
            throw new ArgumentNullException();
        }

        var unit = _dbContext.InspectionUnits.FirstOrDefault(u => u.Name == name);
        if (unit != null)
        {
            return unit;
        }
        else
        {
            Serilog.Log.Error($"存在しないレコードである「{name}」が参照されました（DBエラー）");
            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// IDをキーとしてレコードを取得する
    /// </summary>
    public InspectionUnit GetInspectionUnitById(int id)
    {
        var unit = _dbContext.InspectionUnits.FirstOrDefault(u => u.Id == id);
        if (unit != null)
        {
            return unit;
        }
        else
        {
            Serilog.Log.Error($"存在しないID「{id}」が参照されました（DBエラー）");
            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// すべてのレコードをDisplayOrderの順に取得する
    /// </summary>
    public List<InspectionUnit> GetInspectionUnitAll()
    {
        var units = _dbContext.InspectionUnits.OrderBy(u => u.DisplayOrder).ToList();
        if (units.Count > 0)
        {
            return units;
        }
        else
        {
            Serilog.Log.Error("InspectionUnitにデータが存在しません（DBエラー）");
            throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// InspectionUnitを新規作成する
    /// </summary>
    public void SaveInspectionUnit(InspectionUnit unit)
    {
        try
        {
            var now = DateTime.UtcNow;

            // 新規作成
            unit.UpdatedAt = now;
            unit.CreatedAt = now;

            string.IsNullOrWhiteSpace(unit.Name);

            _dbContext.InspectionUnits.Add(unit);
            _dbContext.SaveChanges();   // 変更を反映
        }
        catch (DbException ex)
        {
            Serilog.Log.Error(ex, "InspectionUnitの保存に失敗しました（DBエラー）");
            throw new DbUpdateException("DB更新時にエラーが発生しました", ex);
        }
        catch (ArgumentException ex)
        {
            Serilog.Log.Error(ex, "InspectionUnitの保存に失敗しました（DBエラー）");
            throw new DbUpdateException("DB更新時にエラーが発生しました", ex);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "InspectionUnitの保存に失敗しました（一般エラー）");
            throw;
        }
    }

    public void UpdateInspectionUnitById(int id, InspectionUnit unit)
    {
        var target = this.GetInspectionUnitById(id);    // IDで更新対象を取得
        if (target != null)
        {
            try
            {
                var now = DateTime.UtcNow;

                // 各々の要素を更新
                target.UpdatedAt = now;
                target.Name = unit.Name;
                target.Description = unit.Description;
                target.AppConfig = unit.AppConfig;
                target.DisplayOrder = unit.DisplayOrder;

                _dbContext.InspectionUnits.Update(target);
                _dbContext.SaveChanges(); // 変更を反映
            }
            catch (DbException ex)
            {
                Serilog.Log.Error(ex, "InspectionUnitの更新に失敗しました（DBエラー）");
                throw new DbUpdateException("DB更新時にエラーが発生しました", ex);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "InspectionUnitの更新に失敗しました（一般エラー）");
                throw;
            }
        }
        else
        {
            Serilog.Log.Error($"無効なID「{id}」を持つInspectionUnitが指定されました(DBエラー)");
            throw new InvalidOperationException();
        }
    }

    public void DeleteInspectionunitById(int id)
    {
        try
        {
            var target = this.GetInspectionUnitById(id);

            if (target != null)
            {
                _dbContext.Remove(target);
                _dbContext.SaveChanges(); // 変更を反映
            }
            else
            {
                Serilog.Log.Error($"指定されたID「{id}」のInspectionUnitが見つかりません(DBエラー)");
                throw new InvalidOperationException();
            }
        }
        catch (DbException ex)
        {
            Serilog.Log.Error(ex, "InspectionUnitの削除に失敗しました（DBエラー）");
            throw new DbUpdateException("DB更新時にエラーが発生しました", ex);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "InspectionUnitの削除に失敗しました（一般エラー）");
            throw;
        }
    }
}
