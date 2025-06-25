using Basler.Pylon;
using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;
using Hutzper.Library.ImageGrabber.Data;
using Hutzper.Library.ImageProcessing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Hutzper.Library.ImageGrabber.Device.Pylon;
public class PylonAreaExternalTriggerGrabber : PylonGrabber, IAreaSensor
{
    #region IAreaSensor

    /// <summary>
    /// フレームレート
    /// </summary>
    public virtual double FramesPerSecond
    {
        get => (base.GetValue<bool?>(PLCamera.AcquisitionFrameRateEnable, GetValueInfo.IsContain) ?? false) ?
            base.GetValue<long?>(PLCamera.AcquisitionFrameRateAbs) ?? 0d :
            base.GetValue<long?>(PLCamera.AcquisitionFrameRateAbs, GetValueInfo.Maximum) ?? 0d;

        set
        {
            if (this.Handles.Device is null) return;
            if (0 >= value) base.SetValue(PLCamera.AcquisitionFrameRateEnable, false);
            else
            {
                base.SetValue(PLCamera.AcquisitionFrameRateEnable, true);
                if (base.GetValue<bool>(PLCamera.AcquisitionFrameRateAbs, GetValueInfo.IsContain))
                {
                    base.SetValue(PLCamera.AcquisitionFrameRateAbs, System.Math.Clamp(value, this.GetValue<double>(PLCamera.AcquisitionFrameRateAbs, GetValueInfo.Minimum), this.GetValue<double>(PLCamera.AcquisitionFrameRateAbs, GetValueInfo.Maximum)));
                }
                else if (base.GetValue<bool>(PLCamera.AcquisitionFrameRate, GetValueInfo.IsContain))
                {
                    base.SetValue(PLCamera.AcquisitionFrameRate, System.Math.Clamp(value, this.GetValue<double>(PLCamera.AcquisitionFrameRate, GetValueInfo.Minimum), this.GetValue<double>(PLCamera.AcquisitionFrameRate, GetValueInfo.Maximum)));
                }
            }
        }
    }

    private ulong ImageCount = 0UL;

    #endregion

    #region IGrabber

    /// <summary>
    /// オープン
    /// </summary>
    /// <returns></returns>
    public override bool Open()
    {
        if (!base.Open()) return false;
        if (this.Handles.Device is null) return false;
        this.SetValue(PLCamera.TriggerSelector, PLCamera.TriggerSelector.FrameStart, true);
        if (this.Parameter is IAreaSensorParameter gp) this.FramesPerSecond = gp.FramesPerSecond;
        this.SetValue(PLCamera.TriggerSource, PLCamera.TriggerSource.Line1, true);
        this.SetValue(PLCamera.TriggerActivation, PLCamera.TriggerActivation.RisingEdge, true);

        return this.Enabled;
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
        if (true != this.Enabled) return;
        if (this.Parameter is IAreaSensorParameter gp) this.FramesPerSecond = gp.FramesPerSecond;
        this.SetValue(PLCamera.TriggerSource, PLCamera.TriggerSource.Line1, true);
        this.SetValue(PLCamera.TriggerActivation, PLCamera.TriggerActivation.RisingEdge, true);
    }

    /// <summary>
    /// パラメーター設定
    /// </summary>
    /// <param name="parameter"></param>
    public override void SetParameter(IControllerParameter? parameter)
    {
        base.SetParameter(parameter);
        if (parameter is IAreaSensorParameter gp) this.FramesPerSecond = gp.FramesPerSecond;
        this.SetValue(PLCamera.TriggerSource, PLCamera.TriggerSource.Line1, true);
        this.SetValue(PLCamera.TriggerActivation, PLCamera.TriggerActivation.RisingEdge, true);
    }

    #endregion

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PylonAreaExternalTriggerGrabber() => this.GrabberType = GrabberType.AreaSensor;
    protected override void AcquisitionThread_Dequeue(object sender, AcquisitionControlInfo info)
    {
        if (null != this.Handles.Device && this.Handles.Device.IsConnected) this.Handles.Device.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
    }
    protected override void StreamGrabber_ImageGrabbed(object? sender, ImageGrabbedEventArgs e)
    {
        try
        {
            IGrabResult grabResult = e.GrabResult;
            Bitmap? imageBitmap = null;
            byte[]? bytes = null;
            int stride = 0;
            // Image grabbed successfully?
            if (true == grabResult.GrabSucceeded)
            {

                if (true == this.Handles.IsColor)
                {
                    imageBitmap = new Bitmap(grabResult.Width, grabResult.Height, PixelFormat.Format32bppRgb);

                    // Lock the bits of the bitmap.
                    var bmpData = imageBitmap.LockBits(new Rectangle(0, 0, imageBitmap.Width, imageBitmap.Height), ImageLockMode.ReadWrite, imageBitmap.PixelFormat);
                    stride = bmpData.Stride;

                    try
                    {
                        this.PixelDataConverter.OutputPixelFormat = PixelType.BGRA8packed;

                        // Place the pointer to the buffer of the bitmap.
                        var ptrBmp = bmpData.Scan0;
                        this.PixelDataConverter.Convert(ptrBmp, bmpData.Stride * imageBitmap.Height, grabResult);
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        imageBitmap.UnlockBits(bmpData);
                    }
                    try
                    {
                        var convertedBitmap = new Bitmap(imageBitmap.Width, imageBitmap.Height, PixelFormat.Format24bppRgb);

                        using (var g = Graphics.FromImage(convertedBitmap))
                        {
                            g.PageUnit = GraphicsUnit.Pixel;
                            g.DrawImageUnscaled(imageBitmap, 0, 0);
                        };
                        using var previousBitmap = imageBitmap;
                        imageBitmap = convertedBitmap;
                        bmpData = imageBitmap.LockBits(new Rectangle(0, 0, imageBitmap.Width, imageBitmap.Height), ImageLockMode.ReadOnly, imageBitmap.PixelFormat);

                        bytes = new byte[bmpData.Stride * bmpData.Height];
                        Marshal.Copy(bmpData.Scan0, bytes, 0, bytes.Length);
                    }
                    catch (Exception ex)
                    {
                        Serilog.Log.Warning(ex, ex.Message);
                    }
                    finally
                    {
                        imageBitmap.UnlockBits(bmpData);
                    }
                }
                else
                {
                    if (grabResult.PixelData is byte[] imageData)
                    {
                        bytes = imageData;
                        stride = grabResult.Width;
                        BitmapProcessor.ConvertToBitmap(bytes, grabResult.Width, grabResult.Height, out imageBitmap);
                    }
                }

                // 撮影データを生成
                var result = new ByteArrayGrabberDataFastBitmapGeneration(location)
                {

                    Counter = ++ImageCount
                ,
                    Image = bytes!
                    ,
                    Stride = stride
                ,
                    Bitmap = imageBitmap
                ,
                    Size = new Common.Drawing.Size(imageBitmap?.Size ?? new Size())
                ,
                    PixelFormat = imageBitmap?.PixelFormat ?? PixelFormat.Format8bppIndexed
                };

                // 画像取得イベント通知
                this.OnDataGrabbed(result);
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
    private void Device_CameraOpened(object? sender, EventArgs e)
    {
        if (null != this.Handles.Device && this.Handles.Device.IsConnected) this.Handles.Device.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
    }
}