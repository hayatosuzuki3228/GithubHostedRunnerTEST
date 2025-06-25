using Hutzper.Library.ImageProcessing.Geometry;
using Hutzper.Sample.Mekiki.ScratchLegacy.Data;
using Hutzper.Sample.Mekiki.ScratchLegacy.Data.Inspection;
using System.Diagnostics;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.Judgement
{
    /// <summary>
    /// セグメンテーション判定ロジック実装サンプル
    /// </summary>
    public class SampleJudgement : InferenceResultJudgment
    {
        #region InferenceResultJudgment

        /// <summary>
        /// 実行
        /// </summary>
        /// <param name="selectedTask">対象のタスクデータ</param>
        /// <returns>処理に成功/失敗</returns>
        public override bool ExcecuteJudgment(IInspectionTask selectedTask, IInferenceResultJudgmentParameter parameter)
        {
            var isSuccess = false;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (parameter is not SampleJudgementParameter param)
                {
                    throw new Exception($"task no={selectedTask.TaskIndex:D7}, {nameof(parameter)} is invalid parameter");
                }

                if (param.JudgementClass is null)
                {
                    throw new Exception($"task no={selectedTask.TaskIndex:D7}, {nameof(param.JudgementClass)} is undefined");
                }

                // クラス名の定義
                selectedTask.ResultClassNames = param.AllClassNames.ToList();

                // 結果格納域の確保
                foreach (var nam in selectedTask.ResultClassNames)
                {
                    selectedTask.GeneralValues.Add(0);
                }

                var isTaskNg = false;
                var totalNgCount = new Dictionary<int, int>();

                // カメラ台数分の処理を行う
                foreach (var item in selectedTask.Items)
                {
                    if (item is null)
                    {
                        continue;
                    }

                    var itemIndex = Array.IndexOf(selectedTask.Items, item);

                    // カメラ別クラス名の設定
                    item.ResultClassNames = param.JudgementClass.ClassNamesPerGrabber[itemIndex].ToList();

                    // カメラ別結果格納域の確保
                    foreach (var nam in item.ResultClassNames)
                    {
                        item.GeneralValues.Add(0);
                    }

                    // カメラ別の分解能
                    var resolution = param.ImageProperties[itemIndex].ResolutionMmPerPixel;

                    #region セグメンテーションの結果判定
                    if (item.AdditionalData is SegmentationAdditionalData sad)
                    {
                        if (sad.LabelsPerClass.Count != item.ResultClassNames.Count)
                        {
                            Serilog.Log.Warning($"{this}, task no={selectedTask.TaskIndex:D7}, class count mismatch. {sad.LabelsPerClass.Count}/ {item.ResultClassNames.Count}");
                        }

                        // しきい値を取得する(カメラ毎の分解能にあわせる)
                        var thresholdLengthPix = param.DiagonalLengthThresholdMm / resolution;

                        // 結合する距離のしきい値を取得する(カメラ毎の分解能にあわせる、かつ推論時のリサイズを考慮)
                        var thresholdMergePix = param.MergeDistanceThresholdMm / resolution / sad.FitToOriginalScale;

                        // 対角線長でしきい値判定する
                        foreach (var classIndex in Enumerable.Range(0, sad.LabelsPerClass.Count))
                        {
                            // クラス毎のラベルを取得する
                            var classLabels = sad.LabelsPerClass[classIndex];

                            // 結合処理
                            classLabels = sad.LabelsPerClass[classIndex] = GeometryUtilities.MergeCloseResults(classLabels, thresholdMergePix);

                            // ラベル毎の判定を行う
                            foreach (var label in classLabels)
                            {
                                // pixel特徴量を取得する
                                var majorAxisResizedPix = label.RotatedRect.Length1 * 2;
                                var minorAxisResizedPix = label.RotatedRect.Length2 * 2;
                                var diagonalResizedPix = Math.Sqrt(majorAxisResizedPix * majorAxisResizedPix + minorAxisResizedPix * minorAxisResizedPix);

                                // 元画像サイズにあわせる
                                var diagonalRawPix = diagonalResizedPix * sad.FitToOriginalScale;

                                // 対角線長さ(mm)を格納する
                                label.DiagonalLengthMm = diagonalResizedPix * sad.FitToOriginalScale * resolution;

                                // 長軸長さ(mm)を格納する
                                label.MajorAxisLengthMm = majorAxisResizedPix * sad.FitToOriginalScale * resolution;

                                // 短軸長さ(mm)を格納する
                                label.MinorAxisLengthMm = minorAxisResizedPix * sad.FitToOriginalScale * resolution;

                                // しきい値以上の場合はNGとする(この例では対角線長で判定している)
                                label.IsNg = thresholdLengthPix <= diagonalRawPix;

                                // タスク全体の判定結果を更新する
                                isTaskNg |= label.IsNg;
                            }

                            // 不良数をカウントする
                            var ngCount = classLabels.Count(x => x.IsNg);

                            // 項目別不良数を格納する
                            if (item.GeneralValues.Count > classIndex)
                            {
                                item.GeneralValues[classIndex] = ngCount;
                            }
                        }

                        // 最大値を持つクラスを判定クラスとする
                        var ngMax = item.GeneralValues.Max();
                        item.JudgementIndex = item.GeneralValues.IndexOf(ngMax);  // 0個ならインデックス0(ok)

                        // 不良数を全体の結果へ反映する
                        foreach (var c in item.ResultClassNames)
                        {
                            var globalClassIndex = selectedTask.ResultClassNames.IndexOf(c);
                            if (0 < globalClassIndex)
                            {
                                var localClassIndex = item.ResultClassNames.IndexOf(c);
                                selectedTask.GeneralValues[globalClassIndex] += item.GeneralValues[localClassIndex];
                            }
                        }
                    }
                    #endregion
                }

                // 不良検出あり
                if (true == isTaskNg)
                {
                    var ngMax = selectedTask.GeneralValues.Max();
                    selectedTask.JudgementIndex = selectedTask.GeneralValues.IndexOf(ngMax);
                }
                // 不良検出なし
                else
                {
                    selectedTask.JudgementIndex = selectedTask.ResultClassNames.IndexOf("ok");
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                stopwatch.Stop();
                Serilog.Log.Information($"{this}, task no={selectedTask.TaskIndex:D7}, judgment={stopwatch.ElapsedMilliseconds}ms");
            }

            return isSuccess;
        }

        #endregion
    }
}