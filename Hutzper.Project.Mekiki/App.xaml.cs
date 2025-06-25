using CommandLine;
using Hutzper.Library.Common;
using Hutzper.Library.Common.Diagnostics;
using Hutzper.Project.Mekiki.Helpers;
using Hutzper.Project.Mekiki.Services.Application;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Windows;
using Windows.ApplicationModel;

namespace Hutzper.Project.Mekiki;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static Options? ParsedOptions { get; set; }

    /// <summary>
    /// アプリケーションの起動時に実行される処理を定義します
    /// </summary>
    /// <param name="e"></param>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        this.InitializeThread();
        this.ParseCommandLineArgs(e.Args);
        this.SetupExceptionHandlers();
        this.TuneGC();
        this.SetupServices();

        this.ShutdownMode = ShutdownMode.OnMainWindowClose;

        this.MainWindow = ServiceCollectionSharing.Instance.ServiceProvider?.GetRequiredService<MainWindow>();
        if (this.MainWindow != null)
        {
            this.MainWindow.Show();
        }
        else
        {
            Serilog.Log.Error("Failed to get main window.");
            this.Shutdown(); // 失敗時には明示的に終了
        }
    }

    /// <summary>
    /// アプリケーションの終了時に実行される処理を定義
    /// </summary>
    /// <param name="e"></param>
    protected override void OnExit(ExitEventArgs e)
    {
        Serilog.Log.Information("application end");
        LaunchPrepare.CleanupApplication();
        base.OnExit(e);
    }

    /// <summary>
    /// アプリケーションのスレッドの初期化
    /// </summary>
    private void InitializeThread()
    {
        if (Thread.CurrentThread.Name == null)
        {
            Thread.CurrentThread.Name = "Main";
        }
        GCSettings.LatencyMode = GCLatencyMode.Interactive;
    }

    /// <summary>
    /// アプリケーションのオプションを設定
    /// </summary>
    /// <param name="args"></param>
    private void ParseCommandLineArgs(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args).WithParsed(opts =>
        {
            Options.RunOptionsAndReturnExitCode(opts);
            ParsedOptions = opts;
        })
        .WithNotParsed(errs =>
        {
            Options.HandleParseError(errs);
        });

    }

    /// <summary>
    /// アプリケーションの例外ハンドラを設定
    /// </summary>
    private void SetupExceptionHandlers()
    {
        this.DispatcherUnhandledException += (_, ex) =>
        {
            HandleException("DispatcherUnhandledException", ex.Exception);
            Environment.Exit(0);
        };
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception ex)
                HandleException("UnhandledException", ex);
            Environment.Exit(0);
        };
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            HandleException("TaskException", e.Exception);
            e.SetObserved();
        };
    }

    /// <summary>
    /// ガベージコレクション(GC)の設定を調整
    /// </summary>
    private void TuneGC()
    {
        try
        {
            ThreadPool.GetMinThreads(out int worker, out int port);
            ThreadPool.SetMinThreads(worker * 2, port * 2);
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Serilog.Log.Information($"Set Work Threads Min to {worker * 2}");
        }
        catch (Exception)
        {
        }
    }

    /// <summary>
    /// アプリケーションのサービスを初期化
    /// </summary>
    private void SetupServices()
    {
        // Serilogの初期化
        UseSerilog.StartLogger(useSentry: false, logType: LogType.Mekiki);
        Version asmVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0, 0);


        try
        {
            var ver = Package.Current.Id.Version;
            string packageVersion = $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
            Serilog.Log.Information($"App.SetupServices, MSIX package version: {packageVersion}");
        }
        catch (Exception)
        {
            // 非MSIX環境（通常のEXE実行）だと例外になる
            Serilog.Log.Information("MSIX package version: unavailable (not running as MSIX)");
            Serilog.Log.Information($"Assembly version: {asmVersion}");
        }

        // DBの初期化処理
        // DIでDBの情報を挿入する
        //try
        //{
        //    config = new ConfigurationBuilder()
        //        .SetBasePath(AppContext.BaseDirectory)
        //        .AddJsonFile("appsettings.json", optional: false)
        //        .Build();
        //}
        //catch (Exception ex)
        //{
        //    Serilog.Log.Error(ex, "SQLのユーザ情報設定ファイルが見つかりません．Hutzper.Project.Mekiki/bin/x64/Debug内にappsettings.jsonを格納してください");
        //    this.Shutdown();
        //    return;
        //}
        //Serilog.Log.Information("SQLの設定ファイルを読み込みました");

        //var services = ServiceCollectionSharing.Instance.ServiceCollection;

        //services.AddSingleton<IConfiguration>(config);

        //// DBContextを登録
        //services.AddDbContext<AppConfigDbContext>(options =>
        //    options.UseNpgsql(config.GetConnectionString("AppConfigDb"))
        //);

        //// DBConfigを登録
        //services.AddSingleton<AppConfigService>();

        LaunchPrepare.ServiceCollectionForm(ServiceCollectionSharing.Instance.ServiceCollection);
        LaunchPrepare.ServiceCollection(ServiceCollectionSharing.Instance.ServiceCollection);

        DumpException.DeleteOldDMP();

        GCNotifier.Collected += () =>
        {
            Serilog.Log.Information("GCが発生しました。");
        };
        GCNotifier.Start();
    }

    internal static void HandleException(string exceptionType, Exception ex)
    {
        Serilog.Log.Fatal(ex, $"{exceptionType},{ex.Message}");
        DumpException.Write(ex);
        //SentrySdk.Flush();
        Serilog.Log.CloseAndFlush();
    }

    public class Options
    {
        [Option('i', "index", Required = false, HelpText = "Set app index.")]
        public int Index { get; set; } = 0;

        [Option('l', "lang", Required = false, HelpText = "Set app lang.")]
        public string? Lang { get; set; } = "ja-JP";

        public static void RunOptionsAndReturnExitCode(Options opts)
        {
            //LoadLang.CreatNewConfig();
            LaunchPrepare.StartupApplication(opts.Index.ToString());
            //LoadLang.LoadConfig(opts.Lang);
            ParsedOptions = opts;
        }

        public static void HandleParseError(IEnumerable<Error> errs) { }
    }
}

