using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.LightController.Device;
using System.Diagnostics;

namespace Hutzper.Library.LightController
{
    [Serializable]
    public class LightControllerSupervisor : ControllerBase, ILightControllerSupervisor
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

                if (parameter is ILightControllerSupervisorParameter p)
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

                this.Enabled = isSuccess;
            }
            catch (Exception ex)
            {
                this.Enabled = false;
                Serilog.Log.Warning(ex, ex.Message);
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
                this.Enabled = false;
                this.Devices.ForEach(g => isSuccess &= g.Close());
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
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

        #region ILightControllerSupervisor

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
        public virtual List<ILightController> Devices { get; init; } = new();

        /// <summary>
        /// 無効状態に変化した
        /// </summary>
        public event Action<object, ILightController>? Disabled;

        /// <summary>
        /// デバイス割り付け
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public virtual bool Attach(params ILightController[] devices)
        {
            var isSuccess = true;

            try
            {
                foreach (var d in devices)
                {
                    if (false == this.Devices.Contains(d))
                    {
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
        /// 点灯
        /// </summary>
        /// <returns></returns>
        public virtual bool TurnOn()
        {
            var isSuccess = true;

            try
            {
                this.Devices.ForEach(d => isSuccess &= d.TurnOn());
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 消灯
        /// </summary>
        /// <returns></returns>
        public virtual bool TurnOff()
        {
            var isSuccess = true;

            try
            {
                this.Devices.ForEach(d => isSuccess &= d.TurnOff());
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        #endregion

        #region フィールド

        /// <summary>
        /// 制御パラメータ
        /// </summary>
        protected ILightControllerSupervisorParameter? ControllerParameter;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public LightControllerSupervisor() : base(typeof(LightControllerSupervisor).Name, -1)
        {
        }

        #endregion

        #region protected メソッド

        /// <summary>
        /// イベント通知:エラー:デバイスロスト
        /// </summary>
        /// <param name="grabberError"></param>
        protected virtual void OnDeviceDisabled(ILightController sender)
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

        #region デバイスイベント

        /// <summary>
        /// デバイスイベント:デバイスロスト
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void Device_Disabled(object sender)
        {
            try
            {
                if (true == this.Enabled)
                {
                    this.OnDeviceDisabled((ILightController)sender);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion
    }
}