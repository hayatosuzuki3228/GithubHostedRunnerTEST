using Basler.Pylon;
using Hutzper.Library.ImageGrabber.Data;
using Hutzper.Library.ImageGrabber.Device.USB.Pylon;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Hutzper.Library.ImageGrabber.Device.Pylon
{
    public class PylonAreaLineSensorGrabber : PylonAreaSensorGrabber
    {
        public PylonAreaLineSensorGrabber() => this.GrabberType = GrabberType.LineSensor;
        private int LineHeight;
        private GrabInfo ThisGrabInfo = GrabInfo.None;
        private ulong GrabCount = 0LU;
        private Stopwatch Stopwatch = new();
        private byte[]? MargeBytes = null;
        private int OnePieceByteCount = 0;
        private int PieceCount = 0;
        private int ALLPieceCount = 0;
        private byte[]? GrabBytes = null;
        private enum GrabInfo
        {
            None,
            Open,
            Idle,
            Running,
            Close,
        }

        /// <summary>
        /// ソフトトリガー
        /// </summary>
        /// <returns></returns>
        public override bool DoSoftTrigger()
        {
            bool isSuccess = false;

            if (this.Handles.Device is not null && this.ThisGrabInfo == GrabInfo.Idle)
            {
                this.TriggerMode = TriggerMode.InternalTrigger;
                base.SetValue(PLCamera.CenterY, true);
                this.GrabContinuously((int)(this.LineHeight / this.Height));
                this.Stopwatch.Restart();
                this.ThisGrabInfo = GrabInfo.Running;
                isSuccess = true;
            }
            return isSuccess;
        }
        public void StopGrabOrReconnect()
        {
            if (null != this.Handles.Device && this.Handles.Device.IsConnected) this.Handles.Device.StreamGrabber.Stop();
            else
            {
                this.Close();
                this.Open();
            }
        }
        public override bool Open()
        {
            bool result = base.Open();
            if (!result) return false;
            if (this.Parameter is UsbPylonAreaLineSensorGrabberParameter param) this.LineHeight = param.LineHeight;
            else if (this.Parameter is GigEPylonAreaLineSensorGrabberParameter param2) this.LineHeight = param2.LineHeight;
            this.ALLPieceCount = LineHeight / this.Height;
            this.Handles.Device?.Parameters[PLCameraInstance.MaxNumBuffer].SetValue((int)(this.LineHeight / this.Height));
            this.ThisGrabInfo = GrabInfo.Idle;
            this.PixelDataConverter.OutputPixelFormat = PixelType.BGR8packed;
            this.OnePieceByteCount = this.Width * this.Height * 3;
            this.GrabBytes = new byte[this.OnePieceByteCount];
            this.MargeBytes = new byte[this.Width * LineHeight * 3];
            return true;
        }
        public override bool Close()
        {
            this.ThisGrabInfo = GrabInfo.Close;
            return base.Close();
        }
        protected override void StreamGrabber_ImageGrabbed(object? sender, ImageGrabbedEventArgs e)
        {
            try
            {
                IGrabResult grabResult = e.GrabResult;
                if (true == grabResult.GrabSucceeded)
                {
                    this.PixelDataConverter.Convert(this.GrabBytes, grabResult);
                    Buffer.BlockCopy(this.GrabBytes!, 0, this.MargeBytes!, this.PieceCount++ * this.OnePieceByteCount, this.OnePieceByteCount);
                    grabResult.Dispose();
                }
                else
                {
                    Serilog.Log.Warning($"error: {grabResult.ErrorCode} {grabResult.ErrorDescription}");
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
                this.OnErrorOccurred(new GrabberErrorBase(location));
            }
        }
        protected override void StreamGrabber_GrabStopped(object? sender, GrabStopEventArgs e)
        {
            try
            {
                this.ThisGrabInfo = GrabInfo.Idle;
                long garbTime = this.Stopwatch.ElapsedMilliseconds;
                try
                {
                    if (this.PieceCount == this.ALLPieceCount)
                    {
                        Bitmap outputBitmap = this.ByteArrayToBitmap();
                        // 撮影データを生成
                        ByteArrayGrabberDataFastBitmapGeneration result = new(location)
                        {
                            Counter = ++GrabCount
                        ,
                            Image = Array.Empty<byte>()
                        ,
                            Stride = 0
                        ,
                            Bitmap = outputBitmap
                        ,
                            Size = new Common.Drawing.Size(outputBitmap?.Size ?? new())
                        ,
                            PixelFormat = outputBitmap?.PixelFormat ?? PixelFormat.Format8bppIndexed
                        };
                        long combineTime = this.Stopwatch.ElapsedMilliseconds - garbTime;
                        this.OnDataGrabbed(result);
                        Serilog.Log.Information($"Area line camera garb time is {garbTime} Combine time is {combineTime}");
                        return;
                    }
                    else
                    {
                        // 撮影データを生成
                        ByteArrayGrabberDataFastBitmapGeneration result = new(location)
                        {
                            Counter = ++GrabCount
                        ,
                            Image = Array.Empty<byte>()
                        ,
                            Stride = 0
                        ,
                            Bitmap = null
                        ,
                            Size = new Common.Drawing.Size(this.Width, this.LineHeight)
                        ,
                            PixelFormat = PixelFormat.Format8bppIndexed
                        };
                        this.OnDataGrabbed(result);
                    }
                }
                catch (Exception taskE)
                {
                    Serilog.Log.Warning(taskE, taskE.Message);
                    this.OnErrorOccurred(new GrabberErrorBase(location));
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
                this.OnErrorOccurred(new GrabberErrorBase(location));
            }
        }
        protected override void AcquisitionThread_Dequeue(object sender, AcquisitionControlInfo info)
        {
            try
            {
                if (null != this.Handles.Device && this.Handles.Device.IsConnected)
                {
                    this.PieceCount = 0;
                    if (info.NumberOfImage > 0) this.Handles.Device.StreamGrabber.Start(info.NumberOfImage, GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
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
                    this.DisposeSafely(info);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Warning(ex, ex.Message);
                }
            }
        }
        private Bitmap ByteArrayToBitmap()
        {
            Bitmap bitmap = new(this.Width, this.Height * this.ALLPieceCount, PixelFormat.Format24bppRgb);
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb
            );
            Marshal.Copy(this.MargeBytes!, 0, bmpData.Scan0, this.MargeBytes!.Length);
            bitmap.UnlockBits(bmpData);
            return bitmap;
        }
    }
}

