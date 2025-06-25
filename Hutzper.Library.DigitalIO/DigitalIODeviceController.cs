using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Common.Threading;
using Hutzper.Library.DigitalIO.Device;
using System.Diagnostics;

namespace Hutzper.Library.DigitalIO
{
    [Serializable]
    public class DigitalIODeviceController : ControllerBase, IDigitalIODeviceController
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

                this.AllDevices.ForEach(d => d.Initialize(serviceCollection));
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

                this.AllDevices.ForEach(d => d.SetConfig(config));
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

                if (parameter is IDigitalIODeviceControllerParameter dp)
                {
                    this.ControllerParameter = dp;

                    foreach (var device in this.AllDevices)
                    {
                        var index = this.AllDevices.IndexOf(device);

                        if (dp.DeviceParameters.Count > index)
                        {
                            device.SetParameter(dp.DeviceParameters[index]);
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

                this.AllDevices.ForEach(d => d.Update());
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
                this.AllDevices.ForEach(d => isSuccess &= d.Open());
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
                this.AllDevices.ForEach(d => isSuccess &= d.Close());
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
                this.ControlInfo.IsContinuable = false;
                this.MonitoringThread.Clear();

                this.AllDevices.ForEach(d => this.DisposeSafely(d));
                this.AllDevices.Clear();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region IDigitalIODeviceController

        /// <summary>
        /// 有効な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public bool Enabled { get; protected set; }

        /// <summary>
        /// 入力デバイス
        /// </summary>
        public virtual int NumberOfInputDevice => this.InputDevices.Count;

        /// <summary>
        /// 出力デバイス
        /// </summary>
        public virtual int NumberOfOutputDevice => this.OutputDevices.Count;

        /// <summary>
        /// 監視中かどうか
        /// </summary>
        public virtual bool IsMonitoring => this.ControlInfo.IsContinuable;

        /// <summary>
        /// 全デバイス
        /// </summary>
        public virtual List<IDigitalIODevice> AllDevices { get; init; }

        /// <summary>
        /// 入力デバイス
        /// </summary>
        public virtual List<IDigitalIOInputDevice> InputDevices { get; init; }

        /// <summary>
        /// 出力デバイス
        /// </summary>
        public virtual List<IDigitalIOOutputDevice> OutputDevices { get; init; }

        /// <summary>
        /// 無効状態に変化した
        /// </summary>
        public event Action<object, IDigitalIODevice>? Disabled;

        /// <summary>
        /// 入力変化
        /// </summary>
        public event Action<object, IDigitalIOInputDevice, int[], int[], bool[]>? InputChanged;

        /// <summary>
        /// デバイス割り付け
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public bool Attach(params IDigitalIODevice[] devices)
        {
            var isSuccess = true;

            try
            {
                foreach (var d in devices)
                {
                    if (false == this.AllDevices.Contains(d))
                    {
                        d.Disabled += this.Device_Disabled;
                        this.AllDevices.Add(d);

                        if (d is IDigitalIOInputDevice id)
                        {
                            this.InputDevices.Add(id);
                        }

                        if (d is IDigitalIOOutputDevice od)
                        {
                            this.OutputDevices.Add(od);
                        }
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
        /// 入力値を取得する
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public virtual bool[] GetVolatileInputValues(IDigitalIOInputDevice device)
        {
            var values = Array.Empty<bool>();

            try
            {
                if (true == this.MonitoringItems.ContainsKey(device))
                {
                    values = this.MonitoringItems[device].InputValues;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return values;
        }

        /// <summary>
        /// 出力値を取得する
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public virtual bool[] GetVolatileOutputValues(IDigitalIOInputDevice device)
        {
            var values = Array.Empty<bool>();

            try
            {
                if (true == this.MonitoringItems.ContainsKey(device))
                {
                    values = this.MonitoringItems[device].OutputValues;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return values;
        }

        /// <summary>
        /// 監視を開始する
        /// </summary>
        /// <returns></returns>
        public virtual bool StartMonitoring()
        {
            try
            {
                this.ControlInfo.IsContinuable = false;
                this.MonitoringThread.Clear();
                this.MonitoringItems.Clear();

                this.ControlInfo = new();

                var times = new List<int>(this.ControllerParameter?.MonitoringIntervalMs ?? Enumerable.Repeat(10, this.AllDevices.Count));
                if (times.Count < this.AllDevices.Count)
                {
                    times.AddRange(Enumerable.Range(10, this.AllDevices.Count - times.Count));
                }

                foreach (var d in this.AllDevices)
                {
                    if (false == d.Enabled)
                    {
                        continue;
                    }

                    var thread = new QueueThread<MonitoringControlItem>() { Priority = ThreadPriority.Highest };
                    thread.Dequeue += Thread_Dequeue;

                    var index = this.AllDevices.IndexOf(d);
                    var item = new MonitoringControlItem(d, this.ControlInfo, times[index]);

                    thread.Enqueue(item);
                    this.MonitoringItems.Add(d, item);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return this.IsMonitoring;
        }

        private void Thread_Dequeue(object sender, MonitoringControlItem item)
        {
            try
            {
                var device = item.InputDevice as IDigitalIODevice;

                device ??= item.OutputDevice;

                if (device == null || device.Enabled == false)
                {
                    return;
                }

                // 開始時入力値
                var inputValues = Array.Empty<int>();
                if (null != item.InputDevice)
                {
                    if (false == item.InputDevice.Enabled)
                    {
                        return;
                    }

                    inputValues = new int[item.InputDevice.NumberOfInputs];

                    if (item.InputDevice.ReadInput(out inputValues))
                    {
                        item.InputValues = Array.ConvertAll(inputValues, v => v != 0);
                    }
                }

                // 開始時出力値
                var outputValues = Array.Empty<int>();
                if (null != item.OutputDevice)
                {
                    if (false == item.OutputDevice.Enabled)
                    {
                        return;
                    }

                    outputValues = new int[item.OutputDevice.NumberOfOutputs];

                    if (item.OutputDevice.ReadOutput(out outputValues))
                    {
                        item.OutputValues = Array.ConvertAll(outputValues, v => v != 0);
                    }
                }

                var stopwatch = Stopwatch.StartNew();
                do
                {
                    // 監視間隔調整
                    var waitTime = item.IntervalMs - stopwatch.ElapsedMilliseconds;
                    if (0 < waitTime)
                    {
                        Thread.Sleep((int)waitTime);
                    }

                    if (false == device.Enabled)
                    {
                        break;
                    }

                    // 入力値読み取り
                    if (null != item.InputDevice)
                    {
                        if (item.InputDevice.ReadInput(out int[] tempValues))
                        {
                            item.InputValues = Array.ConvertAll(tempValues, v => v != 0);

                            this.CheckInputChangedAndNotifyEvent(item.InputDevice, tempValues, inputValues);

                            inputValues = tempValues;
                        }
                    }

                    // 出力値読み取り
                    if (null != item.OutputDevice)
                    {
                        if (item.OutputDevice.ReadOutput(out bool[] tempValues))
                        {
                            item.OutputValues = tempValues;
                        }
                    }

                    stopwatch.Restart();

                } while (item.Info.IsContinuable);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 監視を終了する
        /// </summary>
        public virtual void StopMonitoring()
        {
            try
            {
                this.ControlInfo.IsContinuable = false;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        /// <summary>
        /// 監視制御情報
        /// </summary>
        protected class MonitoringControlInfo
        {
            public bool IsContinuable { get; set; } = true;
        }

        /// <summary>
        /// 監視制御項目
        /// </summary>
        protected class MonitoringControlItem
        {
            public IDigitalIOInputDevice? InputDevice { get; init; }

            public IDigitalIOOutputDevice? OutputDevice { get; init; }

            public MonitoringControlInfo Info { get; init; }

            public int IntervalMs { get; set; } = 10;

            public bool[] InputValues { get; set; } = Array.Empty<bool>();

            public bool[] OutputValues { get; set; } = Array.Empty<bool>();

            public MonitoringControlItem(IDigitalIODevice device, MonitoringControlInfo info, int intervalMs)
            {
                this.InputDevice = device as IDigitalIOInputDevice;
                this.OutputDevice = device as IDigitalIOOutputDevice;

                this.Info = info;
                this.IntervalMs = intervalMs;
            }
        }

        protected List<QueueThread<MonitoringControlItem>> MonitoringThread = new();
        protected MonitoringControlInfo ControlInfo = new();
        protected Dictionary<IDigitalIODevice, MonitoringControlItem> MonitoringItems = new();

        protected IDigitalIODeviceControllerParameter? ControllerParameter;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public DigitalIODeviceController() : base(typeof(DigitalIODeviceController).Name, -1)
        {
            this.AllDevices = new List<IDigitalIODevice>();
            this.InputDevices = new List<IDigitalIOInputDevice>();
            this.OutputDevices = new List<IDigitalIOOutputDevice>();
        }

        private void Device_Disabled(object sender)
        {
            this.OnDisabled((IDigitalIODevice)sender);
        }

        /// <summary>
        /// イベント通知
        /// </summary>
        /// <param name="grabberError"></param>
        protected virtual void OnDisabled(IDigitalIODevice device)
        {
            try
            {
                this.Disabled?.Invoke(this, device);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// イベント通知
        /// </summary>
        /// <param name="grabberError"></param>
        protected virtual void CheckInputChangedAndNotifyEvent(IDigitalIOInputDevice device, int[] currentValues, int[] previousValues)
        {
            try
            {
                var risingEdgeIndex = new List<int>();
                var fallingEdgeIndex = new List<int>();

                var changed = false;
                for (int i = 0; i < currentValues.Length; i++)
                {
                    var xOrBit = currentValues[i] ^ previousValues[i];

                    if (0 != (xOrBit & currentValues[i]))
                    {
                        risingEdgeIndex.Add(i);
                        changed = true;
                    }

                    if (0 != (xOrBit & ~currentValues[i]))
                    {
                        fallingEdgeIndex.Add(i);
                        changed = true;
                    }
                }

                if (true == changed)
                {
                    this.InputChanged?.Invoke(this, device, risingEdgeIndex.ToArray(), fallingEdgeIndex.ToArray(), Array.ConvertAll(currentValues, v => v != 0));
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }
    }
}