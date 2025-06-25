namespace Hutzper.Library.Common
{
    /// <summary>
    /// ログ出力を使用する場合のインタフェース
    /// </summary>
    public interface ILoggable
    {
        /// <summary>
        /// インスタンスを識別する名前
        /// </summary>
        /// <remarks>通常PNicknameとIndexプロパティから成る文字列でログ等に使用されることを想定</remarks>
        public string UniqueName { get; }

        /// <summary>
        /// 通称
        /// </summary>
        /// <remarks>クラス名を直接使用しない場合に設定する</remarks>
        public string Nickname { get; set; }

        /// <summary>
        /// インデックス
        /// </summary>
        /// <remarks>インスタンスを識別するための整数値</remarks>
        public int Index { get; set; }

        /// <summary>
        /// ロガーへの参照付与
        /// </summary>
        /// <param name="logger"></param>
        public void Attach(string nickname, int index = -1);
    }
}