namespace Hutzper.Sample.Mekiki.ScratchLegacy.Data
{
    /// <summary>
    /// 検査結果
    /// </summary>
    public interface IInspectionResult : IDisposable
    {
        /// <summary>
        /// 処理番号
        /// </summary>
        public int TaskIndex { get; }

        /// <summary>
        /// 検査日時
        /// </summary>
        public DateTime DateTime { get; }

        /// <summary>
        /// 紐づく情報
        /// </summary>
        public string LinkedInfomation { get; set; }

        /// <summary>
        /// 判定結果文字列
        /// </summary>
        public string JudgementText { get; }

        /// <summary>
        /// 判定結果インデックス
        /// </summary>
        public int JudgementIndex { get; set; }

        /// <summary>
        /// 結果クラス名リスト
        /// </summary>
        public List<string> ResultClassNames { get; set; }

        /// <summary>
        /// 画像データ
        /// </summary>
        public Bitmap?[] Images { get; set; }

        /// <summary>
        /// 追加データ
        /// </summary>
        public IAdditionalDataContainer?[] AdditionalData { get; set; }

        /// <summary>
        /// 汎用格納域
        /// </summary>
        public List<double> GeneralValues { get; set; }

        /// <summary>
        /// 項目別汎用格納域
        /// </summary>
        public List<double>[] ItemsGeneralValues { get; set; }
    }
}