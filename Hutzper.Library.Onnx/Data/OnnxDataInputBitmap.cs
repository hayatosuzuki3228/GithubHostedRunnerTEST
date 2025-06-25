using Hutzper.Library.Onnx.Model;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Hutzper.Library.Onnx.Data;

/// <summary>
/// ONNX入力データ Bitmap型データ
/// </summary>
[Serializable]
public class OnnxDataInputBitmap : OnnxDataInput, IOnnxDataBitmap
{
    #region IOnnxDataBitmap

    /// <summary>
    /// Bitmapリスト
    /// </summary>
    public virtual Dictionary<string, Bitmap[]>? Images { get; set; }

    /// <summary>
    /// Bitmap化
    /// </summary>
    /// <returns></returns>
    public virtual Bitmap[] ToBitmap()
    {
        try
        {
            return this.Images?.Values
                .SelectMany(imageList => imageList.Select(image => (Bitmap)image.Clone()))
                .ToArray() ?? Array.Empty<Bitmap>();
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, ex.Message);
            return Array.Empty<Bitmap>();
        }
    }

    #endregion

    #region IOnnxDataInput

    /// <summary>
    /// モデル入力データ作成
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public override IReadOnlyCollection<NamedOnnxValue> ToReadOnlyCollection()
    {
        if (this.Images == null) throw new ArgumentNullException("Images");

        var collection = new List<NamedOnnxValue>();
        foreach (var image in this.Images)
        {
            if (!this.InputList.ContainsKey(image.Key)) continue;
            var input = this.InputList[image.Key];
            var dims = input.Item1.Dimensions; // 形状を取得

            // グレースケールの場合は形状の最後にチャンネル数を追加
            if (image.Value[0].PixelFormat == PixelFormat.Format8bppIndexed)
            {
                if (dims.Length == 2)
                {
                    dims = new int[] { dims[0], dims[1], 1 };
                }
                else if (dims.Length == 3)
                {
                    dims = new int[] { dims[0], dims[1], dims[2], 1 };
                }
            }

            if (dims.Length == 3) // バッチ推論未対応onnx: [h, w, c] (Rank=3)
            {
                var bitmaps = image.Value;
                if (bitmaps.Length != 1)
                {
                    throw new ArgumentException($"Model expects single image (rank=3), but got {bitmaps.Length} images.");
                }

                // 画像がonnxの期待する形式と一致するかを判定
                int h = dims[0];
                int w = dims[1];
                int ch = dims.Length > 2 ? dims[2] : 1;
                int singleImageSize = h * w * ch;
                if (input.Item2.Length != singleImageSize)
                {
                    throw new ArgumentException($"Backing array size mismatch. Expected {singleImageSize}, got {input.Item2.Length}.");
                }
                var bitmap = bitmaps[0];
                this.IsValidBitmapShape(bitmap, h, w, ch);

                // 画像を配列にコピー
                this.CopyBitmapToBackingArray(bitmap, input, 0, ch);
            }
            else if (dims.Length == 4) // バッチ推論対応onnx: [b, h, w, c] (Rank=4)
            {
                // 画像がonnxの期待する形式と一致するかを判定
                int batch = dims[0];
                int h = dims[1];
                int w = dims[2];
                int ch = dims.Length > 3 ? dims[3] : 1;
                var bitmaps = image.Value;
                if (bitmaps.Length != batch)
                {
                    throw new ArgumentException($"Batch size mismatch. Model expects {batch}, but got {bitmaps.Length} image(s).");
                }
                int singleImageSize = h * w * ch;
                if (input.Item2.Length != singleImageSize * batch)
                {
                    throw new ArgumentException($"Backing array size mismatch. Expected {singleImageSize * batch}, got {input.Item2.Length}.");
                }

                for (int b = 0; b < batch; b++)
                {
                    // 画像がonnxの期待する形式と一致するかを判定
                    var bitmap = bitmaps[b];
                    this.IsValidBitmapShape(bitmap, h, w, ch);
                    int dstOffsetBatch = b * singleImageSize;
                    this.CopyBitmapToBackingArray(bitmap, input, dstOffsetBatch, ch);
                }
            }
            else
            {
                throw new NotSupportedException($"Unexpected tensor rank. Supported ranks are 3 or 4, but got {dims.Length}.");
            }
            collection.Add(NamedOnnxValue.CreateFromTensor(image.Key, input.Item1));
        }
        return new ReadOnlyCollection<NamedOnnxValue>(collection);
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
            base.DisposeExplicit();
            if (this.Images == null) return;
            foreach (var imageList in this.Images.Values)
            {
                foreach (var image in imageList)
                {
                    this.DisposeSafely(image);
                }
            }
        }
        catch
        {
        }
        finally
        {
            this.Images?.Clear();
        }
    }

    #endregion

    #region メソッド

    public override void Initialize(IOnnxModel model)
    {
        try
        {
            base.Initialize(model);
            this.Images = new(this.BatchSize);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, ex.Message);
        }
    }

    private void IsValidBitmapShape(Bitmap bitmap, int height, int width, int chanel)
    {
        var pixelUnit = bitmap.PixelFormat == PixelFormat.Format8bppIndexed ? 1 : 3;
        bool result = true;
        result &= bitmap.Height == height;
        result &= bitmap.Width == width;
        result &= pixelUnit == chanel;
        if (!result)
        {
            throw new ArgumentException($"Shape mismatch between the captured image and the onnx model.\nCaptured image [height, width, channel] = {bitmap.Height}, {bitmap.Width}, {pixelUnit}], onnx model [height, width, channel] = {height}, {width}, {chanel}]");
        }
    }

    private void CopyBitmapToBackingArray(Bitmap bmp, Tuple<DenseTensor<byte>, byte[]> input, int dstOffset, int pixelUnit)
    {
        if (bmp.PixelFormat == PixelFormat.Format32bppRgb)
        {
            // 画像が32bppの時は24bppに変換
            using var convertedBitmap = this.Convert32bppTo24bpp(bmp);
            this.CopyBitmapToBackingArray(convertedBitmap, input, dstOffset, pixelUnit);
            return;
        }

        bool isGray = (bmp.PixelFormat == PixelFormat.Format8bppIndexed && pixelUnit == 1);
        bool isColor = (bmp.PixelFormat == PixelFormat.Format24bppRgb && pixelUnit == 3);

        if (!isGray && !isColor)
        {
            throw new ArgumentException($"Unsupported format: {bmp.PixelFormat}, expected 8bpp or 24bpp matching modelCh={pixelUnit}");
        }

        var bmpData = bmp.LockBits(
            new Rectangle(0, 0, bmp.Width, bmp.Height),
            ImageLockMode.ReadOnly,
            bmp.PixelFormat
        );

        int dataStride = Math.Abs(bmpData.Stride);  // 1行あたりの実際のバイト数（パディング込み）
        int byteLengthFromWidth = bmpData.Width * pixelUnit; // 有効画素部分 (width×ピクセルサイズ)
        int height = bmpData.Height;
        if (dataStride == byteLengthFromWidth)
        {
            // Strideと画像サイズバイト長が同じ場合は一括コピー
            int reqiredSize = dataStride * height;
            if (dstOffset + reqiredSize > input.Item2.Length)
            {
                throw new ArgumentException($"The input image size does not match the ONNX model's expected size.");
            }
            Marshal.Copy(bmpData.Scan0, input.Item2, dstOffset, reqiredSize);
        }
        else
        {
            // 画像データを一括コピー
            var rawArray = new byte[dataStride * bmpData.Height];
            Marshal.Copy(bmpData.Scan0, rawArray, 0, rawArray.Length);

            // Strideと画像サイズバイト長が異なる場合は行単位でコピー
            var srcIndex = 0;
            var dstIndex = dstOffset;
            for (int i = 0; i < height; i++)
            {
                if (dstIndex + byteLengthFromWidth > input.Item2.Length)
                {
                    throw new ArgumentException("The input image size does not match the ONNX model's expected size (line mode).");
                }
                Array.Copy(rawArray, srcIndex, input.Item2, dstIndex, byteLengthFromWidth);

                srcIndex += dataStride;
                dstIndex += byteLengthFromWidth;
            }
        }
        bmp.UnlockBits(bmpData);
    }

    private Bitmap Convert32bppTo24bpp(Bitmap src)
    {
        Bitmap bitmap = new Bitmap(src.Width, src.Height, PixelFormat.Format24bppRgb);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.DrawImage(src, new Rectangle(0, 0, src.Width, src.Height));
        }
        return bitmap;
    }

    #endregion
}