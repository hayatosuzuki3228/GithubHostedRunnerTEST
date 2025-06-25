using Hutzper.Library.Common.Data;
using Hutzper.Library.LightController.Device;

namespace Hutzper.Library.LightController
{
    public interface ILightControllerSupervisorParameter : IControllerParameter
    {
        #region プロパティ

        /// <summary>
        /// デバイスパラメータ
        /// </summary>
        public List<ILightControllerParameter> DeviceParameters { get; }

        #endregion
    }
}