using Hutzper.Library.Common.Drawing;
using Hutzper.Library.ImageProcessing.Geometry;
using Hutzper.Library.Onnx.Data;
using Hutzper.Library.Onnx.PostProcessing;
using Hutzper.Sample.Mekiki.ScratchLegacy.Data;
using Hutzper.Sample.Mekiki.ScratchLegacy.Data.Inspection;
using System.Diagnostics;

namespace Hutzper.Sample.Mekiki.ScratchLegacy.Inspection.PostProcessing
{
    /// <summary>
    /// セグメンテーション用後処理
    /// </summary>
    public class SegmentationPostProcessing : InferenceResultPostProcessing
    {
        #region フィールド

        /// <summary>
        /// ブロブ検出
        /// </summary>
        private BlobDetection BlobDetection = new();

        /// <summary>
        /// パラメータ
        /// </summary>
        private SegmentationPostProcessingParameter Parameter = new();

        #endregion

        #region IInferenceResultPostProcessing

        /// <summary>
        /// 初期化
        /// </summary>
        public override void Initialize(IInferenceResultPostProcessingParameter parameter)
        {
            try
            {
                if (parameter is SegmentationPostProcessingParameter sp)
                {
                    this.Parameter = sp;

                    var blobDetectionParameter = new BlobDetectionParameter();
                    blobDetectionParameter.MaxNumberOfClasses = sp.MaxNumberOfClasses;
                    blobDetectionParameter.RleControllerParameter.MaximumLineNumber = sp.MaximumImageHeight;
                    blobDetectionParameter.RleControllerParameter.MaximumLabelNumber = sp.MaximumLabelNumber;
                    blobDetectionParameter.RleControllerParameter.MaximumRleNumber = sp.MaximumRegionNumber;

                    this.BlobDetection.SetParameter(blobDetectionParameter);
                    this.BlobDetection.Open();
                }
                else
                {
                    throw new ArgumentException("invalid parameter type.");
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 実行
        /// </summary>
        public override bool ExcecutePostProcessing(IInspectionTaskItem selectedTaskItem)
        {
            var isSuccess = false;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // セグメンテーション結果の取得
                if (selectedTaskItem.InferenceResult is IOnnxDataOutputClassIndexed dataIndexed)
                {
                    // 結果格納用
                    var additionalData = new SegmentationAdditionalData();

                    // ラベリング実行
                    var allLabels = this.BlobDetection.LabelingWithRle(dataIndexed);

                    // クラス別にラベルを取り出す
                    foreach (var classIndex in Enumerable.Range(0, allLabels.Length))
                    {
                        // 検出ラベル情報を初期化
                        var validLabels = new List<LabelInfo>();

                        // 背景以外のクラスの場合
                        if (0 < classIndex)
                        {
                            // 検出ラベル情報をリストに追加する
                            foreach (var rawLabel in allLabels[classIndex])
                            {
                                // Hi画素のみ対象にする
                                if (0 == rawLabel.RleDataElem)
                                {
                                    continue;
                                }

                                // 基本ラベル情報
                                var newLabel = new LabelInfo
                                {
                                    ClassIndex = classIndex,
                                    Area = rawLabel.PixelNumber,
                                    Rect = new Library.Common.Drawing.Rectangle(rawLabel.RectBegin, rawLabel.RectSize),
                                };

                                // 塗りつぶし用のデータ
                                this.BlobDetection.GetLabelRunLength(rawLabel, classIndex, out List<RunLength> listRunLength);
                                newLabel.RunLength = listRunLength;

                                // 回転矩形
                                newLabel.RotatedRect = GeometryUtilities.GetSmallestRectangle2(listRunLength);

                                // 検出ラベル情報を追加
                                validLabels.Add(newLabel);
                            }
                        }

                        // クラス別ラベル情報を追加(面積値で降順ソートしておく)
                        additionalData.LabelsPerClass.Add(validLabels.OrderByDescending(x => x.Area).ToList());
                    }

                    // 後処理結果を格納
                    selectedTaskItem.AdditionalData = additionalData;

                    // 推論画像サイズ
                    additionalData.ClassFrameSize = dataIndexed.ClassFrameSize.Clone();
                    additionalData.RawImageSize = additionalData.ClassFrameSize.Clone();
                    if (selectedTaskItem.Bitmap is Bitmap sourceImage)
                    {
                        // 元画像サイズ
                        additionalData.RawImageSize = new Library.Common.Drawing.Size(sourceImage.Width, sourceImage.Height);
                    }

                    // 画像サイズ比
                    additionalData.FitToOriginalScale = additionalData.RawImageSize.Width / (double)additionalData.ClassFrameSize.Width;
                    if (additionalData.RawImageSize.Height > additionalData.RawImageSize.Width)
                    {
                        additionalData.FitToOriginalScale = additionalData.RawImageSize.Height / (double)additionalData.ClassFrameSize.Height;
                    }

                    // 画像サイズ比に合わせたパディング
                    additionalData.FitToOriginalPadding.Width = (additionalData.RawImageSize.Width - additionalData.ClassFrameSize.Width * additionalData.FitToOriginalScale) / 2;
                    additionalData.FitToOriginalPadding.Height = (additionalData.RawImageSize.Height - additionalData.ClassFrameSize.Height * additionalData.FitToOriginalScale) / 2;
                }
                else
                {
                    throw new ArgumentException("invalid inference result type.");
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
                Serilog.Log.Information($"{this}, task no={selectedTaskItem.ParentTask.TaskIndex:D7}, camera={selectedTaskItem.GrabberIndex + 1}, pp={stopwatch.ElapsedMilliseconds}ms");
            }

            return isSuccess;
        }

        #endregion

        #region ILoggable<LogCategory>

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="logger"></param>
        public override void Attach(string nickname, int index)
        {
            base.Attach(nickname, index);
        }

        #endregion

        #region コンストラクタ


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SegmentationPostProcessing() : this(typeof(SegmentationPostProcessing).Name, 0)
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SegmentationPostProcessing(string nickname, int index) : base(nickname, index)
        {
        }

        #endregion
    }
}
