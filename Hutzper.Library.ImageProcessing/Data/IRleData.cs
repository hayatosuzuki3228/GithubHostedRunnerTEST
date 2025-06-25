namespace Hutzper.Library.ImageProcessing.Data
{
    public interface IRleData
    {
        #region プロパティ

        /// <summary>
        /// 無効ラベルインデックス
        /// </summary>
        public ushort InvalidLabelIndex { get; }

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

        #region パブリックメソッド

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public IRleData Clone();

        #endregion
    }
}