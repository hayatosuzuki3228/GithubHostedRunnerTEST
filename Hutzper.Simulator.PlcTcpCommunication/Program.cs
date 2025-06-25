using Hutzper.Library.Common;
using Hutzper.Library.Common.Diagnostics;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Common.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace Hutzper.Simulator.PlcTcpCommunication
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 起動制御
            var selfBootController = (SelfBootController?)null;
            selfBootController = new SelfBootController(Application.ProductName ?? "simulator");
            if (!Control.ModifierKeys.HasFlag(Keys.Shift))
            {
                if (false == selfBootController.CanBootApplication)
                {
                    return;
                }
            }

            // タイマの精度変更
            WinSystemController.TimeBeginPeriod();

            // 例外の補足
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);

            ApplicationConfiguration.Initialize();

            try
            {
                #region サービスを追加(DIの指定)
                {
                    // Form
                    ServiceCollectionSharing.Instance.ServiceCollection.AddSingleton<MainForm>();

                    ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IServiceCollectionSharing, ServiceCollectionSharing>(p => ServiceCollectionSharing.Instance);

                    #region パス
                    {
                        // パス管理
                        ServiceCollectionSharing.Instance.ServiceCollection.AddTransient<IPathManager, PathManager>();
                    }
                    #endregion

                    // サービスの作成
                    ServiceCollectionSharing.Instance.BuildServiceProvider();
                }
                #endregion

                // アプリ設定
                var appConfigFile = ApplicationConfig.GetStandardConfigFileInfo();

                var logPath = System.IO.Path.Combine(appConfigFile.Directory?.Parent?.FullName ?? "..\\", "log_Simulator.Plc");

                // ログの設定
                // ここにSerilogの初期化書く
                UseSerilog.StartLogger();
                // ビルド情報
                var processBits = Environment.Is64BitProcess ? "64bit" : "32bit";
                var processMode = "release build";
#if DEBUG
                processMode = "debug build";
#endif
                //バージョンの取得
                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                var version = asm.GetName().Version;

                // 起動ログ出力
                Serilog.Log.Information($"application start ver.{version?.ToString() ?? Application.ProductVersion} ({processBits} {processMode})");

                // 起動フォーム生成
                var form = ServiceCollectionSharing.Instance.ServiceProvider?.GetRequiredService<MainForm>();

                // 実行
                Application.Run(form!);

                // 再起動要求の場合
                if (null != form && DialogResult.Abort == form.DialogResult)
                {
                    // アプリケーション再起動ログ出力
                    Serilog.Log.Information("application reboot");

                    SelfBootController.RebootApplication();
                }
            }
            finally
            {
                Serilog.Log.Information("application end");

                selfBootController?.NotifyExitApplication();

                // タイマの精度変更
                WinSystemController.TimeEndPeriod();
            }
        }

        /// <summary>
        /// 処理されていない例外をキャッチします。
        /// </summary>
        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debug.WriteLine("UnhandledException: " + e.ExceptionObject.ToString() + ". Terminating:" + e.IsTerminating);
            Exception ex;
            if (null != (ex = (Exception)e.ExceptionObject))
            {
                Serilog.Log.Error($"UnhandledException,{ex.Message}");
            }
            Environment.Exit(0);
        }

        /// <summary>
        /// 処理されていないThreadの例外をキャッチします。
        /// </summary>
        private static void ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Serilog.Log.Error($"ThreadException,{e.Exception.Message}");
            Application.Exit();
        }
    }
}