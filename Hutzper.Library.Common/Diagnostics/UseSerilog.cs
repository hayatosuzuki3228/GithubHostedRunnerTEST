using Serilog;
using Serilog.Events;
using Xunit.Abstractions;

namespace Hutzper.Library.Common.Diagnostics
{
    /// <summary>
    /// Serilogを使用したログ出力の構成と管理を行うユーティリティクラスです
    /// </summary>
    /// <remarks>
    /// 以下の機能を提供します:
    /// - ファイルログの出力設定
    /// - Sentryへのエラー通知
    /// - デバッグ/リリースビルドでの異なるログ設定
    /// - テスト出力のサポート
    /// - 非同期ログ出力
    /// </remarks>
    static public class UseSerilog
    {
        /// <summary>
        /// Serilogロガーを初期化し、設定された出力先へのログ記録を開始します
        /// </summary>
        /// <param name="useSentry">Sentryへのログ出力を有効にするかどうか</param>
        /// <param name="useFileLogger">ファイルへのログ出力を有効にするかどうか</param>
        /// <param name="sentryLogLevel">Sentryへ送信するログの最小レベル</param>
        /// <param name="fileLogLevel">ファイルに出力するログの最小レベル</param>
        /// <param name="saveLimit">保持するログファイルの最大数</param>
        /// <param name="output">xUnit等のテストフレームワークで使用するテスト出力ヘルパー</param>
        /// <param name="logType">ログの種別（Mekiki/DigitalIO/Test/Other）</param>
        public static void StartLogger(bool useSentry = false, bool useFileLogger = true, LogEventLevel sentryLogLevel = LogEventLevel.Error, LogEventLevel fileLogLevel = LogEventLevel.Information, int saveLimit = 10, ITestOutputHelper? output = null, LogType logType = LogType.Other)
        {
            Serilog.Log.CloseAndFlush();

            // ログ出力のフォーマット
            string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{ThreadName}] {Level:u4} {SourceContext} {Message:lj} {NewLine}{Exception}";

            LoggerConfiguration builder = new LoggerConfiguration();
            builder = builder.Enrich.WithThreadId();
            builder = builder.Enrich.WithProcessId();
            builder = builder.Enrich.WithProperty("ThreadName", Thread.CurrentThread.Name ?? "NoName");
#if DEBUG
            builder = builder.WriteTo.Debug(outputTemplate: outputTemplate);
#endif

            if (useFileLogger)
            {
                var logDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Mekiki", "log"
                );
                Directory.CreateDirectory(logDir); // フォルダがなければ作成
#if DEBUG
                // デバッグビルド: バッファなし
                builder = builder.WriteTo.File(
                    path: $"{logDir}/{GetLogName(logType)}.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: saveLimit,
                    outputTemplate: outputTemplate,
                    restrictedToMinimumLevel: fileLogLevel
                );
#else
                // リリースビルド: バッファあり
                builder = builder.WriteTo.Async(a =>
            a.File(
                path: $"{logDir}/{GetLogName(logType)}.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: saveLimit,
                outputTemplate: outputTemplate,
                restrictedToMinimumLevel: fileLogLevel
            )
        );
#endif
            }

            if (useSentry)
            {
#if DEBUG
                // デバッグビルド: バッファなし
                builder = builder.WriteTo.Sentry(o =>
                {
                    o.Dsn = "https://a0615e681d502ce77b95da795e80f308@o414950.ingest.us.sentry.io/4506515933167616";
                    o.Environment = "dev";
                    o.MinimumEventLevel = sentryLogLevel;
                    o.IsGlobalModeEnabled = true;
                    o.SendDefaultPii = true;
                });
#else
                // リリースビルド: バッファあり
                builder = builder.WriteTo.Async(a =>
                    a.Sentry(o =>
                    {
                        o.Dsn = "https://a13f825776b56091c0cc8e11a083a291@o414950.ingest.us.sentry.io/4506612200964096";
                        o.Environment = "production";
                        o.MinimumEventLevel = sentryLogLevel;
                        o.IsGlobalModeEnabled = true;
                        o.SendDefaultPii = true;
                    })
                );
#endif
            }

            if (output != null)
            {
                builder = builder.WriteTo.TestOutput(testOutputHelper: output, outputTemplate: outputTemplate);
            }

            Log.Logger = builder.CreateLogger();
            Serilog.Log.Information("UseSerilog.StartLogger, Serilog is initialized and start logging.");
        }

        /// <summary>
        /// ログタイプに応じたログファイル名を取得します
        /// </summary>
        /// <param name="logType">ログタイプ</param>
        /// <returns>ログファイル名の文字列</returns>
        static string GetLogName(LogType type)
        {
            string[] logNames = { "", "IO_", "TEST_", "OTHER_" };

            return logNames[(int)type];
        }
    }

    /// <summary>
    /// 出力ログタイプ
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// Mekikiアプリケーション用(gloggで表示するログ）
        /// </summary>
        Mekiki,
        /// <summary>
        /// 疑似IO用
        /// </summary>
        DigitalIO,
        /// <summary>
        /// テスト用
        /// </summary>
        Test,
        /// <summary>
        /// その他用
        /// </summary>
        Other,
    }
}