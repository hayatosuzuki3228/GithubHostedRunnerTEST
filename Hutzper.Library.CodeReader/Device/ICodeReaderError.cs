namespace Hutzper.Library.CodeReader.Device
{
    /// <summary>
    /// コードリーダーエラーインタフェース
    /// </summary>
    public interface ICodeReaderError
    {
        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// エラー内容を示す文字列
        /// </summary>
        public string ErrorText { get; set; }
    }
}