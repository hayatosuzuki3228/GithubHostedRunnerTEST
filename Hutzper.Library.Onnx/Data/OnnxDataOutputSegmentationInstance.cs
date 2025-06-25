using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace Hutzper.Library.Onnx.Data
{
    /// <summary>
    /// ONNN用出力データ:インスタンスセグメンテーション用
    /// </summary>
    [Serializable]
    public class OnnxDataOutputSegmentationInstance : OnnxDataOutput, IOnnxDataOutputClassIndexedInstance
    {
        #region IOnnxDataOutput

        /// <summary>
        /// モデル出力データ設定
        /// </summary>
        /// <param name="resultOfRun"></param>
        public override void CopyFrom(IDisposableReadOnlyCollection<DisposableNamedOnnxValue>? resultOfRun)
        {
            try
            {
                this.ClassIndexed.Clear();
                this.ClassCounts.Clear();

                base.CopyFrom(resultOfRun);

                if (this.RawCollection is not null)
                {
                    // クラス別検出個数
                    var header = this.RawCollection.Where(c => c.ElementType == TensorElementType.Int32).FirstOrDefault();
                    if (header?.AsTensor<Int32>() is Tensor<Int32> dataForCounts)
                    {
                        this.ClassCounts.AddRange(dataForCounts.ToArray());
                    }

                    // クラス別検出インデックス
                    var body = this.RawCollection.Where(c => c.ElementType == TensorElementType.UInt8).FirstOrDefault();
                    if (body?.AsTensor<byte>() is Tensor<byte> dataForIndexed)
                    {
                        var classNum = dataForIndexed.Dimensions[0];
                        var frameHeight = dataForIndexed.Dimensions[1];
                        var frameWidth = dataForIndexed.Dimensions[2];

                        this.ClassFrameSize = new Common.Drawing.Size(frameWidth, frameHeight);

                        var allBytes = dataForIndexed.ToArray();
                        var frameLength = frameHeight * frameWidth;
                        foreach (var classId in Enumerable.Range(0, classNum))
                        {
                            var frameArray = new ArraySegment<byte>(allBytes, classId * frameLength, frameLength);

                            this.ClassIndexed.Add(frameArray.ToArray());
                        }
                    }
                }
            }
            catch
            {
                this.RawCollection = null;
            }
        }

        #endregion

        #region IOnnxDataOutputClassIndexed

        /// <summary>
        /// フレームサイズ
        /// </summary>
        public Common.Drawing.Size ClassFrameSize { get; set; } = new();

        /// <summary>
        /// クラス別検出インデックス
        /// </summary>
        public List<byte[]> ClassIndexed { get; } = new();

        #endregion

        #region IOnnxDataOutputClassInstanceIndexed

        /// <summary>
        /// クラス別検出個数
        /// </summary>
        public List<int> ClassCounts { get; } = new();

        #endregion
    }
}