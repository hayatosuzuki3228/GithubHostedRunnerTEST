using Hutzper.Library.Common.Data;
using Hutzper.Library.ImageGrabber.Device;

namespace Hutzper.Library.ImageGrabber
{
    public interface IGrabberControllerParameter : IControllerParameter
    {
        /// <summary>
        /// 画像取得パラメータ
        /// </summary>
        public List<IGrabberParameter> GrabberParameters { get; }
    }
}