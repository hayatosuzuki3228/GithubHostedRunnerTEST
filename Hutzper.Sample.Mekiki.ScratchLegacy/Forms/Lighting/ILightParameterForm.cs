using Hutzper.Library.ImageGrabber.Device;
using Hutzper.Library.LightController.Device;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Lighting
{
    public interface ILightParameterForm
    {
        /// <summary>
        /// パラメータ変更イベント
        /// </summary>
        public event Action<object, LightParameterSet>? ParameterChanged;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="items">対象とするIGrabberとIGrabberParameterのセット</param>
        public void Initialize(params LightParameterSet[] items);

        /// <summary>
        /// 指定したIGrabberに対応するビューに切り替える
        /// </summary>
        /// <param name="grabber"></param>
        public void ChangeView(IGrabber Device);

        /// <summary>
        /// 指定したインデックスに対応するビューに切り替える
        /// </summary>
        /// <param name="index"></param>
        /// <remarks>初期化時に登録した順番になります</remarks>
        public void ChangeView(int index);

        /// <summary>
        /// 全てのLightParameterSetを取得する
        /// </summary>
        /// <returns></returns>
        public List<LightParameterSet> GetParameterSet();

        /// <summary>
        /// 指定したILightControllerに対応するLightParameterSetを取得する
        /// </summary>
        /// <param name="grabber">ILightControllerを指定する</param>
        /// <returns>指定されたILightControllerに対応するパラメータ</returns>
        public LightParameterSet? GetParameterSet(ILightController device);

        /// <summary>
        /// 指定したILightControllerに対応するILightControllerParameterを取得する
        /// </summary>
        /// <param name="grabber">ILightControllerを指定する</param>
        /// <returns>指定されたILightControllerに対応するパラメータ</returns>
        public ILightControllerParameter? GetLightParameter(ILightController device);

        /// <summary>
        /// 指定したILightControllerに対応するIBehaviorOptionsを取得する
        /// </summary>
        /// <param name="grabber">ILightControllerを指定する</param>
        /// <returns>指定されたILightControllerに対応するパラメータ</returns>
        public IBehaviorOptions? GetBehaviorOptions(ILightController device);

        /// <summary>
        /// 画面の取得
        /// </summary>
        public Form? GetForm();
    }
}
