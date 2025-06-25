using Basler.Pylon;
using Hutzper.Library.ImageGrabber.Device.GigE;
using Hutzper.Library.ImageGrabber.Device.Pylon;

namespace Hutzper.Library.ImageGrabber.Device.USB.Pylon
{
    [Serializable]
    public class GigEPylonAreaLineSensorGrabber : PylonAreaLineSensorGrabber, IGigEDevice
    {
        public override bool Open()
        {
            base.Open();
            base.SetValue(PLCamera.PixelFormat, PLCamera.PixelFormat.YUV422Packed); //デフォルト設定では黒い線が出ます
            return true;
        }
    }
}