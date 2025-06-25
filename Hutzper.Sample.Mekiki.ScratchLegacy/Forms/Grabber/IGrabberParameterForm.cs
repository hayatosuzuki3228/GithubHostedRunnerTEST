using Hutzper.Library.Common.Imaging;
using Hutzper.Library.ImageGrabber.Device;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Forms.Grabber
{
    public interface IGrabberParameterForm
    {
        /// <summary>
        /// パラメータ変更イベント
        /// </summary>
        public event Action<object, GrabberParameterSet>? ParameterChanged;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="items">対象とするIGrabberとIGrabberParameterのセット</param>
        public void Initialize(params GrabberParameterSet[] items);

        /// <summary>
        /// 指定したIGrabberに対応するビューに切り替える
        /// </summary>
        /// <param name="grabber"></param>
        public void ChangeView(IGrabber device);

        /// <summary>
        /// 指定したインデックスに対応するビューに切り替える
        /// </summary>
        /// <param name="index"></param>
        /// <remarks>初期化時に登録した順番になります</remarks>
        public void ChangeView(int index);

        /// <summary>
        /// 表示更新
        /// </summary>
        /// <remarks>外部でパラメータ変更を行った際に表示に反映させる</remarks>
        public void UpdateView();

        /// <summary>
        /// 全てのGrabberParameterSetを取得する
        /// </summary>
        /// <returns></returns>
        public List<GrabberParameterSet> GetParameterSet();

        /// <summary>
        /// 指定したIGrabberに対応するGrabberParameterSetを取得する
        /// </summary>
        /// <param name="grabber">IGrabberを指定する</param>
        /// <returns>指定されたIGrabberに対応するパラメータ</returns>
        public GrabberParameterSet? GetParameterSet(IGrabber device);

        /// <summary>
        /// 指定したIGrabberに対応するIGrabberParameterを取得する
        /// </summary>
        /// <param name="grabber">IGrabberを指定する</param>
        /// <returns>指定されたIGrabberに対応するパラメータ</returns>
        public IGrabberParameter? GetGrabberParameter(IGrabber device);

        /// <summary>
        /// 指定したIGrabberに対応するIImagingParameterを取得する
        /// </summary>
        /// <param name="grabber">IGrabberを指定する</param>
        /// <returns>指定されたIGrabberに対応するパラメータ</returns>
        public IImageProperties? GetImagingParameter(IGrabber device);

        /// <summary>
        /// 指定したIGrabberに対応するIBehaviorOptionsを取得する
        /// </summary>
        /// <param name="grabber">IGrabberを指定する</param>
        /// <returns>指定されたIGrabberに対応するパラメータ</returns>
        public IBehaviorOptions? GetBehaviorOptions(IGrabber device);

        /// <summary>
        /// 画面の取得
        /// </summary>
        public Form? GetForm();
    }
}
