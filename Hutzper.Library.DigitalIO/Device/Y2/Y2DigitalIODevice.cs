/* Y2 IO
    公式サンプルコード:
    https://www.y2c.co.jp/docs/libraries/y2-usbio/
    https://www.y2c.co.jp/docs/softwaremanual/ub/example/dio/cs/
*/
using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using YdciCs;


namespace Hutzper.Library.DigitalIO.Device.Y2
{

    [Serializable]
    public class Y2DigitalIODevice : ControllerBase, IDigitalIOInputDevice, IDigitalIOOutputDevice, IDigitalIODevice
    {
        #region IController
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
        public override void SetConfig(IApplicationConfig? config)
        {
            base.SetConfig(config);
        }
        public override void SetParameter(IControllerParameter? parameter)
        {
            base.SetParameter(parameter);
            if (parameter is IDigitalIODeviceParameter p)
            {
                this.Parameter = p;
                this.DeviceID = p.DeviceID;
            }
        }
        public override void Update()
        {
            base.Update();
        }
        #endregion

        #region IDigitalIODevice
        public string DeviceID { get; protected set; } = string.Empty;
        public Common.Drawing.Point Location { get => this.location.Clone(); set => this.location = value?.Clone() ?? this.location; }

        public bool Enabled => this.IsConnection;

        private bool IsConnection { get; set; } = false;
        ushort Y2Handle = 0;

        public event Action<object>? Disabled;

        public override bool Open()
        {
            try
            {
                if (this.Parameter is Y2DigitalIODeviceParameter mp)
                {
                    var param = mp;
                    var result = Ydci.Open(param.BoardNo, param.DeviceID, out Y2Handle);
                    if (result == Ydci.YDCI_RESULT_SUCCESS)
                    {
                        this.IsConnection = true;
                    }
                    else
                    {
                        throw new Exception("mistake \"Ydci.Open\"");
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
                this.Disabled?.Invoke(this);
            }
            return this.Enabled;
        }

        public override bool Close()
        {
            var isSuccess = false;
            try
            {
                if (this.IsConnection)
                {
                    isSuccess = Ydci.Close(this.Y2Handle);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
                this.Disabled?.Invoke(this);
            }
            finally
            {
                this.IsConnection = false;
            }
            return isSuccess;
        }
        #endregion
        #region IDigitalIOInputDevice

        public int NumberOfInputs => this.Parameter?.InputChannels.Length ?? 0;

        public virtual bool ReadInput(out bool[] values)
        {
            var isSuccess = this.ReadInput(out int[] intValues);
            values = Array.ConvertAll(intValues, v => v != 0);
            return isSuccess;
        }

        public virtual bool ReadInput(out int[] values)
        {
            var isSuccess = this.Enabled;
            values = new int[this.NumberOfInputs];
            try
            {
                byte[] outputData = new byte[this.NumberOfInputs];
                var result = Ydci.DioInput(this.Y2Handle, outputData, 0, (ushort)this.NumberOfInputs);
                if (result != Ydci.YDCI_RESULT_SUCCESS)
                {
                    throw new Exception("mistake  \"Ydci.DioOutput\"");
                }
                Array.Copy(outputData, values, this.NumberOfInputs);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
                this.Disabled?.Invoke(this);
            }
            return isSuccess;
        }

        public virtual bool ReadInput(int index, out bool value)
        {
            var isSuccess = this.ReadInput(index, out int intValue);
            value = intValue != 0;
            return isSuccess;
        }

        public virtual bool ReadInput(int index, out int value)
        {
            bool isSuccess;
            int[] values;
            isSuccess = this.ReadInput(out values);
            value = values[index];
            return isSuccess;
        }
        #endregion
        #region IDigitalIOutputDevice

        public int NumberOfOutputs => this.Parameter?.OutputChannels.Length ?? 0;

        public virtual bool WriteOutput(bool[] values)
        {
            var intValues = Array.ConvertAll(values, v => v ? 1 : 0);
            var isSuccess = this.WriteOutput(intValues);
            return isSuccess;
        }

        public virtual bool WriteOutput(int[] values)
        {
            var isSuccess = this.Enabled;
            try
            {
                var uintValues = Array.ConvertAll(values, v => (byte)v);
                var result = Ydci.DioOutput(this.Y2Handle, uintValues, 0, (ushort)this.NumberOfOutputs);
                if (result != Ydci.YDCI_RESULT_SUCCESS)
                {
                    throw new Exception("mistake  \"Ydci.DioOutput\"");
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
                this.Disabled?.Invoke(this);
            }
            return isSuccess;
        }

        public virtual bool WriteOutput(int index, bool value)
        {
            var isSuccess = this.Enabled;
            try
            {
                byte v = value ? (byte)1 : (byte)0;

                byte[] data = new byte[] { v };

                Ydci.DioOutput(this.Y2Handle, data, (ushort)index, 1);
                var result = Ydci.DioOutput(this.Y2Handle, data, (ushort)index, 1);
                if (result != Ydci.YDCI_RESULT_SUCCESS)
                {
                    throw new Exception("mistake \"Ydci.DioOutput\"");
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
                this.Disabled?.Invoke(this);
            }
            return isSuccess;
        }


        public virtual bool WriteOutput(int index, int value)
        {
            return WriteOutput(index, value != 0);
        }

        /*
            public static int DioOutputStatus(ushort wID, byte[] pbyData, ushort wStart, ushort wCount);
        */
        public virtual bool ReadOutput(out bool[] values)
        {
            var isSuccess = this.Enabled;

            byte[] data = new byte[this.NumberOfOutputs];
            try
            {
                var result = Ydci.DioOutputStatus(this.Y2Handle, data, 0, (ushort)this.NumberOfOutputs);
                if (result != Ydci.YDCI_RESULT_SUCCESS)
                {
                    throw new Exception("mistake \"Ydci.DioOutput\"");
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
                this.Disabled?.Invoke(this);
            }
            values = Array.ConvertAll(data, v => v != 0);
            return isSuccess;
        }

        public virtual bool ReadOutput(out int[] values)
        {
            bool[] data = new bool[this.NumberOfOutputs];
            var isSuccess = this.ReadOutput(out data);
            values = Array.ConvertAll(data, v => v ? 0 : 1);
            return isSuccess;
        }

        public virtual bool ReadOutput(int index, out bool value)
        {
            bool[] values;
            var isSuccess = this.ReadOutput(out values);
            value = values[index];
            return isSuccess;
        }

        public virtual bool ReadOutput(int index, out int value)
        {
            bool[] values;
            var isSuccess = this.ReadOutput(out values);
            value = values[index] ? 0 : 1;
            return isSuccess;
        }
        #endregion

        protected Common.Drawing.Point location;
        protected IDigitalIODeviceParameter? Parameter;

        public Y2DigitalIODevice() : this(typeof(Y2DigitalIODevice).Name, Common.Drawing.Point.New())
        {
        }

        public Y2DigitalIODevice(Common.Drawing.Point location) : this(typeof(Y2DigitalIODevice).Name, location)
        {
        }

        public Y2DigitalIODevice(string nickname, Common.Drawing.Point location) : base(nickname, (location.Y + 1) * 100 + (location.X + 1))
        {
            this.location = location.Clone();
        }
    }
}