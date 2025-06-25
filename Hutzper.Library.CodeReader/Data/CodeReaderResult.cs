
using Hutzper.Library.Common.Runtime;

namespace Hutzper.Library.CodeReader.Data
{
    /// <summary>
    /// コードリーダー読取結果
    /// </summary>
    [Serializable]
    public class CodeReaderResult : ICodeReaderResult
    {
        #region ICodeReaderResult

        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceID { get; set; } = string.Empty;

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
        public List<string> DataStrings { get; set; } = new();

        #endregion

        #region メソッド

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public ICodeReaderResult Clone()
        {
            var clone = new CodeReaderResult();
            PropertyCopier.CopyTo(this, clone);

            return clone;
        }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CodeReaderResult()
        {
            this.ReadDateTime = DateTime.Now;
        }
    }
}