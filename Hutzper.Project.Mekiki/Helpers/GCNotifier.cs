namespace Hutzper.Project.Mekiki.Helpers;

/// <summary>
/// ガベージコレクション(GC)の発生を監視するユーティリティクラス
/// </summary>
/// <remarks>
/// GCの発生タイミングを検知し、イベントを通じて通知を行います。
/// <example>
/// 使用例:
/// <code>
/// // GC監視の開始
/// GCNotifier.Start();
/// 
/// // GC発生時の処理を登録
/// GCNotifier.Collected += () => 
/// {
///     Console.WriteLine("GCが発生しました");
/// };
/// </code>
/// </example>
/// </remarks>
public static class GCNotifier
{
    /// <summary>
    /// GCが発生した時に実行されるイベント
    /// </summary>
    /// <remarks>
    /// アプリケーションドメインのアンロード中やシャットダウン時には発火しません
    /// </remarks>
    public static event Action? Collected;

    // <summary>
    /// GCの監視を開始します
    /// </summary>
    /// <remarks>
    /// 内部でダミーオブジェクトを生成し、そのファイナライザを利用してGCを検知します
    /// </remarks>
    public static void Start()
    {
        new DummyObject();
    }

    /// <summary>
    /// GC検知用の内部クラス
    /// </summary>
    /// <remarks>
    /// ファイナライザでGCを検知し、新しいインスタンスを生成することで継続的な監視を実現します
    /// </remarks>
    private class DummyObject
    {
        /// <summary>
        /// GC発生時に実行されるファイナライザ
        /// </summary>
        ~DummyObject()
        {
            if (!AppDomain.CurrentDomain.IsFinalizingForUnload()
            && !Environment.HasShutdownStarted)
            {
                Collected?.Invoke();
                new DummyObject();
            }
        }
    }
}
