using Hutzper.Sample.Mekiki.ScratchLegacy.Data;
using Hutzper.Sample.Mekiki.ScratchLegacy.Data.Inspection;
using Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.Judgement;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Inspection
{
    /// <summary>
    /// 結果と統計表示のインタフェース
    /// </summary>
    public interface IResultAndStatisticsDisplay
    {
        /// <summary>
        /// 最後の検査日時
        /// </summary>
        /// <remarks>最後にAddResultに与えたIInspectionResult.DateTime</remarks>
        public DateTime? LatestDateTime { get; }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <remarks>表示領域の初期化</remarks>
        public void Initialize(IInferenceResultJudgmentParameter parameter);

        /// <summary>
        /// 結果追加
        /// </summary>
        /// <param name="result">追加する検査結果データ</param>
        /// <returns>直前の検査結果日時(日付比較用)</returns>
        public DateTime? AddResult(IInspectionResult result);

        /// <summary>
        /// 結果クリア
        /// </summary>
        public void ClearResults();

        /// <summary>
        /// 現在の統計データを取得
        /// </summary>
        /// <returns>最後に追加した検査結果の日時</returns>
        public IStatisticsData GetStatisticsData();

        /// <summary>
        /// 統計データを設定して復元
        /// </summary>
        public void SetStatisticsData(IStatisticsData data);
    }
}
