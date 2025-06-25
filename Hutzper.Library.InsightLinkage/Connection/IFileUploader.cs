using Hutzper.Library.InsightLinkage.Data;

namespace Hutzper.Library.InsightLinkage.Connection
{
    /// <summary>
    /// ファイルアップローダー
    /// </summary>
    public interface IFileUploader : IConnection
    {
        /// <summary>
        /// 指定されたファイルをアップロードする
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public bool UploadFile(IFileUploadRequest[] requests, InsightRequestType insightRequestType);
    }
}