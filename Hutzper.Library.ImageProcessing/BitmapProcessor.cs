using Hutzper.Library.Common;
using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.Drawing;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Size = Hutzper.Library.Common.Drawing.Size;

namespace Hutzper.Library.ImageProcessing
{
    /// <summary>
    /// Bitmap処理
    /// </summary>
    public class BitmapProcessor
    {
        #region スタティックプロパティ

        /// <summary>
        /// 並列実行時の最大スレッド数
        /// </summary>
        public static int DefaultMaxDegreeOfParallelism { get; set; }

        #endregion

        #region スタティックフィールド

        private static readonly double[] lutOfConvRgbToGrayFromR;
        private static readonly double[] lutOfConvRgbToGrayFromG;
        private static readonly double[] lutOfConvRgbToGrayFromB;

        private static readonly byte[][] lutOfMaxGray;
        private static readonly byte[][] lutOfMinGray;

        #endregion

        #region スタティックコンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static BitmapProcessor()
        {
            // 並列実行時の最大スレッド数
            BitmapProcessor.DefaultMaxDegreeOfParallelism = 4;

            #region RGB画像をグレイスケール画像変換
            {
                var lenghtOfLut = byte.MaxValue + 1;
                BitmapProcessor.lutOfConvRgbToGrayFromR = new double[lenghtOfLut];
                BitmapProcessor.lutOfConvRgbToGrayFromG = new double[lenghtOfLut];
                BitmapProcessor.lutOfConvRgbToGrayFromB = new double[lenghtOfLut];
                foreach (var i in Enumerable.Range(0, lenghtOfLut))
                {
                    BitmapProcessor.lutOfConvRgbToGrayFromR[i] = i * 0.299;
                    BitmapProcessor.lutOfConvRgbToGrayFromG[i] = i * 0.587;
                    BitmapProcessor.lutOfConvRgbToGrayFromB[i] = i * 0.114;
                }
            }
            #endregion

            #region Maximum変換
            {
                var lenghtOfLut = byte.MaxValue + 1;
                BitmapProcessor.lutOfMaxGray = new byte[lenghtOfLut][];
                foreach (var i in Enumerable.Range(0, lenghtOfLut))
                {
                    BitmapProcessor.lutOfMaxGray[i] = new byte[lenghtOfLut];

                    foreach (var j in Enumerable.Range(0, lenghtOfLut))
                    {
                        BitmapProcessor.lutOfMaxGray[i][j] = System.Math.Max((byte)i, (byte)j);
                    }
                }
            }
            #endregion

            #region Minimum変換
            {
                var lenghtOfLut = byte.MaxValue + 1;
                BitmapProcessor.lutOfMinGray = new byte[lenghtOfLut][];
                foreach (var i in Enumerable.Range(0, lenghtOfLut))
                {
                    BitmapProcessor.lutOfMinGray[i] = new byte[lenghtOfLut];

                    foreach (var j in Enumerable.Range(0, lenghtOfLut))
                    {
                        BitmapProcessor.lutOfMinGray[i][j] = System.Math.Min((byte)i, (byte)j);
                    }
                }
            }
            #endregion
        }

        #endregion

        #region スタティックメソッド

        /// <summary>
        /// Parallelクラスのメソッドの操作を構成するオプションを作成する
        /// </summary>
        /// <returns>作成されたオプション</returns>
        /// <remarks>並列化の最大レベルはDefaultMaxDegreeOfParallelismが適用されます</remarks>
        public static ParallelOptions NewParallelOptions()
        {
            return BitmapProcessor.NewParallelOptions(BitmapProcessor.DefaultMaxDegreeOfParallelism);
        }

        /// <summary>
        /// Parallelクラスのメソッドの操作を構成するオプションを作成する
        /// </summary>
        /// <param name="maxDegreeOfParallelism">並列化の最大レベルを指定する</param>
        /// <returns>作成されたオプション</returns>
        public static ParallelOptions NewParallelOptions(int maxDegreeOfParallelism)
        {
            var parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = (maxDegreeOfParallelism > 0) ? maxDegreeOfParallelism : -1
            };

            return parallelOptions;
        }

        #region 2値化

        /// <summary>
        /// 2値化
        /// </summary>
        /// <param name="source">元ビットマップ</param>
        /// <param name="result">結果ビットマップ</param>
        /// <param name="threshold">2値化しきい値</param>        
        [SupportedOSPlatform("windows7.0")]
        public static void Binarization(Bitmap source, out Bitmap? result, int threshold)
        {
            BitmapProcessor.Binarization(source, out result, threshold, byte.MaxValue);
        }

        /// <summary>
        /// 2値化
        /// </summary>
        /// <param name="source">元ビットマップ</param>
        /// <param name="result">結果ビットマップ</param>
        /// <param name="threshold">2値化しきい値</param>        
        [SupportedOSPlatform("windows7.0")]
        public static void Binarization(Bitmap source, out Bitmap? result, int threshold, ProcessignType processignType)
        {
            BitmapProcessor.Binarization(source, out result, threshold, byte.MaxValue, processignType, null);
        }

        /// <summary>
        /// 2値化
        /// </summary>
        /// <param name="source">元ビットマップ</param>
        /// <param name="result">結果ビットマップ</param>
        /// <param name="threshold">2値化しきい値</param>       
        /// <param name="processignType">処理タイプ</param>
        /// <param name="parallelOptions">並列処理設定</param>
        [SupportedOSPlatform("windows7.0")]
        public static void Binarization(Bitmap source, out Bitmap? result, int threshold, ProcessignType processignType, ParallelOptions parallelOptions)
        {
            BitmapProcessor.Binarization(source, out result, threshold, byte.MaxValue, processignType, parallelOptions);
        }

        /// <summary>
        /// 2値化
        /// </summary>
        /// <param name="source">元ビットマップ</param>
        /// <param name="result">結果ビットマップ</param>
        /// <param name="thresholdMin">下限2値化しきい値</param>
        /// <param name="thresholdMax">上限2値化しきい値</param>
        [SupportedOSPlatform("windows7.0")]
        public static void Binarization(Bitmap source, out Bitmap? result, int thresholdMin, int thresholdMax)
        {
            BitmapProcessor.Binarization(source, out result, thresholdMin, thresholdMax, ProcessignType.Sequential);
        }

        /// <summary>
        /// 2値化
        /// </summary>
        /// <param name="source">元ビットマップ</param>
        /// <param name="result">結果ビットマップ</param>
        /// <param name="thresholdMin">下限2値化しきい値</param>
        /// <param name="thresholdMax">上限2値化しきい値</param>
        /// <param name="processignType">処理タイプ</param>
        [SupportedOSPlatform("windows7.0")]
        public static void Binarization(Bitmap source, out Bitmap? result, int thresholdMin, int thresholdMax, ProcessignType processignType)
        {
            BitmapProcessor.Binarization(source, out result, thresholdMin, thresholdMax, processignType, null);
        }

