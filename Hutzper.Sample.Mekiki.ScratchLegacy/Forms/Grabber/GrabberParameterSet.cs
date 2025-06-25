using Hutzper.Library.Common.Imaging;
using Hutzper.Library.ImageGrabber.Device;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Grabber
{
    /// <summary>
    /// 管理対象のIGrabberとIGrabberParameterのセット
    /// </summary>
    public record GrabberParameterSet(IGrabber Device, IGrabberParameter GrabberParameter, IImageProperties ImageProperties, IBehaviorOptions BehaviorOptions, string Nickname);
}
