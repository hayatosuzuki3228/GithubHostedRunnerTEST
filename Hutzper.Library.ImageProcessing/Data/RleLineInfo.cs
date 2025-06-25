namespace Hutzper.Library.ImageProcessing.Data
{
    /// <summary>
    /// Rleデータ行情報
    /// </summary>
    [Serializable]
    public class RleLineInfo : IRleLineInfo
    {
        #region プロパティ

        /// <summary>
        /// RLEデータ開始インデックス
        /// </summary>
        public int FirstIndex { get; set; }

        /// <summary>
        /// Rleデータ終了インデックス
        /// </summary>
        public int LastIndex { get; set; }

        /// <summary>
        /// Rleデータ数
        /// </summary>
        public int DataNumber { get; set; }

        /// <summary>
        /// ラベルインデックス
        /// </summary>
        public int LableIndex { get; set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RleLineInfo()
        {
            this.Invalidate();
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        public RleLineInfo(RleLineInfo source)
        {
            this.FirstIndex = source.FirstIndex;
            this.LastIndex = source.LastIndex;
            this.DataNumber = source.DataNumber;
            this.LableIndex = source.LableIndex;
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 無効化
        /// </summary>
        public void Invalidate()
        {
            this.FirstIndex = 0;
            this.LastIndex = 0;
            this.DataNumber = 0;
            this.LableIndex = 0;
        }

        public IRleLineInfo Cloe()
        {
            return new RleLineInfo(this);
        }

        #endregion
    }
}