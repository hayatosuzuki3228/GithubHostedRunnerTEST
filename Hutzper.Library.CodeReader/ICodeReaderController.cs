using Hutzper.Library.CodeReader.Data;
using Hutzper.Library.CodeReader.Device;
using Hutzper.Library.Common.Controller;

namespace Hutzper.Library.CodeReader
{
    /// <summary>
    /// コードリーダー制御インタフェース
    /// </summary>
    public interface ICodeReaderController : IController
    {
        #region プロパティ

        /// <summary>
        /// 有効な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public bool Enabled { get; }

        /// <summary>
        /// デバイス数
        /// </summary>
        public int NumberOfDevice { get; }

        /// <summary>
        /// 全デバイス
        /// </summary>
        public List<ICodeReader> Devices { get; init; }

        #endregion

        #region イベント

        /// <summary>
        /// エラーイベント
        /// </summary>
        public event Action<object, ICodeReader, ICodeReaderError>? ErrorOccurred;

        /// <summary>
        /// データ取得イベント
        /// </summary>
        public event Action<object, ICodeReader, ICodeReaderResult>? DataRead;

        /// <summary>
        /// 無効状態に変化した
        /// </summary>
        public event Action<object, ICodeReader>? Disabled;

        #endregion

        #region メソッド

        /// <summary>
        /// デバイス割り付け
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public bool Attach(params ICodeReader[] devices);

        /// <summary>
        /// 読取
        /// </summary>
        public Task<ICodeReaderResult[]> Read(int timeoutMs = -1);

        /// <summary>
        /// 連続読取
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