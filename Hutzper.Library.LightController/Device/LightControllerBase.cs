using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;

namespace Hutzper.Library.LightController.Device
{
    [Serializable]
    public abstract class LightControllerBase : ControllerBase, ILightController
    {
        #region ILightController

        /// <summary>
        /// デバイスID
        /// </summary>
        public virtual string DeviceID { get; protected set; } = string.Empty;

        /// <summary>
        /// 識別
        /// </summary>
        public virtual Common.Drawing.Point Location { get => this.location.Clone(); set => this.location = value?.Clone() ?? this.location; }

        /// <summary>
        /// 制御が可能な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public virtual bool Enabled { get; } = false;

        /// <summary>
        /// チャンネル
        /// </summary>
        public virtual int Channel { get; set; } = -1;

        /// <summary>
        /// 無効状態に変化した
        /// </summary>
        public event Action<object>? Disabled;

        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open() => throw new NotImplementedException();

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public override bool Close() => throw new NotImplementedException();

        /// <summary>
        /// 点灯
        /// </summary>
        /// <returns></returns>
        public virtual bool TurnOn() => throw new NotImplementedException();
        public virtual bool TurnOn(int channel) => throw new NotImplementedException();

        /// <summary>
        /// 消灯
        /// </summary>
        /// <returns></returns>
        public virtual bool TurnOff() => throw new NotImplementedException();
        public virtual bool TurnOff(int channel) => throw new NotImplementedException();

        /// <summary>
        /// 調光
        /// </summary>
        public virtual bool Modulate(double value) => throw new NotImplementedException();
        public virtual bool Modulate(int channel, double value) => throw new NotImplementedException();

        /// <summary>
        /// 調光
        /// </summary>
        public virtual Task<double> ModulationAsync => throw new NotImplementedException();

        /// <summary>
        /// 外部トリガー点灯時間変更
        /// </summary>
        public abstract bool ChangeExternalTriggerStrobeTimeUs(int channel, int strobeTimeUs = 0);
        public abstract bool ChangeExternalTriggerStrobeTimeUs(int strobeTimeUs = 0);

        /// <summary>
        /// 外部トリガー点灯時間
        /// </summary>
        public virtual Task<int> ExternalTriggerStrobeTimeUsAsync => throw new NotImplementedException();

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
                if (parameter is ILightControllerParameter lp)
                {
                    this.Parameter = lp;
                    this.Location = lp.Location;
                    this.Channel = System.Math.Max(-1, lp.Channel);
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

        /// <summary>
        /// 識別
        /// </summary>
        protected Common.Drawing.Point location;

        protected ILightControllerParameter? Parameter;

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public LightControllerBase(string nickname, int locationX, int locationY) : this(nickname, new Common.Drawing.Point(locationX, locationY))
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public LightControllerBase(string nickname, Common.Drawing.Point location) : base(nickname, (location.Y + 1) * 100 + (location.X + 1))
        {
            this.location = location.Clone();
        }

        #endregion

        #region イベント通知

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