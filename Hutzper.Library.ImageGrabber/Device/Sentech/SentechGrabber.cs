using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Common.Threading;
using Hutzper.Library.ImageGrabber.Data;
using Hutzper.Library.ImageProcessing;
using Sentech.GenApiDotNET;
using Sentech.StApiDotNET;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Hutzper.Library.ImageGrabber.Device.Sentech
{
    /// <summary>
    /// sentech
    /// </summary>
    [Serializable]
    public abstract class SentechGrabber : GrabberBase, ISentechGrabber
    {
        #region IGrabber

        /// <summary>
        /// 画像が取得可能な状態かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public override bool Enabled => this.Handles?.Enabled ?? false;

        /// <summary>
        /// トリガーモード
        /// </summary>
        public override TriggerMode TriggerMode
        {
            get
            {
                var value = TriggerMode.InternalTrigger;
                string result = string.Empty;
                this.GetValue("TriggerMode", out result);
                if ("On" == result)
                {
                    this.GetValue("TriggerSource", out result);
                    value = "Software" == result ? TriggerMode.SoftTrigger : TriggerMode.ExternalTrigger;
                }
                return value;
            }

            set
            {
                bool setParameter = true;
                if (TriggerMode.InternalTrigger == value) this.SetValue("TriggerMode", "Off", setParameter);
                else if (TriggerMode.SoftTrigger == value)
                {
                    this.SetValue("TriggerMode", "On", setParameter);
                    this.SetValue("TriggerSource", TriggerMode.SoftTrigger == value ? "Software" : "Line0", setParameter);
                }
            }
        }

        /// <summary>
        /// 露光時間μ秒
        /// </summary>
        public override double ExposureTimeMicroseconds
        {
            get => this.GetValue("ExposureTime", out double value) ? value : 0d;

            set
            {
                double now = 0, min = 0, max = 0;
                this.GetValue("ExposureTime", out now, out min, out max);
                if (0 >= value && 0 < max) this.SetValue("ExposureTime", (int)max);
                else if (0 < max) this.SetValue("ExposureTime", System.Math.Clamp((int)value, min, max));
            }
        }

        /// <summary>
        /// アナログゲイン
        /// </summary>
        public override double AnalogGain
        {
            get
            {
                this.SetValue("GainSelector", "AnalogAll");
                return this.GetValue("Gain", out double value) ? value : 0d;
            }

            set
            {
                this.SetValue("GainSelector", "AnalogAll");
                this.SetValue("Gain", value);
            }
        }

        /// <summary>
        /// デジタルゲイン
        /// </summary>
        public override double DigitalGain
        {
            get
            {
                this.SetValue("GainSelector", "DigitalAll");
                return this.GetValue("Gain", out double value) ? value : 0d;
            }

            set
            {
                this.SetValue("GainSelector", "DigitalAll");
                this.SetValue("Gain", value);
            }
        }

        /// <summary>
        /// RGBカラーかどうか
        /// </summary>
        public override bool IsRgbColor
        {
            get
            {
                string strValue = string.Empty;
                this.GetValue("PixelFormat", out strValue);
                return false == string.IsNullOrEmpty(strValue) && false == strValue.ToLower().Contains("mono");
            }
        }

        /// <summary>
        /// 画像幅
        /// </summary>
        public override int Width
        {
            get => this.GetValue("Width", out int value) ? value : 0;

            set
            {
                int min = 0, max = 0;
                this.GetValue("Width", out int _, out min, out max);
                if (0 >= value && 0 < max) this.SetValue("Width", max);
                else if (0 < max)
                {
                    var unitValue = (value / 16) * 16;
                    if (unitValue != value) Serilog.Log.Warning($"Width is replaced {value} -> {unitValue}");
                    this.SetValue("Width", System.Math.Clamp(unitValue, min, max));
                }
            }
        }

        /// <summary>
        /// 画像高さ
        /// </summary>
        public override int Height
        {
            get => this.GetValue("Height", out int value) ? value : 0;

            set
            {
                int min = 0, max = 0;
                this.GetValue("Height", out int _, out min, out max);
                if (0 >= value && 0 < max) this.SetValue("Height", max);
                else if (0 < max)
                {
                    var unitValue = (value / 4) * 4;
                    if (unitValue != value) Serilog.Log.Warning($"Height is replaced {value} -> {unitValue}");
                    this.SetValue("Height", System.Math.Clamp(unitValue, min, max));
                }
            }
        }

        /// <summary>
        /// Xオフセット
        /// </summary>
        public override int OffsetX
        {
            get => this.GetValue("OffsetX", out int value) ? value : 0;

            set
            {
                int min = 0, max = 0;
                this.GetValue("OffsetX", out int _, out min, out max);
                if (0 >= value) this.SetValue("OffsetX", min);
                else if (0 < max)
                {
                    var unitValue = (value / 4) * 4;
                    if (unitValue != value) Serilog.Log.Warning($"OffsetX is replaced {value} -> {unitValue}");
                    this.SetValue("OffsetX", System.Math.Clamp(unitValue, min, max));
                }
            }
        }

        /// <summary>
        /// Yオフセット
        /// </summary>
        public override int OffsetY
        {
            get => this.GetValue("OffsetY", out int value) ? value : 0;

            set
            {
                int min = 0, max = 0;
                this.GetValue("OffsetY", out int _, out min, out max);
                if (0 >= value) this.SetValue("OffsetY", min);
                else if (0 < max)
                {
                    int unitValue = (value / 4) * 4;
                    if (unitValue != value) Serilog.Log.Warning($"OffsetY is replaced {value} -> {unitValue}");
                    this.SetValue("OffsetY", System.Math.Clamp(unitValue, min, max));
                }
            }
        }

        /// <summary>
        /// カメラ温度
        /// </summary>
        public override Dictionary<string, double> DeviceTemperature
        {
            get
            {
                var result = new Dictionary<string, double>();

                var option = new string[] { "Mainboard" };
                foreach (var location in option)
                {
                    try
                    {
                        this.SetValue("DeviceTemperatureSelector", location);
                        if (true == this.GetValue("DeviceTemperature", out double value))
                        {
                            result.Add(location, value);
                        }
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                }

                return result;
            }
        }
        public int UseUserSetIndex = 0;
        /// <summary>
        /// 画像取得モード
        /// </summary>
        public override AcquisitionMode AcquisitionMode
        {
            get
            {
                var result = string.Empty;
                this.GetValue("AcquisitionMode", out result);

                return result switch
                {
                    "SingleFrame" => AcquisitionMode.SingleFrame,
                    "MultiFrame" => AcquisitionMode.MultiFrame,
                    _ => AcquisitionMode.Continuous,
                };
            }

            set
            {
                switch (value)
                {
                    case AcquisitionMode.SingleFrame:
                        this.SetValue("AcquisitionMode", "SingleFrame");
                        break;
                    case AcquisitionMode.MultiFrame:
                        this.SetValue("AcquisitionMode", "MultiFrame");
                        break;
                    default:
                        this.SetValue("AcquisitionMode", "Continuous");
                        break;
                };
            }
        }

        /// <summary>
        /// 画像取得フレーム数
        /// </summary>
        public override int AcquisitionFrameCount
        {
            get
            {
                var result = this.GetValue("AcquisitionFrameCount", out Int64 value) ? value : 0;

                return (int)value;
            }

            set
            {
                int now = 0, min = 0, max = 0;
                this.GetValue("AcquisitionFrameCount", out now, out min, out max);
                if (0 >= value && 0 < max) this.SetValue("AcquisitionFrameCount", max);
                else if (0 < max) this.SetValue("AcquisitionFrameCount", System.Math.Clamp(value, min, max));
            }
        }

        /// <summary>
        /// トリガータイプ
        /// </summary>
        public override TriggerSelector TriggerSelector
        {
            get
            {
                var result = string.Empty;
                this.GetValue("TriggerSelector", out result);

                return result switch
                {
                    "FrameStart" => TriggerSelector.FrameStart,
                    "AcquisitionStart" => TriggerSelector.AcquisitionStart,
                    "LineStart" => TriggerSelector.LineStart,
                    _ => TriggerSelector.Unsupported,
                };
            }

            set
            {
                switch (value)
                {
                    case TriggerSelector.FrameStart:
                        this.SetValue("TriggerSelector", "FrameStart");
                        break;
                    case TriggerSelector.AcquisitionStart:
                        this.SetValue("TriggerSelector", "AcquisitionStart");
                        break;
                    case TriggerSelector.LineStart:
                        this.SetValue("TriggerSelector", "LineStart");
                        break;
                    default:
                        Serilog.Log.Warning($"{this}, {value.StringValueOf()} is not supported.");
                        break;
                };
            }
        }

        public bool GetValue<T>(string parameterName, out T val)
        {
            val = default!;
            return this.Handles.NodeAccessor?.GetValue(parameterName, out val) ?? false;
        }
        public bool GetValue<T>(string parameterName, out T val, out T min, out T max)
        {
            val = default!;
            min = default!;
            max = default!;
            return this.Handles.NodeAccessor?.GetNodeValues(parameterName, out val, out min, out max) ?? false;
        }

        public void SetValue(string parameterName, object value, bool setParameter = false)
        {
            if (this.UseSetParameter || setParameter) this.Handles.NodeAccessor?.SetValue(parameterName, value);
        }


        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            try
            {
                if (this.Parameter is IGrabberParameter gp)
                {
                    if (null != this.Handles.Device && false == this.Handles.Device.IsDeviceLost)
                    {
                        this.DisposeSafely(this.Handles.Device);
                    }
                    this.UseUserSetIndex = gp.UseUserSetIndex;
                    if (true == string.IsNullOrEmpty(gp.DeviceID))
                    {
                        this.Handles.Device = this.Handles.System.CreateFirstStDevice();
                    }
                    else
                    {
                        foreach (var u in Enumerable.Range(0, (int)this.Handles.System.InterfaceCount))
                        {
                            var unit = this.Handles.System.GetIStInterface((uint)u);
                            {
                                foreach (var d in Enumerable.Range(0, (int)unit.DeviceCount))
                                {
                                    var info = unit.GetIStDeviceInfo((uint)d);

                                    if (info.SerialNumber == gp.DeviceID || info.ID == gp.DeviceID)
                                    {
                                        this.Handles.Device = unit.CreateStDevice((uint)d);
                                        break;
                                    }
                                }
                            }

                            if (this.Handles.Device is not null)
                            {
                                break;
                            }
                        }
                    }

                    this.Handles.DataStream = null;
                    this.Handles.NodeAccessor = null;

                    if (null != this.Handles.Device)
                    {
                        this.Handles.DataStream = this.Handles.Device.CreateStDataStream();
                        this.Handles.NodeAccessor = new(this.Handles.Device.GetRemoteIStPort().GetINodeMap(), this.Handles.Device.GetLocalIStPort().GetINodeMap());

                        this.Handles.NodeAccessor.RegisterCallbackMethodDeviceLost(this.Handles.Device, this.DeviceLostCallback);

                        this.Handles.DataStream.RegisterCallbackMethod(this.DataStreamNewBufferCallback);

                        this.LoadUserSet(this.UseUserSetIndex);
                        this.Handles.NodeAccessor.SetValue("ExposureMode", "Timed");
                        this.TriggerMode = gp.TriggerMode;
                        #region パラメータ反映
                        this.ExposureTimeMicroseconds = gp.ExposureTimeMicroseconds;
                        this.AnalogGain = gp.AnalogGain;
                        this.DigitalGain = gp.DigitalGain;

                        this.OffsetX = 0;
                        this.OffsetY = 0;

                        this.Width = gp.Width;
                        this.Height = gp.Height;

                        this.OffsetX = gp.OffsetX;
                        this.OffsetY = gp.OffsetY;
                        #endregion

                        if (true == this.IsRgbColor)
                        {
                            this.BitmapConvertor = this.ToBitmapOfRgb24;
                            this.BitmapConvention = eStPixelFormatNamingConvention.BGR8;
                        }
                        else
                        {
                            this.BitmapConvertor = this.ToBitmapOfMono8;
                            this.BitmapConvention = eStPixelFormatNamingConvention.Mono8;
                        }

                        this.Handles.Device.StartEventAcquisitionThread();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Close();
                Serilog.Log.Warning(ex, ex.Message);
                this.Handles.Device = null;
            }

            return this.Enabled;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            try
            {
                if (this.Handles is not null)
                {
                    this.StopGrabbing();
                    this.Handles.NodeAccessor?.DeregisterCallbackMethodDeviceLost();
                    this.Handles.DataStream?.DeregisterCallbackMethods();

                    if (this.Handles.Device is not null && this.Handles.Device?.IsDeviceLost == false)
                    {
                        var stopwatch = Stopwatch.StartNew();
                        while (true == (this.Handles.DataStream?.IsGrabbing ?? false))
                        {
                            Thread.Sleep(100);

                            if (stopwatch.ElapsedMilliseconds > 3000)
                            {
                                Serilog.Log.Warning($"{this.ToString()},Close timeout");
                                break;
                            }
                        }
                        this.Handles.Device.StopEventAcquisitionThread();
                        this.DisposeSafely(this.Handles.DataStream);

                        var dev = this.Handles.Device;
                        this.Handles.Device = null;
                        this.DisposeSafely(dev);
                    }

                    this.Handles.DataStream = null;
                    this.Handles.NodeAccessor = null;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return !this.Enabled;
        }

        /// <summary>
        /// 画像取得
        /// </summary>
        /// <returns></returns>
        public override bool Grab()
        {
            var isSuccess = this.Enabled;

            try
            {
                if (true == isSuccess)
                {
                    if (null != this.ControlInfo)
                    {
                        this.ControlInfo.IsContinuable = false;
                    }

                    this.ControlInfo = new AcquisitionControlInfo(1);
                    this.AcquisitionThread.Enqueue(this.ControlInfo);
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
        /// 連続画像取得
        /// </summary>
        /// <param name="isBegin"></param>
        /// <returns></returns>
        public override bool GrabContinuously(int number = -1)
        {
            var isSuccess = this.Enabled;

            try
            {
                if (true == isSuccess)
                {
                    if (null != this.ControlInfo)
                    {
                        this.ControlInfo.IsContinuable = false;
                        this.ControlInfo = null;
                    }

                    this.ControlInfo = new AcquisitionControlInfo(number);
                    this.AcquisitionThread.Enqueue(this.ControlInfo);
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
        /// 連続撮影停止
        /// </summary>
        /// <returns></returns>
        public override bool StopGrabbing()
        {
            var isSuccess = this.Enabled;

            try
            {
                if (true == isSuccess)
                {
                    if (null != this.ControlInfo)
                    {
                        this.ControlInfo.IsContinuable = false;
                        this.ControlInfo = null;
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
        /// ソフトトリガー
        /// </summary>
        /// <returns></returns>
        public override bool DoSoftTrigger()
        {
            var isSuccess = false;

            try
            {
                var commandNode = this.Handles.NodeAccessor?.GetCommand("TriggerSoftware");

                if (null != commandNode)
                {
                    commandNode.Execute();
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// AW実行
        /// </summary>
        /// <returns></returns>
        public override bool DoAutoWhiteBalancing()
        {
            var isSuccess = false;

            try
            {
                this.SetValue("BalanceWhiteAuto", "Once");
                this.SetValue("BalanceRatioSelector", "BalanceRatio");
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// Balance Ratio（RGBゲイン値）取得
        /// </summary>
        /// <returns></returns>
        public override bool GetBalanceRatio(out double gainRedValue, out double gainGreenValue, out double gainBlueValue)
        {
            bool isSuccess = false;
            gainRedValue = gainGreenValue = gainBlueValue = 0;

            if (this.Handles.Device is not null && this.Handles.NodeAccessor is not null)
            {
                this.SetValue("BalanceRatioSelector", "Red");
                isSuccess &= this.GetValue("BalanceRatio", out gainRedValue);
                this.SetValue("BalanceRatioSelector", "Green");
                isSuccess &= this.GetValue("BalanceRatio", out gainGreenValue);
                this.SetValue("BalanceRatioSelector", "Blue");
                isSuccess &= this.GetValue("BalanceRatio", out gainBlueValue);
            }

            return isSuccess;
        }

        /// <summary>
        /// ユーザーセットデフォルトロード
        /// </summary>
        /// <returns></returns>
        public override bool LoadUserSetDefault()
        {
            bool isSuccess = this.Enabled && (this.ControlInfo?.IsContinuable ?? false);

            try
            {
                if (true == isSuccess)
                {
                    this.SetValue("UserSetSelector", "UserSet0");
                    var commandNode = this.Handles.NodeAccessor?.GetCommand("UserSetLoad");

                    if (null != commandNode)
                    {
                        commandNode.Execute();
                        isSuccess = true;
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
        /// ユーザーセットトロード
        /// </summary>
        /// <returns></returns>
        public override bool LoadUserSet(int index = 0)
        {
            bool isSuccess = this.Enabled && !(this.ControlInfo?.IsContinuable ?? false);

            try
            {
                if (true == isSuccess && 0 <= index && 7 > index)
                {
                    this.SetValue("UserSetSelector", $"UserSet{index}", true);

                    var commandNode = this.Handles.NodeAccessor?.GetCommand("UserSetLoad");

                    if (null != commandNode)
                    {
                        commandNode.Execute();
                        isSuccess = true;
                    }
                }
                else
                {
                    isSuccess = false;
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
        /// ユーザーセットトセーブ
        /// </summary>
        /// <returns></returns>
        public override bool SaveUserSet(int index = 0)
        {
            bool isSuccess = this.Enabled && !(this.ControlInfo?.IsContinuable ?? false);

            try
            {
                if (true == isSuccess && 0 <= index && 3 > index)
                {
                    this.SetValue("UserSetSelector", $"UserSet{index}");

                    var commandNode = this.Handles.NodeAccessor?.GetCommand("UserSetSave");

                    if (null != commandNode)
                    {
                        commandNode.Execute();
                        isSuccess = true;
                    }
                }
                else
                {
                    isSuccess = false;
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
        /// double型パラメータ値を取得する
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override void GetValues(out Dictionary<ParametersKey, IClampedValue<double>> values, params ParametersKey[] keys)
        {
            values = new Dictionary<ParametersKey, IClampedValue<double>>();

            try
            {
                base.GetValues(out values, keys);

                if (this.Handles.NodeAccessor is not null)
                {
                    foreach (var key in keys)
                    {
                        if (false == values.ContainsKey(key))
                        {
                            var value = (ClampedValue<double>?)null;

                            switch (key)
                            {
                                case ParametersKey.ExposureTime:
                                    {
                                        this.GetValue("ExposureTime", out double now, out double min, out double max);

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;

                                case ParametersKey.AnalogGain:
                                    {
                                        this.SetValue("GainSelector", "AnalogAll");
                                        this.GetValue("Gain", out double now, out double min, out double max);

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;

                                case ParametersKey.DigitalGain:
                                    {
                                        this.SetValue("GainSelector", "DigitalAll");
                                        this.GetValue("Gain", out double now, out double min, out double max);

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;

                                case ParametersKey.FrameRate:
                                    {
                                        this.GetValue("AcquisitionFrameRate", out double now, out double min, out double max);

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;

                                case ParametersKey.OffsetX:
                                    {
                                        this.GetValue("OffsetX", out int now, out int min, out int max);

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;
                                case ParametersKey.OffsetY:
                                    {
                                        this.GetValue("OffsetY", out int now, out int min, out int max);

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;
                                case ParametersKey.Width:
                                    {
                                        this.GetValue("Width", out int now, out int min, out int max);

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;
                                case ParametersKey.Height:
                                    {
                                        this.GetValue("Height", out int now, out int min, out int max);

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;
                                case ParametersKey.LineRate:
                                    {
                                        this.GetValue("AcquisitionLineRate", out double now, out double min, out double max);

                                        value = new ClampedValue<double>(now, min, max);
                                    }
                                    break;
                            }

                            if (value is not null)
                            {
                                values.Add(key, value);
                            }
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
        /// int型パラメータ値を取得する
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override void GetValues(out Dictionary<ParametersKey, IClampedValue<int>> values, params ParametersKey[] keys)
        {
            values = new Dictionary<ParametersKey, IClampedValue<int>>();

            try
            {
                base.GetValues(out values, keys);

                if (this.Handles.NodeAccessor is not null)
                {
                    foreach (var key in keys)
                    {
                        if (false == values.ContainsKey(key))
                        {
                            var value = (ClampedValue<int>?)null;

                            switch (key)
                            {
                                case ParametersKey.ExposureTime:
                                case ParametersKey.AnalogGain:
                                case ParametersKey.DigitalGain:
                                case ParametersKey.FrameRate:
                                    break;

                                case ParametersKey.OffsetX:
                                    {
                                        this.GetValue("OffsetX", out int now, out int min, out int max);

                                        value = new ClampedValue<int>(now, min, max);
                                    }
                                    break;
                                case ParametersKey.OffsetY:
                                    {
                                        this.GetValue("OffsetY", out int now, out int min, out int max);

                                        value = new ClampedValue<int>(now, min, max);
                                    }
                                    break;
                                case ParametersKey.Width:
                                    {
                                        this.GetValue("Width", out int now, out int min, out int max);

                                        value = new ClampedValue<int>(now, min, max);
                                    }
                                    break;
                                case ParametersKey.Height:
                                    {
                                        this.GetValue("Height", out int now, out int min, out int max);

                                        value = new ClampedValue<int>(now, min, max);
                                    }
                                    break;
                            }

                            if (value is not null)
                            {
                                values.Add(key, value);
                            }
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
        /// 露光時間をセットする
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override bool SetExposureTimeValue(double value)
        {
            var isSuccess = false;

            try
            {
                if (this.Handles.NodeAccessor is not null)
                {
                    this.SetValue("ExposureMode", "Timed", true);
                    this.SetValue("ExposureTime", value, true);

                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// アナログゲインをセットする
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override bool SetAnalogGainValue(double value)
        {
            var isSuccess = false;

            try
            {
                if (this.Handles.NodeAccessor is not null)
                {
                    this.SetValue("GainSelector", "AnalogAll");
                    this.SetValue("Gain", value);

                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// デジタルゲインをセットする
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override bool SetDigitalGainValue(double value)
        {
            var isSuccess = false;

            try
            {
                if (this.Handles.NodeAccessor is not null)
                {
                    this.SetValue("DigitalGain", value);

                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// ラインレートをセットする
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override bool SetLineRateValue(double value)
        {
            var isSuccess = false;

            try
            {
                if (this.Handles.NodeAccessor is not null)
                {
                    this.SetValue("AcquisitionLineRate", value);

                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// フレームレートセット
        /// </summary>
        /// <returns></returns>
        public override bool SetFrameRateValue(double value)
        {
            var isSuccess = false;

            try
            {
                if (this.Handles.NodeAccessor is not null)
                {
                    this.SetValue("AcquisitionFrameRate", value);

                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        #endregion

        #region IController

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="serviceCollection"></param>
        public override void Initialize(IServiceCollectionSharing? serviceCollection)
        {
            base.Initialize(serviceCollection);
            if (true == this.Enabled)
            {
                this.LoadUserSet(this.UseUserSetIndex);
                if (this.Parameter is IGrabberParameter gp)
                {
                    this.SetValue("ExposureMode", "Timed", true);
                    this.TriggerMode = gp.TriggerMode;

                    this.ExposureTimeMicroseconds = gp.ExposureTimeMicroseconds;
                    this.AnalogGain = gp.AnalogGain;
                    this.DigitalGain = gp.DigitalGain;

                    this.TriggerMode = gp.TriggerMode;

                    this.OffsetX = 0;
                    this.OffsetY = 0;

                    this.Width = gp.Width;
                    this.Height = gp.Height;

                    this.OffsetX = gp.OffsetX;
                    this.OffsetY = gp.OffsetY;
                }
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
            base.SetParameter(parameter);
            if (this.Parameter is IGrabberParameter gp)
            {
                this.SetValue("ExposureMode", "Timed", true);
                this.TriggerMode = gp.TriggerMode;

                this.ExposureTimeMicroseconds = gp.ExposureTimeMicroseconds;
                this.AnalogGain = gp.AnalogGain;
                this.DigitalGain = gp.DigitalGain;

                this.OffsetX = 0;
                this.OffsetY = 0;

                this.Width = gp.Width;
                this.Height = gp.Height;

                this.OffsetX = gp.OffsetX;
                this.OffsetY = gp.OffsetY;
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

        #region SafelyDisposable

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
                this.Close();
                this.DisposeSafely(this.AcquisitionThread);
                this.DisposeSafely(this.Handles);
                this.DisposeSafely(this.PixelFormatConverter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        /// <summary>
        /// API
        /// </summary>
        protected class CStHandles : SafelyDisposable
        {
            public bool Enabled => null != this.Device && null != this.DataStream;

            public CStSystem System { get; init; }

            public CStDevice? Device { get; set; }

            public CStDataStream? DataStream { get; set; }

            public NodeAccessor? NodeAccessor { get; set; }

            #region SafelyDisposable

            /// <summary>
            /// リソースの解放
            /// </summary>
            protected override void DisposeExplicit()
            {
                try
                {
                    this.DisposeSafely(this.Device);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    this.DataStream = null;
                    this.Device = null;
                    this.NodeAccessor = null;
                }
            }

            #endregion

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public CStHandles(CStSystem commonSystem)
            {
                this.System = commonSystem;
            }
        }

        /// <summary>
        /// ノードアクセサ
        /// </summary>
        protected class NodeAccessor
        {
            protected readonly INodeMap RemoteNodeMap;
            protected readonly INodeMap LocalNodeMap;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="remoteNodeMap"></param>
            /// <param name="localNodeMap"></param>
            /// <param name="this.Logger"></param>
            public NodeAccessor(INodeMap remoteNodeMap, INodeMap localNodeMap)
            {
                this.RemoteNodeMap = remoteNodeMap;
                this.LocalNodeMap = localNodeMap;
            }

            /// <summary>
            /// デバイスロストイベント登録
            /// </summary>
            /// <param name="device"></param>
            /// <param name="nodeEventHandler"></param>
            public void RegisterCallbackMethodDeviceLost(IStDevice device, NodeEventHandler nodeEventHandler)
            {
                this.RegisterCallbackMethod(device, nodeEventHandler, "EventDeviceLost", "DeviceLost");
            }

            /// <summary>
            /// イベント登録
            /// </summary>
            /// <param name="device"></param>
            /// <param name="nodeEventHandler"></param>
            /// <param name="callbackNodeName"></param>
            /// <param name="targetEventName"></param>
            /// <param name="isOn"></param>
            /// <exception cref="AccessException"></exception>
            public void RegisterCallbackMethod(IStDevice device, NodeEventHandler nodeEventHandler, string callbackNodeName, string targetEventName, bool isOn = true)
            {
                try
                {
                    // Get the INode interface for the EventDeviceLost node.
                    var nodeCallback = this.LocalNodeMap.GetNode<INode>(callbackNodeName);
                    if (nodeCallback == null)
                    {
                        throw new AccessException(callbackNodeName + " node does not exist.");
                    }
                    else
                    {
                        // Register a callback method.
                        object[] param = { device };
                        nodeCallback.RegisterCallbackMethod(nodeEventHandler, param, eCallbackType.PostOutsideLock);

                        // Enabling the transmission of the target event.
                        IEnum eventSelector = this.LocalNodeMap.GetNode<IEnum>("EventSelector");
                        eventSelector.StringValue = targetEventName;

                        IEnum eventNotification = this.LocalNodeMap.GetNode<IEnum>("EventNotification");
                        eventNotification.StringValue = isOn ? "On" : "Off";
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }

            /// <summary>
            /// デバイスロストイベント解除
            /// </summary>
            public void DeregisterCallbackMethodDeviceLost()
            {
                this.DeregisterCallbackMethod("EventDeviceLost");
            }

            /// <summary>
            /// イベント解除
            /// </summary>
            /// <param name="callbackNodeName"></param>
            /// <exception cref="AccessException"></exception>
            public void DeregisterCallbackMethod(string callbackNodeName)
            {
                try
                {
                    // Get the INode interface for the EventDeviceLost node.
                    var nodeCallback = this.LocalNodeMap.GetNode<INode>(callbackNodeName);
                    if (nodeCallback == null)
                    {
                        throw new AccessException(callbackNodeName + " node does not exist.");
                    }
                    else
                    {
                        // Deregister a callback method.
                        nodeCallback.DeregisterCallbackMethods();
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }

            /// <summary>
            /// ICommandノード取得
            /// </summary>
            /// <param name="nodeName"></param>
            /// <returns></returns>
            public ICommand? GetCommand(string nodeName)
            {
                ICommand? commandNode = null;

                try
                {
                    commandNode = this.RemoteNodeMap.GetNode<ICommand>(nodeName);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }

                return commandNode;
            }

            public bool GetValue<T>(string nodeName, out T value)
            {
                bool isSuccess = true;
                value = default!;
                try
                {
                    if (this.RemoteNodeMap[nodeName] is IBool boolNode && boolNode.IsReadable) value = (T)(object)boolNode.Value!;
                    else if (this.RemoteNodeMap[nodeName] is IFloat floatNode && floatNode.IsReadable) value = (T)(object)floatNode.Value!;
                    else if (this.RemoteNodeMap[nodeName] is IInteger intNode && intNode.IsReadable) value = (T)(object)intNode.Value!;
                    else if (this.RemoteNodeMap[nodeName] is IEnum enumNode && enumNode.IsReadable && enumNode.IsAvailable)
                    {
                        foreach (var e in enumNode.Entries.Where(e => e.IsAvailable))
                        {
                            if (enumNode.IntValue == e.Value)
                            {
                                value = (T)(object)enumNode.StringValue;
                                isSuccess = !string.IsNullOrEmpty(enumNode.StringValue);
                                break;
                            }
                        }
                    }
                    else isSuccess = true;
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    Serilog.Log.Warning(ex, ex.Message);
                }
                return isSuccess;
            }

            /// <summary>
            /// ノード値の設定
            /// </summary>
            /// <param name="nodeName"></param>
            /// <param name="value"></param>
            public void SetValue(string nodeName, object value)
            {
                try
                {
                    if (this.RemoteNodeMap[nodeName] is IBool boolNode && boolNode.IsWritable) boolNode.Value = (bool)value;
                    else if (this.RemoteNodeMap[nodeName] is IFloat floatNode && floatNode.IsWritable) floatNode.Value = Convert.ToInt64(value);
                    else if (this.RemoteNodeMap[nodeName] is IInteger intNode && intNode.IsWritable && (intNode.Minimum <= (int)value && (int)value <= intNode.Maximum)) intNode.Value = Convert.ToInt64(value);
                    else if (value is string stringValue)
                    {
                        IEnum enumNode = this.RemoteNodeMap.GetNode<IEnum>(nodeName);
                        if (!(enumNode.IsAvailable && enumNode.IsWritable)) return;
                        foreach (var e in enumNode.Entries.Where(e => e.IsAvailable))
                        {
                            if (e.Symbolic == stringValue)
                            {
                                enumNode.FromString((string)e);
                                break;
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
            /// ノード値取得
            /// </summary>
            /// <param name="nodeName"></param>
            /// <param name="node"></param>
            public bool GetNodeValues<T>(string nodeName, out T value, out T minimum, out T maximum)
            {
                bool isSuccess = true;
                value = minimum = maximum = default!;
                try
                {
                    if (this.RemoteNodeMap[nodeName] is IInteger intNode && intNode.IsWritable)
                    {
                        value = (T)(object)Convert.ToInt32(intNode.Value);
                        minimum = (T)(object)Convert.ToInt32(intNode.Minimum);
                        maximum = (T)(object)Convert.ToInt32(intNode.Maximum);
                    }
                    else if (this.RemoteNodeMap[nodeName] is IFloat floatNode && floatNode.IsWritable)
                    {
                        value = (T)(object)floatNode.Value;
                        minimum = (T)(object)floatNode.Minimum;
                        maximum = (T)(object)floatNode.Maximum;
                    }
                    else isSuccess = false;
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    Serilog.Log.Warning(ex, ex.Message);
                }
                return isSuccess;
            }
        }

        /// <summary>
        /// 画像取得制御情報
        /// </summary>
        protected class AcquisitionControlInfo : SafelyDisposable
        {
            #region プロパティ

            /// <summary>
            /// 画像取得必要枚数
            /// </summary>
            public int NumberOfImage { get; init; }

            /// <summary>
            /// 画像取得を続行可能かどうか
            /// </summary>
            public bool IsContinuable
            {
                get => isContinuable;
                set
                {
                    isContinuable = value;
                    if (false == isContinuable)
                    {
                        if (0 < this.WaitHandles.Length && this.WaitHandles[0] is ManualResetEvent e)
                        {
                            e.Set();
                        }
                    }
                }
            }

            #endregion

            #region フィールド

            public WaitHandle[] WaitHandles = new WaitHandle[2];
            protected bool isContinuable = true;
            protected ConcurrentQueue<ImageContainer> bufferQueue = new();

            #endregion

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="numberOfImage"></param>
            public AcquisitionControlInfo(int numberOfImage)
            {
                this.NumberOfImage = numberOfImage;
                this.WaitHandles[0] = new ManualResetEvent(false);
                this.WaitHandles[1] = new AutoResetEvent(false);
            }

            #region SafelyDisposable

            protected override void DisposeExplicit()
            {
                try
                {
                    foreach (var e in this.WaitHandles)
                    {
                        e.Close();
                    }

                    var disposeList = new List<ImageContainer>();
                    while (bufferQueue.TryDequeue(out ImageContainer? item))
                    {
                        disposeList.Add(item);
                    }

                    disposeList.ForEach(item => this.DisposeSafely(item));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    this.WaitHandles = Array.Empty<WaitHandle>();
                }
            }

            #endregion

            #region メソッド

            /// <summary>
            /// 画像取得を通委t
            /// </summary>
            /// <param name="imageData"></param>
            /// <param name="imageWidth"></param>
            /// <param name="imageHeight"></param>
            public void OnIDataStreamNewBuffer(ImageContainer stImage)
            {
                try
                {
                    if (this.WaitHandles[1] is AutoResetEvent e)
                    {
                        this.bufferQueue.Enqueue(stImage);

                        e.Set();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            /// <summary>
            ///  画像取得を待機
            /// </summary>
            /// <returns></returns>
            public List<ImageContainer> GetDataStreamNewBuffer()
            {
                var images = new List<ImageContainer>();

                try
                {
                    WaitHandle.WaitAny(WaitHandles);

                    while (bufferQueue.TryDequeue(out var item))
                    {
                        images.Add(item); // データを取得
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                return images;
            }

            #endregion
        }

        #region staticフィールド

        protected static CStApiAutoInit StApiAutoInit;
        protected static CStSystem StSystem;

        #endregion

        #region フィールド

        /// <summary>
        /// ハンドル
        /// </summary>
        protected SentechGrabber.CStHandles Handles = new(SentechGrabber.StSystem);

        /// <summary>
        /// 画像取得スレッド
        /// </summary>
        protected QueueThread<AcquisitionControlInfo> AcquisitionThread;

        /// <summary>
        /// 画像取得制御情報
        /// </summary>
        protected AcquisitionControlInfo? ControlInfo;

        /// <summary>
        /// 画像フォーマット変換
        /// </summary>
        protected CStPixelFormatConverter PixelFormatConverter;

        /// <summary>
        /// Bitmap変換
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        protected delegate System.Drawing.Bitmap? ToBitmapDelegate(ImageContainer data, out byte[] bytes, out int stride);
        protected ToBitmapDelegate BitmapConvertor;
        protected eStPixelFormatNamingConvention BitmapConvention = eStPixelFormatNamingConvention.Mono8;

        #endregion

        #region staticコンストラクタ

        /// <summary>
        /// staticコンストラクタ
        /// </summary>
        static SentechGrabber()
        {
            SentechGrabber.StApiAutoInit = new();
            SentechGrabber.StSystem = new();
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SentechGrabber() : this(typeof(SentechGrabber).Name, Common.Drawing.Point.New())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SentechGrabber(Common.Drawing.Point location) : this(typeof(SentechGrabber).Name, location)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="location"></param>
        public SentechGrabber(string nickname, Common.Drawing.Point location) : base(nickname, location)
        {
            this.AcquisitionThread = new()
            {
                Priority = ThreadPriority.Highest
            };
            this.AcquisitionThread.Dequeue += this.AcquisitionThread_Dequeue;

            this.BitmapConvertor = this.ToBitmapOfMono8;
            this.BitmapConvention = eStPixelFormatNamingConvention.Mono8;
            this.PixelFormatConverter = new();
        }

        #endregion

        /// <summary>
        /// デバイスロストコールバック
        /// </summary>
        /// <param name="node"></param>
        /// <param name="param"></param>
        protected virtual void DeviceLostCallback(INode node, object[] param)
        {
            try
            {
                if (this.Handles.Device is not null)
                {
                    Serilog.Log.Error($"device lost (id = {this.DeviceID})");
                    this.Close();
                    this.OnDisabled();
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 画像取得コールバック
        /// </summary>
        /// <param name="paramBase"></param>
        /// <param name="param"></param>
        protected virtual void DataStreamNewBufferCallback(IStCallbackParamBase paramBase, object[] param)
        {
            try
            {
                if (paramBase.CallbackType == eStCallbackType.TL_DataStreamNewBuffer && null != this.Handles.DataStream)
                {
                    // In case of receiving a NewBuffer events:
                    // Convert received callback parameter into IStCallbackParamGenTLEventNewBuffer for acquiring additional information.
                    if (paramBase is IStCallbackParamGenTLEventNewBuffer callbackParam)
                    {
                        // Get the IStDataStream interface object from the received callback parameter.                    
                        var dataStream = callbackParam.GetIStDataStream();

                        // Retrieve the buffer of image data with a timeout of 5000ms.
                        // Use the 'using' statement for automatically managing the buffer re-queue action when it's no longer needed.
                        using var streamBuffer = dataStream.RetrieveBuffer(0);

                        // Check if the acquired data contains image data.
                        if (streamBuffer.GetIStStreamBufferInfo().IsImagePresent)
                        {
                            var stImage = streamBuffer.GetIStImage();
                            var imageBuffer = CStApiDotNet.CreateStImageBuffer();

                            this.PixelFormatConverter.DestinationPixelFormat = this.BitmapConvention;
                            this.PixelFormatConverter.Convert(stImage, imageBuffer);
                            this.ControlInfo?.OnIDataStreamNewBuffer(new ImageContainer(imageBuffer, new Size((int)stImage.ImageWidth, (int)stImage.ImageHeight)));
                        }
                        else
                        {
                            // If the acquired data contains no image data.
                            Serilog.Log.Warning("Image data does not exist.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);

                this.OnErrorOccurred(new GrabberErrorBase(location));
            }
        }

        /// <summary>
        /// 画像取得スレッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="info"></param>
        protected virtual void AcquisitionThread_Dequeue(object sender, AcquisitionControlInfo info)
        {
            try
            {
                if (null != this.Handles.Device && null != this.Handles.DataStream)
                {
                    if (0 < info.NumberOfImage)
                    {
                        this.Handles.DataStream.StartAcquisition((ulong)info.NumberOfImage);
                    }
                    else
                    {
                        this.Handles.DataStream.StartAcquisition();
                    }

                    this.Handles.Device.AcquisitionStart();

                    var imageCounter = 0UL;

                    var stopwatch = new Stopwatch();

                    while (this.Handles.DataStream.IsGrabbing && info.IsContinuable)
                    {
                        // 画像取得(コールバック)を待機する
                        var bufferImages = info.GetDataStreamNewBuffer();

                        // 画像バッファからデータを列挙
                        foreach (var buffer in bufferImages)
                        {
                            imageCounter++;

                            stopwatch.Restart();

                            // 画像データ変換
                            var bitmap = this.BitmapConvertor(buffer, out byte[] bytes, out int stride);
                            buffer.Dispose();

                            // 撮影データを生成
                            var result = new ByteArrayGrabberDataFastBitmapGeneration(location)
                            {
                                Counter = imageCounter
                            ,
                                Image = bytes
                            ,
                                Stride = stride
                            ,
                                Bitmap = bitmap
                            ,
                                Size = new Common.Drawing.Size(bitmap?.Size ?? new Size())
                            ,
                                PixelFormat = bitmap?.PixelFormat ?? PixelFormat.Format8bppIndexed
                            };

                            var copyTimeMs = stopwatch.ElapsedMilliseconds;

                            // 画像取得イベント通知
                            this.OnDataGrabbed(result);

                            var callTimeMs = stopwatch.ElapsedMilliseconds - copyTimeMs;

                            stopwatch.Stop();
                            var totalTimeMs = stopwatch.ElapsedMilliseconds;
                        }
                    }

                    if (false == this.Handles.Device.IsDeviceLost)
                    {
                        // Stop the image acquisition of the camera side.
                        this.Handles.Device?.AcquisitionStop();
                    }

                    // Stop the image acquisition of the host side.
                    this.Handles.DataStream?.StopAcquisition();
                }
            }
            catch (Exception ex)
            {
                info.IsContinuable = false;

                Serilog.Log.Warning(ex, ex.Message);
                this.OnErrorOccurred(new GrabberErrorBase(location));
            }
            finally
            {
                this.DisposeSafely(info);
            }
        }

        /// <summary>
        /// データコンテナ
        /// </summary>
        protected class ImageContainer : SafelyDisposable
        {
            public CStImageBuffer ConvertedBuffer;
            public Size Size;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="size"></param>
            public ImageContainer(CStImageBuffer buffer, Size size)
            {
                this.ConvertedBuffer = buffer;
                this.Size = size;
            }

            #region SafelyDisposable

            protected override void DisposeExplicit()
            {
                try
                {
                    this.DisposeSafely(this.ConvertedBuffer);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            #endregion
        }

        /// <summary>
        /// Bitmap変換:Mono8
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        protected virtual System.Drawing.Bitmap? ToBitmapOfMono8(ImageContainer data, out byte[] bytes, out int stride)
        {
            var bitmap = (System.Drawing.Bitmap?)null;

            bytes = Array.Empty<byte>();
            stride = 0;

            try
            {
                bitmap = new Bitmap(data.Size.Width, data.Size.Height, PixelFormat.Format8bppIndexed);
                BitmapProcessor.AssignColorPaletteOfDefault(bitmap);

                // Lock the bits of the bitmap.
                var bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

                stride = bmpData.Stride;

                // Place the pointer to the buffer of the bitmap.
                var ptrBmp = bmpData.Scan0;
                bytes = data.ConvertedBuffer.GetIStImage().GetByteArray();
                Marshal.Copy(bytes, 0, ptrBmp, bytes.Length);
                bitmap.UnlockBits(bmpData);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return bitmap;
        }

        /// <summary>
        /// Bitmap変換:RGB24
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        protected virtual System.Drawing.Bitmap? ToBitmapOfRgb24(ImageContainer data, out byte[] bytes, out int stride)
        {
            var bitmap = (System.Drawing.Bitmap?)null;

            bytes = Array.Empty<byte>();
            stride = 0;

            try
            {
                bitmap = new Bitmap(data.Size.Width, data.Size.Height, PixelFormat.Format24bppRgb);

                // Lock the bits of the bitmap.
                var bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

                stride = bmpData.Stride;

                // Place the pointer to the buffer of the bitmap.
                var ptrBmp = bmpData.Scan0;
                bytes = data.ConvertedBuffer.GetIStImage().GetByteArray();
                Marshal.Copy(bytes, 0, ptrBmp, bytes.Length);
                bitmap.UnlockBits(bmpData);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return bitmap;
        }
    }
}