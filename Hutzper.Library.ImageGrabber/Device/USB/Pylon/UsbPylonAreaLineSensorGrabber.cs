using Basler.Pylon;
using Hutzper.Library.ImageGrabber.Device.Pylon;

namespace Hutzper.Library.ImageGrabber.Device.USB.Pylon
{
    [Serializable]
    public class UsbPylonAreaLineSensorGrabber : PylonAreaLineSensorGrabber, IUsbDevice
    {
        public override bool Open()
        {
            base.Open();
            base.SetValue(PLCamera.PixelFormat, PLCamera.PixelFormat.RGB8); //デフォルト設定では黒い線が出ます
            return true;
        }
    }
}