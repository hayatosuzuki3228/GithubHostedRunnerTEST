using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;

namespace Hutzper.Library.DigitalIO.Device.Hutzper
{
    public class HutzperTimingInjectedIO : ControllerBase, IDigitalIOInputDevice, IDigitalIOOutputDevice, IDigitalIODeviceInputSimulatable
    {
        #region IController

        /// <summary>
        /// パラメーター設定
        /// </summary>
        /// <param name="parameter"></param>
        public override void SetParameter(IControllerParameter? parameter)
        {
            try
            {
                base.SetParameter(parameter);

                if (parameter is IDigitalIODeviceParameter p)
                {
                    this.Parameter = p;
                    this.DeviceID = p.DeviceID;
                    this.Location = p.Location;

                    lock (this.SyncInput)
                    {
                        this.InputChannels = new int[this.Parameter.InputChannels.Length];
                    }

                    lock (this.SyncOutput)
                    {
                        this.OutputChannels = new int[this.Parameter.OutputChannels.Length];
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region IDigitalIODevice

        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceID { get; protected set; } = string.Empty;

        /// <summary>
        /// 識別
        /// </summary>
        public Common.Drawing.Point Location { get => this.location.Clone(); set => this.location = value?.Clone() ?? this.location; }

        /// <summary>
        /// 有効か
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// 無効状態に変化した
        /// </summary>
        public event Action<object>? Disabled;

        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open() => this.Enabled = true;

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            this.Enabled = false;
            this.Disabled?.Invoke(this);
            return true;
        }

        #endregion

        #region IDigitalIOInputDevice

        /// <summary>
        /// 入力点数
        /// </summary>
        public int NumberOfInputs => this.Parameter?.InputChannels.Length ?? 0;

        /// <summary>
        /// 全読み出し
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool ReadInput(out bool[] values)
        {
            var isSuccess = this.ReadInput(out int[] intValues);

            values = Array.ConvertAll(intValues, v => v != 0);

            return isSuccess;
        }

        /// <summary>
        /// 全読み出し
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool ReadInput(out int[] values)
        {
            var isSuccess = this.Enabled;
            values = new int[this.InputChannels.Length];

            try
            {
                lock (this.SyncInput)
                {
                    Array.Copy(this.InputChannels, values, values.Length);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 指定読み出し
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool ReadInput(int index, out bool value)
        {
            var isSuccess = this.ReadInput(index, out int intValue);

            value = intValue != 0;

            return isSuccess;
        }

        /// <summary>
        /// 指定読み出し
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool ReadInput(int index, out int value)
        {
            var isSuccess = this.Enabled;
            value = 0;

            try
            {
                if (true == this.Enabled && this.ReadInput(out int[] values))
                {
                    if (values.Length > index)
                    {
                        value = values[index];
                    }
                    else
                    {
                        isSuccess = false;
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

        #endregion

        #region IDigitalIOOutputDevice

        /// <summary>
        /// 出力点数
        /// </summary>
        public int NumberOfOutputs => this.Parameter?.OutputChannels.Length ?? 0;

        /// <summary>
        /// 全書き込み
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool WriteOutput(bool[] values)
        {
            var isSuccess = this.Enabled;

            try
            {
                lock (this.SyncOutput)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        this.OutputChannels[i] = values[i] ? 1 : 0;
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
        /// 全書き込み
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool WriteOutput(int[] values)
        {
            var isSuccess = this.Enabled;

            try
            {
                lock (this.SyncOutput)
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        this.OutputChannels[i] = (values[i] != 0) ? 1 : 0;
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
        /// 指定書き込み
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool WriteOutput(int index, bool value)
        {
            var isSuccess = this.Enabled;

            try
            {
                if (true == isSuccess)
                {
                    isSuccess = this.WriteOutput(index, value ? 1 : 0);
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
        /// 指定書き込み
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool WriteOutput(int index, int value)
        {
            var isSuccess = this.Enabled;

            try
            {
                lock (this.SyncOutput)
                {
                    this.OutputChannels[index] = (value != 0) ? 1 : 0;
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
        /// 全読み出し
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool ReadOutput(out bool[] values)
        {
            var isSuccess = this.ReadOutput(out int[] intValues);

            values = Array.ConvertAll(intValues, v => v != 0);

            return isSuccess;
        }

        /// <summary>
        /// 全読み出し
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool ReadOutput(out int[] values)
        {
            var isSuccess = this.Enabled;
            values = new int[this.NumberOfOutputs];

            try
            {
                lock (this.SyncOutput)
                {
                    Array.Copy(this.OutputChannels, values, values.Length);
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
        /// 指定読み出し
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool ReadOutput(int index, out bool value)
        {
            var isSuccess = this.ReadOutput(index, out int intValue);

            value = intValue != 0;

            return isSuccess;
        }

        /// <summary>
        /// 指定読み出し
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool ReadOutput(int index, out int value)
        {
            var isSuccess = this.Enabled;
            value = 0;

            try
            {
                if (true == this.Enabled && this.ReadOutput(out int[] values))
                {
                    if (values.Length > index)
                    {
                        value = values[index];
                    }
                    else
                    {
                        isSuccess = false;
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

        #endregion

        #region フィールド

        /// <summary>
        /// 識別
        /// </summary>
        protected Common.Drawing.Point location;

        protected IDigitalIODeviceParameter? Parameter;

        protected int[] InputChannels = Array.Empty<int>();
        protected int[] OutputChannels = Array.Empty<int>();

        protected object SyncInput = new object();
        protected object SyncOutput = new object();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public HutzperTimingInjectedIO() : this(typeof(HutzperTimingInjectedIO).Name, Common.Drawing.Point.New())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public HutzperTimingInjectedIO(Common.Drawing.Point location) : this(typeof(HutzperTimingInjectedIO).Name, location)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="index"></param>
        public HutzperTimingInjectedIO(string nickname, Common.Drawing.Point location) : base(nickname, (location.Y + 1) * 100 + (location.X + 1))
        {
            this.location = location.Clone();
        }

        #endregion

        #region publicメソッド

        /// <summary>
        /// Inputをシミュレート
        /// </summary>
        /// <param name="index">対象のChannel</param>
        /// <param name="value">信号値</param>
        public void SimulateInput(int index, bool value) => this.SimulateInput(index, value ? 1 : 0);

        /// <summary>
        /// Inputをシミュレート
        /// </summary>
        /// <param name="index">対象のChannel</param>
        /// <param name="value">信号値</param>
        public void SimulateInput(int index, int value)
        {
            if (this.InputChannels.Length < index)
            {
                Serilog.Log.Warning($"Input Signal Count Exceeded");
                return;
            }

            lock (this.SyncInput)
            {
                this.InputChannels[index] = (value != 0) ? 1 : 0;
            }
        }

        /// <summary>
        /// Inputをシミュレート(連続ch)
        /// </summary>
        /// <param name="values">連続する信号値</param>
        public void SimulateInput(params bool[] values)
        {
            if (this.InputChannels.Length < values.Length)
            {
                Serilog.Log.Warning($"Input Signal Count Exceeded");
                return;
            }

            lock (this.SyncInput)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    this.InputChannels[i] = values[i] ? 1 : 0;
                }
            }
        }

        /// <summary>
        /// Inputをシミュレート(連続ch)
        /// </summary>
        /// <param name="values">連続する信号値</param>
        public void SimulateInput(params int[] values)
        {
            if (this.InputChannels.Length < values.Length)
            {
                Serilog.Log.Warning($"Input Signal Count Exceeded");
                return;
            }

            lock (this.SyncInput)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    this.InputChannels[i] = (values[i] != 0) ? 1 : 0;
                }
            }
        }

        #endregion
    }
}