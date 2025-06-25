using Hutzper.Library.CodeReader.Data;
using Hutzper.Library.Common.Controller;

namespace Hutzper.Library.CodeReader.Device
{
    /// <summary>
    /// コードリーダーデバイスインタフェース
    /// </summary>
    public interface ICodeReader : IController
    {
        #region プロパティ

        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceID { get; }

        /// <summary>
        /// 識別
        /// </summary>
        public Common.Drawing.Point Location { get; protected set; }

        /// <summary>
        /// 読取可能な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public bool Enabled { get; }

        /// <summary>
        /// 自動調整が利用可能かどうか
        /// </summary>
        public bool AutotuningAvailable { get; }

        /// <summary>
        /// 最新の結果
        /// </summary>
        public ICodeReaderResult? LatestResult { get; }

        #endregion

        #region イベント

        /// <summary>
        /// エラーイベント
        /// </summary>
        public event Action<object, ICodeReaderError>? ErrorOccurred;

        /// <summary>
        /// データ取得イベント
        /// </summary>
        public event Action<object, ICodeReaderResult>? DataRead;

        /// <summary>
        /// 無効状態に変化した
        /// </summary>
        public event Action<object>? Disabled;

        #endregion

        #region メソッド

        /// <summary>
        /// 自動チューニング実行
        /// </summary>
        /// <returns></returns>
        public Task<ICodeReaderTuningResult> RunAutotuningAsync();

        /// <summary>
        /// 読取
        /// </summary>
        public Task<ICodeReaderResult> Read(int timeoutMs = -1);

        /// <summary>
        /// 連続読取開始
        /// </summary>
        /// <returns></returns>
        public bool ReadContinuously(int number = -1);

        /// <summary>
        /// 連続読取停止
        /// </summary>
        /// <returns></returns>
        public void StopReading();

        #endregion
    }
}