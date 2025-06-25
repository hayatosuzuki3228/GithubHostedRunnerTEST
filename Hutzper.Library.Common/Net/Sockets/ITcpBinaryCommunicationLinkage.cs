namespace Hutzper.Library.Common.Net.Sockets
{
    /// <summary>
    /// 読み込み処理コールバック
    /// </summary>
    /// <param name="memoryStream"></param>
    /// <param name="transferData"></param>
    /// <returns>読み込んだデータがある場合はtrue  transferDataに格納</returns>
    /// <remarks></remarks>
    public delegate bool TcpBinaryCommunicationReadCallback(MemoryStream memoryStream, out ArraySegment<byte> transferData);

    /// <summary>
    /// TCPバイナリ読み書きインタフェース
    /// </summary>
    public interface ITcpBinaryCommunicationLinkage : ISafelyDisposable, ILoggable
    {
        /// <summary>
        /// ストリーム無効
        /// </summary>
        public event Action<object>? StreamInvalidated;

        /// <summary>
        /// データ読み込み
        /// </summary>
        public event Action<object, ArraySegment<byte>>? TransferDataRead;

        /// <summary>
        /// データ書き込み完了
        /// </summary>
        public event Action<object, ArraySegment<byte>>? TransferDataWriteCompleted;

        /// <summary>
        /// ストリーム
        /// </summary>
        /// <param name="stream"></param>
        public void Attach(Stream stream, TcpBinaryCommunicationReadCallback? readProcess = null);

        /// <summary>
        /// 非同期書き込み
        /// </summary>
        /// <param name="data"></param>
        public void AysncWrite(ArraySegment<byte> data);
    }
}