using Hutzper.Library.CodeReader.Data;
using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;

namespace Hutzper.Library.CodeReader.Device
{
    /// <summary>
    /// コードリーダーデバイス基底クラス
    /// </summary>
    [Serializable]
    public abstract class CodeReaderBase : ControllerBase, ICodeReader
    {
        #region ICodeReader

        /// <summary>
        /// デバイスID
        /// </summary>
        public virtual string DeviceID { get; protected set; } = string.Empty;

        /// <summary>
        /// 識別
        /// </summary>
        public virtual Common.Drawing.Point Location { get => this.location.Clone(); set => this.location = value?.Clone() ?? this.location; }

        /// <summary>
        /// 読取可能な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public virtual bool Enabled { get; }

        /// <summary>
        /// 自動調整が利用可能かどうか
        /// </summary>
        public virtual bool AutotuningAvailable { get; } = false;

        /// <summary>
        /// 最新の結果
        /// </summary>
        public virtual ICodeReaderResult? LatestResult { get; protected set; }

        /// <summary>
        /// 自動チューニング実行
        /// </summary>
        /// <returns></returns>
        public virtual Task<ICodeReaderTuningResult> RunAutotuningAsync() => throw new NotImplementedException();

        /// <summary>
        /// 読取
        /// </summary>
        public virtual Task<ICodeReaderResult> Read(int timeoutMs = -1) => throw new NotImplementedException();

        /// <summary>
        /// 連続読取開始
        /// </summary>
        /// <returns></returns>
        public virtual bool ReadContinuously(int number = -1) => throw new NotImplementedException();

        /// <summary>
        /// 連続読取停止
        /// </summary>
        /// <returns></returns>
        public virtual void StopReading() => throw new NotImplementedException();

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

        #region IController

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="serviceCollection"></param>
        public override void Initialize(IServiceCollectionSharing? serviceCollection)
        {
            try
            {
                base.Initialize(serviceCollection);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 設定
        /// </summary>
        /// <param name="config"></param>
        public override void SetConfig(IApplicationConfig? config)
        {
            try
            {
                base.SetConfig(config);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// パラメーター設定
        /// </summary>
        /// <param name="parameter"></param>
        public override void SetParameter(IControllerParameter? parameter)
        {
            try
            {
                if (parameter is ICodeReaderParameter p)
                {
                    this.Parameter = p;
                    this.DeviceID = p.DeviceID;
                    this.Location = p.Location;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region フィールド

        /// <summary>
        /// 識別
        /// </summary>
        protected Common.Drawing.Point location;

        /// <summary>
        /// コードリーダーデバイスパラメータ
        /// </summary>
        protected ICodeReaderParameter? Parameter;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public CodeReaderBase(string nickname, int locationX, int locationY) : this(nickname, new Common.Drawing.Point(locationX, locationY))
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public CodeReaderBase(string nickname, Common.Drawing.Point location) : base(nickname, (location.Y + 1) * 100 + (location.X + 1))
        {
            this.location = location.Clone();
        }

        #endregion

        #region protected メソッド

        /// <summary>
        /// エラーイベント通知
        /// </summary>
        protected virtual void OnErrorOccurred(ICodeReaderError readerError)
        {
            try
            {
                readerError.DeviceID = this.DeviceID;
                this.ErrorOccurred?.Invoke(this, readerError);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// データ取得イベント通知
        /// </summary>
        protected virtual void OnDataRead(ICodeReaderResult readerResult)
        {
            try
            {
                readerResult.DeviceID = this.DeviceID;
                this.DataRead?.Invoke(this, readerResult);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 無効状態に変化通知
        /// </summary>
        protected virtual void OnDisabled()
        {
            try
            {
                this.Disabled?.Invoke(this);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion
    }
}