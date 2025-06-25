namespace Hutzper.Library.Common.Diagnostics
{
    /// <summary>
    /// 診断ユーティリティ
    /// </summary>
    [Serializable]
    public class DiagnosticsUtilities
    {
        /// <summary>
        /// システム稼働時間を取得する
        /// </summary>
        /// <param name="timeSpan">稼働時間を示すTimeSpan値</param>
        /// <returns>稼働時間を示す秒数</returns>
        static public void GetSystemUpTime(out TimeSpan timeSpan)
        {
            timeSpan = TimeSpan.FromSeconds(DiagnosticsUtilities.GetSystemUpTime());
        }

        /// <summary>
        /// システム稼働時間を取得する
        /// </summary>
        /// <returns>稼働時間を示す秒数</returns>
        static public float GetSystemUpTime()
        {
            var counter = new System.Diagnostics.PerformanceCounter("System", "System Up Time");

            // 2度呼ぶ必要がある(1回目は必ず0を返す)
            counter.NextValue();
            return counter.NextValue();
        }
    }
}