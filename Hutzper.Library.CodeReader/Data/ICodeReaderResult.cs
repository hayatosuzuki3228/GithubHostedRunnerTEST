namespace Hutzper.Library.CodeReader.Data
{
    /// <summary>
    /// コードリーダー読取結果インタフェース
    /// </summary>
    public interface ICodeReaderResult
    {
        #region プロパティ

        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// 読取成否
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 日時
        /// </summary>
        public DateTime ReadDateTime { get; set; }

        /// <summary>
        /// 読取結果文字列
        /// </summary>
        public List<string> DataStrings { get; set; }

        #endregion

        #region メソッド

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public ICodeReaderResult Clone();

        #endregion
    }
}