using Hutzper.Library.CodeReader.Data;
using Hutzper.Library.CodeReader.Device;
using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using System.Diagnostics;

namespace Hutzper.Library.CodeReader
{
    /// <summary>
    /// コードリーダー制御クラス
    /// </summary>
    [Serializable]
    public class CodeReaderController : ControllerBase, ICodeReaderController
    {
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
                this.Devices.ForEach(g => g.Initialize(serviceCollection));
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
                this.Devices.ForEach(d => d.SetConfig(config));
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
                base.SetParameter(parameter);

                if (parameter is ICodeReaderControllerParameter p)
                {
                    this.ControllerParameter = p;

                    foreach (var device in this.Devices)
                    {
                        var index = this.Devices.IndexOf(device);

                        if (p.DeviceParameters.Count > index)
                        {
                            device.SetParameter(p.DeviceParameters[index]);
                        }
                    }
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
                base.Update();

                this.Devices.ForEach(d => d.Update());
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            var isSuccess = true;

            try
            {
                this.Devices.ForEach(g => isSuccess &= g.Open());
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.Enabled = isSuccess;
            }

            return isSuccess;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            var isSuccess = true;

            try
            {
                this.Devices.ForEach(g => isSuccess &= g.Close());
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.Enabled = false;
            }

            return isSuccess;
        }

        #endregion

        #region SafelyDisposable

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
                this.Devices.ForEach(d => this.DisposeSafely(d));
                this.Devices.Clear();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region ICodeReaderController

        /// <summary>
        /// 有効な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public virtual bool Enabled { get; protected set; }

        /// <summary>
        /// デバイス数
        /// </summary>
        public virtual int NumberOfDevice => this.Devices.Count;

        /// <summary>
        /// デバイス
        /// </summary>
        public virtual List<ICodeReader> Devices { get; init; } = new();

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

        /// <summary>
        /// デバイス割り付け
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public virtual bool Attach(params ICodeReader[] devices)
        {
            var isSuccess = true;

            try
            {
                foreach (var d in devices)
                {
                    if (false == this.Devices.Contains(d))
                    {
                        d.ErrorOccurred += this.Device_ErrorOccurred;
                        d.DataRead += this.Device_DataRead;
                        d.Disabled += this.Device_Disabled;

                        this.Devices.Add(d);
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 読取
        /// </summary>
        public virtual async Task<ICodeReaderResult[]> Read(int timeoutMs = -1)
        {
            var results = new ICodeReaderResult[this.Devices.Count];

            try
            {
                var deviceResults = new Task<ICodeReaderResult>[this.Devices.Count];

                Parallel.For(0, this.Devices.Count, i =>
                {
                    deviceResults[i] = this.Devices[i].Read(timeoutMs);
                });

                await Task.WhenAll(deviceResults);

                Parallel.For(0, this.Devices.Count, i =>
                {
                    results[i] = deviceResults[i].Result;
                });
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return results;
        }

        /// <summary>
        /// 連続読取
        /// </summary>
        /// <returns></returns>
        public virtual bool ReadContinuously(int number = -1)
        {
            var isSuccess = new int[this.Devices.Count];

            try
            {
                Parallel.For(0, this.Devices.Count, i =>
                {
                    var result = this.Devices[i].ReadContinuously(number);

                    isSuccess[i] = Convert.ToInt32(result);
                });
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess.Sum() == this.Devices.Count;
        }

        /// <summary>
        /// 連続読取停止
        /// </summary>
        /// <returns></returns>
        public virtual void StopReading()
        {
            try
            {
                Parallel.ForEach(this.Devices, d =>
                {
                    d.StopReading();
                });
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region フィールド

        /// <summary>
        /// 制御パラメータ
        /// </summary>
        protected ICodeReaderControllerParameter? ControllerParameter;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public CodeReaderController() : base(typeof(CodeReaderController).Name, -1)
        {
        }

        #endregion

        #region protected メソッド

        /// <summary>
        /// イベント通知:エラー
        /// </summary>
        /// <param name="grabberError"></param>
        protected virtual void OnErrorOccurred(ICodeReader sender, ICodeReaderError readerError)
        {
            try
            {
                this.ErrorOccurred?.Invoke(this, sender, readerError);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// イベント通知:読取結果
        /// </summary>
        /// <param name="grabberData"></param>
        protected virtual void OnDataRead(ICodeReader sender, ICodeReaderResult readerResult)
        {
            try
            {
                this.DataRead?.Invoke(this, sender, readerResult);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// イベント通知:エラー:デバイスロスト
        /// </summary>
        /// <param name="grabberError"></param>
        protected virtual void OnDeviceDisabled(ICodeReader sender)
        {
            try
            {
                this.Disabled?.Invoke(this, sender);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region コードリーダーデバイスイベント

        /// <summary>
        /// コードリーダーデバイスイベント:読取結果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        protected virtual void Device_DataRead(object sender, ICodeReaderResult data)
        {
            try
            {
                this.OnDataRead((ICodeReader)sender, data);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// コードリーダーデバイスイベント:エラー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="error"></param>
        protected virtual void Device_ErrorOccurred(object sender, ICodeReaderError error)
        {
            try
            {
                this.OnErrorOccurred((ICodeReader)sender, error);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// コードリーダーデバイスイベント:デバイスロスト
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void Device_Disabled(object sender)
        {
            try
            {
                this.OnDeviceDisabled((ICodeReader)sender);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion
    }
}