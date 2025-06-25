using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.Net.Sockets;
using Hutzper.Library.Common.Serialization;
using Hutzper.Library.Vibrometer.Data;
using Hutzper.Library.Vibrometer.Data.Vibrometer.Hutzper;
using Hutzper.Library.Vibrometer.Vibrometer.Socket;

namespace Hutzper.Library.Vibrometer.Vibrometer.Hutzper
{
    /// <summary>
    /// TCP通信 振動計クラス
    /// </summary>
    public class HutzperTcpClientIVibrometer : ControllerBase, ITcpClientIVibrometer
    {
        #region SafelyDisposable

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
                this.Stop();
                this.Close();
                this.communicatorClient.Dispose();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
            }
        }

        #endregion

        #region IController

        /// <summary>
        /// パラメーター設定
        /// </summary>
        /// <param name="parameter"></param>
        public override void SetParameter(IControllerParameter? parameter)
        {
            try
            {
                if (parameter is HutzperTcpClientVibrometerParameter param)
                {
                    this.parameter = param;
                }
                else
                {
                    throw new InvalidCastException("HutzperTcpClientVibrometerParameter param");
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region IVibrometer

        /// <summary>
        /// 有効かどうか
        /// </summary>
        public bool Enabled => this.isOpend;

        /// <summary>
        /// 機械停止フラグ
        /// </summary>
        public bool StopDeviceFlag { get; set; }

        public event Action<object, IVibrometerResult>? DataReceived;
        public event Action<object, IVibrometerErrorInfo>? Error;

        /// <summary>
        /// オープン
        /// </summary>
        /// <param name="parameter"></param>
        public override bool Open()
        {
            try
            {
                var myParam = this.parameter;
                if (myParam == null)
                {
                    throw new NullReferenceException("this.parameter is null");
                }
                this.isOpend = this.communicatorClient.Connect(myParam.IpAddress, myParam.PortNumber);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return this.isOpend;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        public override bool Close()
        {
            this.isOpend = false;
            this.communicatorClient.Disconnect();

            return true;
        }

        /// <summary>
        /// 開始
        /// </summary>
        public virtual void Start()
        {
            if (true == this.communicatorClient.Enabled)
            {
                this.IsRunning = true;
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public virtual void Stop()
        {
            this.IsRunning = false;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// 実行中かどうか
        /// </summary>
        public bool IsRunning { get; protected set; }

        #endregion

        #region フィールド

        protected HutzperTcpClientVibrometerParameter? parameter;

        /// <summary>
        /// TcpCommunicatorClient
        /// </summary>
        private readonly TcpSerializationCommunicatorClient<SerializableTransferData> communicatorClient;

        /// <summary>
        /// JsonSerializer
        /// </summary>
        private readonly JsonSerializer serializer = new();

        private bool isOpend;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HutzperTcpClientIVibrometer() : base("HutzperTcp")
        {
            this.communicatorClient = new TcpSerializationCommunicatorClient<SerializableTransferData>();
            this.communicatorClient.Disconnected += this.CommunicatorClient_Disconnected;
            this.communicatorClient.TransferDataRead += this.CommunicatorClient_TransferDataRead;
        }

        #region TcpCommunicatorClientイベント

        /// <summary>
        /// TcpCommunicatorClientイベント;データ読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void CommunicatorClient_TransferDataRead(object sender, ISerializableTransferData data)
        {
            try
            {
                var result = serializer.Deserialize<HutzperTcpClientVibrometerResult>(data.SerializedBytes);
                if (result == null)
                {
                    throw new NullReferenceException("null. serializer.Deserialize<HutzperTcpClientVibrometerResult>(data.SerializedBytes)");
                }
                if (true == this.IsRunning)
                {
                    this.DataReceived?.Invoke(this, result);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// TcpCommunicatorClientイベント:切断
        /// </summary>
        /// <param name="sender"></param>
        private void CommunicatorClient_Disconnected(object sender)
        {
            this.IsRunning = false;
            if (true == this.isOpend)
            {
                this.isOpend = false;
                this.Error?.Invoke(this, new VibrometerErrorInfo());
            }
        }

        #endregion

        #endregion
    }
}