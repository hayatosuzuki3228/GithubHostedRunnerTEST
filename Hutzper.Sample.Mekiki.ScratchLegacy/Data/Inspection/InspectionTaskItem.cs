using Hutzper.Library.Common;
using Hutzper.Library.Onnx.Data;
using System.Diagnostics;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Data
{
    /// <summary>
    /// 検査タスク項目
    /// </summary>
    /// <remarks>カメラ単位のタスク</remarks>
    public class InspectionTaskItem : SafelyDisposable, IInspectionTaskItem
    {
        #region プロパティ

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
        public int JudgementIndex { get; set; } = -1;

        /// <summary>
        /// 結果クラス名リスト
        /// </summary>
        public List<string> ResultClassNames { get; set; } = new();

        /// <summary>
        /// 汎用格納域
        /// </summary>
        public List<double> GeneralValues { get; set; } = new();

        /// <summary>
        /// 画像保存に成功したかどうか
        /// </summary>
        /// <remarks>保存が必要で実行した場合の結果(保存を実行しない場合はtrue)</remarks>
        public bool ImageSaveSucceeded { get; set; } = true;

        /// <summary>
        /// 追加データ
        /// </summary>
        public IAdditionalDataContainer? AdditionalData { get; set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InspectionTaskItem(IInspectionTask parentTask, int grabberIndex, Bitmap? bitmap)
        {
            this.ParentTask = parentTask;
            this.GrabberIndex = grabberIndex;
            this.Bitmap = bitmap;
        }

        #endregion

        #region SafelyDisposable

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
                this.DisposeSafely(this.Bitmap);
                this.DisposeSafely(this.InferenceResult);
                this.DisposeSafely(this.AdditionalData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                this.Bitmap = null;
                this.InferenceResult = null;
                this.AdditionalData = null;
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// シャローコピー
        /// </summary>
        public virtual IInspectionTaskItem ShallowCopy() => (IInspectionTaskItem)this.MemberwiseClone();

        #endregion
    }
}