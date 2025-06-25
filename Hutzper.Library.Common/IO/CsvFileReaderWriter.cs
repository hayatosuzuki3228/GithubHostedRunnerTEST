using System.Text;

namespace Hutzper.Library.Common.IO
{
    /// <summary>
    /// CSVファイル読み書き
    /// </summary>
    [Serializable]
    public class CsvFileReaderWriter
    {
        #region プロパティ

        /// <summary>
        /// ファイルの名前
        /// </summary>
        public string FileName { get; protected set; }

        /// <summary>
        /// ファイルが存在するかどうか
        /// </summary>
        public bool IsFileExists => System.IO.File.Exists(this.FileName);

        /// <summary>
        /// ファイルヘッダ有無
        /// </summary>
        /// <remarks>read時のヘッダ1行読み飛ばします</remarks>
        public bool IsHeaderSkip { get; protected set; }

        /// <summary>
        /// ダブルクォーテーション自動付加有無
        /// </summary>
        public bool IsDoubleQuote { get; protected set; }

        /// <summary>
        /// ファイルエンコード
        /// </summary>
        public Encoding FileEncoding { get; protected set; }

        /// <summary>
        /// 改行コードの指定
        /// </summary>
        public string NewLine { get; protected set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fileName">ファイルパス</param>
        /// <param name="doubleQuoteFlag">ダブルクォーテーション自動付加有無</param>
        /// <param name="headerFlag">ヘッダ付加有無</param>
        /// <param name="fileEncoding">文字コード</param>
        public CsvFileReaderWriter(string fileName) : this(fileName, Encoding.UTF8, Environment.NewLine)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fileName">ファイルパス</param>
        /// <param name="fileEncoding">文字コード</param>
        /// <param name="doubleQuoteFlag">ダブルクォーテーション自動付加有無</param>
        /// <param name="headerFlag">ヘッダ付加有無</param>
        /// <param name="newLine">改行コード</param>
        public CsvFileReaderWriter(string fileName, Encoding fileEncoding, string newLine, bool doubleQuoteFlag = false, bool isHeaderSkip = false)
        {
            this.FileName = fileName;
            this.FileEncoding = fileEncoding;
            this.NewLine = newLine;
            this.IsDoubleQuote = doubleQuoteFlag;
            this.IsHeaderSkip = isHeaderSkip;
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 書込(追記)
        /// </summary>
        /// <param name="writeData">書込文字列(１行)</param>        
        /// <returns>true:成功 false:失敗</returns>        
        public bool AppedLine(params string[] writeData)
        {
            //  csvデータ作成
            this.JoinToCsv(writeData, out string writeString);

            //  書き込み
            try
            {
                System.IO.File.AppendAllText(this.FileName, writeString + this.NewLine, this.FileEncoding);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 書込(上書き)
        /// </summary>
        /// <param name="writeData">書込文字列(１行)</param>
        /// <returns>true:成功 false:失敗</returns>
        public bool WriteLine(params string[] writeData)
        {
            //  csvデータ作成
            this.JoinToCsv(writeData, out string writeString);

            //  書き込み
            try
            {
                System.IO.File.WriteAllText(this.FileName, writeString + this.NewLine, this.FileEncoding);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 一括書込(上書き)
        /// </summary>
        /// <param name="writeData">書込文字列(n行)</param>
        /// <returns>true:成功 false:失敗</returns>
        public bool WriteAllLine(params string[][] writeData)
        {
            //  csvデータ作成
            this.JoinToCsv(writeData, out string[] writeStrings);

            //  書き込み
            try
            {
                System.IO.File.WriteAllLines(this.FileName, writeStrings, this.FileEncoding);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 一括読込
        /// </summary>        
        /// <param name="readData">読込文字列(n行)</param>
        /// <returns>true:成功 false:失敗</returns>
        public bool ReadAllLine(out string[][] readData)
        {
            string[]? readStrings;

            //  ファイル読み込み
            try
            {
                readStrings = System.IO.File.ReadAllLines(this.FileName, this.FileEncoding);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                readData = Array.Empty<string[]>();

                return false;
            }

            var adoptedData = new ArraySegment<string>(readStrings, 0, readStrings.Length);

            //  ヘッダ除外
            if (this.IsHeaderSkip)
            {
                adoptedData = new ArraySegment<string>(readStrings, 1, readStrings.Length - 1);
            }

            //  ダブルクォーテーションによる文字列括り有無
            this.GetOptions(out string options);

            //  csv分解
            readData = new string[adoptedData.Count][];
            foreach (var i in Enumerable.Range(0, readData.Length))
            {
                this.SplitFromCsv(adoptedData[i], options, out readData[i]);
            }

            return true;
        }

        #endregion

        #region プライベートメソッド

        /// <summary>
        /// 文字列の括り文字取得
        /// </summary>
        /// <param name="options">括り文字</param>
        protected virtual void GetOptions(out string options)
        {
            //  ダブルクォーテーションによる文字列括り有無
            if (this.IsDoubleQuote)
            {
                options = "\"";
            }
            else
            {
                options = string.Empty;
            }
        }

        /// <summary>
        /// csv文字列作成
        /// </summary>
        /// <param name="beforeData">元文字列</param>
        /// <param name="csvData">csvデータ</param>
        protected virtual void JoinToCsv(string[] splited, out string joinedCsv)
        {
            this.JoinToCsv(new string[][] { splited }, out string[] csvData);

            joinedCsv = csvData[0];
        }

        /// <summary>
        /// csv文字列作成
        /// </summary>
        /// <param name="beforeData">元文字列</param>
        /// <param name="csvData">csvデータ</param>
        protected virtual void JoinToCsv(string[][] splitedArray, out string[] csvData)
        {
            //  ダブルクォーテーションによる文字列括り有無
            this.GetOptions(out string options);

            //  csv変換
            csvData = new string[splitedArray.Length];
            foreach (var i in Enumerable.Range(0, splitedArray.Length))
            {
                this.JoinToCsv(splitedArray[i], options, out csvData[i]);
            }
        }

        /// <summary>
        /// 文字列→Csv変換
        /// </summary>
        /// <param name="before">元文字列</param>
        /// <param name="options">付加情報</param>
        /// <param name="after">csv文字列</param>
        protected virtual void JoinToCsv(string[] splited, string options, out string joinedCsv)
        {
            var csvString = string.Empty;

            foreach (var i in Enumerable.Range(0, splited.Length))
            {
                csvString += string.Format($"{options}{splited[i]}{options},");
            }

            try
            {
                joinedCsv = csvString[..^1];
            }
            catch (Exception)
            {
                joinedCsv = string.Empty;
            }
        }

        /// <summary>
        /// Csv→文字列変換
        /// </summary>
        /// <param name="before">csv文字列</param>
        /// <param name="options">付加情報</param>
        /// <param name="splited">元文字列</param>
        protected virtual void SplitFromCsv(string joinedCsv, string options, out string[] splited)
        {
            splited = joinedCsv.Split(',');

            foreach (var i in Enumerable.Range(0, splited.Length))
            {
                splited[i] = splited[i].Trim(options.ToCharArray());
            }
        }

        #endregion
    }
}