using Hutzper.Library.Common.IO;

namespace Hutzper.Library.Common.Laungage
{
    /// <summary>
    /// ITranslator実装
    /// </summary>
    [Serializable]
    public class Translator : ITranslator
    {
        #region ILoggable

        /// <summary>
        /// インスタンスを識別する名前
        /// </summary>
        /// <remarks>NicknameとIndexプロパティから成る文字列</remarks>
        public string UniqueName
        {
            #region 取得
            get
            {
                if (0 <= this.Index)
                {
                    return string.Format("{0}{1:D2}", this.Nickname, this.Index + 1);
                }
                else
                {
                    return string.Format("{0}", this.Nickname);
                }
            }
            #endregion
        }

        /// <summary>
        /// 通称を取得、設定する
        /// </summary>
        public string Nickname
        {
            #region 取得
            get
            {
                var value = base.ToString() ?? string.Empty;

                if (false == string.IsNullOrEmpty(this.nickname))
                {
                    value = this.nickname;
                }

                return value;
            }
            #endregion

            #region 更新
            set
            {
                this.nickname = value;
            }
            #endregion
        }
        protected string nickname = String.Empty;

        /// <summary>
        /// 整数インデックス
        /// </summary>
        public int Index { get; set; } = -1;

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="logger"></param>
        public void Attach(string nickname, int index)
        {
            try
            {
                this.nickname = nickname;
                this.Index = index;
            }
            catch
            {
            }
        }

        #endregion

        #region ITranslator

        #region プロパティ

        /// <summary>
        /// 現在の言語設定が日本語であるかどうかを取得します。
        /// </summary>
        /// <returns>true:日本語 false:日本語以外</returns>
        public virtual bool IsJapanese { get => System.Threading.Thread.CurrentThread.CurrentUICulture.Name.StartsWith("ja"); }

        /// <summary>
        /// 現在の言語設定が英語であるかどうかを取得します。
        /// </summary>
        /// <returns>true:英語 false:英語以外</returns>
        public virtual bool IsEnglish { get => System.Threading.Thread.CurrentThread.CurrentUICulture.Name.StartsWith("en"); }

        /// <summary>
        /// 選択されている翻訳種類
        /// </summary>
        public TranslationKind TranslationKind { get; protected set; } = TranslationKind.None;

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
        public void Initiailze(bool isAlwaysDefault)
        {
            this.Initiailze(isAlwaysDefault, Thread.CurrentThread.CurrentUICulture.Name);
        }

        /// <summary>
        /// 初期化します。
        /// </summary>
        /// <param name="isAlwaysDefault">常に開発標準言語で表示するか</param>
        /// <param name="culturesLanguage">適用するカルチャ</param>
        /// <remarks>OSの現在の言語設定の言語の翻訳に対応していない場合、言語設定を英語に変更する</remarks>
        public virtual void Initiailze(bool isAlwaysDefault, string culturesLanguage)
        {
            try
            {
                this.TranslationTables.Clear();
                this.TranslationTables.Add(TranslationKind.None, new Dictionary<string, string>());
                this.TranslationTables.Add(TranslationKind.ToEnglish, new Dictionary<string, string>());
                this.SelectedTable = this.TranslationTables[TranslationKind.None];

                if (true == isAlwaysDefault)
                {
                    culturesLanguage = "ja-JP";
                }

                switch (culturesLanguage)
                {
                    case "ja-JP":
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culturesLanguage);
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culturesLanguage);
                        break;

                    case "en-US":
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(culturesLanguage);
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culturesLanguage);
                        this.SelectedTable = this.TranslationTables[TranslationKind.ToEnglish];
                        break;

                    default:
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                        this.SelectedTable = this.TranslationTables[TranslationKind.ToEnglish];
                        break;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }


        /// <summary>
        /// 現在の言語設定に従って翻訳された文字列を取得します。
        /// </summary>
        /// <param name="sourceString">英語文字列</param>
        /// <returns>翻訳済み文字列</returns>
        public virtual string Translate(params string[] sourceStrings)
        {
            var translatedString = string.Empty;

            // 英語の場合
            if (true == this.IsEnglish)
            {
                foreach (var str in sourceStrings)
                {
                    // 翻訳しない文字
                    if (str == "\n")
                    {
                        translatedString += str;
                    }
                    else if (true == this.SelectedTable.ContainsKey(str))
                    {
                        // 対応する文字列を使用する
                        translatedString += (string?)this.SelectedTable[str];
                    }
                    else
                    {
                        translatedString += str;
                        Serilog.Log.Information("untranslated {str}", str);
                    }
                }
            }
            else
            {
                translatedString = string.Join("", sourceStrings);
            }

            // 翻訳された文字列を返す
            return translatedString ?? string.Empty;
        }

        /// <summary>
        /// 翻訳ファイルのロード
        /// </summary>
        /// <param name="kind">対象の翻訳種類</param>
        /// <param name="fileInfo">ファイル情報</param>
        /// <returns></returns>
        public virtual bool Load(TranslationKind kind, FileInfo fileInfo)
        {
            var isSuccess = false;

            try
            {
                // 対象の翻訳テーブルへの参照を取得して初期化する
                var targetTable = new Dictionary<string, string>();
                if (true == this.TranslationTables.ContainsKey(kind))
                {
                    targetTable = this.TranslationTables[kind];
                }
                targetTable.Clear();

                // ファイルが存在する場合
                if (true == fileInfo.Exists)
                {
                    // CSVファイルとして読み込む
                    var csvReader = new CsvFileReaderWriter(fileInfo.FullName);
                    isSuccess = csvReader.ReadAllLine(out string[][] readData);

                    // 翻訳テーブルに登録する
                    foreach (var line in readData)
                    {
                        if (2 > line.Length || string.IsNullOrEmpty(line[0]) || string.IsNullOrEmpty(line[1]))
                        {
                            continue;
                        }

                        if (false == targetTable.ContainsKey(line[0]))
                        {
                            targetTable.Add(line[0], line[1]);
                        }
                    }

                    // 翻訳登録があり、かつ未登録の翻訳種の場合
                    if (0 < targetTable.Count && false == this.TranslationTables.ContainsKey(kind))
                    {
                        // 翻訳テーブルリストに追加する
                        this.TranslationTables.Add(kind, targetTable);
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 翻訳ファイルのロード
        /// </summary>
        /// </summary>
        /// <param name="directoryInfo">翻訳ファイルが格納されているディレクトリ情報</param>
        /// <returns></returns>
        public virtual bool Load(DirectoryInfo directoryInfo)
        {
            var isSuccess = false;

            try
            {
                if (true == directoryInfo.Exists)
                {
                    isSuccess = true;
                    foreach (TranslationKind kind in Enum.GetValues(typeof(TranslationKind)))
                    {
                        var fileInfo = new FileInfo(System.IO.Path.Combine(directoryInfo.FullName, $"Translation{System.IO.Path.ChangeExtension(kind.ToString(), ".csv")}"));

                        isSuccess &= this.Load(kind, fileInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        #endregion

        #endregion

        #region フィールド

        /// <summary>
        /// 翻訳文字列のテーブルリスト
        /// </summary>
        protected Dictionary<TranslationKind, Dictionary<string, string>> TranslationTables = new();

        /// <summary>
        /// 選択中の翻訳テーブル
        /// </summary>
        protected Dictionary<string, string> SelectedTable;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Translator()
        {
            this.SelectedTable = new();
            this.Initiailze(false, "ja-JP");
        }

        #endregion
    }
}