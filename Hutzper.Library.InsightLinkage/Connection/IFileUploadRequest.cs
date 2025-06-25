namespace Hutzper.Library.InsightLinkage.Connection
{
    /// <summary>
    /// ファイルアップロード要求
    /// </summary>
    public interface IFileUploadRequest
    {
        /// <summary>
        /// アップロードしたいファイル情報
        /// </summary>
        public FileInfo SourceFileInfo { get; set; }

        /// <summary>
        /// アップロード先のフォルダ階層
        /// </summary>
        public List<string> DestinationFolderHierarchy { get; set; }

        /// <summary>
        /// アップロード先のファイル名
        /// </summary>
        public string DestinationFileName { get; set; }

        /// <summary>
        /// アップロード先のプロジェクトUUID
        /// </summary>
        public string ProjectUUID { get; set; }
    }
}