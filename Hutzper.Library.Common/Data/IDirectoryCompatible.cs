namespace Hutzper.Library.Common.Data
{
    /// <summary>
    /// ディレクトリ単位のデータインタフェース
    /// </summary>
    /// <remarks>ディレクトリ単位でデータの読み書きを行うデータクラスはこのインタフェースを継承します。</remarks>
    public interface IDirectoryCompatible
    {
        /// <summary>
        /// ディレクトリを指定して読み込み
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public bool Load(DirectoryInfo directory, params string[] fileNames);

        /// <summary>
        /// ディレクトリを指定して書き込み
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public bool Save(DirectoryInfo directory, params string[] fileNames);
    }
}