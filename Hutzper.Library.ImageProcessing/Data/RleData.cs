namespace Hutzper.Library.ImageProcessing.Data
{
    /// <summary>
    /// ランレングスデータ
    /// </summary>
    [Serializable]
    public class RleData : IRleData
    {
        #region プロパティ

        /// <summary>
        /// 無効ラベルインデックス
        /// </summary>
        public ushort InvalidLabelIndex { get; init; } = ushort.MaxValue;

        /// <summary>
        /// 先頭かどうか
        /// </summary>
        public ushort IsLeading { get; set; }

        /// <summary>
        /// Y座標
        /// </summary>
        public int CoordY { get; set; }

        /// <summary>
        /// 開始X座標
        /// </summary>
        public ushort CoordX_Begin { get; set; }

        /// <summary>
        /// 終了X座標
        /// </summary>
        public ushort CoordX_End { get; set; }

        /// <summary>
        /// 要素
        /// </summary>
        public ushort Elem { get; set; }

        /// <summary>
        /// ラベルインデックス
        /// </summary>
        public int LabelIndex { get; set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RleData()
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        public RleData(RleData source)
        {
            this.IsLeading = source.IsLeading;
            this.CoordY = source.CoordY;
            this.CoordX_Begin = source.CoordX_Begin;
            this.CoordX_End = source.CoordX_End;
            this.Elem = source.Elem;
            this.LabelIndex = source.LabelIndex;
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public IRleData Clone()
        {
            return new RleData(this);
        }

        #endregion
    }
}