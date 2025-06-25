using Basler.Pylon;
using Hutzper.Library.Common.Data;
using Hutzper.Library.ImageGrabber.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Hutzper.Library.ImageGrabber.Device.Pylon
{
    /// <summary>
    /// Pylonラインセンサ画像取得
    /// </summary>
    /// <remarks>2024.10.03 raL8192-12gmにて動作確認</remarks>
    public class PylonLineSensorGrabber : PylonGrabber, ILineSensor
    {
        #region ILineSensor

        /// <summary>
        /// X反転
        /// </summary>
        public virtual bool ReverseX
        {
            get
            {
                var result = false;
                if (true == this.GetValue<bool?>(PLCamera.ReverseX, GetValueInfo.IsContain))
                {
                    result = this.GetValue<bool>(PLCamera.ReverseX);
                }

                return result;
            }

            set
            {
                try
                {
                    if (true == this.GetValue<bool?>(PLCamera.ReverseX, GetValueInfo.IsContain))
                    {
                        this.SetValue(PLCamera.ReverseX, value);
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }
        }

        /// <summary>
        /// ラインレート
        /// </summary>
        public virtual double LineRateHz
        {
            get
            {
                var value = 0d;

                try
                {
                    if (true == this.GetValue<bool?>(PLCamera.AcquisitionLineRateAbs, GetValueInfo.IsContain))
                    {
                        value = this.GetValue<double>(PLCamera.AcquisitionLineRateAbs);
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }

                return value;
            }

            set
            {
                try
                {
                    if (this.Handles.Device is not null)
                    {
                        if (true == this.GetValue<bool?>(PLCamera.AcquisitionLineRateAbs, GetValueInfo.IsContain))
                        {
                            this.SetValue(PLCamera.AcquisitionLineRateAbs, value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }
        }

        #endregion

        #region IGrabber

        /// <summary>
        /// 露光時間μ秒
        /// </summary>
        public override double ExposureTimeMicroseconds
        {
            get
            {
                var value = 0d;

                if (true == this.GetValue<bool?>(PLCamera.ExposureTimeAbs, GetValueInfo.IsContain))
                {
                    value = this.GetValue<double?>(PLCamera.ExposureTimeAbs) ?? value;
                }

                return value;
            }

            set
            {
                if (this.Handles.Device is not null)
                {
                    if (true == this.GetValue<bool?>(PLCamera.ExposureTimeAbs, GetValueInfo.IsContain))
                    {
                        var valueMin = this.GetValue<double>(PLCamera.ExposureTimeAbs, GetValueInfo.Minimum);
                        var valueMax = this.GetValue<double>(PLCamera.ExposureTimeAbs, GetValueInfo.Maximum);
                        var valueClamped = Math.Clamp(value, valueMin, valueMax);

                        this.SetValue(PLCamera.ExposureTimeAbs, valueClamped);
                    }
                }
            }
        }

        /// <summary>
        /// アナログゲイン
        /// </summary>
        /// <remarks>raL8192-12gm はアナログゲイン変更不可</remarks>
        public override double AnalogGain
        {
            get
            {
                var value = 0d;

                #region AnalogGainは読み取りだけ出来るので一応コードだけ残しておく
                //if (true == this.GetValue<bool?>(PLCamera.GainRaw, GetValueInfo.IsContain))
                //{
                //    this.SetValue(PLCamera.GainSelector, PLCamera.GainSelector.AnalogAll);
                //    value = this.GetValue<long>(PLCamera.GainRaw);
                //}
                #endregion

                value = this.DigitalGain;

                return value;
            }

            set => this.DigitalGain = value;
        }

        /// <summary>
        /// デジタルゲイン
        /// </summary>
        public override double DigitalGain
        {
            get
            {
                var value = 0d;

                if (true == this.GetValue<bool?>(PLCamera.GainRaw, GetValueInfo.IsContain))
                {
                    this.SetValue(PLCamera.GainSelector, PLCamera.GainSelector.DigitalAll);
                    value = this.GetValue<long>(PLCamera.GainRaw);
                }

                return value;
            }

            set
            {
                if (this.Handles.Device is not null)
                {
                    if (true == this.GetValue<bool?>(PLCamera.GainRaw, GetValueInfo.IsContain))
                    {
                        this.SetValue(PLCamera.GainSelector, PLCamera.GainSelector.DigitalAll);

                        var valueMin = this.GetValue<long>(PLCamera.GainRaw, GetValueInfo.Minimum);
                        var valueMax = this.GetValue<long>(PLCamera.GainRaw, GetValueInfo.Maximum);
                        var valueClamped = Math.Clamp((long)value, valueMin, valueMax);

                        this.SetValue(PLCamera.GainRaw, valueClamped);
                    }
                }
            }
        }

        /// <summary>
        /// Yオフセット
        /// </summary>
        public override int OffsetY
        {
            get => 0;
            set
            {
            }
        }

        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            try
            {
                if (base.Open())
                {
                    if (null != this.Handles.Device)
                    {
                        if (this.Parameter is ILineSensorParameter gp)
                        {
                            this.ReverseX = gp.ReverseX;
                            this.LineRateHz = gp.LineRateHz;
                        }

                        if (true == this.IsRgbColor)
                        {
                            this.JaggedArrayConvertor = this.ToJaggedArrayRgb24;
                        }
                        else
                        {
                            this.JaggedArrayConvertor = this.ToJaggedArrayMono8;
                        }
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
        /// AW実行
        /// </summary>
        /// <returns></returns>
        /// <remarks>非対応</remarks>
        public override bool DoAutoWhiteBalancing()
        {
            var isSuccess = false;

            try
            {
                Serilog.Log.Warning("AWB is not supported.");
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
        /// パラメーター設定
        /// </summary>
        /// <param name="parameter"></param>
        public override void SetParameter(IControllerParameter? parameter)
        {
            try
            {
                base.SetParameter(parameter);

                if (parameter is ILineSensorParameter gp)
                {
                    this.ReverseX = gp.ReverseX;
                    this.LineRateHz = gp.LineRateHz;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        #endregion

        #region PylonGrabber

        /// <summary>
        /// 画像取得スレッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="info"></param>
        protected override void AcquisitionThread_Dequeue(object sender, AcquisitionControlInfo info)
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

                                // 画像データ変換
                                var bytes = this.JaggedArrayConvertor(grabResult, out Common.Drawing.Size imageSize, out PixelFormat pixelFormat);

                                // 撮影データを生成
                                var result = new ByteJaggedArrayGrabberData(location)
                                {
                                    Counter = imageCounter
                                ,
                                    Image = bytes
                                ,
                                    Size = imageSize
                                ,
                                    PixelFormat = pixelFormat
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

        #endregion

        /// <summary>
        /// 配列データへの変換
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        protected delegate byte[][] ToJaggedArrayDelegate(IGrabResult grabResult, out Common.Drawing.Size imageSize, out PixelFormat pixelFormat);
        protected ToJaggedArrayDelegate JaggedArrayConvertor;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PylonLineSensorGrabber()
        {
            this.GrabberType = GrabberType.LineSensor;
            this.JaggedArrayConvertor = this.ToJaggedArrayMono8;
        }

        /// <summary>
        /// 配列データへの変換:Mono8
        /// </summary>
        /// <param name="grabResult"></param>
        /// <param name="imageSize"></param>
        /// <param name="pixelFormat"></param>
        /// <returns>得られた画像の配列</returns>
        protected virtual byte[][] ToJaggedArrayMono8(IGrabResult grabResult, out Common.Drawing.Size imageSize, out PixelFormat pixelFormat)
        {
            var jaggedArray = Array.Empty<byte[]>();

            imageSize = new Common.Drawing.Size(grabResult.Width, grabResult.Height);
            pixelFormat = PixelFormat.Format8bppIndexed;

            try
            {
                jaggedArray = new byte[imageSize.Height][];

                if (grabResult.PixelData is byte[] oneDimBytes)
                {
                    var sourceIndex = 0;
                    foreach (var i in Enumerable.Range(0, imageSize.Height))
                    {
                        jaggedArray[i] = new byte[imageSize.Width];
                        Array.Copy(oneDimBytes, sourceIndex, jaggedArray[i], 0, jaggedArray[i].Length);

                        sourceIndex += imageSize.Width;
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return jaggedArray;
        }

        /// <summary>
        /// 配列データへの変換:RGB24
        /// </summary>
        /// <param name="grabResult"></param>
        /// <param name="imageSize"></param>
        /// <param name="pixelFormat"></param>
        /// <returns>得られた画像の配列</returns>
        /// <remarks>★未テスト（テスト用カラーカメラが無いのでとりあえずメソッド準備）</remarks>
        protected virtual byte[][] ToJaggedArrayRgb24(IGrabResult grabResult, out Common.Drawing.Size imageSize, out PixelFormat pixelFormat)
        {
            var jaggedArray = Array.Empty<byte[]>();

            imageSize = new Common.Drawing.Size(grabResult.Width, grabResult.Height);
            pixelFormat = PixelFormat.Format24bppRgb;

            try
            {
                jaggedArray = new byte[imageSize.Height][];

                var bitsPerPixel = Image.GetPixelFormatSize(pixelFormat);
                var stride = (grabResult.Width * bitsPerPixel + 7) / 8;
                var oneDimBytes = new byte[stride * imageSize.Height];

                var handle = GCHandle.Alloc(oneDimBytes, GCHandleType.Pinned);
                try
                {
                    var pointer = handle.AddrOfPinnedObject();

                    this.PixelDataConverter.OutputPixelFormat = PixelType.BGR8packed;
                    this.PixelDataConverter.Convert(pointer, oneDimBytes.LongLength, grabResult);
                }
                finally
                {
                    handle.Free();
                }

                var sourceIndex = 0;
                var packedWidth = imageSize.Width * 3;
                foreach (var i in Enumerable.Range(0, imageSize.Height))
                {
                    jaggedArray[i] = new byte[packedWidth];
                    Array.Copy(oneDimBytes, sourceIndex, jaggedArray[i], 0, jaggedArray[i].Length);

                    sourceIndex += packedWidth;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return jaggedArray;
        }
    }
}