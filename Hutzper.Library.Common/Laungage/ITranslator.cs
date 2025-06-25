namespace Hutzper.Library.Common.Laungage
{
    /// <summary>
    /// 翻訳インタフェース
    /// </summary>
    public interface ITranslator : ILoggable
    {
        #region プロパティ

        /// <summary>
        /// 現在の言語設定が日本語であるかどうか
        /// </summary>
        public bool IsJapanese { get; }

        /// <summary>
        /// 現在の言語設定が英語であるかどうか
        /// </summary>
        /// <returns>true:英語 false:英語以外</returns>
        public bool IsEnglish { get; }

        /// <summary>
        /// 選択されている翻訳種類
        /// </summary>
        public TranslationKind TranslationKind { get; }

        /// <summary>
        /// 未翻訳文字列のログ出力
        /// </summary>
        public bool LoggingOfUntranslatedStrings { get; }

        #endregion

        #region メソッド

        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="isAlwaysDefault">常に開発標準言語で表示するか</param>
        /// <remarks>OSの現在の言語設定の言語の翻訳に対応していない場合、言語設定を英語に変更する</remarks>
        public void Initiailze(bool isAlwaysDefault);

        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="isAlwaysDefault">常に開発標準言語で表示するか</param>
        /// <param name="culturesLanguage">適用するカルチャ</param>
        /// <remarks>OSの現在の言語設定の言語の翻訳に対応していない場合、言語設定を英語に変更する</remarks>
        public void Initiailze(bool isAlwaysDefault, string culturesLanguage);

        /// <summary>
        /// 翻訳ファイルのロード
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public bool Load(TranslationKind kind, FileInfo fileInfo);

        /// <summary>
        /// 翻訳ファイルのロード
        /// </summary>
        /// </summary>
        /// <param name="directoryInfo">翻訳ファイルが格納されているディレクトリ情報</param>
        /// <returns></returns>
        public bool Load(DirectoryInfo directoryInfo);

        /// <summary>
        /// 現在の言語設定に従って翻訳された文字列を取得します。
        /// </summary>
        /// <param name="englishString">英語文字列</param>
        /// <returns>翻訳済み文字列</returns>
        public string Translate(params string[] sourceString);

        #endregion
    }
}