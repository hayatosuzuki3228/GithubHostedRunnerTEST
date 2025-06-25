using Hutzper.Library.Common;
using Hutzper.Library.InsightLinkage;
using System.Diagnostics;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Data
{
    /// <summary>
    /// 検査タスクインタフェース
    /// </summary>
    /// <remarks>検査1回についての情報を格納する</remarks>
    public interface IInspectionTask : ISafelyDisposable
    {
        #region プロパティ

        /// <summary>
        /// 処理番号
        /// </summary>
        public int TaskIndex { get; }

        /// <summary>
        /// 汎用タグ
        /// </summary>
        public object? TaskTag { get; set; }

        /// <summary>
        /// 起点の日時
        /// </summary>
        /// <remarks>撮影トリガの立ち上がり</remarks>
        public DateTime DateTimeOrigin { get; }

        /// <summary>
        /// ストップウォッチ:起点からの経過時間
        /// </summary>
        public Stopwatch StopwatchFromOrigin { get; }

        /// <summary>
        /// タスクが有効かどうか
        /// </summary>
        public bool Enabled { get; }

        /// <summary>
        /// ストップウォッチ:物有りOFFからの経過時間
        /// </summary>
        public Stopwatch? StopwatchFromIoFalled { get; }

        /// <summary>
        /// ストップウォッチ:撮影完了からの経過時間        
        /// </summary>
        public Stopwatch? StopwatchFromGrabEnd { get; }

        /// <summary>
        /// 物有り信号がOFFしたかどうか
        /// </summary>
        public bool IsIoFalled { get; }

        /// <summary>
        /// 撮影が完了しているかどうか
        /// </summary>
        public bool IsGrabEnd { get; }

        /// <summary>
        /// 推論が完了しているかどうか
        /// </summary>
        public bool IsInferenceEnd { get; }

        /// <summary>
        /// 紐づく情報
        /// </summary>
        public string LinkedInfomation { get; set; }

        /// <summary>
        /// タスク項目
        /// </summary>
        public IInspectionTaskItem?[] Items { get; }
        /// <summary>
        /// 判定結果インデックス
        /// </summary>
        public int JudgementIndex { get; set; }

        /// <summary>
        /// 結果クラス名リスト
        /// </summary>
        public List<string> ResultClassNames { get; set; }

        /// <summary>
        /// 画像保存を行うかどうか
        /// </summary>
        public bool ImageSavingEnabled { get; set; }

        /// <summary>
        /// 画像保存に成功したかどうか
        /// </summary>
        /// <remarks>保存が必要で実行した場合の結果(保存を実行しない場合はtrue)</remarks>
        public bool ImageSaveSucceeded { get; }

        /// <summary>
        /// 汎用格納域
        /// </summary>
        public List<double> GeneralValues { get; set; }

        /// <summary>
        /// 全ての処理が完了したかどうか
        /// </summary>
        public bool IsTaskEnd { get; set; }

        /// <summary>
        /// メキキデータ
        /// </summary>
        public List<MekikiUnit> MekikiUnitList { get; init; }

        /// <summary>
        /// 撮影完了フラグ
        /// </summary>
        public bool[] FlagsOfGrabEnd { get; }

        /// <summary>
        /// 推論完了フラグ
        /// </summary>
        public bool[] FlagsInferenceEnd { get; }

        #endregion

        #region メソッド

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="taskIndex">検査順序を示す0から始まるインデックス</param>
        /// <param name="origin">検査開始日時(トリガ信号ON日時)</param>
        /// <param name="validGrabberNumber">有効な画像取得数</param>
        public void Initialize(int taskIndex, DateTime origin, Stopwatch stopwatchFromOrigin, int validGrabberNumber);

        /// <summary>
        /// 物有り信号OFFを待機する
        /// </summary>
        public bool WaitIoFalled(int timeoutMs, out bool isTimeout);

        /// <summary>
        /// 撮影完了を待機する
        /// </summary>
        public bool WaitGrabEnd();

        /// <summary>
        /// 処理完了を待機する
        /// </summary>
        public bool WaitInferenceEnd();

        /// <summary>
        /// タスク無効化
        /// </summary>
        public void Invalidate();

        /// <summary>
        /// 信号変化通知
        /// </summary>
        public void NotifyIoFalled(Stopwatch signalStopwatch);

        /// <summary>
        /// 撮影完了通知
        /// </summary>
        public IInspectionTaskItem? NotifyGrabEnd(int index, Bitmap? bitmap);

        /// <summary>
        /// 処理完了通知
        /// </summary>
        public void NotifyInferenceEnd(int index);

        /// <summary>
        /// 検査結果を生成する
        /// </summary>
        /// <remarks>このメソッドで検査結果を生成することで、GetInspectionResultで検査結果を取得できるようになります</remarks>
        public void CreateInspectionResult();

        /// <summary>
        /// 検査結果を取得する
        /// </summary>
        /// <remarks>CreateInspectionResultメソッドが事前に実行されている必要があります</remarks>
        public IInspectionResult? GetInspectionResult();

        /// <summary>
        /// シャローコピー
        /// </summary>
        public IInspectionTask ShallowCopy();

        #endregion
    }
}