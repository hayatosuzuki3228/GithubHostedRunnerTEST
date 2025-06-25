using Hutzper.Library.Common.Data;

namespace Hutzper.Library.CodeReader.Device
{
    /// <summary>
    /// コードリーダーデバイスパラメータ
    /// </summary>
    public interface ICodeReaderParameter : IControllerParameter
    {
        #region プロパティ

        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceID { get; }

        /// <summary>
        /// 識別
        /// </summary>
        public Common.Drawing.Point Location { get; }

        /// <summary>
        /// 読み込みタイムアウトミリ秒
        /// </summary>
        public int ReadTimeoutMs { get; set; }

        /// <summary>
        /// 連続読み取り間隔ミリ秒
        /// </summary>
        public int ContinuousReadingIntervalMs { get; set; }

        #endregion
    }
}