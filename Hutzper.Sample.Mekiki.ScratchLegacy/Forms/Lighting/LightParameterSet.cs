using Hutzper.Library.LightController.Device;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Lighting
{
    /// <summary>
    /// 管理対象のILightControllerとILightControllerParameterのセット
    /// </summary>
    public record LightParameterSet(ILightController Device, ILightControllerParameter LightParameter, IBehaviorOptions BehaviorOptions, string Nickname);
}
