namespace Hutzper.Library.Common.Net.Sockets
{
    /// <summary>
    /// PLCデバイス読み書きインタフェース
    /// </summary>
    public interface IPlcDeviceReaderWriter : ISafelyDisposable, ILoggable
    {
        #region プロパティ
        /// <summary>
        /// 読み書きが有効かどうか
        /// </summary>
        public bool Enabled { get; }

        /// <summary>
        /// タイムアウトミリ秒
        /// </summary>
        public int TimeoutMs { get; set; }

        #endregion

        #region イベント

        /// <summary>
        /// 切断イベント
        /// </summary>
        public event Action<object>? Disconnected;

        #endregion

        #region メソッド

        /// <summary>
        /// オープン
        /// </summary>
        /// <param name="ipAddress">接続先のIPアドレス</param>
        /// <param name="portNumber">接続先のポート番号</param>
        /// <returns>接続に成功したかどうか</returns>
        /// <remarks>接続を試みます</remarks>
        public bool Open(string ipAddress, int portNumber);

        /// <summary>
        /// クローズ
        /// </summary>
        /// <remarks>切断します</remarks>
        public void Close();

        /// <summary>
        /// ワードデバイスを読み込む
        /// </summary>
        /// <param name="kind">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス点数</param>
        /// <param name="values">読み取り値</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>読取に成功したかどうか</returns>
        public bool ReadDevice(WordDeviceType deviceType, int address, int number, out List<int> values, out string errorMessage);

        /// <summary>
        /// ビットデバイスを読み込む
        /// </summary>
        /// <param name="kind">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス点数</param>
        /// <param name="values">読み取り値</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>読取に成功したかどうか</returns>
        /// <remarks>論理値として結果を取得します</remarks>
        public bool ReadDevice(BitDeviceType deviceType, int address, int number, out List<bool> values, out string errorMessage);

        /// <summary>
        /// ビットデバイスを読み込む
        /// </summary>
        /// <param name="kind">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス点数</param>
        /// <param name="values">読み取り値</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>読取に成功したかどうか</returns>
        /// <remarks>整数値として結果を取得します</remarks>
        public bool ReadDevice(BitDeviceType deviceType, int address, int number, out List<int> values, out string errorMessage);

        /// <summary>
        /// ワードデバイスに書き込む
        /// </summary>
        /// <param name="kind">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス点数</param>
        /// <param name="values">書き込み値</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>書き込みにに成功したかどうか</returns>
        public bool WriteDevice(WordDeviceType deviceType, int address, int number, List<int> values, out string errorMessage);

        /// <summary>
        /// ビットデバイスに書き込む
        /// </summary>
        /// <param name="kind">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス点数</param>
        /// <param name="values">書き込み値</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>書き込みにに成功したかどうか</returns>
        public bool WriteDevice(BitDeviceType deviceType, int address, int number, List<bool> values, out string errorMessage);

        /// <summary>
        /// ビットデバイスに書き込む
        /// </summary>
        /// <param name="kind">デバイス種</param>
        /// <param name="address">先頭アドレス</param>
        /// <param name="number">デバイス点数</param>
        /// <param name="values">書き込み値</param>
        /// <param name="errorMessage">エラーメッセージ</param>
        /// <returns>書き込みにに成功したかどうか</returns>
        /// <remarks>書き込み値は0か0以外の論理型として扱われます</remarks>
        public bool WriteDevice(BitDeviceType deviceType, int address, int number, List<int> values, out string errorMessage);

        #endregion
    }

    /// <summary>
    /// ワードデバイス種
    /// </summary>
    [Serializable]
    public enum WordDeviceType : int
    {
        W = 0,
        D = 1,
    }

    /// <summary>
    /// ビットデバイス種
    /// </summary>
    [Serializable]
    public enum BitDeviceType : int
    {
        B = 0,
        M = 1,
    }
}