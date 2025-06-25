namespace Hutzper.Library.CodeReader.Data
{
    public interface ICodeReaderTuningResult : ICodeReaderResult
    {
        /// <summary>
        /// チューニング情報
        /// </summary>
        public Dictionary<string, string> TuningInfomation { get; set; }
    }
}