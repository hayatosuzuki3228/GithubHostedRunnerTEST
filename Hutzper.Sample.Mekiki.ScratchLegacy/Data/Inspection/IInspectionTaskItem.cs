using Hutzper.Library.Common;
using Hutzper.Library.Onnx.Data;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Data
{
    public interface IInspectionTaskItem : ISafelyDisposable
    {
        /// <summary>
        /// 親タスク
        /// </summary>
        public IInspectionTask ParentTask { get; init; }

        /// <summary>
        /// カメラインデックス
        /// </summary>
        public int GrabberIndex { get; init; }

        /// <summary>
        /// 入力画像
        /// </summary>
        public Bitmap? Bitmap { get; set; }

        /// <summary>
        /// 推論結果
        /// </summary>
        public IOnnxDataOutput? InferenceResult { get; set; }

        /// <summary>
        /// 判定結果インデックス
        /// </summary>
        public int JudgementIndex { get; set; }

        /// <summary>
        /// 結果クラス名リスト
        /// </summary>
        public List<string> ResultClassNames { get; set; }


        /// <summary>
        /// 汎用格納域
        /// </summary>
        public List<double> GeneralValues { get; set; }

        /// <summary>
        /// 画像保存に成功したかどうか
        /// </summary>
        /// <remarks>保存が必要で実行した場合の結果(保存を実行しない場合はtrue)</remarks>
        public bool ImageSaveSucceeded { get; set; }

        /// <summary>
        /// 追加データ
        /// </summary>
        public IAdditionalDataContainer? AdditionalData { get; set; }

        /// <summary>
        /// シャローコピー
        /// </summary>
        /// <returns></returns>
        public IInspectionTaskItem ShallowCopy();
    }
}