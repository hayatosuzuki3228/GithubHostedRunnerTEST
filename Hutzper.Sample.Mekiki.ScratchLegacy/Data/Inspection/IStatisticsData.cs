using System.Text.Json;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Data.Inspection
{
    /// <summary>
    /// 統計データのインタフェース
    /// </summary>
    public interface IStatisticsData
    {
        /// <summary>
        /// 最後の検査日時
        /// </summary>
        public DateTime? LatestDateTime { get; set; }

        /// <summary>
        /// 全体統計
        /// </summary>
        public Dictionary<string, int> OverallStatistics { get; set; }

        /// <summary>
        /// Ng詳細統計
        /// </summary>
        public Dictionary<string, int> NgDetailsStatistics { get; set; }

        /// <summary>
        /// ファイルから読み込む
        /// </summary>
        /// <returns>成功の場合はtrue</returns>
        public bool LoadFromFile(FileInfo fileInfo);

        /// <summary>
        /// ファイルに保存する
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns>成功の場合はtrue</returns>
        public bool SaveToFile(FileInfo fileInfo);
    }

    /// <summary>
    /// 統計データ
    /// </summary>
    public class StatisticsData : IStatisticsData
    {
        /// <summary>
        /// 最後の検査日時
        /// </summary>
        public DateTime? LatestDateTime { get; set; }

        /// <summary>
        /// 全体統計
        /// </summary>
        public Dictionary<string, int> OverallStatistics { get; set; } = new();

        /// <summary>
        /// Ng詳細統計
        /// </summary>
        public Dictionary<string, int> NgDetailsStatistics { get; set; } = new();

        /// <summary>
        /// ファイルから読み込む
        /// </summary>
        /// <returns>成功の場合はtrue</returns>
        public bool LoadFromFile(FileInfo fileInfo)
        {
            if (false == fileInfo.Exists)
            {
                return false;
            }

            var json = File.ReadAllText(fileInfo.FullName);
            var data = JsonSerializer.Deserialize<StatisticsData>(json);

            if (data is not null)
            {
                this.LatestDateTime = data.LatestDateTime;
                this.OverallStatistics = data.OverallStatistics;
                this.NgDetailsStatistics = data.NgDetailsStatistics;
            }

            return true;
        }

        /// <summary>
        /// ファイルに保存する
        /// </summary>
        /// <returns>成功の場合はtrue</returns>
        public bool SaveToFile(FileInfo fileInfo)
        {
            try
            {
                var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fileInfo.FullName, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存エラー: {ex.Message}");
                return false;
            }
        }
    }
}
