namespace Hutzper.Library.CodeReader.Data
{
    [Serializable]
    public class CodeReaderTuningResult : CodeReaderResult, ICodeReaderTuningResult
    {
        /// <summary>
        /// チューニング情報
        /// </summary>
        public virtual Dictionary<string, string> TuningInfomation { get; set; } = new();
    }
}