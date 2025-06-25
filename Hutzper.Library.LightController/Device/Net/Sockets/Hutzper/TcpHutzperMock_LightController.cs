namespace Hutzper.Library.LightController.Device.Net.Sockets.Hutzper
{
    /// <summary>
    /// テスト用モッククラス
    /// </summary>
    /// <remarks>将来的にシミュレータを用意して挙動確認や異常系のテストを想定</remarks>
    public class TcpHutzperMock_LightController : LightControllerBase, ITcpLightController
    {
        #region ILightController

        /// <summary>
        /// 制御が可能な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public override bool Enabled => this.IsOpend;

        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            this.IsOpend = true;

            this.Connected?.Invoke(this);

            #region "調光設定"
            // 調光設定反映（レシピの調光設定とは無関係）
            if (null != this.CommunicationParameter && this.CommunicationParameter is ILightControllerParameter)
            {
                this.Modulate(this.CommunicationParameter.Modulation);
            }
            #endregion

            return true;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            this.IsOpend = false;

            this.OnDisabled();
            this.Disconnected?.Invoke(this);

            return true;
        }

        /// <summary>
        /// 点灯
        /// </summary>
        /// <returns></returns>
        public override bool TurnOn() => true;

        /// <summary>
        /// 消灯
        /// </summary>
        /// <returns></returns>
        public override bool TurnOff() => true;

        /// <summary>
        /// 調光
        /// </summary>
        public override bool Modulate(double value)
        {
            try
            {
                this.ModulationValue = Convert.ToInt32(System.Math.Max(value, 0));
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return true;
        }

        /// <summary>
        /// 調光
        /// </summary>
        public override Task<double> ModulationAsync => Task.FromResult((double)this.ModulationValue);

        /// <summary>
        /// 外部トリガー点灯時間変更
        /// </summary>
        public override bool ChangeExternalTriggerStrobeTimeUs(int strobeTimeUs = 0) => false;
        public override bool ChangeExternalTriggerStrobeTimeUs(int channel, int strobeTimeUs = 0) => false;

        /// <summary>
        /// 外部トリガー点灯時間
        /// </summary>
        public override Task<int> ExternalTriggerStrobeTimeUsAsync => Task.FromResult(0);

        #endregion

        #region ITcpLightController

        /// <summary>
        /// 接続イベント
        /// </summary>
        public event Action<object>? Connected;

        /// <summary>
        /// 切断イベント
        /// </summary>
        public event Action<object>? Disconnected;

        /// <summary>
        /// 再接続
        /// </summary>
        public virtual bool IsReconnectable
        {
            get => this.CommunicationParameter?.IsReconnectable ?? false;

            set
            {
                if (null != this.CommunicationParameter)
                {
                    this.CommunicationParameter.IsReconnectable = value;
                }
            }
        }

        /// <summary>
        /// 再接続試行間隔
        /// </summary>
        public virtual int ReconnectionAttemptsIntervalSec
        {
            get => this.CommunicationParameter?.ReconnectionAttemptsIntervalSec ?? -1;

            set
            {
                if (null != this.CommunicationParameter)
                {
                    this.CommunicationParameter.ReconnectionAttemptsIntervalSec = value;
                }
            }
        }

        #endregion

        #region フィールド

        protected ITcpCommunicationParameter? CommunicationParameter;

        protected bool IsOpend;
        protected int ModulationValue;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TcpHutzperMock_LightController() : this(typeof(TcpHutzperMock_LightController).Name, Common.Drawing.Point.New())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="location">インスタンスを識別するインデックス</param>
        public TcpHutzperMock_LightController(Common.Drawing.Point location) : this(typeof(TcpHutzperMock_LightController).Name, location)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname">クラスを示す文字列</param>
        /// <param name="location">インスタンスを識別するインデックス</param>
        public TcpHutzperMock_LightController(string nickname, Common.Drawing.Point location) : base(nickname, location)
        {
        }

        #endregion
    }
}