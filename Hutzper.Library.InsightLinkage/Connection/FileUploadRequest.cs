namespace Hutzper.Library.InsightLinkage.Connection
{
    /// <summary>
    /// ファイルアップロード要求
    /// </summary>
    [Serializable]
    public class FileUploadRequest : IFileUploadRequest
    {
        #region IFileUploadRequest

        /// <summary>
        /// アップロードしたいファイル情報
        /// </summary>
        public FileInfo SourceFileInfo { get; set; }

        /// <summary>
        /// アップロード先のフォルダ階層
        /// </summary>
        public List<string> DestinationFolderHierarchy { get; set; } = new();

        /// <summary>
        /// アップロード先のファイル名
        /// </summary>
        public string DestinationFileName { get; set; }

        /// <summary>
        /// アップロード先のプロジェクトUUID
        /// </summary>
        public string ProjectUUID { get; set; }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="destinationFileName"></param>
        public FileUploadRequest(FileInfo fileInfo, string destinationFileName = "", string projectUUID = "")
        {
            this.SourceFileInfo = fileInfo;
            this.DestinationFileName = destinationFileName;

            if (true == string.IsNullOrEmpty(this.DestinationFileName))
            {
                this.DestinationFileName = this.SourceFileInfo.Name;
            }
            this.ProjectUUID = projectUUID;
        }
    }
}