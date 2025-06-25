using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Hutzper.Library.Common.IO
{
    /// <summary>
    /// INIファイル
    /// </summary>
    /// <remarks>コメントの読込み、出力、エンコーディング指定に対応</remarks>
    [Serializable]
    public class ManagedIniFile
    {
        #region static readonly

        /// <summary>
        /// セクションチェック正規表現パターン
        /// </summary>
        /// 
        private static readonly string SECTION_PATTERN = @"^\s*\[(?<SECTION>[^\[\]]+?)\]\s*$";

        /// <summary>
        /// キー/値チェック正規表現パターン
        /// </summary>
        private static readonly string KEYVALUE_PATTERN = @"^\s*(?<KEY>[^=;#]+?)\s*=\s*(?<VALUE>[^=;#]*?)(;.*|#.*|\s*)$";

        /// <summary>
        /// コメントチェック正規表現パターン
        /// </summary>
        private static readonly string COMMENT_PATTERN = @"^.*(;|#)(?<COMMENT>.*)$";

        /// <summary>
        /// 正規表現グループ セクション
        /// </summary>
        private static readonly string SECTION = "SECTION";

        /// <summary>
        /// 正規表現グループ キー
        /// </summary>
        private static readonly string KEY = "KEY";

        /// <summary>
        /// 正規表現グループ 値
        /// </summary>
        private static readonly string VALUE = "VALUE";

        /// <summary>
        /// 正規表現グループ コメント
        /// </summary>
        private static readonly string COMMENT = "COMMENT";

        /// <summary>
        /// Mapの値を表すキー
        /// </summary>
        private const string KEY_VALUE = "value";

        /// <summary>
        /// Mapのコメントを表すキー
        /// </summary>
        private const string KEY_COMMENT = "comment";

        #endregion

        #region プロパティ

        /// <summary>
        /// INIファイル構造ディクショナリ
        /// </summary>
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> Map { get; init; }

        /// <summary>
        /// ファイルのエンコーディングを取得
        /// </summary>
        public Encoding Encoding { get; init; }

        /// <summary>
        /// ファイルの名前を取得または設定
        /// </summary>
        public string Name => System.IO.Path.GetFileName(this.Path);

        /// <summary>
        /// ファイルのパスを取得
        /// </summary>
        public string Path { get; init; }

        /// <summary>
        /// コメントの開始文字を取得または設定します。
        /// 出力時のみ使用されます。読込み時のコメント開始文字は";"または"#"が使用可能です。
        /// </summary>
        public string CommentChar { get; set; } = "#";

        #endregion

        #region コンストラクタ

        /// <summary>
        /// ファイル名を含む絶対パスを指定してINIファイルインスタンスを初期化します。
        /// </summary>
        /// <param name="filePath"></param>
        public ManagedIniFile(string filePath) : this(filePath, Encoding.UTF8) { }

        /// <summary>
        /// ファイル名を含む絶対パスと文字エンコーディングを指定してINIファイルクラスインスタンスを初期化します。
        /// </summary>
        /// <param name="filePath">ファイル名を含む絶対パス</param>
        /// <param name="enc">エンコーディング</param>
        public ManagedIniFile(string filePath, Encoding enc)
        {
            this.Path = filePath;
            this.Encoding = (enc is null) ? Encoding.UTF8 : enc;

            this.Map = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            if (File.Exists(this.Path))
            {
                this.Load();
            }
        }
        #endregion

        #region メソッド

        /// <summary>
        /// 指定したパスのINIファイルをDictionaryに読み込みます。
        /// </summary>
        /// <param name="path">ファイル名を含む絶対パス</param>
        public bool Load()
        {
            var lines = new List<string>();

            // 全行取得
            using (var sr = new StreamReader(this.Path, this.Encoding))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    if (line == null) continue;

                    lines.Add(line);
                }
            }

            if (!lines.Any()) return true;

            // セクションとキー/値をチェック
            var secRegex = new Regex(ManagedIniFile.SECTION_PATTERN);
            var keyRegex = new Regex(ManagedIniFile.KEYVALUE_PATTERN);
            var commentRegex = new Regex(ManagedIniFile.COMMENT_PATTERN);
            var lastSec = string.Empty;

            foreach (var line in lines)
            {
                // セクション行
                if (secRegex.IsMatch(line))
                {
                    // セクション名取得
                    var section = secRegex.Match(line).Groups[ManagedIniFile.SECTION].Value.Trim();

                    // 読込み済みではないセクションの場合
                    if (!this.Map.ContainsKey(section))
                    {
                        this.Map[section] = new Dictionary<string, Dictionary<string, string>>();
                    }

                    // 最終読込みセクションをメモ
                    lastSec = section;
                }
                // キー＆値行
                else if (keyRegex.IsMatch(line))
                {
                    // 最終読込みセクションが空
                    if (string.IsNullOrWhiteSpace(lastSec))
                    {
                        continue;
                    }

                    // キーと値を取得
                    var key = keyRegex.Match(line).Groups[ManagedIniFile.KEY].Value.Trim();
                    var value = keyRegex.Match(line).Groups[ManagedIniFile.VALUE].Value.Trim();

                    // キーと値をセクションに追加
                    this.Map[lastSec][key] = new Dictionary<string, string>
                    {
                        [ManagedIniFile.KEY_VALUE] = value
                    };

                    // コメント有り
                    if (commentRegex.IsMatch(line))
                    {
                        var comment = commentRegex.Match(line).Groups[ManagedIniFile.COMMENT].Value;
                        this.Map[lastSec][key][ManagedIniFile.KEY_COMMENT] = comment;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// INIファイルを指定のパスに保存します。
        /// <paramref name="path"/>がnullの場合、インスタンス初期化時のパスに上書き出力します。
        /// </summary>
        /// <param name="path">ファイル名を含む絶対パス</param>
        /// <returns>true:保存成功</returns>
        public bool Save()
        {
            using (var fs = new FileStream(this.Path, FileMode.Create))
            using (var sw = new StreamWriter(fs, this.Encoding))
            {
                sw.WriteLine($"acceptable prefixes for comments are ; and #.");
                sw.WriteLine();

                foreach (var section in this.Map.Keys)
                {
                    sw.WriteLine($"[{section}]");

                    foreach (var pair in this.Map[section])
                    {
                        // キーと値を出力
                        var text = $"{pair.Key} = {pair.Value[ManagedIniFile.KEY_VALUE]}";

                        if (pair.Value.ContainsKey(ManagedIniFile.KEY_COMMENT))
                        {
                            // コメントを出力
                            text += $"\t{this.CommentChar}{pair.Value[ManagedIniFile.KEY_COMMENT]}";
                        }
                        sw.WriteLine(text);
                    }

                    sw.WriteLine();
                }

                sw.Flush();
                fs.Flush();
            }

            return true;
        }

        /// <summary>
        /// セクション名一覧を取得します。
        /// </summary>
        /// <returns>セクション名コレクション</returns>
        public IEnumerable<string> GetSections()
        {
            if (!this.Map.Any())
            {
                return Array.Empty<string>();
            }
            else
            {
                return this.Map.Keys;
            }
        }

        /// <summary>
        /// キー名一覧を取得します。
        /// </summary>
        /// <param name="section">セクション名</param>
        /// <returns>キー名コレクション</returns>
        public IEnumerable<string> GetKeys(string section)
        {
            if (!this.IsSectionExists(section))
            {
                return Array.Empty<string>();
            }
            else
            {
                return this.Map[section].Keys;
            }
        }

        /// <summary>
        /// キー名と値のディクショナリを取得します。
        /// </summary>
        /// <param name="section">セクション名</param>
        /// <returns>キー名=値のディクショナリ</returns>
        public IDictionary<string, string> GetKeyValuePair(string section)
        {
            if (!this.IsSectionExists(section))
            {
                return new Dictionary<string, string>();
            }
            else
            {
                return this.Map[section].ToDictionary(m => m.Key, m => m.Value[KEY_VALUE]);
            }
        }

        /// <summary>
        /// INIファイルに指定のセクションが存在するかチェックします。
        /// </summary>
        /// <param name="section">セクション名</param>
        /// <returns>true:セクション有/false:セクション無</returns>
        public bool IsSectionExists(string section)
        {
            if (!this.Map.Any())
            {
                return false;
            }
            else
            {
                return this.Map.ContainsKey(section);
            }
        }

        /// <summary>
        /// INIファイルの指定のセクションに指定のキー名が存在するかチェックします。
        /// </summary>
        /// <param name="section">セクション名</param>
        /// <param name="key">キー名</param>
        /// <returns>true:キー有/false:キー無</returns>
        public bool IsKeyExists(string section, string key)
        {
            if (!this.IsSectionExists(section))
            {
                return false;
            }
            else
            {
                return this.Map[section].ContainsKey(key);
            }
        }

        /// <summary>
        /// INIファイルの指定のセクションの指定のキー名から値を取得します。
        /// 取得に失敗した場合、デフォルト値を返却します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section">セクション名</param>
        /// <param name="key">キー名</param>
        /// <param name="defVal">デフォルト値</param>
        /// <returns>値</returns>
        public T? GetValue<T>(string section, string key, T? defVal = default)
        {
            T? result = defVal;

            if (!this.IsKeyExists(section, key))
            {
                return result;
            }

            var val = this.Map[section][key][ManagedIniFile.KEY_VALUE];
            Type t = typeof(T);

            try
            {
                result = (T)Convert.ChangeType(val, t);
            }
            catch (Exception e)
            {
                Debug.Write(e.Message);
            }

            return result;
        }

        /// <summary>
        /// INIファイルの指定のセクションの指定のキー名からコメントを取得します。
        /// </summary>
        /// <param name="section">セクション名</param>
        /// <param name="key">キー名</param>
        /// <returns>コメント</returns>
        public string GetComment(string section, string key)
        {
            if (this.IsKeyExists(section, key))
            {
                return this.Map[section][key][ManagedIniFile.KEY_COMMENT];
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// INIファイルの指定のセクションの指定のキー名に値を設定します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section">セクション名</param>
        /// <param name="key">キー名</param>
        /// <param name="value">設定値</param>
        /// <param name="comment">コメント</param>
        public void SetValue<T>(string section, string key, T? value)
        {
            // 存在しないセクションの場合は追加する
            if (!this.Map.ContainsKey(section))
            {
                this.Map[section] = new Dictionary<string, Dictionary<string, string>>();
            }

            // 存在しないキーの場合追加する
            if (!this.Map[section].ContainsKey(key))
            {
                this.Map[section][key] = new Dictionary<string, string>();
            }

            this.Map[section][key][ManagedIniFile.KEY_VALUE] = value?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// INIファイルの指定のセクションの指定のキー名にコメントを設定します。
        /// </summary>
        /// <param name="section">セクション名</param>
        /// <param name="key">キー名</param>
        /// <param name="comment">コメント</param>
        public void SetComment(string section, string key, string comment)
        {
            if (this.IsKeyExists(section, key))
            {
                this.Map[section][key][ManagedIniFile.KEY_COMMENT] = comment;
            }
        }
        #endregion
    }
}