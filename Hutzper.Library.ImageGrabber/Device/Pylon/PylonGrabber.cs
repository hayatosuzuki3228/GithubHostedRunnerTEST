using Basler.Pylon;
using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.IO.Configuration;
using Hutzper.Library.Common.Threading;
using Hutzper.Library.ImageGrabber.Data;
using Hutzper.Library.ImageProcessing;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Hutzper.Library.ImageGrabber.Device.Pylon
{
    /// <summary>
    /// Pylon
    /// </summary>
    [Serializable]
    public abstract class PylonGrabber : GrabberBase, IPylonGrabber
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
                TriggerMode value = TriggerMode.InternalTrigger;
                string valueStr = this.GetValue<string>(PLCamera.TriggerMode) ?? string.Empty;
                if ("On" == valueStr)
                {
                    valueStr = this.GetValue<string>(PLCamera.TriggerSource) ?? string.Empty;
                    value = "Software" == valueStr ? TriggerMode.SoftTrigger : TriggerMode.ExternalTrigger;
                }
                return value;
            }

            set
            {
                if (TriggerMode.InternalTrigger == value) this.SetValue(PLCamera.TriggerMode, PLCamera.TriggerMode.Off, true);
                else
                {
                    this.SetValue(PLCamera.TriggerMode, PLCamera.TriggerMode.On, true);
                    if (TriggerMode.SoftTrigger == value) this.SetValue(PLCamera.TriggerSource, PLCamera.TriggerSource.Software, true);
                    else this.SetValue(PLCamera.TriggerSource, PLCamera.TriggerSource.Line1, true);
                }
            }
        }

        /// <summary>
        /// 露光時間μ秒
        /// </summary>
        public override double ExposureTimeMicroseconds
        {
            get => (this.GetValue<bool?>(PLCamera.ExposureTime, GetValueInfo.IsContain) ?? false) ? this.GetValue<float?>(PLCamera.ExposureTime) ?? 0f : this.GetValue<float?>(PLCamera.ExposureTimeAbs) ?? 0f;

            set
            {
                if (this.Handles.Device is null) return;
                if (this.GetValue<bool>(PLCamera.ExposureTime, GetValueInfo.IsContain))
                {
                    if (this.GetValue<double>(PLCamera.ExposureTime, GetValueInfo.Minimum) <= value && value <= this.GetValue<double>(PLCamera.ExposureTime, GetValueInfo.Maximum)) this.SetValue(PLCamera.ExposureTime, value);
                }
                else
                {
                    if (this.GetValue<double>(PLCamera.ExposureTimeAbs, GetValueInfo.Minimum) <= value && value <= this.GetValue<double>(PLCamera.ExposureTimeAbs, GetValueInfo.Maximum)) this.SetValue(PLCamera.ExposureTimeAbs, value);
                }
            }
        }

        /// <summary>
        /// アナログゲイン
        /// </summary>
        public override double AnalogGain
        {
            get => (this.GetValue<bool?>(PLCamera.Gain, GetValueInfo.IsContain) ?? false) ? this.GetValue<long?>(PLCamera.Gain) ?? 0d : this.GetValue<long?>(PLCamera.GainRaw) ?? 0d;

            set
            {
                if (this.Handles.Device is null) return;
                if (this.GetValue<bool>(PLCamera.Gain, GetValueInfo.IsContain))
                {
                    if (this.GetValue<double>(PLCamera.Gain, GetValueInfo.Minimum) <= value && value <= this.GetValue<double>(PLCamera.Gain, GetValueInfo.Maximum))
                    {
                        this.SetValue(PLCamera.GainSelector, PLCamera.GainSelector.All);
                        this.SetValue(PLCamera.Gain, value);
                    }
                }
                else
                {
                    if (this.GetValue<long>(PLCamera.GainRaw, GetValueInfo.Minimum) <= value && value <= this.GetValue<long>(PLCamera.GainRaw, GetValueInfo.Maximum))
                    {
                        this.SetValue(PLCamera.GainSelector, PLCamera.GainSelector.All);
                        this.SetValue(PLCamera.GainRaw, value);
                    }
                }
            }
        }

        /// <summary>
        /// デジタルゲイン
        /// </summary>
        public override double DigitalGain
        {
            get => this.GetValue<long?>(PLCamera.DigitalShift) ?? 0L;

            set
            {
                if (this.Handles.Device is null) return;
                long lvalue = (long)value;
                if (this.GetValue<long>(PLCamera.DigitalShift, GetValueInfo.Minimum) <= lvalue && lvalue <= this.GetValue<long>(PLCamera.DigitalShift, GetValueInfo.Maximum))
                {
                    this.SetValue(PLCamera.GainSelector, PLCamera.GainSelector.All);
                    this.SetValue(PLCamera.DigitalShift, lvalue);
                }
            }
        }

        /// <summary>
        /// RGBカラーかどうか
        /// </summary>
        public override bool IsRgbColor
        {
            get
            {
                bool result = false;
                if (this.Handles.Device is null) return result;
                string selected = this.GetValue<string>(PLCamera.PixelFormat)?.ToLower() ?? string.Empty;
                return (selected.Contains("bayer") || selected.Contains("rgb") || selected.Contains("bgr"));
            }
        }

        /// <summary>
        /// 画像幅
        /// </summary>
        public override int Width
        {
            get => (int)(this.GetValue<long?>(PLCamera.Width) ?? 0L);

            set
            {
                if (this.Handles.Device is null) return;
                if (!this.GetValue<bool>(PLCamera.Width, GetValueInfo.IsContain)) return;
                if (0 >= value) this.SetValue(PLCamera.Width, this.GetValue<long>(PLCamera.Width));
                else if (this.GetValue<long>(PLCamera.Width, GetValueInfo.Minimum) <= value && value <= this.GetValue<long>(PLCamera.Width, GetValueInfo.Maximum))
                {
                    long inc = this.GetValue<long>(PLCamera.Width, GetValueInfo.Increment);
                    long v = (value / inc) * inc;
                    this.SetValue(PLCamera.Width, v);
                    if (v != value) Serilog.Log.Warning($"Width is replaced {value} -> {v}");
                }
            }
        }

        /// <summary>
        /// 画像高さ
        /// </summary>
        public override int Height
        {
            get => (int)(this.GetValue<long?>(PLCamera.Height) ?? 0L);

            set
            {
                if (this.Handles.Device is null) return;
                if (!this.GetValue<bool>(PLCamera.Height, GetValueInfo.IsContain)) return;
                if (0 >= value) this.SetValue(PLCamera.Height, this.GetValue<long>(PLCamera.Height, GetValueInfo.Maximum));
                else if (this.GetValue<long>(PLCamera.Height, GetValueInfo.Minimum) <= value && value <= this.GetValue<long>(PLCamera.Height, GetValueInfo.Maximum))
                {
                    long inc = this.GetValue<long>(PLCamera.Height, GetValueInfo.Increment);
                    long v = (value / inc) * inc;
                    this.SetValue(PLCamera.Height, v);
                    if (v != value) Serilog.Log.Warning($"Height is replaced {value} -> {v}");
                }
            }
        }

        /// <summary>
        /// Xオフセット
        /// </summary>
        public override int OffsetX
        {
            get => (int)(this.GetValue<long?>(PLCamera.OffsetX) ?? 0L);

            set
            {
                if (this.Handles.Device is null) return;
                if (!(this.GetValue<bool>(PLCamera.OffsetX, GetValueInfo.IsContain) && this.Handles.Device.Parameters[PLCamera.OffsetX].IsWritable)) return;
                if (this.GetValue<long>(PLCamera.OffsetX, GetValueInfo.Minimum) <= value && value <= this.GetValue<long>(PLCamera.OffsetX, GetValueInfo.Maximum))
                {
                    long inc = this.GetValue<long>(PLCamera.OffsetX, GetValueInfo.Increment);
                    long unitValue = (value / inc) * inc;
                    if (unitValue != value) Serilog.Log.Warning($"OffsetX is replaced {value} -> {unitValue}");
                    this.SetValue(PLCamera.OffsetX, unitValue);
                }
            }
        }

        /// <summary>
        /// Yオフセット
        /// </summary>
        public override int OffsetY
        {
            get => (int)(this.GetValue<long?>(PLCamera.OffsetY) ?? 0L);

            set
            {
                if (this.Handles.Device is null) return;
                if (!(this.GetValue<bool>(PLCamera.OffsetY, GetValueInfo.IsContain) && this.Handles.Device.Parameters[PLCamera.OffsetY].IsWritable)) return;
                if (this.GetValue<long>(PLCamera.OffsetY, GetValueInfo.Minimum) <= value && value <= this.GetValue<long>(PLCamera.OffsetY, GetValueInfo.Maximum))
                {
                    long inc = this.GetValue<long>(PLCamera.OffsetY, GetValueInfo.Increment);
                    long unitValue = (value / inc) * inc;
                    if (unitValue != value) Serilog.Log.Warning($"OffsetY is replaced {value} -> {unitValue}");
                    this.SetValue(PLCamera.OffsetY, unitValue);
                }
            }
        }

        public int UseUserSetIndex = 0;
        public bool SetValue(object parameterName, object value, bool useSetParameter = false)
        {
            bool result = false;
            try
            {
                if (this.GetValue<bool>(parameterName, GetValueInfo.IsContain))
                {
                    if (this.Handles.Device is not null && (useSetParameter || base.UseSetParameter))
                    {
                        if (parameterName is IntegerName intParameterName && value is long longValue)
                        {
                            result = this.Handles.Device.Parameters[intParameterName].TrySetValue(longValue);
                            if (!result) Serilog.Log.Warning($"parameter {intParameterName.Name} can`t write");
                        }
                        else if (parameterName is FloatName floatParameterName && value is double floatValue)
                        {
                            result = this.Handles.Device.Parameters[floatParameterName].TrySetValue(floatValue);
                            if (!result) Serilog.Log.Warning($"parameter {floatParameterName.Name} can`t write");
                        }
                        else if (parameterName is BooleanName boolParameterName && value is bool boolValue)
                        {
                            result = this.Handles.Device.Parameters[boolParameterName].TrySetValue(boolValue);
                            if (!result) Serilog.Log.Warning($"parameter {boolParameterName.Name} can`t write");
                        }
                        else if (parameterName is ParameterListEnum enumParameterName && value is string stringValue)
                        {
                            result = this.Handles.Device.Parameters[enumParameterName].TrySetValue(stringValue);
                            if (!result) Serilog.Log.Warning($"parameter {enumParameterName.Name} can`t write");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            return result;
        }
        public enum GetValueInfo
        {
            Value,
            Maximum,
            Minimum,
            Increment,
            IsContain,
        }

        public T? GetValue<T>(object parameterName, GetValueInfo GetValueInfo = GetValueInfo.Value)
        {
            try
            {
                if (parameterName is IntegerName intParameterName)
                {
                    bool IsContain = Handles.Device?.Parameters.Contains(intParameterName) ?? false;
                    if (!IsContain) Serilog.Log.Warning($"not find parameter {intParameterName.Name}");
                    else
                    {
                        if (!(Handles.Device?.Parameters[intParameterName].IsReadable ?? false)) Serilog.Log.Warning($"parameter {intParameterName.Name} can`t read");
                        else
                        {
                            return GetValueInfo switch
                            {
                                GetValueInfo.Value => (T)(object)(Handles.Device?.Parameters[intParameterName].GetValue() ?? 0L),
                                GetValueInfo.Maximum => (T)(object)(Handles.Device?.Parameters[intParameterName].GetMaximum() ?? 0L),
                                GetValueInfo.Minimum => (T)(object)(Handles.Device?.Parameters[intParameterName].GetMinimum() ?? 0L),
                                GetValueInfo.Increment => (T)(object)(Handles.Device?.Parameters[intParameterName].GetIncrement() ?? 0L),
                                GetValueInfo.IsContain => (T)(object)IsContain,
                                _ => default,
                            };
                        }
                    }
                }
                else if (parameterName is FloatName floatParameterName)
                {
                    bool IsContain = Handles.Device?.Parameters.Contains(floatParameterName) ?? false;
                    if (!IsContain) Serilog.Log.Warning($"not find parameter {floatParameterName.Name}");
                    else
                    {
                        if (!(Handles.Device?.Parameters[floatParameterName].IsReadable ?? false)) Serilog.Log.Warning($"parameter {floatParameterName.Name} can`t read");
                        else
                        {
                            return GetValueInfo switch
                            {
                                GetValueInfo.Value => (T)(object)(Handles.Device?.Parameters[floatParameterName].GetValue() ?? 0d),
                                GetValueInfo.Maximum => (T)(object)(Handles.Device?.Parameters[floatParameterName].GetMaximum() ?? 0d),
                                GetValueInfo.Minimum => (T)(object)(Handles.Device?.Parameters[floatParameterName].GetMinimum() ?? 0d),
                                GetValueInfo.Increment => (T)(object)(Handles.Device?.Parameters[floatParameterName].GetIncrement() ?? 0d),
                                GetValueInfo.IsContain => (T)(object)IsContain,
                                _ => default,
                            };
                        }
                    }
                }
                else if (parameterName is ParameterListEnum enumParameterName)
                {
                    bool IsContain = Handles.Device?.Parameters.Contains(enumParameterName) ?? false;
                    if (!IsContain) Serilog.Log.Warning($"not find parameter {enumParameterName.Name}");
                    else
                    {
                        if (!(Handles.Device?.Parameters[enumParameterName].IsReadable ?? false)) Serilog.Log.Warning($"parameter {enumParameterName.Name} can`t read");
                        else
                        {
                            return GetValueInfo switch
                            {
                                GetValueInfo.Value => (T)(object)Handles.Device?.Parameters[enumParameterName].GetValue()! ?? default,
                                GetValueInfo.IsContain => (T)(object)IsContain,
                                _ => default,
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            return default;
        }

        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            try
            {
                if (this.Handles.Device is null && this.Parameter is IGrabberParameter gp)
                {
                    if (false == int.TryParse(gp.DeviceID, out int intID)) intID = -1;
                    this.UseUserSetIndex = gp.UseUserSetIndex;
                    ICameraInfo? selectedCameraInfo = null;
                    foreach (var cameraInfo in CameraFinder.Enumerate())
                    {
                        if (0 > intID)
                        {
                            selectedCameraInfo = cameraInfo;
                            break;
                        }
                        else if (cameraInfo.ContainsKey(CameraInfoKey.DeviceID))
                        {
                            if (true == gp.DeviceID.Equals(cameraInfo[CameraInfoKey.DeviceID]))
                            {
                                selectedCameraInfo = cameraInfo;
                                break;
                            }
                        }
                        else if (cameraInfo.ContainsKey(CameraInfoKey.SerialNumber))
                        {
                            if (true == gp.DeviceID.Equals(cameraInfo[CameraInfoKey.SerialNumber]))
                            {
                                selectedCameraInfo = cameraInfo;
                                break;
                            }
                        }
                    }

                    if (selectedCameraInfo is not null)
                    {
                        this.Handles.Device = new Camera(selectedCameraInfo);

                        this.Handles.Device.CameraOpened += Configuration.AcquireContinuous;

                        // Register for the events of the image provider needed for proper operation.
                        this.Handles.Device.ConnectionLost += this.Device_ConnectionLost;
                        this.Handles.Device.CameraOpened += this.Device_CameraOpened;
                        this.Handles.Device.CameraClosed += this.Device_CameraClosed;
                        this.Handles.Device.StreamGrabber.GrabStarted += this.StreamGrabber_GrabStarted;
                        this.Handles.Device.StreamGrabber.ImageGrabbed += this.StreamGrabber_ImageGrabbed;
                        this.Handles.Device.StreamGrabber.GrabStopped += this.StreamGrabber_GrabStopped;

                        this.Handles.Device.Open();
                        this.LoadUserSet(this.UseUserSetIndex);
                        this.SetValue(PLCamera.GainAuto, PLCamera.GainAuto.Off, true);
                        this.SetValue(PLCamera.ExposureAuto, PLCamera.GainAuto.Off, true);
                        this.SetValue(PLCamera.ExposureMode, PLCamera.ExposureMode.Timed, true);
                        this.OffsetX = 0;
                        this.OffsetY = 0;

                        #region パラメータ反映
                        {
                            this.ExposureTimeMicroseconds = this.GetValue<bool>(PLCamera.ExposureTime, GetValueInfo.IsContain) ?
                                System.Math.Clamp(gp.ExposureTimeMicroseconds, this.GetValue<double>(PLCamera.ExposureTime, GetValueInfo.Minimum), this.GetValue<double>(PLCamera.ExposureTime, GetValueInfo.Maximum)) :
                                System.Math.Clamp(gp.ExposureTimeMicroseconds, this.GetValue<double>(PLCamera.ExposureTimeAbs, GetValueInfo.Minimum), this.GetValue<double>(PLCamera.ExposureTimeAbs, GetValueInfo.Maximum));

                            this.AnalogGain = this.GetValue<bool>(PLCamera.ExposureTime, GetValueInfo.IsContain) ?
                                System.Math.Clamp(gp.AnalogGain, this.GetValue<double>(PLCamera.Gain, GetValueInfo.Minimum), this.GetValue<double>(PLCamera.Gain, GetValueInfo.Maximum)) :
                                System.Math.Clamp(gp.AnalogGain, this.GetValue<long>(PLCamera.GainRaw, GetValueInfo.Minimum), this.GetValue<long>(PLCamera.GainRaw, GetValueInfo.Maximum));

                            this.DigitalGain = gp.DigitalGain;
                            this.Width = gp.Width;
                            this.Height = gp.Height;
                            this.OffsetX = gp.OffsetX;
                            this.OffsetY = gp.OffsetY;
                        }
                        #endregion
                        this.TriggerMode = gp.TriggerMode;
                        this.Handles.IsColor = this.IsRgbColor;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Close();
                Serilog.Log.Warning(ex, ex.Message);
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
                this.StopGrabbing();
                this.Handles.Device?.Close();
                this.Handles.Device?.Dispose();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.Handles.Device = null;
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
                    if (this.ControlInfo is AcquisitionControlInfo info)
                    {
                        info.IsContinuable = false;
                        info.Dispose();
                    }

                    this.ControlInfo = new AcquisitionControlInfo(1);
                    this.AcquisitionThread.Enqueue(ControlInfo);
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
                    this.AcquisitionThread.Enqueue(ControlInfo);
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
                if (this.Handles.Device is not null)
                {
                    // Some camera models don't signal their FrameTriggerReady state.
                    if (true == this.Handles.Device.CanWaitForFrameTriggerReady)
                    {
                        this.Handles.Device.WaitForFrameTriggerReady(1000, TimeoutHandling.ThrowException);
                    }

                    this.Handles.Device.ExecuteSoftwareTrigger();

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
                if (this.Handles.Device is not null)
                {
                    this.SetValue(PLCamera.BalanceWhiteAuto, PLCamera.BalanceWhiteAuto.Off);
                    this.SetValue(PLCamera.BalanceWhiteAuto, PLCamera.BalanceWhiteAuto.Once);

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
        /// Balance Ratio（RGBゲイン値）取得
        /// </summary>
        /// <returns></returns>
        public override bool GetBalanceRatio(out double gainRedValue, out double gainGreenValue, out double gainBlueValue)
        {
            var isSuccess = false;

            gainRedValue = 0;
            gainGreenValue = 0;
            gainBlueValue = 0;

            if (this.Handles.Device is not null)
            {
                try
                {
                    this.SetValue(PLCamera.BalanceRatioSelector, PLCamera.BalanceRatioSelector.Red);
                    gainRedValue = this.GetValue<double>(PLCamera.BalanceRatio);

                    this.SetValue(PLCamera.BalanceRatioSelector, PLCamera.BalanceRatioSelector.Green);
                    gainGreenValue = this.GetValue<double>(PLCamera.BalanceRatio);

                    this.SetValue(PLCamera.BalanceRatioSelector, PLCamera.BalanceRatioSelector.Blue);
                    gainBlueValue = this.GetValue<double>(PLCamera.BalanceRatio);

                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }

            return isSuccess;
        }

        /// <summary>
        /// ユーザーセットデフォルトロード
        /// </summary>
        /// <returns></returns>
        public override bool LoadUserSetDefault()
        {
            bool isSuccess = this.Enabled && (this.ControlInfo?.IsContinuable ?? true);

            try
            {
                if (true == isSuccess)
                {
                    this.SetValue(PLCamera.UserSetSelector, PLCamera.UserSetSelector.Default);
                    this.Handles.Device?.Parameters[PLCamera.UserSetLoad].Execute();
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
            var isSuccess = this.Enabled && !(this.ControlInfo?.IsContinuable ?? false);

            try
            {
                if (true == isSuccess && 0 <= index && 7 > index)
                {
                    switch (index)
                    {
                        case 1: this.SetValue(PLCamera.UserSetSelector, PLCamera.UserSetSelector.UserSet1, true); break;
                        case 2: this.SetValue(PLCamera.UserSetSelector, PLCamera.UserSetSelector.UserSet2, true); break;
                        case 3: this.SetValue(PLCamera.UserSetSelector, PLCamera.UserSetSelector.UserSet3, true); break;
                        default: this.SetValue(PLCamera.UserSetSelector, PLCamera.UserSetSelector.Default, true); break;
                    }

                    this.Handles.Device?.Parameters[PLCamera.UserSetLoad].Execute();
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
        /// ユーザーセットセーブ
        /// </summary>
        /// <returns></returns>
        public override bool SaveUserSet(int index = 0)
        {
            var isSuccess = this.Enabled && !(this.ControlInfo?.IsContinuable ?? false);

            try
            {
                if (true == isSuccess && 0 <= index && 3 > index)
                {
                    switch (index)
                    {
                        case 1: this.SetValue(PLCamera.UserSetSelector, PLCamera.UserSetSelector.UserSet2); break;
                        case 2: this.SetValue(PLCamera.UserSetSelector, PLCamera.UserSetSelector.UserSet3); break;
                        default: this.SetValue(PLCamera.UserSetSelector, PLCamera.UserSetSelector.UserSet1); break;
                    }

                    this.Handles.Device?.Parameters[PLCamera.UserSetSave].Execute();
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

                if (this.Handles.Device is null) return;

                foreach (var key in keys)
                {
                    if (false != values.ContainsKey(key)) continue;

                    var value = (ClampedValue<double>?)null;
                    switch (key)
                    {
                        case ParametersKey.ExposureTime:
                            {
                                FloatName target = this.GetValue<bool>(PLCamera.ExposureTime, GetValueInfo.IsContain) ? PLCamera.ExposureTime : PLCamera.ExposureTimeAbs;
                                value = new(
                                                this.GetValue<double>(target),
                                                this.GetValue<double>(target, GetValueInfo.Minimum),
                                                this.GetValue<double>(target, GetValueInfo.Maximum)
                                           );
                            }
                            break;

                        case ParametersKey.AnalogGain:
                            {
                                value = this.GetValue<bool>(PLCamera.Gain, GetValueInfo.IsContain) ?
                                            new(
                                                    this.GetValue<double>(PLCamera.Gain),
                                                    this.GetValue<double>(PLCamera.Gain, GetValueInfo.Minimum),
                                                    this.GetValue<double>(PLCamera.Gain, GetValueInfo.Maximum)
                                               ) :
                                            new(
                                                    this.GetValue<long>(PLCamera.GainRaw),
                                                    this.GetValue<long>(PLCamera.GainRaw, GetValueInfo.Minimum),
                                                    this.GetValue<long>(PLCamera.GainRaw, GetValueInfo.Maximum)
                                               );
                            }
                            break;

                        case ParametersKey.DigitalGain:
                            {
                                value = new(
                                                this.GetValue<long>(PLCamera.DigitalShift),
                                                this.GetValue<long>(PLCamera.DigitalShift, GetValueInfo.Minimum),
                                                this.GetValue<long>(PLCamera.DigitalShift, GetValueInfo.Maximum)
                                           );
                            }
                            break;

                        case ParametersKey.FrameRate:
                            {
                                FloatName target = this.GetValue<bool>(PLCamera.AcquisitionFrameRate, GetValueInfo.IsContain) ? PLCamera.AcquisitionFrameRate : PLCamera.AcquisitionFrameRateAbs;
                                value = new(
                                                this.GetValue<double>(target),
                                                this.GetValue<double>(target, GetValueInfo.Minimum),
                                                this.GetValue<double>(target, GetValueInfo.Maximum)
                                           );
                            }
                            break;

                        case ParametersKey.OffsetX:
                            {
                                value = new(
                                                this.GetValue<double>(PLCamera.OffsetX),
                                                this.GetValue<double>(PLCamera.OffsetX, GetValueInfo.Minimum),
                                                this.GetValue<double>(PLCamera.OffsetX, GetValueInfo.Maximum)
                                           );
                            }
                            break;

                        case ParametersKey.OffsetY:
                            {
                                value = new(
                                                this.GetValue<double>(PLCamera.OffsetY),
                                                this.GetValue<double>(PLCamera.OffsetY, GetValueInfo.Minimum),
                                                this.GetValue<double>(PLCamera.OffsetY, GetValueInfo.Maximum)
                                           );
                            }
                            break;

                        case ParametersKey.Width:
                            {
                                value = new(
                                                this.GetValue<double>(PLCamera.Width),
                                                this.GetValue<double>(PLCamera.Width, GetValueInfo.Minimum),
                                                this.GetValue<double>(PLCamera.Width, GetValueInfo.Maximum)
                                           );
                            }
                            break;

                        case ParametersKey.Height:
                            {
                                value = new(
                                                this.GetValue<double>(PLCamera.Height),
                                                this.GetValue<double>(PLCamera.Height, GetValueInfo.Minimum),
                                                this.GetValue<double>(PLCamera.Height, GetValueInfo.Maximum)
                                           );
                            }
                            break;
                        case ParametersKey.LineRate:
                            {
                                value = new(
                                                this.GetValue<double>(PLCamera.AcquisitionLineRateAbs),
                                                this.GetValue<double>(PLCamera.AcquisitionLineRateAbs, GetValueInfo.Minimum),
                                                this.GetValue<double>(PLCamera.AcquisitionLineRateAbs, GetValueInfo.Maximum)
                                           );
                            }
                            break;
                    }
                    if (value is not null) values.Add(key, value);
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

                if (this.Handles.Device is null) return;

                foreach (var key in keys)
                {
                    if (false != values.ContainsKey(key)) continue;

                    var value = (ClampedValue<int>?)null;
                    switch (key)
                    {
                        case ParametersKey.DigitalGain:
                            {
                                value = new(
                                        (int)this.GetValue<long>(PLCamera.DigitalShift),
                                        (int)this.GetValue<long>(PLCamera.DigitalShift, GetValueInfo.Minimum),
                                        (int)this.GetValue<long>(PLCamera.DigitalShift, GetValueInfo.Maximum)
                                    );
                            }
                            break;

                        case ParametersKey.OffsetX:
                            {
                                value = new(
                                        (int)this.GetValue<long>(PLCamera.OffsetX),
                                        (int)this.GetValue<long>(PLCamera.OffsetX, GetValueInfo.Minimum),
                                        (int)this.GetValue<long>(PLCamera.OffsetX, GetValueInfo.Maximum)
                                    );
                            }
                            break;

                        case ParametersKey.OffsetY:
                            {
                                value = new(
                                        (int)this.GetValue<long>(PLCamera.OffsetY),
                                        (int)this.GetValue<long>(PLCamera.OffsetY, GetValueInfo.Minimum),
                                        (int)this.GetValue<long>(PLCamera.OffsetY, GetValueInfo.Maximum)
                                    );
                            }
                            break;

                        case ParametersKey.Width:
                            {
                                value = new(
                                        (int)this.GetValue<long>(PLCamera.Width),
                                        (int)this.GetValue<long>(PLCamera.Width, GetValueInfo.Minimum),
                                        (int)this.GetValue<long>(PLCamera.Width, GetValueInfo.Maximum)
                                    );
                            }
                            break;

                        case ParametersKey.Height:
                            {
                                value = new(
                                        (int)this.GetValue<long>(PLCamera.Height),
                                        (int)this.GetValue<long>(PLCamera.Height, GetValueInfo.Minimum),
                                        (int)this.GetValue<long>(PLCamera.Height, GetValueInfo.Maximum)
                                    );
                            }
                            break;
                    }

                    if (value is not null)
                    {
                        values.Add(key, value);
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
        public override bool SetExposureTimeValue(double value) => this.Handles.Device is not null ? (this.SetValue(PLCamera.ExposureMode, PLCamera.ExposureMode.Timed) && this.SetValue(PLCamera.ExposureTime, value)) : false;

        /// <summary>
        /// アナログゲインをセットする
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override bool SetAnalogGainValue(double value) => this.Handles.Device is not null ? this.SetValue(PLCamera.Gain, value) : false;

        /// <summary>
        /// ラインレートをセットする
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override bool SetLineRateValue(double value) => this.Handles.Device is not null ? this.SetValue(PLCamera.ResultingLineRateAbs, value) : false;

        /// <summary>
        /// フレームレートをセットする
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public override bool SetFrameRateValue(double value)
        {
            if (this.Handles.Device is null) return false;
            var target = this.GetValue<bool>(PLCamera.AcquisitionFrameRate, GetValueInfo.IsContain) ? PLCamera.AcquisitionFrameRate : PLCamera.AcquisitionFrameRateAbs;
            return this.SetValue(target, value);
        }

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
                this.LoadUserSet(this.UseUserSetIndex);
                this.SetValue(PLCamera.GainAuto, PLCamera.GainAuto.Off, true);
                this.SetValue(PLCamera.ExposureAuto, PLCamera.GainAuto.Off, true);
                this.SetValue(PLCamera.ExposureMode, PLCamera.ExposureMode.Timed, true);
                if (true != this.Enabled) return;
                if (this.Handles.Device is not null && this.Parameter is IGrabberParameter gp)
                {
                    this.ExposureTimeMicroseconds = this.GetValue<bool>(PLCamera.ExposureTime, GetValueInfo.IsContain) ?
                        System.Math.Clamp(gp.ExposureTimeMicroseconds, this.GetValue<double>(PLCamera.ExposureTime, GetValueInfo.Minimum), this.GetValue<double>(PLCamera.ExposureTime, GetValueInfo.Maximum)) :
                        System.Math.Clamp(gp.ExposureTimeMicroseconds, this.GetValue<double>(PLCamera.ExposureTimeAbs, GetValueInfo.Minimum), this.GetValue<double>(PLCamera.ExposureTimeAbs, GetValueInfo.Maximum));

                    this.AnalogGain = this.GetValue<bool>(PLCamera.ExposureTime, GetValueInfo.IsContain) ?
                        System.Math.Clamp(gp.AnalogGain, this.GetValue<double>(PLCamera.Gain, GetValueInfo.Minimum), this.GetValue<double>(PLCamera.Gain, GetValueInfo.Maximum)) :
                        System.Math.Clamp(gp.AnalogGain, this.GetValue<double>(PLCamera.GainRaw, GetValueInfo.Minimum), this.GetValue<double>(PLCamera.GainRaw, GetValueInfo.Maximum));

                    if (this.GetValue<bool>(PLCamera.CenterX, GetValueInfo.IsContain)) this.SetValue(PLCamera.CenterX, false);
                    if (this.GetValue<bool>(PLCamera.CenterY, GetValueInfo.IsContain)) this.SetValue(PLCamera.CenterY, false);

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
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 設定
        /// </summary>
        /// <param name="config"></param>
        public override void SetConfig(IApplicationConfig? config) => base.SetConfig(config);

        /// <summary>
        /// パラメーター設定
        /// </summary>
        /// <param name="parameter"></param>
        public override void SetParameter(IControllerParameter? parameter)
        {
            base.SetParameter(parameter);

            if (this.Handles.Device is not null && this.Parameter is IGrabberParameter gp)
            {
                this.ExposureTimeMicroseconds = this.GetValue<bool>(PLCamera.ExposureTime, GetValueInfo.IsContain) ?
                    System.Math.Clamp(gp.ExposureTimeMicroseconds, this.GetValue<double>(PLCamera.ExposureTime, GetValueInfo.Minimum), this.GetValue<double>(PLCamera.ExposureTime, GetValueInfo.Maximum)) :
                    System.Math.Clamp(gp.ExposureTimeMicroseconds, this.GetValue<double>(PLCamera.ExposureTimeAbs, GetValueInfo.Minimum), this.GetValue<double>(PLCamera.ExposureTimeAbs, GetValueInfo.Maximum));

                this.AnalogGain = this.GetValue<bool>(PLCamera.ExposureTime, GetValueInfo.IsContain) ?
                    System.Math.Clamp(gp.AnalogGain, this.GetValue<double>(PLCamera.Gain, GetValueInfo.Minimum), this.GetValue<double>(PLCamera.Gain, GetValueInfo.Maximum)) :
                    System.Math.Clamp(gp.AnalogGain, this.GetValue<double>(PLCamera.GainRaw, GetValueInfo.Minimum), this.GetValue<double>(PLCamera.GainRaw, GetValueInfo.Maximum));

                if (this.GetValue<bool>(PLCamera.CenterX, GetValueInfo.IsContain)) this.SetValue(PLCamera.CenterX, false);
                if (this.GetValue<bool>(PLCamera.CenterY, GetValueInfo.IsContain)) this.SetValue(PLCamera.CenterY, false);

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

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update() { }

        #endregion

        #region SafelyDisposable

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            this.DisposeSafely(this.AcquisitionThread);
            this.DisposeSafely(this.Handles);
        }

        #endregion

        /// <summary>
        /// API
        /// </summary>
        protected class PylonHandles : SafelyDisposable
        {
            public bool Enabled => null != this.Device && this.Device.IsConnected;

            public Basler.Pylon.Camera? Device { get; set; }

            public bool IsColor { get; set; }

            #region SafelyDisposable

            /// <summary>
            /// リソースの解放
            /// </summary>
            protected override void DisposeExplicit()
            {
                try
                {
                    this.Device?.Close();
                    this.DisposeSafely(this.Device);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            #endregion
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

            protected List<IGrabResult> bufferImages = new();

            #endregion

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="numberOfImage"></param>
            public AcquisitionControlInfo(int numberOfImage)
            {
                this.NumberOfImage = numberOfImage;
                this.WaitHandles[0] = new ManualResetEvent(false);
                this.WaitHandles[1] = new AutoResetEvent(true);
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
            public void OnIDataStreamNewBuffer(IGrabResult grabResult)
            {
                try
                {
                    if (this.WaitHandles[1] is AutoResetEvent e)
                    {
                        lock (this.WaitHandles)
                        {
                            bufferImages.Add(grabResult);
                        }

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
            public List<IGrabResult> GetDataStreamNewBuffer()
            {
                var images = new List<IGrabResult>();

                try
                {
                    WaitHandle.WaitAny(this.WaitHandles);

                    lock (this.WaitHandles)
                    {
                        if (0 < bufferImages.Count)
                        {
                            images.AddRange(bufferImages);
                            bufferImages.Clear();
                        }
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

        #region フィールド

        /// <summary>
        /// 
        /// </summary>
        protected PylonHandles Handles = new();

        /// <summary>
        /// 画像取得スレッド
        /// </summary>
        protected QueueThread<AcquisitionControlInfo> AcquisitionThread;

        /// <summary>
        /// 画像取得制御情報
        /// </summary>
        protected AcquisitionControlInfo? ControlInfo;

        protected PixelDataConverter PixelDataConverter = new();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PylonGrabber() : this(typeof(PylonGrabber).Name, Common.Drawing.Point.New())
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PylonGrabber(Common.Drawing.Point location) : this(typeof(PylonGrabber).Name, location)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="nickname"></param>
        /// <param name="location"></param>
        public PylonGrabber(string nickname, Common.Drawing.Point location) : base(nickname, location)
        {
            this.AcquisitionThread = new()
            {
                Priority = ThreadPriority.Highest
            };
            this.AcquisitionThread.Dequeue += this.AcquisitionThread_Dequeue;
        }

        #endregion

        /// <summary>
        /// イベント:GrabStarted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StreamGrabber_GrabStarted(object? sender, EventArgs e)
        {
            try
            {
                Serilog.Log.Debug("device grab started.");
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// イベント:GrabStopped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void StreamGrabber_GrabStopped(object? sender, GrabStopEventArgs e)
        {
            try
            {
                Serilog.Log.Debug("device grab stopped.");
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// イベント:CameraClosed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Device_CameraClosed(object? sender, EventArgs e)
        {
            try
            {
                Serilog.Log.Information("device closed.");
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// イベント:CameraOpened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Device_CameraOpened(object? sender, EventArgs e)
        {
            try
            {
                Serilog.Log.Information("device opend.");
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// イベント:ConnectionLost
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Device_ConnectionLost(object? sender, EventArgs e)
        {
            try
            {
                Serilog.Log.Error($"device lost (id = {this.DeviceID})");
                this.Close();
                this.OnDisabled();
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// イベント:ImageGrabbed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void StreamGrabber_ImageGrabbed(object? sender, ImageGrabbedEventArgs e)
        {
            try
            {
                Serilog.Log.Verbose("device image grabbed.");

                // The grab result is automatically disposed when the event call back returns.
                // The grab result can be cloned using IGrabResult.Clone if you want to keep a copy of it (not shown in this sample).
                var grabResult = e.GrabResult;

                // Image grabbed successfully?
                if (true == grabResult.GrabSucceeded)
                {
                    this.ControlInfo?.OnIDataStreamNewBuffer(grabResult.Clone());
                }
                else
                {
                    Serilog.Log.Warning($"error: {grabResult.ErrorCode} {grabResult.ErrorDescription}");
                    this.ControlInfo?.OnIDataStreamNewBuffer(grabResult.Clone());
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
                if (null != this.Handles.Device && this.Handles.Device.IsConnected)
                {
                    if (info.NumberOfImage < 0)
                    {
                        this.Handles.Device.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
                    }
                    else
                    {
                        this.Handles.Device.StreamGrabber.Start(info.NumberOfImage, GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
                    }

                    var imageCounter = 0UL;

                    var stopwatch = new Stopwatch();

                    while (info.IsContinuable)
                    {
                        // 画像取得(コールバック)を待機する
                        var bufferImages = info.GetDataStreamNewBuffer();

                        // 画像バッファからデータを列挙
                        foreach (var grabResult in bufferImages)
                        {
                            try
                            {
                                if (false == grabResult.GrabSucceeded)
                                {
                                    break;
                                }

                                stopwatch.Restart();

                                var bitmap = (Bitmap?)null;
                                var bytes = Array.Empty<byte>();
                                var stride = 0;

                                if (true == this.Handles.IsColor)
                                {
                                    bitmap = new Bitmap(grabResult.Width, grabResult.Height, PixelFormat.Format24bppRgb);

                                    // Lock the bits of the bitmap.
                                    var bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

                                    stride = bmpData.Stride;

                                    try
                                    {
                                        this.PixelDataConverter.OutputPixelFormat = PixelType.BGR8packed;

                                        // Place the pointer to the buffer of the bitmap.
                                        var ptrBmp = bmpData.Scan0;
                                        this.PixelDataConverter.Convert(ptrBmp, bmpData.Stride * bitmap.Height, grabResult);
                                    }
                                    catch (Exception ex)
                                    {
                                        Serilog.Log.Warning(ex, ex.Message);
                                    }
                                    finally
                                    {
                                        bitmap.UnlockBits(bmpData);
                                    }
                                }
                                else
                                {
                                    if (grabResult.PixelData is byte[] imageData)
                                    {
                                        bytes = imageData;
                                    }

                                    stride = grabResult.Width;

                                    BitmapProcessor.ConvertToBitmap(bytes, grabResult.Width, grabResult.Height, out bitmap);
                                }

                                // 撮影データを生成
                                var result = new ByteArrayGrabberDataFastBitmapGeneration(location)
                                {

                                    Counter = ++imageCounter
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
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                            finally
                            {
                                grabResult.Dispose();
                            }
                        }
                    }
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
                try
                {
                    this.Handles.Device?.StreamGrabber.Stop();

                    this.DisposeSafely(info);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }
        }
    }
}