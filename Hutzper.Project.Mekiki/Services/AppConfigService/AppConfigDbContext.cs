using Hutzper.Project.Mekiki.Models.Auth;
using Hutzper.Project.Mekiki.Models.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Hutzper.Project.Mekiki.Services.AppConfigService;

/// <summary>
/// PostgreSQLとの接続管理にまつわるサービス
/// </summary>
public class AppConfigDbContext : DbContext
{
    public AppConfigDbContext(DbContextOptions<AppConfigDbContext> options)
        : base(options) { }

    public DbSet<AppConfig> AppConfigs { get; set; } = null!;
    public DbSet<AppUser> AppUsers { get; set; } = null!;

    /// <summary>
    /// マイグレーション前に呼び出される初期化処理
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // 通常は外部から注入されるためここでは何もしない
    }

    /// <summary>
    /// マイグレーションにおける初期化処理
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // AppConfigで定義されるテーブルを作成する処理
        modelBuilder.Entity<AppConfig>(entity =>
        {
            // Configテーブルの作成
            entity.ToTable("Config");
            entity.HasKey(e => e.Id); // 主キーのみ指定

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");
        });

        // AppUserで定義されるテーブルを作成する処理
        modelBuilder.Entity<AppUser>(entity =>
        {
            // AppUserテーブルの作成
            entity.ToTable("User", t =>
            {
                // ROLEプロパティが0, 1, 2のみを持つことを制約に含む
                t.HasCheckConstraint("CK_User_Role_Enum", "\"role\" IN (0, 1, 2)");
            });
            entity.HasKey(e => e.Id); //主キーのみ指定

            entity.Property(e => e.Username)
                .HasColumnName("username");
            entity.Property(e => e.PasswordHash)
                .HasColumnName("password")
                .IsRequired();
            entity.Property(e => e.Salt)
                .HasColumnName("salt")
                .IsRequired();
            entity.Property(e => e.Role)
                .HasColumnName("role")
                .IsRequired();
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

            entity.HasIndex(e => e.Username).IsUnique();    // ユーザ名に重複は許さない
        });
    }
}

/// <summary>
/// マイグレーションの実行前に実行されるクラス
/// </summary>
public class AppConfigTimeDbContextFactory : IDesignTimeDbContextFactory<AppConfigDbContext>
{
    public AppConfigDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration;
        try
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
        }
        catch (Exception)
        {
            Serilog.Log.Information("SQLのユーザ情報設定ファイルが見つかりません．Hutzper.Project.Mekiki/Debug内にappsettings.jsonを格納してください");
            Environment.Exit(1);
            return null!;
        }

        Serilog.Log.Information("SQLの設定ファイルを読み込みました");
        var connectionString = configuration.GetConnectionString("AppConfigDb");

        var optionsBuilder = new DbContextOptionsBuilder<AppConfigDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppConfigDbContext(optionsBuilder.Options);
    }
}
