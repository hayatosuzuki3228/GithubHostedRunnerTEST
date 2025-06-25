using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Hutzper.Project.Mekiki.Test.Application;

/// <summary>
/// 未処理例外のログ出力とダンプファイル作成を検証
/// </summary>
[CollectionDefinition("DumpFileTests", DisableParallelization = true)]
public class UnhandledExceptionLoggingCollection { }

[Collection("DumpFileTests")]
public class UnhandledExceptionLoggingTests : IDisposable
{
    /// <summary>
    /// テスト用の Serilog ログイベント受け取りシンク
    /// </summary>
    public class TestLogSink : ILogEventSink
    {
        public readonly System.Collections.Concurrent.ConcurrentQueue<LogEvent> LogEvents = new();
        public void Emit(LogEvent logEvent) => LogEvents.Enqueue(logEvent);
    }

    //フィールド
    private readonly TestLogSink _testSink;
    private readonly Logger _testLogger;
    private readonly string _dumpDirectory;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public UnhandledExceptionLoggingTests()
    {
        _testSink = new TestLogSink();
        _testLogger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(_testSink)
            .CreateLogger();
        Log.Logger = _testLogger;

        _dumpDirectory = Path.GetFullPath(
                         Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Mekiki", "dump"));
        if (!Directory.Exists(_dumpDirectory))
        {
            Directory.CreateDirectory(_dumpDirectory);
        }
    }
    /// <summary>
    /// dumpの履歴を削除
    /// </summary>
    public void Dispose()
    {
        if (Directory.Exists(_dumpDirectory))
        {
            foreach (var file in Directory.GetFiles(_dumpDirectory, "*.dmp"))
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Dump file deletion failed.");
                }
            }
        }
        Log.CloseAndFlush();
    }

    /// <summary>
    /// DispatcherUnhandledException の正常系：Fatalログとダンプが出力されること
    /// </summary>
    [Fact(DisplayName = "DispatcherUnhandledException の正常系：Fatalログとダンプが出力されること")]
    public void HandleException_WithDispatcherUnhandledException_ShouldLogFatalAndCreateDump()
    {
        // Arrange
        const string type = "DispatcherUnhandledException";
        const string message = "Dispatcher test exception";
        var beforeCount = Directory.GetFiles(_dumpDirectory, "*.dmp").Length;

        // Act
        App.HandleException(type, new Exception(message));

        // Assert: Fatal log
        var fatal = _testSink.LogEvents.FirstOrDefault(e => e.Level == LogEventLevel.Fatal);
        Assert.NotNull(fatal);
        var rendered = fatal!.RenderMessage();
        Assert.Contains(type, rendered);
        Assert.Contains(message, rendered);

        // Assert: dump created
        var afterCount = Directory.GetFiles(_dumpDirectory, "*.dmp").Length;
        Assert.True(afterCount > beforeCount, "Dump file was not created for DispatcherUnhandledException.");
    }

    /// <summary>
    /// UnhandledException の正常系：Fatalログとダンプが出力されること
    /// </summary>
    [Fact(DisplayName = "UnhandledException の正常系：Fatalログとダンプが出力されること")]
    public void HandleException_WithAppDomainUnhandledException_ShouldLogFatalAndCreateDump()
    {
        // Arrange
        const string type = "UnhandledException";
        const string message = "AppDomain test exception";
        var beforeCount = Directory.GetFiles(_dumpDirectory, "*.dmp").Length;

        // Act
        App.HandleException(type, new Exception(message));

        // Assert: Fatal log
        var fatal = _testSink.LogEvents.FirstOrDefault(e => e.Level == LogEventLevel.Fatal);
        Assert.NotNull(fatal);
        var rendered = fatal!.RenderMessage();
        Assert.Contains(type, rendered);
        Assert.Contains(message, rendered);

        // Assert: dump created
        var afterCount = Directory.GetFiles(_dumpDirectory, "*.dmp").Length;
        Assert.True(afterCount > beforeCount, "Dump file was not created for UnhandledException.");
    }

    /// <summary>
    /// TaskException の正常系：Fatalログとダンプが出力されること
    /// </summary>
    [Fact(DisplayName = "TaskException の正常系：Fatalログとダンプが出力されること")]
    public void HandleException_WithTaskException_ShouldLogFatalAndCreateDump()
    {
        // Arrange
        const string type = "TaskException";
        const string message = "Task test exception";
        var beforeCount = Directory.GetFiles(_dumpDirectory, "*.dmp").Length;

        // Act
        App.HandleException(type, new Exception(message));

        // Assert: Fatal log
        var fatal = _testSink.LogEvents.FirstOrDefault(e => e.Level == LogEventLevel.Fatal);
        Assert.NotNull(fatal);
        var rendered = fatal!.RenderMessage();
        Assert.Contains(type, rendered);
        Assert.Contains(message, rendered);

        // Assert: dump created
        var afterCount = Directory.GetFiles(_dumpDirectory, "*.dmp").Length;
        Assert.True(afterCount > beforeCount, "Dump file was not created for TaskException.");
    }
}
