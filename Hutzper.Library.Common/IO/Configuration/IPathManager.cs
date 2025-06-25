namespace Hutzper.Library.Common.IO.Configuration
{
    /// <summary>
    /// パス管理インタフェース
    /// </summary>
    public interface IPathManager : IEnumerable<KeyValuePair<string, DirectoryInfo>>, IIniFileCompatible
    {
        /// <summary>
        /// ルートディレクトリ
        /// </summary>
        public DirectoryInfo Root { get; }

        /// <summary>
        /// 設定
        /// </summary>
        public DirectoryInfo Config { get; }

        /// <summary>
        /// データ
        /// </summary>
        public DirectoryInfo Data { get; }

        /// <summary>
        /// レシピ
        /// </summary>
        public DirectoryInfo Recipe { get; }

        /// <summary>
        /// ログ
        /// </summary>
        public DirectoryInfo Log { get; }

        /// <summary>
        /// テンポラリ
        /// </summary>
        public DirectoryInfo Temp { get; }

        /// <summary>
        /// 管理ディレクトリリスト
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, DirectoryInfo> GetDirectoryInfo();

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="path"></param>
        public void Initialize(params KeyValuePair<string, string>[] path);

        /// <summary>
        /// インデクサ
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks>指定したディレクトリ情報を取得します</remarks>
        public DirectoryInfo? this[string name] { get; }
    }
}