        /// <summary>
        /// 2値化
        /// </summary>
        /// <param name="source">元ビットマップ</param>
        /// <param name="result">結果ビットマップ</param>
        /// <param name="thresholdMin">下限2値化しきい値</param>
        /// <param name="thresholdMax">上限2値化しきい値</param>
        /// <param name="processignType">処理タイプ</param>
        /// <param name="parallelOptions">並列処理設定</param>
        [SupportedOSPlatform("windows7.0")]
        public static void Binarization(Bitmap source, out Bitmap? result, int thresholdMin, int thresholdMax, ProcessignType processignType, ParallelOptions? parallelOptions)
        {
            result = null;

            try
            {
                switch (source.PixelFormat)
                {
                    case PixelFormat.Format8bppIndexed:
                        {
                            #region 2値化処理

                            var lockedData = (BitmapData?)null;

                            try
                            {
                                // 256階調の2値化LUT作成
                                var thresholdLut = new byte[byte.MaxValue + 1];
                                var hiValues = Enumerable.Repeat<byte>(byte.MaxValue, thresholdMax - thresholdMin + 1).ToArray();
                                Array.Copy(hiValues, 0, thresholdLut, thresholdMin, hiValues.Length);

                                // ビットマップ複製
                                result = (Bitmap)source.Clone();

                                // ビットマップロック                                                                     
                                lockedData = result.LockBits(
                                                               new System.Drawing.Rectangle(System.Drawing.Point.Empty, result.Size)
                                                             , ImageLockMode.ReadWrite
                                                             , result.PixelFormat
                                                             );
                                // 処理データ配列
                                var fullPixels = new byte[lockedData.Stride * lockedData.Height];

                                // ビットマップデータから処理用配列へコピー
                                Marshal.Copy(lockedData.Scan0, fullPixels, 0, fullPixels.Length);

                                // 2値化処理
                                void binarizer(int move)
                                {
                                    for (int x = 0; x < lockedData.Width; x++)
                                    {
                                        fullPixels[move] = thresholdLut[fullPixels[move]];
                                        move++;
                                    }
                                }

                                // 並列処理の場合
                                if (ProcessignType.Parallel == processignType)
                                {
                                    // 並列処理設定が指定されていない場合
                                    if (null == parallelOptions)
                                    {
                                        var maxDegreeOfParallelism = BitmapProcessor.DefaultMaxDegreeOfParallelism;
                                        parallelOptions = new ParallelOptions()
                                        {
                                            MaxDegreeOfParallelism = (maxDegreeOfParallelism > 0) ? maxDegreeOfParallelism : -1
                                        };
                                    }

                                    // 2値化処理を並列実行
                                    Parallel.ForEach(Enumerable.Range(0, lockedData.Height), parallelOptions, y =>
                                    {
                                        binarizer(y * lockedData.Stride);
                                    });
                                }
                                // 並列処理が有効でない場合
                                else
                                {
                                    // 2値化処理を逐次実行
                                    foreach (var y in Enumerable.Range(0, lockedData.Height))
                                    {
                                        binarizer(y * lockedData.Stride);
                                    }
                                }

                                // 処理用配列からビットマップデータへコピー
                                Marshal.Copy(fullPixels, 0, lockedData.Scan0, fullPixels.Length);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }
                            finally
                            {
                                if (null != lockedData)
                                {
                                    // ビットマップロック解除
                                    result?.UnlockBits(lockedData);
                                }
                            }
                            #endregion
                        }
                        break;

                    default:
                        {
                            result = (Bitmap)source.Clone();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                result = (Bitmap)source.Clone();
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #region フィルタ

        /// <summary>
        /// 平滑化
        /// </summary>
        /// <param name="source">元ビットマップ</param>
        /// <param name="result">結果ビットマップ</param>
        /// <param name="maskSize">平滑化マスクサイズ(中心±)</param>
        /// <param name="processignType">処理タイプ</param>
        [SupportedOSPlatform("windows7.0")]
        public static void Smoothing(Bitmap source, out Bitmap? result, Size maskSize, ProcessignType processignType)
        {
            BitmapProcessor.Smoothing(source, out result, maskSize, processignType, null);
        }

        /// <summary>
        /// 平滑化
        /// </summary>
        /// <param name="source">元ビットマップ</param>
        /// <param name="result">結果ビットマップ</param>
        /// <param name="maskSize">平滑化マスクサイズ(中心±)</param>
        /// <param name="processignType">処理タイプ</param>
        /// <param name="parallelOptions">並列処理設定</param>
        [SupportedOSPlatform("windows7.0")]
        public static void Smoothing(Bitmap source, out Bitmap? result, Size maskSize, ProcessignType processignType, ParallelOptions? parallelOptions)
        {
            result = null;

            try
            {
                switch (source.PixelFormat)
                {
                    case PixelFormat.Format8bppIndexed:
                        {
                            #region 平滑化

                            var lockedData = (BitmapData?)null;

                            try
                            {
                                // ビットマップ複製
                                result = (Bitmap)source.Clone();

                                // ビットマップロック                                                                     
                                lockedData = result.LockBits(
                                                               new System.Drawing.Rectangle(System.Drawing.Point.Empty, result.Size)
                                                             , ImageLockMode.ReadWrite
                                                             , result.PixelFormat
                                                             );
                                // 処理データ配列
                                var referencePixels = new byte[lockedData.Stride * lockedData.Height];
                                var filteringPixels = new byte[lockedData.Stride * lockedData.Height];

                                // ビットマップデータから処理用配列へコピー
                                Marshal.Copy(lockedData.Scan0, referencePixels, 0, referencePixels.Length);
                                Marshal.Copy(lockedData.Scan0, filteringPixels, 0, filteringPixels.Length);

                                // フィルタオフセット作成
                                BitmapProcessor.GetFilteringMaskOffsetArray(maskSize, lockedData.Stride, out int[] roi);

                                // 平滑化処理
                                var begin = new System.Drawing.Point(maskSize.Width, maskSize.Height);
                                var end = new System.Drawing.Point(lockedData.Stride - maskSize.Width, lockedData.Height - maskSize.Height);
                                var count = new System.Drawing.Size(end.X - begin.Y, end.Y - begin.Y);
                                void smoothing(int move)
                                {
                                    #region フィルタ処理
                                    for (int x = 0; x < count.Width; x++)
                                    {
                                        var sum = 0;
                                        for (int i = 0; i < roi.Length; i++)
                                        {
                                            sum += referencePixels[roi[i] + move];
                                        }

                                        filteringPixels[move++] = (byte)(sum / roi.Length);
                                    }
                                    #endregion
                                }

                                // 並列処理の場合
                                if (ProcessignType.Parallel == processignType)
                                {
                                    // 並列処理設定が指定されていない場合
                                    if (null == parallelOptions)
                                    {
                                        var maxDegreeOfParallelism = BitmapProcessor.DefaultMaxDegreeOfParallelism;
                                        parallelOptions = new ParallelOptions()
                                        {
                                            MaxDegreeOfParallelism = (maxDegreeOfParallelism > 0) ? maxDegreeOfParallelism : -1
                                        };
                                    }

                                    // 平滑化処理を並列実行
                                    Parallel.ForEach(Enumerable.Range(begin.Y, count.Height), parallelOptions, y =>
                                    {
                                        smoothing(y * lockedData.Stride + begin.X);
                                    });
                                }
                                // 並列処理が有効でない場合
                                else
                                {
                                    // 平滑化処理を逐次実行
                                    foreach (var y in Enumerable.Range(begin.Y, count.Height))
                                    {
                                        smoothing(y * lockedData.Stride + begin.X);
                                    }
                                }

                                // 処理用配列からビットマップデータへコピー
                                Marshal.Copy(filteringPixels, 0, lockedData.Scan0, filteringPixels.Length);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }
                            finally
                            {
                                if (null != lockedData)
                                {
                                    // ビットマップロック解除
                                    result?.UnlockBits(lockedData);
                                }
                            }
                            #endregion
                        }
                        break;

                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppRgb:
                    case PixelFormat.Format32bppArgb:
                        {
                            #region 平滑化

                            var lockedData = (BitmapData?)null;

                            var pixelSkip = (PixelFormat.Format24bppRgb == source.PixelFormat) ? 0 : 1;
                            var pixelUnit = pixelSkip + 3;

                            try
                            {
                                // ビットマップ複製
                                result = (Bitmap)source.Clone();

                                // ビットマップロック                                                                     
                                lockedData = result.LockBits(
                                                               new System.Drawing.Rectangle(System.Drawing.Point.Empty, result.Size)
                                                             , ImageLockMode.ReadWrite
                                                             , result.PixelFormat
                                                             );
                                // 処理データ配列
                                var referencePixels = new byte[lockedData.Stride * lockedData.Height];
                                var filteringPixels = new byte[lockedData.Stride * lockedData.Height];

                                // ビットマップデータから処理用配列へコピー
                                Marshal.Copy(lockedData.Scan0, referencePixels, 0, referencePixels.Length);
                                Marshal.Copy(lockedData.Scan0, filteringPixels, 0, filteringPixels.Length);

                                // フィルタオフセット作成
                                BitmapProcessor.GetFilteringMaskOffsetArray(maskSize, lockedData.Stride, pixelUnit, out int[][] roi);

                                // 平滑化処理
                                var begin = new System.Drawing.Point(maskSize.Width, maskSize.Height);
                                var end = new System.Drawing.Point(lockedData.Stride - maskSize.Width * pixelUnit, lockedData.Height - maskSize.Height * pixelUnit);
                                var count = new System.Drawing.Size(end.X - begin.Y, end.Y - begin.Y);
                                void smoothing(int move)
                                {
                                    #region フィルタ処理
                                    var sum = new int[pixelUnit];
                                    for (int x = 0; x < count.Width; x++)
                                    {
                                        sum[0] = 0;
                                        sum[1] = 0;
                                        sum[2] = 0;
                                        for (int i = 0; i < roi[0].Length; i++)
                                        {
                                            sum[0] += referencePixels[roi[0][i] + move];
                                            sum[1] += referencePixels[roi[1][i] + move];
                                            sum[2] += referencePixels[roi[2][i] + move];
                                        }

                                        filteringPixels[move++] = (byte)(sum[0] / roi[0].Length);
                                        filteringPixels[move++] = (byte)(sum[1] / roi[1].Length);
                                        filteringPixels[move++] = (byte)(sum[2] / roi[2].Length);

                                        move += pixelSkip;
                                    }
                                    #endregion
                                }

                                // 並列処理の場合
                                if (ProcessignType.Parallel == processignType)
                                {
                                    // 並列処理設定が指定されていない場合
                                    if (null == parallelOptions)
                                    {
                                        var maxDegreeOfParallelism = BitmapProcessor.DefaultMaxDegreeOfParallelism;
                                        parallelOptions = new ParallelOptions()
                                        {
                                            MaxDegreeOfParallelism = (maxDegreeOfParallelism > 0) ? maxDegreeOfParallelism : -1
                                        };
                                    }

                                    // 平滑化処理を並列実行
                                    Parallel.ForEach(Enumerable.Range(begin.Y, count.Height), parallelOptions, y =>
                                    {
                                        smoothing(y * lockedData.Stride + begin.X * pixelUnit);
                                    });
                                }
                                // 並列処理が有効でない場合
                                else
                                {
                                    // 平滑化処理を逐次実行
                                    foreach (var y in Enumerable.Range(begin.Y, count.Height))
                                    {
                                        smoothing(y * lockedData.Stride + begin.X * pixelUnit);
                                    }
                                }

                                // 処理用配列からビットマップデータへコピー
                                Marshal.Copy(filteringPixels, 0, lockedData.Scan0, filteringPixels.Length);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }
                            finally
                            {
                                if (null != lockedData)
                                {
                                    // ビットマップロック解除
                                    result?.UnlockBits(lockedData);
                                }
                            }
                            #endregion
                        }
                        break;

                    default:
                        {
                            result = (Bitmap)source.Clone();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// ソーベルフィルタ3x3
        /// </summary>
        /// <param name="source">元ビットマップ</param>
        /// <param name="result">結果ビットマップ</param>
        /// <param name="processignType">処理タイプ</param>
        [SupportedOSPlatform("windows7.0")]
        public static void Sobel_3x3(Bitmap source, out Bitmap? result, ProcessignType processignType)
        {
            BitmapProcessor.Sobel_3x3(source, out result, processignType, null);
        }

        /// <summary>
        /// ソーベルフィルタ3x3
        /// </summary>
        /// <param name="source">元ビットマップ</param>
        /// <param name="result">結果ビットマップ</param>
        /// <param name="processignType">処理タイプ</param>
        /// <param name="parallelOptions">並列処理設定</param>
        [SupportedOSPlatform("windows7.0")]
        public static void Sobel_3x3(Bitmap source, out Bitmap? result, ProcessignType processignType, ParallelOptions? parallelOptions)
        {
            result = null;

            try
            {
                switch (source.PixelFormat)
                {
                    case PixelFormat.Format8bppIndexed:
                        {
                            #region ソーベルフィルタ3x3

                            var lockedData = (BitmapData?)null;

                            try
                            {
                                // ビットマップ複製
                                result = (Bitmap)source.Clone();

                                // ビットマップロック                                                                     
                                lockedData = result.LockBits(
                                                               new System.Drawing.Rectangle(System.Drawing.Point.Empty, result.Size)
                                                             , ImageLockMode.ReadWrite
                                                             , result.PixelFormat
                                                             );
                                // 処理データ配列
                                var referencePixels = new byte[lockedData.Stride * lockedData.Height];
                                var filteringPixels = new byte[lockedData.Stride * lockedData.Height];

                                // ビットマップデータから処理用配列へコピー
                                Marshal.Copy(lockedData.Scan0, referencePixels, 0, referencePixels.Length);
                                Marshal.Copy(lockedData.Scan0, filteringPixels, 0, filteringPixels.Length);

                                // フィルタオフセット作成
                                var maskSize = new Size(1, 1);
                                BitmapProcessor.GetFilteringMaskOffsetArray(maskSize, lockedData.Stride, out int[] roi);

                                // ソーベルフィルタ3x3
                                var begin = new System.Drawing.Point(maskSize.Width, maskSize.Height);
                                var end = new System.Drawing.Point(lockedData.Width - maskSize.Width, lockedData.Height - maskSize.Height);
                                var count = new System.Drawing.Size(end.X - begin.Y, end.Y - begin.Y);
                                void sobel(int move)
                                {
                                    #region フィルタ処理
                                    for (int x = 0; x < count.Width; x++)
                                    {
                                        var sumH = 0d;
                                        var sumV = 0d;

                                        sumV = referencePixels[roi[0] + move]
                                              + referencePixels[roi[1] + move] + referencePixels[roi[1] + move]
                                              + referencePixels[roi[2] + move]
                                              - referencePixels[roi[6] + move]
                                              - referencePixels[roi[7] + move] - referencePixels[roi[7] + move]
                                              - referencePixels[roi[8] + move];

                                        sumH = referencePixels[roi[0] + move]
                                              + referencePixels[roi[3] + move] + referencePixels[roi[3] + move]
                                              + referencePixels[roi[6] + move]
                                              - referencePixels[roi[2] + move]
                                              - referencePixels[roi[5] + move] - referencePixels[roi[5] + move]
                                              - referencePixels[roi[8] + move];

                                        var value = Math.Sqrt(sumV * sumV + sumH * sumH);

                                        if (byte.MaxValue < value)
                                        {
                                            value = byte.MaxValue;
                                        }

                                        filteringPixels[move++] = (byte)(value);
                                    }
                                    #endregion
                                }

                                // 並列処理の場合
                                if (ProcessignType.Parallel == processignType)
                                {
                                    // 並列処理設定が指定されていない場合
                                    if (null == parallelOptions)
                                    {
                                        var maxDegreeOfParallelism = BitmapProcessor.DefaultMaxDegreeOfParallelism;
                                        parallelOptions = new ParallelOptions()
                                        {
                                            MaxDegreeOfParallelism = (maxDegreeOfParallelism > 0) ? maxDegreeOfParallelism : -1
                                        };
                                    }

                                    // フィルタ処理を並列実行
                                    Parallel.ForEach(Enumerable.Range(begin.Y, count.Height), parallelOptions, y =>
                                    {
                                        sobel(y * lockedData.Stride + begin.X);
                                    });
                                }
                                // 並列処理が有効でない場合
                                else
                                {
                                    // フィルタ処理を逐次実行
                                    foreach (var y in Enumerable.Range(begin.Y, count.Height))
                                    {
                                        sobel(y * lockedData.Stride + begin.X);
                                    }
                                }

                                // 処理用配列からビットマップデータへコピー
                                Marshal.Copy(filteringPixels, 0, lockedData.Scan0, filteringPixels.Length);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }
                            finally
                            {
                                if (null != lockedData)
                                {
                                    // ビットマップロック解除
                                    result?.UnlockBits(lockedData);
                                }
                            }
                            #endregion
                        }
                        break;

                    default:
                        {
                            result = (Bitmap)source.Clone();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// フィルタ処理マスクオフセット配列を取得する
        /// </summary>
        /// <param name="maskSize">化マスクサイズ(中心±)</param>
        /// <param name="stride">データ行サイズ</param>
        /// <param name="mask">作成されたマスクオフセット配列</param>
        public static void GetFilteringMaskOffsetArray(Size maskSize, int stride, out int[] mask)
        {
            mask = Array.Empty<int>();

            try
            {
                var roiH = maskSize.Height + maskSize.Height + 1;
                var roiW = maskSize.Width + maskSize.Width + 1;
                var roi = new int[roiH * roiW];
                var offset = stride * maskSize.Height * -1 - maskSize.Width;
                var index = 0;
                for (int i = 0; i < roiH; i++)
                {
                    var line = Enumerable.Range(offset, roiW).ToArray();

                    Array.Copy(line, 0, roi, index, line.Length);

                    index += roiW;
                    offset += stride;
                }

                mask = roi;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// フィルタ処理マスクオフセット配列を取得する
        /// </summary>
        /// <param name="maskSize">化マスクサイズ(中心±)</param>
        /// <param name="stride">データ行サイズ</param>
        /// <param name="mask">作成されたマスクオフセット配列</param>
        public static void GetFilteringMaskOffsetArray(Size maskSize, int stride, int pixelUnit, out int[][] mask)
        {
            mask = new int[pixelUnit][];

            try
            {
                foreach (var p in Enumerable.Range(0, mask.Length))
                {
                    var roiH = maskSize.Height + maskSize.Height + 1;
                    var roiW = maskSize.Width + maskSize.Width + 1;
                    var roi = new int[roiH * roiW];
                    var offset = stride * maskSize.Height * -1 - maskSize.Width * pixelUnit;
                    var index = 0;
                    for (int i = 0; i < roiH; i++)
                    {
                        var pixIndex = offset + p;
                        var rowIndex = index;
                        for (int j = 0; j < roiW; j++)
                        {
                            roi[rowIndex++] = pixIndex;
                            pixIndex += pixelUnit;
                        }

                        index += roiW;
                        offset += stride;
                    }

                    mask[p] = roi;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 2つの画像の最大値を採用した画像を作成する
        /// </summary>
        /// <param name="source1">入力画像1</param>
        /// <param name="source2">入力画像2</param>
        /// <param name="result">最大値画像</param>
        /// <remarks>PixelFormat.Format8bppIndexedに対応</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void Max(Bitmap source1, Bitmap source2, out Bitmap? result)
        {
            BitmapProcessor.Max(source1, source2, out result, ProcessignType.Sequential, null);
        }

        /// <summary>
        /// 2つの画像の最大値を採用した画像を作成する
        /// </summary>
        /// <param name="source1">入力画像1</param>
        /// <param name="source2">入力画像2</param>
        /// <param name="result">最大値画像</param>
        /// <remarks>PixelFormat.Format8bppIndexedに対応</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void Max(Bitmap source1, Bitmap source2, out Bitmap? result, ProcessignType processignType)
        {
            BitmapProcessor.Max(source1, source2, out result, processignType, null);
        }

        /// <summary>
        /// 2つの画像の最大値を採用した画像を作成する
        /// </summary>
        /// <param name="source1">入力画像1</param>
        /// <param name="source2">入力画像2</param>
        /// <param name="result">最大値画像</param>
        /// <remarks>PixelFormat.Format8bppIndexedに対応</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void Max(Bitmap source1, Bitmap source2, out Bitmap? result, ProcessignType processignType, ParallelOptions? parallelOptions)
        {
            BitmapProcessor.Max(new Bitmap[] { source1, source2 }, out result, processignType, parallelOptions);
        }

        /// <summary>
        /// 入力画像の最大値を採用した画像を作成する
        /// </summary>
        /// <param name="source">入力画像</param>
        /// <param name="result">最大値画像</param>
        /// <remarks>PixelFormat.Format8bppIndexedに対応</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void Max(Bitmap[] source, out Bitmap? result, ProcessignType processignType, ParallelOptions? parallelOptions)
        {
            result = null;

            try
            {
                var srcArray = new byte[source.Length][];

                #region 入力画像をbyte配列に変換する
                for (int i = 0; i < srcArray.Length; i++)
                {
                    var selectedBitmap = source[i];

                    var srcLockedData = selectedBitmap.LockBits(
                                                                 new System.Drawing.Rectangle(System.Drawing.Point.Empty, selectedBitmap.Size)
                                                               , ImageLockMode.ReadOnly
                                                               , selectedBitmap.PixelFormat
                                                               );

                    srcArray[i] = new byte[srcLockedData.Stride * srcLockedData.Height];
                    Marshal.Copy(srcLockedData.Scan0, srcArray[i], 0, srcArray[i].Length);

                    selectedBitmap.UnlockBits(srcLockedData);
                }
                #endregion

                // 結果画像ビットマップを作成する
                result = new Bitmap(source[0].Width, source[0].Height, PixelFormat.Format8bppIndexed);
                BitmapProcessor.AssignColorPaletteOfDefault(result);

                // ビットマップロック
                var dstLockedData = result.LockBits(
                                                    new System.Drawing.Rectangle(System.Drawing.Point.Empty, result.Size)
                                                    , ImageLockMode.WriteOnly
                                                    , result.PixelFormat
                                                    );

                var dstArray = new byte[dstLockedData.Stride * dstLockedData.Height];

                // データ変換
                var width = dstLockedData.Width;
                var height = dstLockedData.Height;

                // データ変換処理
                void comparison(byte[][] inputArray, byte[] outputArray, int index)
                {
                    if (2 < inputArray.Length)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            var value = inputArray[0][index];
                            for (int i = 0; i < inputArray.Length - 1; i++)
                            {
                                value = BitmapProcessor.lutOfMaxGray[inputArray[i][index]][inputArray[i + 1][index]];
                            }
                            outputArray[index++] = value;
                        }
                    }
                    else
                    {
                        for (int x = 0; x < width; x++)
                        {
                            outputArray[index] = BitmapProcessor.lutOfMaxGray[inputArray[0][index]][inputArray[1][index]];
                            index++;
                        }
                    }
                }

                // 並列処理の場合
                if (ProcessignType.Parallel == processignType)
                {
                    // 並列処理設定が指定されていない場合
                    if (null == parallelOptions)
                    {
                        var maxDegreeOfParallelism = BitmapProcessor.DefaultMaxDegreeOfParallelism;
                        parallelOptions = new ParallelOptions()
                        {
                            MaxDegreeOfParallelism = (maxDegreeOfParallelism > 0) ? maxDegreeOfParallelism : -1
                        };
                    }

                    // データ変換処理を並列実行
                    Parallel.ForEach(Enumerable.Range(0, dstLockedData.Height), parallelOptions, y =>
                    {
                        comparison(srcArray, dstArray, y * dstLockedData.Stride);
                    });
                }
                // 並列処理が有効でない場合
                else
                {
                    //データ変換処理を逐次実行
                    foreach (var y in Enumerable.Range(0, dstLockedData.Height))
                    {
                        comparison(srcArray, dstArray, y * dstLockedData.Stride);
                    }
                }

                Marshal.Copy(dstArray, 0, dstLockedData.Scan0, dstArray.Length);

                // ビットマップロック解除
                result.UnlockBits(dstLockedData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 2つの画像の最小値を採用した画像を作成する
        /// </summary>
        /// <param name="source1">入力画像1</param>
        /// <param name="source2">入力画像2</param>
        /// <param name="result">最大値画像</param>
        /// <remarks>PixelFormat.Format8bppIndexedに対応</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void Min(Bitmap source1, Bitmap source2, out Bitmap? result)
        {
            BitmapProcessor.Min(source1, source2, out result, ProcessignType.Sequential, null);
        }

        /// <summary>
        /// 2つの画像の最小値を採用した画像を作成する
        /// </summary>
        /// <param name="source1">入力画像1</param>
        /// <param name="source2">入力画像2</param>
        /// <param name="result">最大値画像</param>
        /// <remarks>PixelFormat.Format8bppIndexedに対応</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void Min(Bitmap source1, Bitmap source2, out Bitmap? result, ProcessignType processignType)
        {
            BitmapProcessor.Min(source1, source2, out result, processignType, null);
        }

        /// <summary>
        /// 2つの画像の最小値を採用した画像を作成する
        /// </summary>
        /// <param name="source1">入力画像1</param>
        /// <param name="source2">入力画像2</param>
        /// <param name="result">最大値画像</param>
        /// <remarks>PixelFormat.Format8bppIndexedに対応</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void Min(Bitmap source1, Bitmap source2, out Bitmap? result, ProcessignType processignType, ParallelOptions? parallelOptions)
        {
            BitmapProcessor.Min(new Bitmap[] { source1, source2 }, out result, processignType, parallelOptions);
        }

        /// <summary>
        /// 入力画像の最小値を採用した画像を作成する
        /// </summary>
        /// <param name="source">入力画像</param>
        /// <param name="result">最大値画像</param>
        /// <remarks>PixelFormat.Format8bppIndexedに対応</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void Min(Bitmap[] source, out Bitmap? result, ProcessignType processignType, ParallelOptions? parallelOptions)
        {
            result = null;

            try
            {
                var srcArray = new byte[source.Length][];

                #region 入力画像をbyte配列に変換する
                for (int i = 0; i < srcArray.Length; i++)
                {
                    var selectedBitmap = source[i];

                    var srcLockedData = selectedBitmap.LockBits(
                                                                 new System.Drawing.Rectangle(System.Drawing.Point.Empty, selectedBitmap.Size)
                                                               , ImageLockMode.ReadOnly
                                                               , selectedBitmap.PixelFormat
                                                               );

                    srcArray[i] = new byte[srcLockedData.Stride * srcLockedData.Height];
                    Marshal.Copy(srcLockedData.Scan0, srcArray[i], 0, srcArray[i].Length);

                    selectedBitmap.UnlockBits(srcLockedData);
                }
                #endregion

                // 結果画像ビットマップを作成する
                result = new Bitmap(source[0].Width, source[0].Height, PixelFormat.Format8bppIndexed);
                BitmapProcessor.AssignColorPaletteOfDefault(result);

                // ビットマップロック
                var dstLockedData = result.LockBits(
                                                    new System.Drawing.Rectangle(System.Drawing.Point.Empty, result.Size)
                                                    , ImageLockMode.WriteOnly
                                                    , result.PixelFormat
                                                    );

                var dstArray = new byte[dstLockedData.Stride * dstLockedData.Height];

                // データ変換
                var width = dstLockedData.Width;
                var height = dstLockedData.Height;

                // データ変換処理
                void comparison(byte[][] inputArray, byte[] outputArray, int index)
                {
                    if (2 < inputArray.Length)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            var value = inputArray[0][index];
                            for (int i = 0; i < inputArray.Length - 1; i++)
                            {
                                value = BitmapProcessor.lutOfMinGray[inputArray[i][index]][inputArray[i + 1][index]];
                            }
                            outputArray[index++] = value;
                        }
                    }
                    else
                    {
                        for (int x = 0; x < width; x++)
                        {
                            outputArray[index] = BitmapProcessor.lutOfMinGray[inputArray[0][index]][inputArray[1][index]];
                            index++;
                        }
                    }
                }

                // 並列処理の場合
                if (ProcessignType.Parallel == processignType)
                {
                    // 並列処理設定が指定されていない場合
                    if (null == parallelOptions)
                    {
                        var maxDegreeOfParallelism = BitmapProcessor.DefaultMaxDegreeOfParallelism;
                        parallelOptions = new ParallelOptions()
                        {
                            MaxDegreeOfParallelism = (maxDegreeOfParallelism > 0) ? maxDegreeOfParallelism : -1
                        };
                    }

                    // データ変換処理を並列実行
                    Parallel.ForEach(Enumerable.Range(0, dstLockedData.Height), parallelOptions, y =>
                    {
                        comparison(srcArray, dstArray, y * dstLockedData.Stride);
                    });
                }
                // 並列処理が有効でない場合
                else
                {
                    //データ変換処理を逐次実行
                    foreach (var y in Enumerable.Range(0, dstLockedData.Height))
                    {
                        comparison(srcArray, dstArray, y * dstLockedData.Stride);
                    }
                }

                Marshal.Copy(dstArray, 0, dstLockedData.Scan0, dstArray.Length);

                // ビットマップロック解除
                result.UnlockBits(dstLockedData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #region ヒストグラム

        /// <summary>
        /// ヒストグラムを取得する
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        /// <param name="info">ヒストグラム情報</param>
        /// <returns>処理が正常に終了した場合はtrue, それ以外の場合はfalse</returns>
        public static bool CalcHistogram(Bitmap bitmap, out HistogramData info)
        {
            var result = BitmapProcessor.CalcHistogram(bitmap, out HistogramData[] data);

            if (data.FirstOrDefault() is HistogramData validData)
            {
                info = validData;
            }
            else
            {
                info = new HistogramData();
            }

            return result;
        }

        /// <summary>
        /// ヒストグラムを取得する
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        /// <param name="datas">ヒストグラム情報</param>
        /// <returns></returns>
        public static bool CalcHistogram(Bitmap bitmap, out HistogramData[] datas)
        {
            // 戻り値を初期化する        
            var isSuccess = false;
            datas = Array.Empty<HistogramData>();

            try
            {
                datas = BitmapProcessor.CalcHistograms(bitmap);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Debug.WriteLine(ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// ヒストグラムを取得する
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        /// <param name="histogram">ヒストグラム</param>
        /// <param name="minimum">最小値</param>
        /// <param name="maximum">最大値</param>
        /// <param name="average">平均値</param>
        /// <param name="mode">最頻値</param>
        /// <returns>処理が正常に終了した場合はtrue, それ以外の場合はfalse</returns>
        [SupportedOSPlatform("windows7.0")]
        public static bool CalcHistogram(Bitmap bitmap, out int[] histogram, out double minimum, out double maximum, out double average, out double mode)
        {
            var result = BitmapProcessor.CalcHistogram(bitmap, out HistogramData info);

            histogram = info.Histogram;
            minimum = info.Min;
            maximum = info.Max;
            average = info.Ave;
            mode = info.Mode;

            return result;
        }

        /// <summary>
        /// グレースケールおよびカラー画像のヒストグラムを計算します。
        /// </summary>
        /// <param name="bitmap">入力画像</param>
        /// <returns>ヒストグラムデータ(Gray, R, G, B の4チャンネル分または Gray のみ)</returns>
        public static HistogramData[] CalcHistograms(Bitmap bitmap)
        {
            var histograms = Array.Empty<HistogramData>();

            using var src = BitmapConverter.ToMat(bitmap);

            // グレースケール画像の場合
            if (1 == src.Channels())
            {
                histograms = new HistogramData[1];
                histograms[0] = BitmapProcessor.CalcHistogram(src);
            }
            // カラー画像の場合
            else if (3 <= src.Channels())
            {
                histograms = new HistogramData[4];

                using var gray = new Mat();
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

                var channels = Array.Empty<Mat>();
                Cv2.Split(src, out channels);

                histograms[0] = BitmapProcessor.CalcHistogram(gray);
                histograms[1] = BitmapProcessor.CalcHistogram(channels[2]);   // R チャンネル
                histograms[2] = BitmapProcessor.CalcHistogram(channels[1]);   // G チャンネル
                histograms[3] = BitmapProcessor.CalcHistogram(channels[0]);   // B チャンネル

                channels[0].Dispose();
                channels[1].Dispose();
                channels[2].Dispose();
            }

            return histograms;
        }

        /// <summary>
        /// 単一チャンネル画像のヒストグラムを計算します。
        /// </summary>
        /// <param name="image">単一チャンネルの Mat オブジェクト</param>
        /// <returns>ヒストグラムデータ（256ビン）</returns>
        public static HistogramData CalcHistogram(Mat image)
        {
            var size = byte.MaxValue + 1;

            using var hist = new Mat();
            var histSize = new int[] { size }; // ヒストグラムのビン数
            Rangef[] ranges = { new Rangef(0, size) }; // ピクセル値の範囲

            // ヒストグラムを計算
            Cv2.CalcHist(new Mat[] { image }, new int[] { 0 }, null, hist, 1, histSize, ranges);

            var data = new HistogramData(size);
            for (var i = 0; i < size; i++)
            {
                data.Histogram[i] = (int)hist.Get<float>(i);
            }

            data.Recalculate(true);

            return data;
        }

        /// <summary>
        /// ヒストグラムから指定値を中心として範囲を求める
        /// </summary>
        /// <param name="histogram">ヒストグラム</param>
        /// <param name="specified">中心値</param>
        /// <param name="ratio">総数に対する採用数の比(～1.0)</param>
        /// <param name="minimum">範囲下限値</param>
        /// <param name="maximum">範囲上限値</param>
        public static void SelectRangeAroundSpecifiedValue(int[] histogram, int specified, double ratio, out int minimum, out int maximum)
        {
            minimum = 0;
            maximum = 0;

            #region パラメータチェック
            if (1 <= ratio)
            {
                minimum = 0;
                maximum = histogram.Length - 1;
                return;
            }
            else if (0 >= ratio)
            {
                return;
            }
            else if (0 > specified)
            {
                return;
            }
            else if (histogram.Length <= specified)
            {
                return;
            }
            #endregion

            try
            {
                // 採用数を取得する
                int adoption = Convert.ToInt32(histogram.Sum() * ratio);

                // 検索用LUT
                var directionLut = new int[] { 1, -1 };

                // 範囲を初期化する
                minimum = specified;
                maximum = specified;

                // 範囲を検索する
                int summation = 0;
                int previous = specified;
                for (int i = 0; i < histogram.Length; i++)
                {
                    int selected = directionLut[i % 2] * i + previous;

                    if ((0 <= selected) && (histogram.Length > selected))
                    {
                        if (minimum > selected)
                        {
                            minimum = selected;
                        }

                        if (maximum < selected)
                        {
                            maximum = selected;
                        }

                        summation += histogram[selected];

                        if (adoption <= summation)
                        {
                            break;
                        }
                    }

                    previous = selected;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #region スケール変換

        /// <summary>
        /// グレイ値を最大グレイ値の範囲に拡張する。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="result"></param>
        [SupportedOSPlatform("windows7.0")]
        public static void ImageRangeScaling(Bitmap source, out Bitmap? result)
        {
            BitmapProcessor.ImageRangeScaling(source, out result, ProcessignType.Sequential);
        }

        /// <summary>
        /// グレイ値を最大グレイ値の範囲に拡張する。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="result"></param>
        [SupportedOSPlatform("windows7.0")]
        public static void ImageRangeScaling(Bitmap source, out Bitmap? result, ProcessignType processignType)
        {
            BitmapProcessor.ImageRangeScaling(source, out result, processignType, null);
        }

        /// <summary>
        /// グレイ値を最大グレイ値の範囲に拡張する。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="result"></param>
        [SupportedOSPlatform("windows7.0")]
        public static void ImageRangeScaling(Bitmap source, out Bitmap? result, ProcessignType processignType, ParallelOptions? parallelOptions)
        {
            if (true == BitmapProcessor.CalcHistogram(source, out _, out double min, out double max, out _, out _))
            {
                BitmapProcessor.ImageRangeScaling(source, out result, (int)min, (int)max, processignType, parallelOptions);
            }
            else
            {
                result = (Bitmap)source.Clone();
            }
        }

        /// <summary>
        /// グレイ値を minimum ～ maximum の最大グレイ値の範囲に拡張する。
        /// </summary>
        /// <param name="source">元のビットマップ</param>
        /// <param name="result">グレイ値を拡張したビットマップ</param>
        [SupportedOSPlatform("windows7.0")]
        public static void ImageRangeScaling(Bitmap source, out Bitmap? result, int minimum, int maximum)
        {
            BitmapProcessor.ImageRangeScaling(source, out result, minimum, maximum, ProcessignType.Sequential);
        }

        /// <summary>
        /// グレイ値を minimum ～ maximum の最大グレイ値の範囲に拡張する。
        /// </summary>
        /// <param name="source">元のビットマップ</param>
        /// <param name="result">グレイ値を拡張したビットマップ</param>
        [SupportedOSPlatform("windows7.0")]
        public static void ImageRangeScaling(Bitmap source, out Bitmap? result, int minimum, int maximum, ProcessignType processignType)
        {
            BitmapProcessor.ImageRangeScaling(source, out result, minimum, maximum, processignType, null);
        }

        /// <summary>
        /// グレイ値を minimum ～ maximum の最大グレイ値の範囲に拡張する。
        /// </summary>
        /// <param name="source">元のビットマップ</param>
        /// <param name="result">グレイ値を拡張したビットマップ</param>
        [SupportedOSPlatform("windows7.0")]
        public static void ImageRangeScaling(Bitmap source, out Bitmap? result, int minimum, int maximum, ProcessignType processignType, ParallelOptions? parallelOptions)
        {
            result = null;

            try
            {
                switch (source.PixelFormat)
                {
                    case PixelFormat.Format8bppIndexed:
                        {
                            #region スケーリング

                            var lockedData = (BitmapData?)null;

                            try
                            {
                                // 256階調のスケーリングLUT作成
                                var scalingLut = new byte[byte.MaxValue + 1];
                                var maxValues = Enumerable.Repeat<byte>(byte.MaxValue, byte.MaxValue - maximum + 1).ToArray();
                                Array.Copy(maxValues, 0, scalingLut, maximum, maxValues.Length);

                                double coeff = (double)scalingLut.Length / (double)(maximum - minimum + 1);
                                double summation = 0;
                                for (int i = minimum; i < maximum; i++)
                                {
                                    scalingLut[i] = (byte)summation;
                                    summation += coeff;
                                }

                                // ビットマップ複製
                                result = (Bitmap)source.Clone();

                                // ビットマップロック                                                                     
                                lockedData = result.LockBits(
                                                               new System.Drawing.Rectangle(System.Drawing.Point.Empty, result.Size)
                                                             , ImageLockMode.ReadWrite
                                                             , result.PixelFormat
                                                             );
                                // 処理データ配列
                                var fullPixels = new byte[lockedData.Stride * lockedData.Height];

                                // ビットマップデータから処理用配列へコピー
                                Marshal.Copy(lockedData.Scan0, fullPixels, 0, fullPixels.Length);

                                // スケーリング処理
                                void scaler(int move)
                                {
                                    for (int x = 0; x < lockedData.Width; x++)
                                    {
                                        fullPixels[move] = scalingLut[fullPixels[move]];
                                        move++;
                                    }
                                }

                                // 並列処理の場合
                                if (ProcessignType.Parallel == processignType)
                                {
                                    // 並列処理設定が指定されていない場合
                                    if (null == parallelOptions)
                                    {
                                        var maxDegreeOfParallelism = BitmapProcessor.DefaultMaxDegreeOfParallelism;
                                        parallelOptions = new ParallelOptions()
                                        {
                                            MaxDegreeOfParallelism = (maxDegreeOfParallelism > 0) ? maxDegreeOfParallelism : -1
                                        };
                                    }

                                    // スケーリング処理を並列実行
                                    Parallel.ForEach(Enumerable.Range(0, lockedData.Height), parallelOptions, y =>
                                    {
                                        scaler(y * lockedData.Stride);
                                    });
                                }
                                // 並列処理が有効でない場合
                                else
                                {
                                    // スケーリング処理を逐次実行
                                    foreach (var y in Enumerable.Range(0, lockedData.Height))
                                    {
                                        scaler(y * lockedData.Stride);
                                    }
                                }

                                // 処理用配列からビットマップデータへコピー
                                Marshal.Copy(fullPixels, 0, lockedData.Scan0, fullPixels.Length);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }
                            finally
                            {
                                if (null != lockedData)
                                {
                                    // ビットマップロック解除
                                    result?.UnlockBits(lockedData);
                                }
                            }
                            #endregion
                        }
                        break;

                    default:
                        {
                            result = (Bitmap)source.Clone();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #region 連結

        /// <summary>
        /// Bitmapを連結する
        /// </summary>
        /// <param name="direction">連結方向</param>
        /// <param name="bitmaps">連結するBitmap</param>
        [SupportedOSPlatform("windows7.0")]
        public static Bitmap? Concat(ConcatDirection direction, params Bitmap[] bitmaps)
        {
            var result = (Bitmap?)null;

            if ((null == bitmaps)
            || (0 > bitmaps.Length))
            {
                return result;
            }

            if (1 == bitmaps.Length)
            {
                result = (Bitmap)bitmaps[0].Clone();
                return result;
            }

            var listWidth = new List<int>();
            var listHeight = new List<int>();
            foreach (var selected in bitmaps)
            {
                listWidth.Add(selected.Width);
                listHeight.Add(selected.Height);
            }

            switch (direction)
            {
                case ConcatDirection.Vertical:
                    {
                        #region 垂直方向に連結する

                        // 連結サイズを取得する
                        int width = listWidth.Max();
                        int height = listHeight.Sum();

                        // 先頭画像を取得する
                        var typicalBitmap = bitmaps[0];

                        // 連結用ビットマップを作成する
                        result = new Bitmap(width, height, typicalBitmap.PixelFormat);

                        // カラーパレットを設定する
                        if (0 < typicalBitmap.Palette.Entries.Length)
                        {
                            result.Palette = typicalBitmap.Palette;
                        }

                        // インデックス付きのピクセル形式の場合
                        if ((PixelFormat.Format1bppIndexed == result.PixelFormat)
                        || (PixelFormat.Format4bppIndexed == result.PixelFormat)
                        || (PixelFormat.Format8bppIndexed == result.PixelFormat))
                        {
                            // ロック
                            var resultData = result.LockBits(
                                                                new System.Drawing.Rectangle(System.Drawing.Point.Empty, result.Size)
                                                                , ImageLockMode.WriteOnly
                                                                , result.PixelFormat
                                                                );
                            try
                            {
                                // 処理データ配列
                                var fullPixels = new byte[resultData.Stride * resultData.Height];

                                // ビットマップデータから処理用配列へコピー
                                Marshal.Copy(resultData.Scan0, fullPixels, 0, fullPixels.Length);

                                int destinationOffset = 0;
                                foreach (var item in bitmaps)
                                {
                                    // ロック
                                    var itemData = item.LockBits(
                                                                    new System.Drawing.Rectangle(System.Drawing.Point.Empty, item.Size)
                                                                , ImageLockMode.ReadOnly
                                                                , item.PixelFormat
                                                                );

                                    try
                                    {
                                        // 処理データ配列
                                        var itemPixels = new byte[itemData.Stride * itemData.Height];

                                        // ビットマップデータから処理用配列へコピー
                                        Marshal.Copy(itemData.Scan0, itemPixels, 0, itemPixels.Length);

                                        // 1行ずつコピーする
                                        int sourceOffset = 0;
                                        for (int i = 0; i < item.Height; i++)
                                        {
                                            Array.Copy(itemPixels, sourceOffset, fullPixels, destinationOffset, itemData.Width);
                                            sourceOffset += itemData.Stride;
                                            destinationOffset += resultData.Stride;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine(ex.Message);
                                    }

                                    // ロック解除
                                    item.UnlockBits(itemData);
                                }

                                // 処理用配列からビットマップデータへコピー
                                Marshal.Copy(fullPixels, 0, resultData.Scan0, fullPixels.Length);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }

                            // ロック解除
                            result.UnlockBits(resultData);
                        }
                        // インデックス付きのピクセル形式でない場合
                        else
                        {
                            using var g = Graphics.FromImage(result);
                            float offset = 0;
                            foreach (var item in bitmaps)
                            {
                                g.DrawImage(item, 0, offset);
                                offset += item.Height;
                            }
                        }

                        #endregion
                    }
                    break;
            }

            return result;
        }

        #endregion

        #region 配列化

        /// <summary>
        /// ビットマップから配列データを作成する
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        /// <param name="array">配列</param>
        [SupportedOSPlatform("windows7.0")]
        public static void ConvertToArray(Bitmap bitmap, out Dictionary<ArrayConversionDirection, byte[][]> array)
        {
            array = new Dictionary<ArrayConversionDirection, byte[][]>();

            foreach (ArrayConversionDirection selectedDirection in Enum.GetValues(typeof(ArrayConversionDirection)))
            {
                BitmapProcessor.ConvertToArray(selectedDirection, bitmap, out byte[][] selectedArray);

                array.Add(selectedDirection, selectedArray);
            }
        }

        /// <summary>
        /// ビットマップから配列データを作成する
        /// </summary>
        /// <param name="direction">走査方向</param>
        /// <param name="bitmap">ビットマップ</param>
        /// <param name="array">配列</param>
        [SupportedOSPlatform("windows7.0")]
        public static void ConvertToArray(ArrayConversionDirection direction, Bitmap bitmap, out byte[][] array)
        {
            array = Array.Empty<byte[]>();

            switch (bitmap.PixelFormat)
            {
                #region Format8bppIndexed
                case PixelFormat.Format8bppIndexed:
                    {
                        using var optimized = (Bitmap)bitmap.Clone();

                        // コピー高速化のための変換を行う
                        switch (direction)
                        {
                            case ArrayConversionDirection.Vertical:
                                {
                                    optimized.RotateFlip(RotateFlipType.Rotate90FlipX);
                                }
                                break;
                        }

                        // データロック
                        var lockedData = optimized.LockBits(
                                                          new System.Drawing.Rectangle(System.Drawing.Point.Empty, optimized.Size)
                                                        , ImageLockMode.ReadOnly
                                                        , optimized.PixelFormat
                                                        );

                        try
                        {
                            int lineOffset = 0;

                            // 1ラインずつコピーを行う
                            array = new byte[lockedData.Height][];

                            for (int i = 0; i < array.Length; i++)
                            {
                                var line = new byte[lockedData.Width];
                                Marshal.Copy(lockedData.Scan0 + lineOffset, line, 0, line.Length);
                                array[i] = line;

                                lineOffset += lockedData.Stride;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }

                        // データアンロック
                        optimized.UnlockBits(lockedData);
                    }
                    break;
                #endregion

                #region Format32bppArgb
                case PixelFormat.Format32bppArgb:
                    {
                        using var optimized = (Bitmap)bitmap.Clone();

                        // コピー高速化のための変換を行う
                        switch (direction)
                        {
                            case ArrayConversionDirection.Vertical:
                                {
                                    optimized.RotateFlip(RotateFlipType.Rotate90FlipX);
                                }
                                break;
                        }

                        // データロック
                        var lockedData = optimized.LockBits(
                                                          new System.Drawing.Rectangle(System.Drawing.Point.Empty, optimized.Size)
                                                        , ImageLockMode.ReadOnly
                                                        , optimized.PixelFormat
                                                        );

                        try
                        {
                            // 処理データ配列
                            var fullPixels = new byte[lockedData.Stride * lockedData.Height];

                            // ビットマップデータから処理用配列へコピー
                            Marshal.Copy(lockedData.Scan0, fullPixels, 0, fullPixels.Length);

                            int lineOffset = 0;

                            // 1ラインずつコピーを行う
                            var tempArray = new byte[lockedData.Height][];

                            for (int i = 0; i < tempArray.Length; i++)
                            {
                                var line = new byte[lockedData.Width];

                                for (int j = 0; j < line.Length; j++)
                                {
                                    byte rgb1 = fullPixels[lineOffset + j * 4 + 0];
                                    byte rgb2 = fullPixels[lineOffset + j * 4 + 1];
                                    byte rgb3 = fullPixels[lineOffset + j * 4 + 2];
                                    if (rgb1 != rgb2 || rgb1 != rgb3)
                                    {
                                        throw new InvalidOperationException("グレースケール画像ではありません");
                                    }
                                    line[j] = rgb1;
                                }

                                tempArray[i] = line;

                                lineOffset += lockedData.Stride;
                            }

                            array = tempArray;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }

                        // データアンロック
                        optimized.UnlockBits(lockedData);
                    }
                    break;
                    #endregion
            }
        }

        /// <summary>
        /// ビットマップから配列データを作成する
        /// </summary>
        /// <param name="direction">走査方向</param>
        /// <param name="bitmap">ビットマップ</param>
        /// <param name="array">配列</param>
        /// <param name="channel">0:B, 1:G, 2:R</param>
        [SupportedOSPlatform("windows7.0")]
        public static void ConvertToArray(Bitmap bitmap, out byte[] array, int channel = 0, Common.Drawing.Rectangle? rect = null, ArrayConversionDirection direction = ArrayConversionDirection.Horizontal)
        {
            array = Array.Empty<byte>();

            switch (bitmap.PixelFormat)
            {
                #region 対応PixelFormat
                case PixelFormat.Format8bppIndexed:
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                    {
                        using var optimized = (Bitmap)bitmap.Clone();

                        rect ??= new Common.Drawing.Rectangle(Common.Drawing.Point.New(), Common.Drawing.Size.New(bitmap.Width, bitmap.Height));

                        // コピー高速化のための変換を行う
                        switch (direction)
                        {
                            case ArrayConversionDirection.Vertical:
                                {
                                    optimized.RotateFlip(RotateFlipType.Rotate90FlipX);

                                    rect.ToPoints(out Common.Drawing.PointD[] points);
                                    var bitmapRect = new Common.Drawing.RectangleD(Common.Drawing.PointD.New(), new Common.Drawing.SizeD(bitmap.Size));
                                    bitmapRect.GetCenterPoint(out Common.Drawing.PointD centerPoint);
                                    Common.Drawing.GraphicsUtilities.GetRotation(points, Common.Mathematics.DegreeToRadian(90), centerPoint, out Common.Drawing.PointD[] destination);
                                    rect = GraphicsUtilities.PointsToRectangle(destination).ToRectangleInt();
                                }
                                break;
                        }

                        // データロック
                        var lockedData = optimized.LockBits(
                                                          new System.Drawing.Rectangle(System.Drawing.Point.Empty, optimized.Size)
                                                        , ImageLockMode.ReadOnly
                                                        , optimized.PixelFormat
                                                        );

                        // byte配列化
                        var srcStride = lockedData.Stride;
                        var srcArray = new byte[srcStride * lockedData.Height];
                        Marshal.Copy(lockedData.Scan0, srcArray, 0, srcArray.Length);

                        // データアンロック
                        optimized.UnlockBits(lockedData);

                        try
                        {
                            array = new byte[rect.Size.Height * rect.Size.Width];

                            var pixUnit = 1;
                            var pixIndex = 0;
                            if (PixelFormat.Format8bppIndexed == bitmap.PixelFormat)
                            {
                                var dstIndex = 0;
                                foreach (var y in Enumerable.Range(rect.Top, rect.Size.Height))
                                {
                                    var line = new ArraySegment<byte>(srcArray, y * srcStride + rect.Left, rect.Size.Width);

                                    line.CopyTo(array, dstIndex);
                                    dstIndex += line.Count;
                                }
                            }
                            else
                            {
                                pixUnit = (PixelFormat.Format24bppRgb == bitmap.PixelFormat) ? 3 : 4;
                                pixIndex = Math.Clamp(channel, 0, pixUnit - 1);

                                var dstIndex = 0;
                                foreach (var y in Enumerable.Range(rect.Top, rect.Size.Height))
                                {
                                    var line = new ArraySegment<byte>(srcArray, y * srcStride + rect.Left * pixUnit, rect.Size.Width * pixUnit);

                                    var srcIndex = 0;
                                    foreach (var x in Enumerable.Range(0, rect.Size.Width))
                                    {
                                        array[dstIndex++] = line[srcIndex + pixIndex];
                                        srcIndex += pixUnit;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                    break;
                    #endregion
            }
        }

        /// <summary>
        /// ビットマップから配列データを作成する
        /// </summary>
        /// <param name="direction">走査方向</param>
        /// <param name="bitmap">ビットマップ</param>
        /// <param name="array">配列</param>
        /// <param name="channel">0:B, 1:G, 2:R</param>
        [SupportedOSPlatform("windows7.0")]
        public static void ConvertToArray(Bitmap bitmap, out float[] array, int channel = 0, Common.Drawing.Rectangle? rect = null, ArrayConversionDirection direction = ArrayConversionDirection.Horizontal)
        {
            array = Array.Empty<float>();

            switch (bitmap.PixelFormat)
            {
                #region 対応PixelFormat
                case PixelFormat.Format8bppIndexed:
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                    {
                        using var optimized = (Bitmap)bitmap.Clone();

                        rect ??= new Common.Drawing.Rectangle(Common.Drawing.Point.New(), Common.Drawing.Size.New(bitmap.Width, bitmap.Height));

                        // コピー高速化のための変換を行う
                        switch (direction)
                        {
                            case ArrayConversionDirection.Vertical:
                                {
                                    optimized.RotateFlip(RotateFlipType.Rotate90FlipX);

                                    rect.ToPoints(out PointD[] points);
                                    var bitmapRect = new Common.Drawing.RectangleD(Common.Drawing.PointD.New(), new Common.Drawing.SizeD(bitmap.Size));
                                    bitmapRect.GetCenterPoint(out PointD centerPoint);
                                    GraphicsUtilities.GetRotation(points, Mathematics.DegreeToRadian(90), centerPoint, out PointD[] destination);
                                    rect = GraphicsUtilities.PointsToRectangle(destination).ToRectangleInt();
                                }
                                break;
                        }

                        // データロック
                        var lockedData = optimized.LockBits(
                                                          new System.Drawing.Rectangle(System.Drawing.Point.Empty, optimized.Size)
                                                        , ImageLockMode.ReadOnly
                                                        , optimized.PixelFormat
                                                        );

                        // byte配列化
                        var srcStride = lockedData.Stride;
                        var srcArray = new byte[srcStride * lockedData.Height];
                        Marshal.Copy(lockedData.Scan0, srcArray, 0, srcArray.Length);

                        // データアンロック
                        optimized.UnlockBits(lockedData);

                        try
                        {
                            array = new float[rect.Size.Height * rect.Size.Width];

                            var pixUnit = 1;
                            var pixIndex = 0;
                            if (PixelFormat.Format8bppIndexed != bitmap.PixelFormat)
                            {
                                pixUnit = (PixelFormat.Format24bppRgb == bitmap.PixelFormat) ? 3 : 4;
                                pixIndex = Math.Clamp(channel, 0, pixUnit - 1);
                            }

                            var dstIndex = 0;
                            foreach (var y in Enumerable.Range(rect.Top, rect.Size.Height))
                            {
                                var line = new ArraySegment<byte>(srcArray, y * srcStride + rect.Left * pixUnit, rect.Size.Width * pixUnit);

                                var srcIndex = 0;
                                foreach (var x in Enumerable.Range(0, rect.Size.Width))
                                {
                                    array[dstIndex++] = line[srcIndex + pixIndex];
                                    srcIndex += pixUnit;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                    break;
                    #endregion
            }
        }

        /// <summary>
        /// ビットマップから配列データを作成する
        /// </summary>
        /// <param name="direction">走査方向</param>
        /// <param name="bitmap">ビットマップ</param>
        /// <param name="array">配列</param>
        /// <param name="channel">0:B, 1:G, 2:R</param>
        [SupportedOSPlatform("windows7.0")]
        public static void ConvertToArray(Bitmap bitmap, out int[] array, int channel = 0, Common.Drawing.Rectangle? rect = null, ArrayConversionDirection direction = ArrayConversionDirection.Horizontal)
        {
            array = Array.Empty<int>();

            switch (bitmap.PixelFormat)
            {
                #region 対応PixelFormat
                case PixelFormat.Format8bppIndexed:
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                    {
                        using var optimized = (Bitmap)bitmap.Clone();

                        rect ??= new Common.Drawing.Rectangle(Common.Drawing.Point.New(), Common.Drawing.Size.New(bitmap.Width, bitmap.Height));

                        // コピー高速化のための変換を行う
                        switch (direction)
                        {
                            case ArrayConversionDirection.Vertical:
                                {
                                    optimized.RotateFlip(RotateFlipType.Rotate90FlipX);

                                    rect.ToPoints(out PointD[] points);
                                    var bitmapRect = new Common.Drawing.RectangleD(Common.Drawing.PointD.New(), new Common.Drawing.SizeD(bitmap.Size));
                                    bitmapRect.GetCenterPoint(out PointD centerPoint);
                                    GraphicsUtilities.GetRotation(points, Mathematics.DegreeToRadian(90), centerPoint, out PointD[] destination);
                                    rect = GraphicsUtilities.PointsToRectangle(destination).ToRectangleInt();
                                }
                                break;
                        }

                        // データロック
                        var lockedData = optimized.LockBits(
                                                          new System.Drawing.Rectangle(System.Drawing.Point.Empty, optimized.Size)
                                                        , ImageLockMode.ReadOnly
                                                        , optimized.PixelFormat
                                                        );

                        // byte配列化
                        var srcStride = lockedData.Stride;
                        var srcArray = new byte[srcStride * lockedData.Height];
                        Marshal.Copy(lockedData.Scan0, srcArray, 0, srcArray.Length);

                        // データアンロック
                        optimized.UnlockBits(lockedData);

                        try
                        {
                            array = new int[rect.Size.Height * rect.Size.Width];

                            var pixUnit = 1;
                            var pixIndex = 0;
                            if (PixelFormat.Format8bppIndexed != bitmap.PixelFormat)
                            {
                                pixUnit = (PixelFormat.Format24bppRgb == bitmap.PixelFormat) ? 3 : 4;
                                pixIndex = Math.Clamp(channel, 0, pixUnit - 1);
                            }

                            var dstIndex = 0;
                            foreach (var y in Enumerable.Range(rect.Top, rect.Size.Height))
                            {
                                var line = new ArraySegment<byte>(srcArray, y * srcStride + rect.Left * pixUnit, rect.Size.Width * pixUnit);

                                var srcIndex = 0;
                                foreach (var x in Enumerable.Range(0, rect.Size.Width))
                                {
                                    array[dstIndex++] = line[srcIndex + pixIndex];
                                    srcIndex += pixUnit;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                    break;
                    #endregion
            }
        }

        /// <summary>
        /// ビットマップから配列データを作成する
        /// </summary>
        /// <param name="direction">走査方向</param>
        /// <param name="bitmap">ビットマップ</param>
        /// <param name="array">配列</param>
        /// <param name="channel">0:B, 1:G, 2:R</param>
        [SupportedOSPlatform("windows7.0")]
        public static void ConvertToArray(Bitmap bitmap, out byte[][] array, Common.Drawing.Rectangle? rect = null, ArrayConversionDirection direction = ArrayConversionDirection.Horizontal)
        {
            array = Array.Empty<byte[]>();

            switch (bitmap.PixelFormat)
            {
                #region 対応PixelFormat
                case PixelFormat.Format8bppIndexed:
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                    {
                        using var optimized = (Bitmap)bitmap.Clone();

                        rect ??= new Common.Drawing.Rectangle(Common.Drawing.Point.New(), Common.Drawing.Size.New(bitmap.Width, bitmap.Height));

                        // コピー高速化のための変換を行う
                        switch (direction)
                        {
                            case ArrayConversionDirection.Vertical:
                                {
                                    optimized.RotateFlip(RotateFlipType.Rotate90FlipX);

                                    rect.ToPoints(out Common.Drawing.PointD[] points);
                                    var bitmapRect = new Common.Drawing.RectangleD(Common.Drawing.PointD.New(), new Common.Drawing.SizeD(bitmap.Size));
                                    bitmapRect.GetCenterPoint(out Common.Drawing.PointD centerPoint);
                                    Common.Drawing.GraphicsUtilities.GetRotation(points, Common.Mathematics.DegreeToRadian(90), centerPoint, out Common.Drawing.PointD[] destination);
                                    rect = GraphicsUtilities.PointsToRectangle(destination).ToRectangleInt();
                                }
                                break;
                        }

                        // データロック
                        var lockedData = optimized.LockBits(
                                                          new System.Drawing.Rectangle(System.Drawing.Point.Empty, optimized.Size)
                                                        , ImageLockMode.ReadOnly
                                                        , optimized.PixelFormat
                                                        );

                        // byte配列化
                        var srcStride = lockedData.Stride;
                        var srcArray = new byte[srcStride * lockedData.Height];
                        Marshal.Copy(lockedData.Scan0, srcArray, 0, srcArray.Length);

                        // データアンロック
                        optimized.UnlockBits(lockedData);

                        try
                        {
                            var pixUnit = 1;
                            if (PixelFormat.Format8bppIndexed == bitmap.PixelFormat)
                            {
                                array = new byte[1][];
                                array[0] = new byte[rect.Size.Height * rect.Size.Width];

                                var dstIndex = 0;
                                foreach (var y in Enumerable.Range(rect.Top, rect.Size.Height))
                                {
                                    var line = new ArraySegment<byte>(srcArray, y * srcStride + rect.Left, rect.Size.Width);

                                    line.CopyTo(array[0], dstIndex);
                                    dstIndex += line.Count;
                                }
                            }
                            else
                            {
                                array = new byte[3][];
                                foreach (var i in Enumerable.Range(0, array.Length))
                                {
                                    array[i] = new byte[rect.Size.Height * rect.Size.Width];
                                }

                                pixUnit = (PixelFormat.Format24bppRgb == bitmap.PixelFormat) ? 3 : 4;

                                var dstIndex = 0;
                                foreach (var y in Enumerable.Range(rect.Top, rect.Size.Height))
                                {
                                    var line = new ArraySegment<byte>(srcArray, y * srcStride + rect.Left * pixUnit, rect.Size.Width * pixUnit);

                                    var srcIndex = 0;
                                    foreach (var x in Enumerable.Range(0, rect.Size.Width))
                                    {
                                        array[0][dstIndex] = line[srcIndex + 0];
                                        array[1][dstIndex] = line[srcIndex + 1];
                                        array[2][dstIndex] = line[srcIndex + 2];

                                        dstIndex++;
                                        srcIndex += pixUnit;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }
                    }
                    break;
                    #endregion
            }
        }

        #endregion

        #region Bitmap作成

        /// <summary>
        /// ビットマップを作成する
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="pixelFormat"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows7.0")]
        public static Bitmap CreateBitmap(int width, int height, PixelFormat pixelFormat = PixelFormat.Format8bppIndexed)
        {
            var bitmap = new Bitmap(width, height, pixelFormat);

            switch (pixelFormat)
            {
                #region Format8bppIndexed
                case PixelFormat.Format8bppIndexed:
                    {
                        BitmapProcessor.AssignColorPaletteOfDefault(bitmap);
                    }
                    break;
                #endregion

                default:
                    {
                    }
                    break;
            }

            return bitmap;
        }

        /// <summary>
        /// 複製
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows7.0")]
        public static Bitmap? Clone(Bitmap? source)
        {
            return BitmapProcessor.Clone(source, ProcessignType.Sequential, BitmapProcessor.NewParallelOptions());
        }

        /// <summary>
        /// 複製
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [SupportedOSPlatform("windows7.0")]
        public static Bitmap? Clone(Bitmap? source, ProcessignType processignType, ParallelOptions parallelOptions)
        {
            if (null == source)
            {
                return null;
            }

            var bitmap = BitmapProcessor.CreateBitmap(source.Width, source.Height, source.PixelFormat);

            try
            {
                var baseData = source.LockBits(new System.Drawing.Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, source.PixelFormat);
                var copyData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                try
                {
                    // 並列処理の場合
                    if (ProcessignType.Parallel == processignType)
                    {
                        // 並列処理設定が指定されていない場合
                        if (null == parallelOptions)
                        {
                            var maxDegreeOfParallelism = BitmapProcessor.DefaultMaxDegreeOfParallelism;
                            parallelOptions = new ParallelOptions()
                            {
                                MaxDegreeOfParallelism = (maxDegreeOfParallelism > 0) ? maxDegreeOfParallelism : -1
                            };
                        }

                        var temporary = new byte[parallelOptions.MaxDegreeOfParallelism][];
                        for (int i = 0; i < temporary.Length; i++)
                        {
                            temporary[i] = new byte[baseData.Stride];
                        }

                        Parallel.ForEach(Enumerable.Range(0, baseData.Height), parallelOptions, y =>
                        {
                            var buffer = temporary[y % temporary.Length];

                            Marshal.Copy(baseData.Scan0 + y * baseData.Stride, buffer, 0, baseData.Stride);
                            Marshal.Copy(buffer, 0, copyData.Scan0 + y * baseData.Stride, baseData.Stride);
                        });
                    }
                    // 並列処理が有効でない場合
                    else
                    {
                        var temporary = new byte[baseData.Stride];
                        var location = 0;
                        for (int i = 0; i < baseData.Height; i++)
                        {
                            Marshal.Copy(baseData.Scan0 + location, temporary, 0, baseData.Stride);
                            Marshal.Copy(temporary, 0, copyData.Scan0 + location, baseData.Stride);

                            location += baseData.Stride;
                        }
                    }

                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    bitmap.UnlockBits(copyData);
                    source.UnlockBits(baseData);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return bitmap;
        }

        /// <summary>
        /// byte型の2次元配列をBitmapに変換する
        /// </summary>
        /// <param name="array"></param>
        /// <param name="imageSize"></param>
        /// <param name="pixelFormat"></param>
        /// <param name="bitmap"></param>
        [SupportedOSPlatform("windows7.0")]
        public static void ConvertToBitmap(byte[][] array, Size imageSize, PixelFormat pixelFormat, out Bitmap? bitmap)
        {
            BitmapProcessor.ConvertToBitmap(array, imageSize, pixelFormat, out bitmap, ProcessignType.Sequential, BitmapProcessor.NewParallelOptions());
        }

        /// <summary>
        /// byte型の2次元配列をBitmapに変換する
        /// </summary>
        /// <param name="array">2次元配列</param>
        /// <param name="imageSize">出力画像サイズ</param>
        /// <param name="pixelFormat">ピクセルフォーマット</param>
        /// <param name="bitmap">作成されたBitmap</param>
        /// <remarks>配列の1次元目は画像のY座標で、2次元目はX座標となるように格納されているものとし、カラーの場合はBGRの順番で並ぶものとする</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void ConvertToBitmap(byte[][] array, Size imageSize, PixelFormat pixelFormat, out Bitmap? bitmap, ProcessignType processignType, ParallelOptions parallelOptions)
        {
            bitmap = null;

            try
            {
                switch (pixelFormat)
                {
                    case PixelFormat.Format8bppIndexed:
                        if (array.Length == imageSize.Height && array[0].Length == imageSize.Width)
                        {
                            BitmapProcessor.ConvertToBitmap(array, out bitmap, processignType, parallelOptions);
                        }
                        else
                        {
                            var cloppedArray = new byte[System.Math.Min(array.Length, imageSize.Height)][];
                            var cloppedWidth = System.Math.Min(array[0].Length, imageSize.Width);

                            Parallel.ForEach(Enumerable.Range(0, cloppedArray.Length), parallelOptions, y =>
                            {
                                cloppedArray[y] = new ArraySegment<byte>(array[y], 0, cloppedWidth).ToArray();
                            });

                            BitmapProcessor.ConvertToBitmap(cloppedArray, out bitmap, processignType, parallelOptions);
                        }
                        break;

                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppRgb:
                    case PixelFormat.Format32bppArgb:
                        {
                            var addIndex = (PixelFormat.Format24bppRgb == pixelFormat) ? 0 : 1;
                            var pixelUnit = addIndex + 3;

                            var cloppedHeight = System.Math.Min(array.Length, imageSize.Height);
                            var cloppedWidth = System.Math.Min(array[0].Length / pixelUnit, imageSize.Width);

                            var tempBitmap = new Bitmap(cloppedWidth, cloppedHeight, pixelFormat);

                            var lockedData = tempBitmap.LockBits(
                                                               new System.Drawing.Rectangle(System.Drawing.Point.Empty, tempBitmap.Size)
                                                             , ImageLockMode.WriteOnly
                                                             , tempBitmap.PixelFormat
                                                             );

                            Parallel.ForEach(Enumerable.Range(0, lockedData.Height), parallelOptions, y =>
                            {
                                Marshal.Copy(array[y], 0, lockedData.Scan0 + lockedData.Stride * y, lockedData.Stride);
                            });

                            tempBitmap.UnlockBits(lockedData);

                            bitmap = tempBitmap;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// byte型の2次元配列をBitmapに変換する(8ビットグレースケール)
        /// </summary>
        /// <param name="array"></param>
        /// <param name="bitmap"></param>
        [SupportedOSPlatform("windows7.0")]
        public static void ConvertToBitmap(byte[][] array, out Bitmap? bitmap)
        {
            BitmapProcessor.ConvertToBitmap(array, out bitmap, ProcessignType.Sequential, BitmapProcessor.NewParallelOptions());
        }

        /// <summary>
        /// byte型の2次元配列をBitmapに変換する(8ビットグレースケール)
        /// </summary>
        /// <param name="array">2次元配列</param>
        /// <param name="bitmap">作成されたBitmap</param>
        /// <param name="processignType">処理タイプ</param>
        /// <param name="parallelOptions">並列処理オプション</param>
        /// <remarks>配列の1次元目は画像のY座標で、2次元目はX座標となるように格納されているものとする</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void ConvertToBitmap(byte[][] array, out Bitmap? bitmap, ProcessignType processignType, ParallelOptions parallelOptions)
        {
            bitmap = null;

            // Bitmapを作成する
            if (null != array)
            {
                // 配列長から画像サイズを得る
                int height = array.Length;
                int width = (null != array[0] && 0 < height) ? array[0].Length : 0;

                bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                BitmapProcessor.AssignColorPaletteOfDefault(bitmap);
            }
            else
            {
                return;
            }

            // ビットマップロック
            var lockedData = bitmap.LockBits(
                                               new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size)
                                             , ImageLockMode.WriteOnly
                                             , bitmap.PixelFormat
                                             );

            // データサイズを調整して処理用配列にコピー
            int destinationIndex = 0;
            for (int i = 0; i < lockedData.Height; i++)
            {
                Marshal.Copy(array[i], 0, lockedData.Scan0 + destinationIndex, array[i].Length);

                destinationIndex += lockedData.Stride;
            }

            // ビットマップロック解除
            bitmap.UnlockBits(lockedData);
        }

        /// <summary>
        /// byte型の1次元配列をBitmapに変換する(8ビットグレースケール)
        /// </summary>
        /// <param name="array">1次元配列</param>
        /// <param name="width">画像幅</param>
        /// <param name="height">画像高さ</param>
        /// <param name="bitmap">作成されたBitmap</param>
        /// <remarks>配列長はwidth x heightで、かつ画像の左上から水平方向に順番に格納されているものとする</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void ConvertToBitmap(byte[] array, int width, int height, out Bitmap bitmap)
        {
            // Bitmapを作成する
            bitmap = new Bitmap((0 < width) ? width : 0, (0 < height) ? height : 0, PixelFormat.Format8bppIndexed);
            BitmapProcessor.AssignColorPaletteOfDefault(bitmap);

            // ビットマップロック
            var lockedData = bitmap.LockBits(
                                               new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size)
                                             , ImageLockMode.WriteOnly
                                             , bitmap.PixelFormat
                                             );

            // 処理データ配列を確保
            var fullPixels = new byte[lockedData.Stride * lockedData.Height];

            // データサイズを調整して処理用配列にコピー
            int sourceIndex = 0;
            int destinationIndex = 0;
            for (int i = 0; i < lockedData.Height; i++)
            {
                Array.Copy(array, sourceIndex, fullPixels, destinationIndex, lockedData.Width);

                sourceIndex += lockedData.Width;
                destinationIndex += lockedData.Stride;
            }

            // 処理用配列からビットマップデータへコピー
            Marshal.Copy(fullPixels, 0, lockedData.Scan0, fullPixels.Length);

            // ビットマップロック解除
            bitmap.UnlockBits(lockedData);
        }

        /// <summary>
        /// サイズを変更したBitmapを作成する
        /// </summary>
        /// <param name="source">元のBitmap</param>
        /// <param name="result">結果のBitmap</param>
        /// <param name="resizeRatio">元の画像に対するサイズ比</param>
        /// <remarks>最近傍補間</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void ResizeSimply(Bitmap source, out Bitmap? result, double resizeRatio)
        {
            BitmapProcessor.ResizeSimply(source, out result, resizeRatio, resizeRatio);
        }

        /// <summary>
        /// サイズを変更したBitmapを作成する
        /// </summary>
        /// <param name="source">元のBitmap</param>
        /// <param name="result">結果のBitmap</param>
        /// <param name="resizeRatioW">元の画像幅に対するサイズ比</param>
        /// <param name="resizeRatioH">元の画像高さに対するサイズ比</param>
        /// <remarks>最近傍補間</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void ResizeSimply(Bitmap source, out Bitmap? result, double resizeRatioW, double resizeRatioH)
        {
            #region サイズを変更したBitmapを作成する
            if (null != source)
            {
                var height = (int)(source.Height * resizeRatioH);
                var width = (int)(source.Width * resizeRatioW);

                result = new Bitmap(width, height, source.PixelFormat);
            }
            else
            {
                result = null;
                return;
            }
            #endregion

            try
            {
                switch (source.PixelFormat)
                {
                    #region Format8bppIndexed
                    case PixelFormat.Format8bppIndexed:
                        {
                            // カラーパレットを設定する
                            BitmapProcessor.AssignColorPaletteOfDefault(result);

                            // 元ビットマップロック
                            var sourceLockedData = source.LockBits(
                                                               new System.Drawing.Rectangle(System.Drawing.Point.Empty, source.Size)
                                                             , ImageLockMode.ReadOnly
                                                             , source.PixelFormat
                                                             );
                            var sourceArray = new byte[sourceLockedData.Width];

                            // 結果ビットマップロック
                            var resultLockedData = result.LockBits(
                                                               new System.Drawing.Rectangle(System.Drawing.Point.Empty, result.Size)
                                                             , ImageLockMode.WriteOnly
                                                             , result.PixelFormat
                                                             );
                            var destinationArray = new byte[resultLockedData.Width];

                            // X座標テーブルを作成する                                                                
                            var sourceX = new int[resultLockedData.Width];
                            for (int i = 0; i < sourceX.Length; i++)
                            {
                                sourceX[i] = (int)(i / resizeRatioW);
                            }

                            // データコピー
                            int destinationIndex = 0;
                            for (int i = 0; i < resultLockedData.Height; i++)
                            {
                                // 元画像のY座標を取得する
                                var sourceY = (int)(i / resizeRatioH);

                                // 元画像のデータを取得する
                                Marshal.Copy(sourceLockedData.Scan0 + sourceLockedData.Stride * sourceY, sourceArray, 0, sourceArray.Length);

                                // データをコピーする
                                for (int j = 0; j < resultLockedData.Width; j++)
                                {
                                    destinationArray[j] = sourceArray[sourceX[j]];
                                }
                                Marshal.Copy(destinationArray, 0, resultLockedData.Scan0 + destinationIndex, destinationArray.Length);

                                destinationIndex += resultLockedData.Stride;
                            }

                            // ビットマップロック解除
                            result.UnlockBits(resultLockedData);
                            source.UnlockBits(sourceLockedData);
                        }
                        break;
                    #endregion

                    default:
                        {
                            using var g = Graphics.FromImage(result);
                            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                            g.DrawImage(source, 0, 0, result.Width, result.Height);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// RGB画像をグレイスケール画像に変換する
        /// </summary>
        /// <param name="source">元のBitmap</param>
        /// <param name="result">結果のBitmap</param>
        /// <remarks>Format24bppRgb対応</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void ConvertRgbToGray(Bitmap source, out Bitmap? result, ProcessignType processignType, ParallelOptions parallelOptions)
        {
            // 出力値を初期化する
            result = null;

            try
            {
                switch (source.PixelFormat)
                {
                    #region Format24bppRgb
                    case PixelFormat.Format24bppRgb:
                        {
                            #region ピクセルフォーマットを変更したBitmapを作成する
                            {
                                result = new Bitmap(source.Width, source.Height, PixelFormat.Format8bppIndexed);
                                BitmapProcessor.AssignColorPaletteOfDefault(result);
                            }
                            #endregion

                            // 元データ配列取得
                            var srcArray = (byte[]?)null;
                            var srcStride = 0;
                            {
                                var sourceLockedData = source.LockBits(
                                                                   new System.Drawing.Rectangle(System.Drawing.Point.Empty, source.Size)
                                                                 , ImageLockMode.ReadOnly
                                                                 , source.PixelFormat
                                                                 );
                                srcStride = sourceLockedData.Stride;
                                srcArray = new byte[srcStride * sourceLockedData.Height];
                                Marshal.Copy(sourceLockedData.Scan0, srcArray, 0, srcArray.Length);
                                source.UnlockBits(sourceLockedData);
                            }

                            // 結果ビットマップロック
                            var dstLockedData = result.LockBits(
                                                               new System.Drawing.Rectangle(System.Drawing.Point.Empty, result.Size)
                                                             , ImageLockMode.WriteOnly
                                                             , result.PixelFormat
                                                             );
                            var dstArray = new byte[dstLockedData.Stride * dstLockedData.Height];

                            // データ変換
                            var width = dstLockedData.Width;
                            var height = dstLockedData.Height;

                            // データ変換処理
                            void convertor(int srcIndex, int dstIndex)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    var grayVal = BitmapProcessor.lutOfConvRgbToGrayFromB[srcArray[srcIndex++]];
                                    grayVal += BitmapProcessor.lutOfConvRgbToGrayFromG[srcArray[srcIndex++]];
                                    grayVal += BitmapProcessor.lutOfConvRgbToGrayFromR[srcArray[srcIndex++]];
                                    dstArray[dstIndex++] = (byte)grayVal;
                                }
                            }

                            // 並列処理の場合
                            if (ProcessignType.Parallel == processignType)
                            {
                                // 並列処理設定が指定されていない場合
                                if (null == parallelOptions)
                                {
                                    var maxDegreeOfParallelism = BitmapProcessor.DefaultMaxDegreeOfParallelism;
                                    parallelOptions = new ParallelOptions()
                                    {
                                        MaxDegreeOfParallelism = (maxDegreeOfParallelism > 0) ? maxDegreeOfParallelism : -1
                                    };
                                }

                                // データ変換処理を並列実行
                                Parallel.ForEach(Enumerable.Range(0, dstLockedData.Height), parallelOptions, y =>
                                {
                                    convertor(y * srcStride, y * dstLockedData.Stride);
                                });
                            }
                            // 並列処理が有効でない場合
                            else
                            {
                                //データ変換処理を逐次実行
                                foreach (var y in Enumerable.Range(0, dstLockedData.Height))
                                {
                                    convertor(y * srcStride, y * dstLockedData.Stride);
                                }
                            }


                            Marshal.Copy(dstArray, 0, dstLockedData.Scan0, dstArray.Length);

                            // ビットマップロック解除
                            result.UnlockBits(dstLockedData);
                        }
                        break;
                    #endregion

                    #region Format32bppArgb
                    case PixelFormat.Format32bppArgb:
                        {
                            #region ピクセルフォーマットを変更したBitmapを作成する
                            {
                                result = new Bitmap(source.Width, source.Height, PixelFormat.Format8bppIndexed);
                                BitmapProcessor.AssignColorPaletteOfDefault(result);
                            }
                            #endregion

                            // 元データ配列取得
                            var srcArray = (byte[]?)null;
                            var srcStride = 0;
                            {
                                var sourceLockedData = source.LockBits(
                                                                   new System.Drawing.Rectangle(System.Drawing.Point.Empty, source.Size)
                                                                 , ImageLockMode.ReadOnly
                                                                 , source.PixelFormat
                                                                 );
                                srcStride = sourceLockedData.Stride;
                                srcArray = new byte[srcStride * sourceLockedData.Height];
                                Marshal.Copy(sourceLockedData.Scan0, srcArray, 0, srcArray.Length);
                                source.UnlockBits(sourceLockedData);
                            }

                            // 結果ビットマップロック
                            var dstLockedData = result.LockBits(
                                                               new System.Drawing.Rectangle(System.Drawing.Point.Empty, result.Size)
                                                             , ImageLockMode.WriteOnly
                                                             , result.PixelFormat
                                                             );
                            var dstArray = new byte[dstLockedData.Stride * dstLockedData.Height];

                            // データ変換
                            var width = dstLockedData.Width;
                            var height = dstLockedData.Height;

                            // データ変換処理
                            void convertor(int srcIndex, int dstIndex)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    var grayVal = BitmapProcessor.lutOfConvRgbToGrayFromB[srcArray[srcIndex++]];
                                    grayVal += BitmapProcessor.lutOfConvRgbToGrayFromG[srcArray[srcIndex++]];
                                    grayVal += BitmapProcessor.lutOfConvRgbToGrayFromR[srcArray[srcIndex++]];
                                    srcIndex++; // α値を読み飛ばす
                                    dstArray[dstIndex++] = (byte)grayVal;
                                }
                            }

                            // 並列処理の場合
                            if (ProcessignType.Parallel == processignType)
                            {
                                // 並列処理設定が指定されていない場合
                                if (null == parallelOptions)
                                {
                                    var maxDegreeOfParallelism = BitmapProcessor.DefaultMaxDegreeOfParallelism;
                                    parallelOptions = new ParallelOptions()
                                    {
                                        MaxDegreeOfParallelism = (maxDegreeOfParallelism > 0) ? maxDegreeOfParallelism : -1
                                    };
                                }

                                // データ変換処理を並列実行
                                Parallel.ForEach(Enumerable.Range(0, dstLockedData.Height), parallelOptions, y =>
                                {
                                    convertor(y * srcStride, y * dstLockedData.Stride);
                                });
                            }
                            // 並列処理が有効でない場合
                            else
                            {
                                //データ変換処理を逐次実行
                                foreach (var y in Enumerable.Range(0, dstLockedData.Height))
                                {
                                    convertor(y * srcStride, y * dstLockedData.Stride);
                                }
                            }


                            Marshal.Copy(dstArray, 0, dstLockedData.Scan0, dstArray.Length);

                            // ビットマップロック解除
                            result.UnlockBits(dstLockedData);
                        }
                        break;
                        #endregion
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// グレースケール画像をRGB画像(24bit)に変換する
        /// </summary>
        /// <param name="source">元のBitmap</param>
        /// <param name="result">結果のBitmap</param>
        /// <remarks>入力画像はFormat8bppIndexedに対応</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void ConvertGrayToRgb(Bitmap source, out Bitmap? result, ProcessignType processignType, ParallelOptions parallelOptions)
        {
            // 出力値を初期化する
            result = null;

            try
            {
                switch (source.PixelFormat)
                {
                    #region Format8bppIndexed
                    case PixelFormat.Format8bppIndexed:
                        {
                            // ピクセルフォーマットを変更したBitmapを作成する
                            result = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);

                            // 元データ配列取得
                            BitmapProcessor.ConvertToArray(ArrayConversionDirection.Horizontal, source, out byte[][] srcArray);

                            // 結果ビットマップロック
                            var dstLockedData = result.LockBits(
                                                               new System.Drawing.Rectangle(System.Drawing.Point.Empty, result.Size)
                                                             , ImageLockMode.WriteOnly
                                                             , result.PixelFormat
                                                             );
                            var dstArray = new byte[dstLockedData.Stride * dstLockedData.Height];

                            // データ変換
                            var width = dstLockedData.Width;
                            var height = dstLockedData.Height;

                            // データ変換処理
                            void convertor(byte[] srcLine, int dstIndex)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    dstArray[dstIndex++] = srcLine[x];
                                    dstArray[dstIndex++] = srcLine[x];
                                    dstArray[dstIndex++] = srcLine[x];
                                }
                            }

                            // 並列処理の場合
                            if (ProcessignType.Parallel == processignType)
                            {
                                // 並列処理設定が指定されていない場合
                                if (null == parallelOptions)
                                {
                                    var maxDegreeOfParallelism = BitmapProcessor.DefaultMaxDegreeOfParallelism;
                                    parallelOptions = new ParallelOptions()
                                    {
                                        MaxDegreeOfParallelism = (maxDegreeOfParallelism > 0) ? maxDegreeOfParallelism : -1
                                    };
                                }

                                // データ変換処理を並列実行
                                Parallel.ForEach(Enumerable.Range(0, dstLockedData.Height), parallelOptions, y =>
                                {
                                    convertor(srcArray[y], y * dstLockedData.Stride);
                                });
                            }
                            // 並列処理が有効でない場合
                            else
                            {
                                //データ変換処理を逐次実行
                                foreach (var y in Enumerable.Range(0, dstLockedData.Height))
                                {
                                    convertor(srcArray[y], y * dstLockedData.Stride);
                                }
                            }


                            Marshal.Copy(dstArray, 0, dstLockedData.Scan0, dstArray.Length);

                            // ビットマップロック解除
                            result.UnlockBits(dstLockedData);
                        }
                        break;
                        #endregion
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// RGB画像をチャンネル毎のグレースケール画像に分解する
        /// </summary>
        /// <param name="source">元のBitmap</param>
        /// <param name="resultR">結果のRチャンネルBitmap</param>
        /// <param name="resultG">結果のGチャンネルBitmap</param>
        /// <param name="resultB">結果のBチャンネルBitmap</param>
        /// <param name="processignType"></param>
        /// <param name="parallelOptions"></param>
        [SupportedOSPlatform("windows7.0")]
        public static void DecomposeRgb(Bitmap source, out Bitmap resultR, out Bitmap resultG, out Bitmap resultB, ProcessignType processignType, ParallelOptions parallelOptions)
        {
            BitmapProcessor.DecomposeBgr(source, out Bitmap[] result, processignType, parallelOptions);

            resultB = result[0];
            resultG = result[1];
            resultR = result[2];
        }

        /// <summary>
        /// RGB画像をチャンネル毎のグレースケール画像に分解する
        /// </summary>
        /// <param name="source">元のBitmap</param>
        /// <param name="result">結果のBitmap</param>
        /// <remarks>Format24bppRgb対応。出力画像はB,G,Rの順で格納されます</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void DecomposeBgr(Bitmap source, out Bitmap[] result, ProcessignType processignType, ParallelOptions parallelOptions)
        {
            // 出力値を初期化する
            result = new Bitmap[3];

            try
            {
                var resultArray = new List<byte[]>();
                var resultLockedData = new List<BitmapData>();
                var resultStride = 0;

                #region 出力画像を作成する
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = new Bitmap(source.Width, source.Height, PixelFormat.Format8bppIndexed);
                    BitmapProcessor.AssignColorPaletteOfDefault(result[i]);

                    var dstLockedData = result[i].LockBits(
                                                            new System.Drawing.Rectangle(System.Drawing.Point.Empty, result[i].Size)
                                                          , ImageLockMode.WriteOnly
                                                          , result[i].PixelFormat
                                                          );
                    resultStride = dstLockedData.Stride;
                    resultLockedData.Add(dstLockedData);
                    resultArray.Add(new byte[resultStride * dstLockedData.Height]);
                }
                #endregion

                // ビットマップロック                
                var srcLockedData = source.LockBits(
                                                    new System.Drawing.Rectangle(System.Drawing.Point.Empty, source.Size)
                                                    , ImageLockMode.ReadOnly
                                                    , source.PixelFormat
                                                    );

                var srcArray = new byte[srcLockedData.Stride * srcLockedData.Height];
                Marshal.Copy(srcLockedData.Scan0, srcArray, 0, srcArray.Length);

                // データ変換
                var width = srcLockedData.Width;
                var height = srcLockedData.Height;

                var addIndex = (PixelFormat.Format24bppRgb == source.PixelFormat) ? 0 : 1;

                // データ変換処理
                void decomposer(byte[] bArray, byte[] gArray, byte[] rArray, int dstIndex, int srcIndex)
                {
                    for (int x = 0; x < width; x++)
                    {
                        bArray[dstIndex] = srcArray[srcIndex++];
                        gArray[dstIndex] = srcArray[srcIndex++];
                        rArray[dstIndex] = srcArray[srcIndex++];
                        srcIndex += addIndex;
                        dstIndex++;
                    }
                }

                // 並列処理の場合
                if (ProcessignType.Parallel == processignType)
                {
                    // 並列処理設定が指定されていない場合
                    if (null == parallelOptions)
                    {
                        var maxDegreeOfParallelism = BitmapProcessor.DefaultMaxDegreeOfParallelism;
                        parallelOptions = new ParallelOptions()
                        {
                            MaxDegreeOfParallelism = (maxDegreeOfParallelism > 0) ? maxDegreeOfParallelism : -1
                        };
                    }

                    // データ変換処理を並列実行
                    Parallel.ForEach(Enumerable.Range(0, srcLockedData.Height), parallelOptions, y =>
                    {
                        decomposer(resultArray[0], resultArray[1], resultArray[2], y * resultStride, y * srcLockedData.Stride);
                    });
                }
                // 並列処理が有効でない場合
                else
                {
                    //データ変換処理を逐次実行
                    foreach (var y in Enumerable.Range(0, srcLockedData.Height))
                    {
                        decomposer(resultArray[0], resultArray[1], resultArray[2], y * resultStride, y * srcLockedData.Stride);
                    }
                }

                // ビットマップロック解除
                source.UnlockBits(srcLockedData);

                for (int i = 0; i < result.Length; i++)
                {
                    Marshal.Copy(resultArray[i], 0, resultLockedData[i].Scan0, resultArray[i].Length);
                    result[i].UnlockBits(resultLockedData[i]);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 3つのグレースケール画像からRGB画像を作成する
        /// </summary>
        /// <param name="resultR">元のRチャンネルBitmap</param>
        /// <param name="resultG">元のGチャンネルBitmap</param>
        /// <param name="resultB">元のBチャンネルBitmap</param>
        /// <param name="result">結果のRGB画像</param>
        /// <param name="processignType"></param>
        /// <param name="parallelOptions"></param>
        [SupportedOSPlatform("windows7.0")]
        public static void ComposeRgb(Bitmap resultR, Bitmap resultG, Bitmap resultB, out Bitmap? result, ProcessignType processignType, ParallelOptions parallelOptions)
        {
            BitmapProcessor.ComposeRgb(new Bitmap[] { resultB, resultG, resultR }, out result, processignType, parallelOptions);
        }

        /// <summary>
        /// 3つのグレースケール画像からRGB画像を作成する
        /// </summary>
        /// <param name="source">長さ3のBitmap配列</param>
        /// <param name="result">RGB画像</param>
        /// <param name="processignType"></param>
        /// <param name="parallelOptions"></param>
        /// <remarks>入力画像はB,G,Rの順で格納されたものとします</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static void ComposeRgb(Bitmap[] source, out Bitmap? result, ProcessignType processignType, ParallelOptions parallelOptions)
        {
            // 出力値を初期化する
            result = null;

            try
            {
                var sourceArray = new List<byte[][]>();

                #region 入力画像をbyte配列に変換する
                for (int i = 0; i < source.Length; i++)
                {
                    BitmapProcessor.ConvertToArray(ArrayConversionDirection.Horizontal, source[i], out byte[][] array);
                    sourceArray.Add(array);
                }
                #endregion

                // Format24bppRgbビットマップを作成する
                result = new Bitmap(source[0].Width, source[0].Height, PixelFormat.Format24bppRgb);

                // ビットマップロック
                var dstLockedData = result.LockBits(
                                                    new System.Drawing.Rectangle(System.Drawing.Point.Empty, result.Size)
                                                    , ImageLockMode.WriteOnly
                                                    , result.PixelFormat
                                                    );

                var dstArray = new byte[dstLockedData.Stride * dstLockedData.Height];

                // データ変換
                var width = dstLockedData.Width;
                var height = dstLockedData.Height;

                // データ変換処理
                void composer(byte[] bArray, byte[] gArray, byte[] rArray, int dstIndex)
                {
                    for (int x = 0; x < width; x++)
                    {
                        dstArray[dstIndex++] = bArray[x];
                        dstArray[dstIndex++] = gArray[x];
                        dstArray[dstIndex++] = rArray[x];
                    }
                }

                // 並列処理の場合
                if (ProcessignType.Parallel == processignType)
                {
                    // 並列処理設定が指定されていない場合
                    if (null == parallelOptions)
                    {
                        var maxDegreeOfParallelism = BitmapProcessor.DefaultMaxDegreeOfParallelism;
                        parallelOptions = new ParallelOptions()
                        {
                            MaxDegreeOfParallelism = (maxDegreeOfParallelism > 0) ? maxDegreeOfParallelism : -1
                        };
                    }

                    // データ変換処理を並列実行
                    Parallel.ForEach(Enumerable.Range(0, dstLockedData.Height), parallelOptions, y =>
                    {
                        composer(sourceArray[0][y], sourceArray[1][y], sourceArray[2][y], y * dstLockedData.Stride);
                    });
                }
                // 並列処理が有効でない場合
                else
                {
                    //データ変換処理を逐次実行
                    foreach (var y in Enumerable.Range(0, dstLockedData.Height))
                    {
                        composer(sourceArray[0][y], sourceArray[1][y], sourceArray[2][y], y * dstLockedData.Stride);
                    }
                }

                Marshal.Copy(dstArray, 0, dstLockedData.Scan0, dstArray.Length);

                // ビットマップロック解除
                result.UnlockBits(dstLockedData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #region ファイル

        /// <summary>
        /// 画像ファイルを読み込んでBitmapを作成する
        /// </summary>
        /// <param name="fullFileName">画像ファイル名</param>
        /// <returns>作成されたビットマップ</returns>
        public static Bitmap? Read(FileInfo fileInfo)
        {
            Bitmap? bitmap = null;

            try
            {
                using var stream = new System.IO.FileStream(fileInfo.FullName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                byte[] sourceArray;

                using var bitmapOnStream = new Bitmap(stream);
                var sourceData = bitmapOnStream.LockBits(new System.Drawing.Rectangle(0, 0, bitmapOnStream.Width, bitmapOnStream.Height), ImageLockMode.ReadOnly, bitmapOnStream.PixelFormat);
                {
                    sourceArray = new byte[sourceData.Stride * sourceData.Height];

                    Marshal.Copy(sourceData.Scan0, sourceArray, 0, sourceArray.Length);
                }
                bitmapOnStream.UnlockBits(sourceData);

                bitmap = new Bitmap(bitmapOnStream.Width, bitmapOnStream.Height, bitmapOnStream.PixelFormat);

                if (0 < bitmapOnStream.Palette.Entries.Length)
                {
                    bitmap.Palette = bitmapOnStream.Palette;
                }

                var destinationData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                {
                    Marshal.Copy(sourceArray, 0, destinationData.Scan0, sourceArray.Length);
                }
                bitmap.UnlockBits(destinationData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return bitmap;
        }

        /// <summary>
        /// 書き込み
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static bool Write(FileInfo fileInfo, Bitmap? bitmap)
        {
            var isSuccess = false;

            try
            {
                if (null != bitmap)
                {
                    // 対応したフォーマットか
                    var props = typeof(System.Drawing.Imaging.ImageFormat).GetProperties(BindingFlags.Static | BindingFlags.Public);
                    var extension = System.IO.Path.GetExtension(fileInfo.Name).ToLower().Trim('.');

                    if (extension == "jpg")
                    {
                        extension = "jpeg";
                    }

                    var info = props.Where(info => info.Name.ToLower() == extension).ToList();

                    if (0 <= info.Count && (info[0].GetValue(null, null) is ImageFormat format))
                    {
                        bitmap.Save(fileInfo.FullName, format);

                        isSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return isSuccess;
        }

        #endregion

        #region カラーパレット

        /// <summary>
        /// 疑似カラー表示用のカラーパレットを設定する
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>        
        [SupportedOSPlatform("windows7.0")]
        public static void AssignColorPaletteOfPseud(Bitmap bitmap, params int[] invalidValues)
        {
            BitmapProcessor.AssignColorPaletteOfPseud(bitmap, false, invalidValues);
        }

        /// <summary>
        /// 疑似カラー表示用のカラーパレットを設定する
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        /// <param name="isInverted">反転させるかどうか</param>
        [SupportedOSPlatform("windows7.0")]
        public static void AssignColorPaletteOfPseud(Bitmap bitmap, bool isInverted, params int[] invalidValues)
        {
            var palette = BitmapProcessor.GetColorPaletteOfPseud(bitmap, isInverted, invalidValues);

            if ((null != palette) && (0 < palette.Entries.Length))
            {
                bitmap.Palette = palette;
            }
        }

        /// <summary>
        /// 線形補正表示用のカラーパレットを設定する
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        [SupportedOSPlatform("windows7.0")]
        public static void AssignColorPaletteOfLinearCorrection(Bitmap bitmap, int minimum, int maximum)
        {
            var palette = BitmapProcessor.GetColorPaletteOfLinearCorrection(bitmap, minimum, maximum);

            if ((null != palette) && (0 < palette.Entries.Length))
            {
                bitmap.Palette = palette;
            }
        }

        /// <summary>
        /// カラーパレットをリセットする
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        [SupportedOSPlatform("windows7.0")]
        public static void AssignColorPaletteOfDefault(Bitmap bitmap)
        {
            var palette = BitmapProcessor.GetColorPaletteOfDefault(bitmap);

            if ((null != palette) && (0 < palette.Entries.Length))
            {
                bitmap.Palette = palette;
            }
        }

        /// <summary>
        /// 疑似カラー表示用のカラーパレットを取得する
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        /// <returns>疑似カラー表示用のカラーパレット</returns>
        [SupportedOSPlatform("windows7.0")]
        public static ColorPalette GetColorPaletteOfPseud(Bitmap bitmap, params int[] invalidValues)
        {
            return BitmapProcessor.GetColorPaletteOfPseud(bitmap, false, invalidValues);
        }

        /// <summary>
        /// 疑似カラー表示用のカラーパレットを取得する
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        /// <param name="isInverted">反転させるかどうか</param>
        /// <param name="invalidValues">無効値</param>
        /// <returns>疑似カラー表示用のカラーパレット</returns>
        /// <remarks>無効値の色は黒になります</remarks>
        [SupportedOSPlatform("windows7.0")]
        public static ColorPalette GetColorPaletteOfPseud(Bitmap bitmap, bool isInverted, params int[] invalidValues)
        {
            var palette = bitmap.Palette;

            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    {
                        #region カラーパレット作成

                        int tone = byte.MaxValue + 1;
                        int step = tone / 4;
                        int slope = Convert.ToInt32((double)tone / (double)step);
                        int index = 0;

                        var indexLut = Enumerable.Range(0, tone).ToArray();
                        if (true == isInverted)
                        {
                            Array.Reverse(indexLut);
                        }

                        for (int i = 0; i < step; i++)
                        {
                            palette.Entries[indexLut[index++]] = Color.FromArgb(0, i * slope, byte.MaxValue);
                        }
                        for (int i = 0; i < step; i++)
                        {
                            palette.Entries[indexLut[index++]] = Color.FromArgb(0, byte.MaxValue, byte.MaxValue + 1 - (i + 1) * slope);
                        }
                        for (int i = 0; i < step; i++)
                        {
                            palette.Entries[indexLut[index++]] = Color.FromArgb(i * slope, byte.MaxValue, 0);
                        }
                        for (int i = 0; i < step; i++)
                        {
                            palette.Entries[indexLut[index++]] = Color.FromArgb(byte.MaxValue, byte.MaxValue + 1 - (i + 1) * slope, 0);
                        }

                        foreach (var blackValue in invalidValues)
                        {
                            palette.Entries[blackValue] = Color.Black;
                        }

                        #endregion
                    }
                    break;
            }

            return palette;
        }

        /// <summary>
        /// 線形補正表示用のカラーパレットを取得する
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        /// <returns>線形補正表示用のカラーパレット</returns>
        [SupportedOSPlatform("windows7.0")]
        public static ColorPalette GetColorPaletteOfLinearCorrection(Bitmap bitmap, int minimum, int maximum)
        {
            var palette = bitmap.Palette;

            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    {
                        #region カラーパレット作成

                        var scalingLut = new byte[byte.MaxValue + 1];
                        var maxValues = Enumerable.Repeat<byte>(byte.MaxValue, byte.MaxValue - maximum + 1).ToArray();
                        Array.Copy(maxValues, 0, scalingLut, maximum, maxValues.Length);

                        double coeff = (double)scalingLut.Length / (double)(maximum - minimum + 1);
                        double summation = 0;
                        for (int i = minimum; i < maximum; i++)
                        {
                            scalingLut[i] = (byte)summation;
                            summation += coeff;
                        }

                        for (int i = 0; i < palette.Entries.Length; i++)
                        {
                            var val = scalingLut[i];
                            palette.Entries[i] = Color.FromArgb(val, val, val);
                        }

                        #endregion
                    }
                    break;
            }

            return palette;
        }

        /// <summary>
        /// リセットしたカラーパレットを取得する
        /// </summary>
        /// <param name="bitmap">ビットマップ</param>
        /// <returns>疑似カラー表示用のカラーパレット</returns>
        [SupportedOSPlatform("windows7.0")]
        public static ColorPalette GetColorPaletteOfDefault(Bitmap bitmap)
        {
            var palette = bitmap.Palette;

            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format8bppIndexed:
                    {
                        for (int i = 0; i < palette.Entries.Length; i++)
                        {
                            palette.Entries[i] = Color.FromArgb(i, i, i);
                        }
                    }
                    break;
            }

            return palette;
        }

        #endregion

        #region データ変換

        /// <summary>
        /// BGRの順で格納された配列をグレースケール配列に変換する
        /// </summary>
        /// <param name="source"></param>
        /// <param name="converted"></param>
        public static void ConvertBgrToGray(ArraySegment<byte> source, out byte[] converted)
        {
            converted = new byte[source.Count / 3];

            var index = 0;
            for (int i = 0; i < converted.Length; i++)
            {
                var grayVal = BitmapProcessor.lutOfConvRgbToGrayFromB[source[index++]];
                grayVal += BitmapProcessor.lutOfConvRgbToGrayFromG[source[index++]];
                grayVal += BitmapProcessor.lutOfConvRgbToGrayFromR[source[index++]];
                converted[i] = (byte)grayVal;
            }
        }

        /// <summary>
        /// BGRの順で格納された配列をグレースケール配列に変換する
        /// </summary>
        /// <param name="source"></param>
        /// <param name="converted"></param>
        public static void ConvertBgrToGray(ArraySegment<byte> source, out float[] converted)
        {
            converted = new float[source.Count / 3];

            var index = 0;
            for (int i = 0; i < converted.Length; i++)
            {
                var grayVal = BitmapProcessor.lutOfConvRgbToGrayFromB[source[index++]];
                grayVal += BitmapProcessor.lutOfConvRgbToGrayFromG[source[index++]];
                grayVal += BitmapProcessor.lutOfConvRgbToGrayFromR[source[index++]];
                converted[i] = (float)grayVal;
            }
        }

        /// <summary>
        /// BGRAの順で格納された配列をグレースケール配列に変換する
        /// </summary>
        /// <param name="source"></param>
        /// <param name="converted"></param>
        public static void ConvertBgraToGray(ArraySegment<byte> source, out byte[] converted)
        {
            converted = new byte[source.Count / 4];

            var index = 0;
            for (int i = 0; i < converted.Length; i++)
            {
                var grayVal = BitmapProcessor.lutOfConvRgbToGrayFromB[source[index++]];
                grayVal += BitmapProcessor.lutOfConvRgbToGrayFromG[source[index++]];
                grayVal += BitmapProcessor.lutOfConvRgbToGrayFromR[source[index++]];
                converted[i] = (byte)grayVal;

                index++;
            }
        }

        /// <summary>
        /// BGRAの順で格納された配列をグレースケール配列に変換する
        /// </summary>
        /// <param name="source"></param>
        /// <param name="converted"></param>
        public static void ConvertBgraToGray(ArraySegment<byte> source, out float[] converted)
        {
            converted = new float[source.Count / 4];

            var index = 0;
            for (int i = 0; i < converted.Length; i++)
            {
                var grayVal = BitmapProcessor.lutOfConvRgbToGrayFromB[source[index++]];
                grayVal += BitmapProcessor.lutOfConvRgbToGrayFromG[source[index++]];
                grayVal += BitmapProcessor.lutOfConvRgbToGrayFromR[source[index++]];
                converted[i] = (float)grayVal;

                index++;
            }
        }

        #region 色空間

        /// <summary>
        /// RGB → HSV
        /// </summary>
        /// <param name="source"></param>
        /// <param name="hsv"></param>
        /// <param name="shiftValue"></param>
        public static void ConvertRgbToHsv(Bitmap source, out float[][] hsv, float shiftHue = 0f)
        {
            BitmapProcessor.ConvertRgbToHsv(source, out hsv, shiftHue, ProcessignType.Sequential, BitmapProcessor.NewParallelOptions());
        }

        /// <summary>
        /// RGB → HSV
        /// </summary>
        /// <param name="source"></param>
        /// <param name="hsv"></param>
        /// <param name="shiftValue"></param>
        /// <param name="processignType"></param>
        /// <param name="parallelOptions"></param>
        public static void ConvertRgbToHsv(Bitmap source, out float[][] hsv, float shiftHue, ProcessignType processignType, ParallelOptions parallelOptions)
        {
            var temp = hsv = Array.Empty<float[]>();

            try
            {
                switch (source.PixelFormat)
                {
                    case PixelFormat.Format24bppRgb:
                    case PixelFormat.Format32bppRgb:
                    case PixelFormat.Format32bppArgb:
                        {
                            var addIndex = (PixelFormat.Format24bppRgb == source.PixelFormat) ? 0 : 1;

                            // ビットマップロック                
                            var srcLockedData = source.LockBits(
                                                                new System.Drawing.Rectangle(System.Drawing.Point.Empty, source.Size)
                                                                , ImageLockMode.ReadOnly
                                                                , source.PixelFormat
                                                                );

                            var srcArray = new byte[srcLockedData.Stride * srcLockedData.Height];

                            if (null == srcArray)
                            {
                                throw new Exception("alloc memroy failed");
                            }

                            Marshal.Copy(srcLockedData.Scan0, srcArray, 0, srcArray.Length);

                            var floatArray = Array.ConvertAll(srcArray, p => p / 255f);

                            // データ変換
                            var width = srcLockedData.Width;
                            var height = srcLockedData.Height;

                            // データ変換処理
                            void converter(float[] hArray, float[] sArray, float[] vArray, int dstIndex, int srcIndex)
                            {
                                for (int x = 0; x < width; x++)
                                {
                                    var b = floatArray[srcIndex++];
                                    var g = floatArray[srcIndex++];
                                    var r = floatArray[srcIndex++];

                                    var max = Math.Max(r, Math.Max(g, b));
                                    var min = Math.Min(r, Math.Min(g, b));

                                    var s = max - min;
                                    var v = max;

                                    var h = 0f;
                                    if (max == min)
                                    {
                                    }
                                    else if (min == b)
                                    {
                                        h = 60.0f * (g - r) / s + 60.0f;
                                    }
                                    else if (min == r)
                                    {
                                        h = 60.0f * (b - g) / s + 180.0f;
                                    }
                                    else
                                    {
                                        h = 60.0f * (r - b) / s + 300.0f;
                                    }

                                    h += shiftHue;
                                    if (0f > h)
                                    {
                                        h += 360f;
                                    }
                                    else if (360f < h)
                                    {
                                        h -= 360f;
                                    }

                                    hArray[dstIndex] = h;
                                    sArray[dstIndex] = s;
                                    vArray[dstIndex] = v;

                                    dstIndex++;
                                    srcIndex += addIndex;
                                }
                            }

                            temp = new float[3][];
                            for (int i = 0; i < temp.Length; i++)
                            {
                                temp[i] = new float[height * width];
                            }

                            // 並列処理の場合
                            if (ProcessignType.Parallel == processignType)
                            {
                                // 並列処理設定が指定されていない場合
                                if (null == parallelOptions)
                                {
                                    var maxDegreeOfParallelism = BitmapProcessor.DefaultMaxDegreeOfParallelism;
                                    parallelOptions = new ParallelOptions()
                                    {
                                        MaxDegreeOfParallelism = (maxDegreeOfParallelism > 0) ? maxDegreeOfParallelism : -1
                                    };
                                }

                                // データ変換処理を並列実行
                                Parallel.ForEach(Enumerable.Range(0, srcLockedData.Height), parallelOptions, y =>
                                {
                                    converter(temp[0], temp[1], temp[2], y * width, y * srcLockedData.Stride);
                                });
                            }
                            // 並列処理が有効でない場合
                            else
                            {
                                //データ変換処理を逐次実行
                                foreach (var y in Enumerable.Range(0, srcLockedData.Height))
                                {
                                    converter(temp[0], temp[1], temp[2], y * width, y * srcLockedData.Stride);
                                }
                            }

                            // ビットマップロック解除
                            source.UnlockBits(srcLockedData);

                            hsv = temp;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// HSV→RGB(Bitmap)
        /// </summary>
        /// <param name="hsv"></param>
        /// <param name="imageSize"></param>
        /// <param name="imageStride"></param>
        /// <param name="bitmap"></param>
        public static void ConvertHsvToRgb(float[][] hsv, Size imageSize, int imageStride, out Bitmap? bitmap)
        {
            BitmapProcessor.ConvertHsvToRgb(hsv, imageSize, imageStride, out bitmap, ProcessignType.Sequential, BitmapProcessor.NewParallelOptions());
        }

        /// <summary>
        /// HSV→RGB(Bitmap)
        /// </summary>
        /// <param name="hsv"></param>
        /// <param name="imageSize"></param>
        /// <param name="bitmap"></param>
        /// <param name="processignType"></param>
        /// <param name="parallelOptions"></param>
        public static void ConvertHsvToRgb(float[][] hsv, Size imageSize, int imageStride, out Bitmap? bitmap, ProcessignType processignType, ParallelOptions parallelOptions)
        {
            var width = imageSize.Width;
            var height = imageSize.Height;

            // Bitmapを作成する
            bitmap = new Bitmap((0 < width) ? width : 0, (0 < height) ? height : 0, PixelFormat.Format24bppRgb);

            try
            {
                // ビットマップロック
                var lockedData = bitmap.LockBits(
                                                   new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size)
                                                 , ImageLockMode.WriteOnly
                                                 , bitmap.PixelFormat
                                                 );

                // 処理データ配列を確保
                var fullPixels = new byte[lockedData.Stride * lockedData.Height];

                // データ変換処理
                void converter(ArraySegment<float> hArray, ArraySegment<float> sArray, ArraySegment<float> vArray, int dstIndex)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var h = hArray[x];
                        var s = sArray[x];
                        var v = vArray[x];

                        var c = s;
                        var _h = h / 60;
                        var _x = c * (1 - System.Math.Abs(_h % 2 - 1));

                        float r, g, b;
                        r = g = b = v - c;

                        if (_h < 1)
                        {
                            r += c;
                            g += _x;
                        }
                        else if (_h < 2)
                        {
                            r += _x;
                            g += c;
                        }
                        else if (_h < 3)
                        {
                            g += c;
                            b += _x;
                        }
                        else if (_h < 4)
                        {
                            g += _x;
                            b += c;
                        }
                        else if (_h < 5)
                        {
                            r += _x;
                            b += c;
                        }
                        else if (_h < 6)
                        {
                            r += c;
                            b += _x;
                        }

                        fullPixels[dstIndex++] = (byte)(b * 255);
                        fullPixels[dstIndex++] = (byte)(g * 255);
                        fullPixels[dstIndex++] = (byte)(r * 255);
                    }
                }

                // 並列処理の場合
                if (ProcessignType.Parallel == processignType)
                {
                    // 並列処理設定が指定されていない場合
                    if (null == parallelOptions)
                    {
                        var maxDegreeOfParallelism = BitmapProcessor.DefaultMaxDegreeOfParallelism;
                        parallelOptions = new ParallelOptions()
                        {
                            MaxDegreeOfParallelism = (maxDegreeOfParallelism > 0) ? maxDegreeOfParallelism : -1
                        };
                    }

                    // データ変換処理を並列実行
                    Parallel.ForEach(Enumerable.Range(0, lockedData.Height), parallelOptions, y =>
                    {
                        var srcIndex = y * imageStride;

                        var hLine = new ArraySegment<float>(hsv[0], srcIndex, imageStride);
                        var sLine = new ArraySegment<float>(hsv[1], srcIndex, imageStride);
                        var vLine = new ArraySegment<float>(hsv[2], srcIndex, imageStride);

                        converter(hLine, sLine, vLine, y * lockedData.Stride);
                    });
                }
                // 並列処理が有効でない場合
                else
                {
                    //データ変換処理を逐次実行
                    foreach (var y in Enumerable.Range(0, lockedData.Height))
                    {
                        var srcIndex = y * imageStride;

                        var hLine = new ArraySegment<float>(hsv[0], srcIndex, imageStride);
                        var sLine = new ArraySegment<float>(hsv[1], srcIndex, imageStride);
                        var vLine = new ArraySegment<float>(hsv[2], srcIndex, imageStride);

                        converter(hLine, sLine, vLine, y * lockedData.Stride);
                    }
                }


                // 処理用配列からビットマップデータへコピー
                Marshal.Copy(fullPixels, 0, lockedData.Scan0, fullPixels.Length);

                // ビットマップロック解除
                bitmap.UnlockBits(lockedData);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        #endregion

        #endregion

        #region アフィン変換

        /// <summary>
        /// 回転(画像中心)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="radian"></param>
        /// <param name="interpolationMode"></param>
        /// <returns></returns>
        public static Bitmap? RotateDegree(Bitmap? source, double degree, InterpolationMode interpolationMode = InterpolationMode.HighQualityBilinear)
        {
            var point = PointD.New();
            if (source is not null)
            {
                point.X = source.Width / 2d;
                point.Y = source.Height / 2d;
            }

            return BitmapProcessor.RotateDegree(source, degree, point, interpolationMode);
        }

        /// <summary>
        /// 回転(任意の中心座標)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="radian"></param>
        /// <param name="point"></param>
        /// <param name="interpolationMode"></param>
        /// <returns></returns>
        public static Bitmap? RotateDegree(Bitmap? source, double degree, PointD point, InterpolationMode interpolationMode = InterpolationMode.HighQualityBilinear)
        {
            using var matrix = new Matrix();
            matrix.RotateAt((float)degree, point.ToPointF());

            return BitmapProcessor.AffineTransform(source, matrix, interpolationMode);
        }

        /// <summary>
        /// 回転(画像中心)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="radian"></param>
        /// <param name="interpolationMode"></param>
        /// <returns></returns>
        public static Bitmap? RotateRadian(Bitmap? source, double radian, InterpolationMode interpolationMode = InterpolationMode.HighQualityBilinear)
        {
            return BitmapProcessor.RotateDegree(source, Mathematics.RadianToDegree(radian), interpolationMode);
        }

        /// <summary>
        /// 回転(任意の中心座標)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="radian"></param>
        /// <param name="point"></param>
        /// <param name="interpolationMode"></param>
        /// <returns></returns>
        public static Bitmap? RotateRadian(Bitmap? source, double radian, PointD point, InterpolationMode interpolationMode = InterpolationMode.HighQualityBilinear)
        {
            return BitmapProcessor.RotateDegree(source, Mathematics.RadianToDegree(radian), point, interpolationMode);
        }

        /// <summary>
        /// アフィン変換
        /// </summary>
        /// <param name="source"></param>
        /// <param name="matrix"></param>
        /// <param name="interpolationMode"></param>
        /// <returns></returns>
        public static Bitmap? AffineTransform(Bitmap? source, Matrix matrix, InterpolationMode interpolationMode = InterpolationMode.HighQualityBilinear)
        {
            var result = (Bitmap?)null;

            try
            {
                if (source is not null)
                {
                    result = new Bitmap(source.Width, source.Height, source.PixelFormat);
                    using var g = Graphics.FromImage(result);

                    g.Transform = matrix.Clone();
                    g.InterpolationMode = interpolationMode;
                    g.DrawImageUnscaled(source, 0, 0);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return result;
        }

        #endregion

        #region 画像分割
        /// <summary>
        /// 射影変換→画像分割
        /// </summary>
        /// <param name="source"></param>
        /// <param name="w_num">分割するグリッドサイズ</param>
        /// <param name="h_num">分割するグリッドサイズ</param>
        /// <param name="threshold_value">画像を切り取るための閾値</param>
        /// <param name="threshold_area">画像を切り取るための面積</param>
        /// <param name="space">射影変換をする際の、目的画像に対する余白</param>
        /// <returns>分割後の画像と、処理途中の射影変換画像</returns>
        public static Tuple<List<Bitmap>, Bitmap>? SplitImageTransformation(Bitmap source, int w_num, int h_num, int threshold_value = 80, int threshold_area = 5000000, int space = 100)
        {
            #region 4隅の座標を求める
            Func<Mat, OpenCvSharp.Point[]?> getCalibrationPoints2f = (OpenCvSharp.Mat src) =>
            {
                Mat gray = new Mat();
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

                Mat binary = new Mat();
                Cv2.Threshold(gray, binary, threshold_value, 255, ThresholdTypes.Binary);
                if (Cv2.CountNonZero(binary) == 0)
                {
                    return null;
                }
                else if (Cv2.CountNonZero(binary) == binary.Width * binary.Height)
                {
                    return null;
                }
                gray.Dispose();

                // Closing処理によってノイズを除去
                Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new OpenCvSharp.Size(5, 5));
                Mat closedBinary = new Mat();
                Cv2.MorphologyEx(binary, closedBinary, MorphTypes.Close, kernel);
                kernel.Dispose();
                binary.Dispose();

                // 輪郭を抽出
                OpenCvSharp.Point[][] contours;
                HierarchyIndex[] hierarchy;
                Cv2.FindContours(closedBinary, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);
                closedBinary.Dispose();


                OpenCvSharp.Point[]? approx = null;
                // 面積が最大の輪郭を抽出
                for (int i = 0; i < contours.Length; i++)
                {
                    OpenCvSharp.Point[] cnt = contours[i];
                    double area = Cv2.ContourArea(cnt);
                    if (area > 5000000)
                    {
                        if (approx == null)
                        {
                            double epsilon = 0.08 * Cv2.ArcLength(cnt, true);
                            approx = Cv2.ApproxPolyDP(cnt, epsilon, true);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                if (approx == null || approx.Length != 4)
                {
                    return null;
                }
                return approx;
            };
            #endregion

            #region 射影変換
            Func<OpenCvSharp.Mat, OpenCvSharp.Point[], OpenCvSharp.Mat?> ProjectiveTransformation = (OpenCvSharp.Mat src, OpenCvSharp.Point[] pts) =>
            {
                OpenCvSharp.Point2f[] points = pts.Select(p => new OpenCvSharp.Point2f(p.X, p.Y)).ToArray();

                double cx = points.Average(p => p.X);
                double cy = points.Average(p => p.Y);

                double w = 0, h = 0;
                foreach (var point in points)
                {
                    if (point.X > cx) w += point.X;
                    else w -= point.X;

                    if (point.Y > cy) h += point.Y;
                    else h -= point.Y;
                }

                w += space;
                h += space;

                // 4隅の座標をspace分拡張
                for (int i = 0; i < points.Length; i++)
                {
                    if (points[i].X > cx) points[i] += new OpenCvSharp.Point2f(space / 2, 0);
                    else points[i] -= new OpenCvSharp.Point2f(space / 2, 0);

                    if (points[i].Y > cy) points[i] += new OpenCvSharp.Point2f(0, space / 2);
                    else points[i] -= new OpenCvSharp.Point2f(0, space / 2);
                }

                // 射影用に4点の順番を左上、左下、右下、右上に並び替え
                OpenCvSharp.Point2f[] tmp = new OpenCvSharp.Point2f[4];
                OpenCvSharp.Point2f[] perspectivePoints = new OpenCvSharp.Point2f[4];

                foreach (var point in points)
                {
                    if (point.X > cx)
                    {
                        if (point.Y > cy) tmp[2] = new OpenCvSharp.Point2f((float)w, (float)h);
                        else tmp[3] = new OpenCvSharp.Point2f((float)w, 0);
                    }
                    else
                    {
                        if (point.Y > cy) tmp[1] = new OpenCvSharp.Point2f(0, (float)h);
                        else tmp[0] = new OpenCvSharp.Point2f(0, 0);
                    }
                }

                foreach (var point in points)
                {
                    if (point.X < cx)
                    {
                        if (point.Y < cy) perspectivePoints[0] = point;
                        else perspectivePoints[1] = point;
                    }
                    else
                    {
                        if (point.Y > cy) perspectivePoints[2] = point;
                        else perspectivePoints[3] = point;
                    }
                }

                OpenCvSharp.Mat perspectiveMatrix = Cv2.GetPerspectiveTransform(perspectivePoints, tmp);

                OpenCvSharp.Mat dst = new OpenCvSharp.Mat();
                Cv2.WarpPerspective(src, dst, perspectiveMatrix, new OpenCvSharp.Size((int)w, (int)h));
                return dst;
            };
            #endregion

            #region 画像分割
            Func<OpenCvSharp.Mat, int, int, List<Mat>?> SplitImageIntoBlocks = (OpenCvSharp.Mat src, int w_num, int h_num) =>
            {
                var blocks = new List<Mat>();
                int blockWidth = src.Cols / w_num;
                int blockHeight = src.Rows / h_num;

                for (int y = 0; y < h_num; y++)
                {
                    for (int x = 0; x < w_num; x++)
                    {
                        int startX = x * blockWidth;
                        int endX = (x + 1) * blockWidth;
                        int startY = y * blockHeight;
                        int endY = (y + 1) * blockHeight;

                        // Ensure the end coordinates do not exceed the image boundaries
                        endX = Math.Min(endX, src.Cols);
                        endY = Math.Min(endY, src.Rows);

                        // Crop the block from the input image
                        Rect roi = new Rect(startX, startY, endX - startX, endY - startY);
                        Mat block = new Mat(src, roi);

                        blocks.Add(block);
                    }
                }
                return blocks.Count > 0 ? blocks : null;
            };
            #endregion

            #region 前処理実行
            OpenCvSharp.Mat in_image = OpenCvSharp.Extensions.BitmapConverter.ToMat(source);

            source.Dispose();
            OpenCvSharp.Point[]? points = getCalibrationPoints2f(in_image);
            if (points == null)
            {
                return null;
            }

            OpenCvSharp.Mat? transformed_image = ProjectiveTransformation(in_image, points);
            if (transformed_image == null)
            {
                return null;
            }
            in_image.Dispose();

            List<Mat>? blocks = SplitImageIntoBlocks(transformed_image, w_num, h_num);
            if (blocks == null)
            {
                return null;
            }

            List<Bitmap>? blockBitmaps = blocks.Select(b => OpenCvSharp.Extensions.BitmapConverter.ToBitmap(b)).ToList();
            if (blockBitmaps == null)
            {
                return null;
            }

            Bitmap transformed_image_bitmap = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(transformed_image);

            return new Tuple<List<Bitmap>, Bitmap>(blockBitmaps, transformed_image_bitmap);
            #endregion
        }
        #endregion

        #region 画像分割のみ
        public static List<Mat> SplitImage(Bitmap src_bitmap, int w_num, int h_num)
        {
            Mat src = BitmapConverter.ToMat(src_bitmap);

            int blockWidth = src.Cols / w_num;
            int blockHeight = src.Rows / h_num;

            var blocks = new List<Mat>(w_num * h_num);

            int[] endXs = Enumerable.Range(0, w_num).Select(x => Math.Min((x + 1) * blockWidth, src.Cols)).ToArray();
            int[] endYs = Enumerable.Range(0, h_num).Select(y => Math.Min((y + 1) * blockHeight, src.Rows)).ToArray();

            Parallel.For(0, h_num, y =>
            {
                for (int x = 0; x < w_num; x++)
                {
                    int startX = x * blockWidth;
                    int startY = y * blockHeight;

                    Rect roi = new Rect(startX, startY, endXs[x] - startX, endYs[y] - startY);
                    Mat block = new Mat(src, roi);

                    lock (blocks)
                    {
                        blocks.Add(block);
                    }
                }
            });

            return blocks;
        }
        #endregion
        #region 画像を切り出す
        /// <summary>
        /// 画像を切り出す
        /// </summary>
        /// <param name="image">オリジナル画像</param>
        /// <param name="targetWidth">変更したい幅</param>
        /// <param name="targetHeight">変更したい高さ</param>
        /// <param name="offSetX">幅の偏移量</param>
        /// <param name="offSetY">高さの偏移量</param>
        /// <returns>画像から必要な部分を切り出す</returns>
        public static Bitmap CropAndResizeImage(Image image, int targetWidth, int targetHeight, int offSetX, int offSetY)
        {
            int cropWidth = Math.Min(image.Width, targetWidth);
            int cropHeight = Math.Min(image.Height, targetHeight);
            int cropX = (image.Width - cropWidth) / 2 + offSetX;
            int cropY = (image.Height - cropHeight) / 2 + offSetY;
            System.Drawing.Rectangle cropArea = new(cropX, cropY, cropWidth, cropHeight);
            Bitmap destImage = new(targetWidth, targetHeight);
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(image, new System.Drawing.Rectangle(0, 0, targetWidth, targetHeight), cropArea, GraphicsUnit.Pixel);
            }
            return destImage;
        }
        #endregion

        #region テンプレートマッチング
        /// <summary>
        /// 画像を切り出す
        /// </summary>
        /// <param name="TemplateImage">テンプレート画像</param>
        /// <param name="SourceImage">SourceImage</param>
        /// <returns>場所、類似度</returns>
        public static (Rect, double) TemplateMatching(Bitmap TemplateImage, Bitmap SourceImage)
        {
            Mat temp = BitmapConverter.ToMat(TemplateImage);
            Mat src = BitmapConverter.ToMat(SourceImage);
            return TemplateMatching(temp, src);
        }
        public static (Rect, double) TemplateMatching(Mat TemplateImage, Mat SourceImage)
        {
            using var result = new Mat();
            Cv2.MatchTemplate(SourceImage, TemplateImage, result, TemplateMatchModes.CCoeffNormed);
            Cv2.MinMaxLoc(result, out _, out double maxVal, out _, out OpenCvSharp.Point maxLoc);
            Rect rect = new Rect(maxLoc.X, maxLoc.Y, TemplateImage.Width, TemplateImage.Height);
            return (rect, maxVal);
        }
        #endregion
        #endregion
    }
}