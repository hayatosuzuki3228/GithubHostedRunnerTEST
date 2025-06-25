using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Drawing;

namespace Hutzper.Library.Onnx.Data;

/// <summary>
///  ONNN用出力データ:セグメンテーション用
/// </summary>
[Serializable]
public class OnnxDataOutputObjectDetection : OnnxDataOutput, IOnnxDataOutputClassIndexed
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
            this.ResultList.Clear();
            base.CopyFrom(resultOfRun);

            if (this.RawCollection is not null)
            {
                foreach (var data in this.RawCollection.Where(c => c.ElementType == TensorElementType.Float))
                {
                    if (data.Value is DenseTensor<float> tensorData)
                    {
                        int rowCount = tensorData.Dimensions[0];
                        int colCount = tensorData.Dimensions[1];
                        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                        {
                            this.ResultList.Add(new()
                            {
                                Classname = "",
                                ClassIndex = (int)tensorData[rowIndex, 5],
                                Score = tensorData[rowIndex, 4],
                                Point1 = new Point((int)tensorData[rowIndex, 0], (int)tensorData[rowIndex, 1]),
                                Point2 = new Point((int)tensorData[rowIndex, 2], (int)tensorData[rowIndex, 3])
                            });
                        }
                    }

                    break;

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
    public List<ResultData> ResultList { get; } = new();
    public record ResultData
    {
        public string? Classname { get; set; }
        public int ClassIndex;
        public double Score;
        public List<int>? RejectList { get; set; }
        public Point Point1;
        public Point Point2;
    }
    public static Bitmap DrawRectangleOnBitmap(Bitmap bitmap, List<OnnxDataOutputObjectDetection.ResultData> resultDatas)
    {
        using Graphics g = Graphics.FromImage(bitmap);
        foreach (var resultData in resultDatas)
        {
            Color boxColor = ColorList[resultData.ClassIndex % 16];
            int x = Math.Min(resultData.Point1.X, resultData.Point2.X);
            int y = Math.Min(resultData.Point1.Y, resultData.Point2.Y);
            int width = Math.Abs(resultData.Point1.X - resultData.Point2.X);
            int height = Math.Abs(resultData.Point1.Y - resultData.Point2.Y);
            using Pen pen = new(boxColor, 5);
            g.DrawRectangle(pen, x, y, width, height);
            using Font font = new("Arial", 22);
            using SolidBrush brush = new(Color.Black);
            string text = $"{resultData.Classname} {resultData.Score:F3}";
            using SolidBrush backgroundBrush = new(boxColor);
            SizeF textSize = g.MeasureString(text, font);
            g.FillRectangle(backgroundBrush, x + 3, y + 3, textSize.Width, textSize.Height);
            g.DrawString(text, font, brush, x + 3, y + 3);
        }
        return bitmap;
    }
    private static List<Color> ColorList = new()
    {
        Color.LightGray,
        Color.White,
        Color.LightPink,
        Color.LightGreen,
        Color.LightBlue,
        Color.LightYellow,
        Color.LightCyan,
        Color.LightSalmon,
        Color.LightGoldenrodYellow,
        Color.LightCoral,
        Color.LightSteelBlue,
        Color.Thistle,
        Color.PowderBlue,
        Color.PeachPuff,
        Color.PaleGreen,
        Color.PaleTurquoise
    };
    #endregion
}