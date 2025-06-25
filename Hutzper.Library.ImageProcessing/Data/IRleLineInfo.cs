namespace Hutzper.Library.ImageProcessing.Data
{
    /// <summary>
    /// Rleデータ行情報インタフェース
    /// </summary>
    public interface IRleLineInfo
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

        #region パブリックメソッド

        /// <summary>
        /// 無効化
        /// </summary>
        public void Invalidate();

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public IRleLineInfo Cloe();

        #endregion
    }
}