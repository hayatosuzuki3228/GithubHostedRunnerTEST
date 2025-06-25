namespace Hutzper.Library.CodeReader.Device
{
    /// <summary>
    /// コードリーダーエラー
    /// </summary>
    [Serializable]
    public class CodeReaderError : ICodeReaderError
    {
        #region プロパティ

        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceID { get; set; } = string.Empty;

        /// <summary>
        /// エラー内容を示す文字列
        /// </summary>
        public string ErrorText { get; set; } = string.Empty;

        #endregion
    }
}