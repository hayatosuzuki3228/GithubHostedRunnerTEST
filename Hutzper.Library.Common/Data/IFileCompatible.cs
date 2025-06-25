namespace Hutzper.Library.Common.Data
{
    /// <summary>
    /// ファイル単位のデータインタフェース
    /// </summary>
    /// <remarks>ファイル単位でデータの読み書きを行うデータクラスはこのインタフェースを継承します。</remarks>
    public interface IFileCompatible
    {
        /// <summary>
        /// ファイルから読みだす
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public bool Load(FileInfo fileInfo);

        /// <summary>
        /// ファイルへ書き込む
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public bool Save(FileInfo fileInfo);
    }
}