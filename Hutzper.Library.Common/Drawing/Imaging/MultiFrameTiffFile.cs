using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Hutzper.Library.Common.Drawing.Imaging
{
    /// <summary>
    /// マルチフレームTIFFファイル
    /// </summary>
    [Serializable]
    public class MultiFrameTiffFile
    {
        #region メソッド

        /// <summary>
        /// マルチフレームTiffファイル読み込み
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="bitmaps"></param>
        /// <returns></returns>
        public bool LoadFromFile(FileInfo fileInfo, out List<Bitmap> bitmaps)
        {
            var result = true;
            bitmaps = new List<Bitmap>();

            try
            {
                if (true == fileInfo.Exists)
                {
                    using var tiffStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using var tiffImage = Image.FromStream(tiffStream);
                    using var memStream = new MemoryStream();
                    var tiffFrame = new FrameDimension(tiffImage.FrameDimensionsList[0]);

                    foreach (var i in Enumerable.Range(0, tiffImage.GetFrameCount(tiffFrame)))
                    {
                        tiffImage.SelectActiveFrame(tiffFrame, i);

                        memStream.Seek(0, SeekOrigin.Begin);
                        tiffImage.Save(memStream, ImageFormat.Bmp);

                        var bmp = this.LoadBitmapFromStream(memStream);

                        if (null != bmp)
                        {
                            bitmaps.Add(bmp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Debug.WriteLine(this, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// ストリームからBitmapを読み出す
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Bitmap? LoadBitmapFromStream(Stream stream)
        {
            var bitmap = (Bitmap?)null;

            try
            {
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
                bitmap = null;
                Debug.WriteLine(this, ex.Message);
            }

            return bitmap;
        }

        /// <summary>
        /// マルチフレームTiffファイル保存
        /// </summary>
        /// <param name="fileInfo">拡張子tifを指定してください</param>
        /// <param name="bitmaps">1つの画像ファイルにまとめるビットマップリスト</param>
        /// <param name="saveMethod">保存手法</param>
        /// <param name="compression">圧縮形式</param>
        /// <returns></returns>
        public bool SaveMultiFrameTiffFile(
                                                        FileInfo fileInfo
                                                      , List<Bitmap> bitmaps
                                                      , MultiFrameTiffFileSaveMethod saveMethod = MultiFrameTiffFileSaveMethod.UsingBufferMemory
                                                      , MultiFrameTiffFileCompression compression = MultiFrameTiffFileCompression.Lzw
                                                      )
        {
            var result = true;

            try
            {
                if (
                    (null != bitmaps)
                && (0 < bitmaps.Count)
                )
                {
                    // ファイル情報が指定されていない場合
                    if (null == fileInfo)
                    {
                        throw new Exception("FileInfo is null");
                    }

                    // 保存先ディレクリの作成
                    if (fileInfo.Directory?.Exists ?? false)
                    {
                        fileInfo.Directory.Create();
                    }

                    #region 画像ファイルの保存

                    // 圧縮フラグ:圧縮無し
                    var compressionFlag = (long)EncoderValue.CompressionNone;

                    if (MultiFrameTiffFileCompression.Lzw == compression)
                    {
                        // 圧縮フラグ:圧縮有り
                        compressionFlag = (long)EncoderValue.CompressionLZW;
                    }

                    // ファイル書き出しまでメモリに保持する手法
                    if (MultiFrameTiffFileSaveMethod.UsingBufferMemory == saveMethod)
                    {
                        #region ファイル書き出しまでメモリに保持する手法
                        using var tiffStream = new MemoryStream();

                        // 1ページ目の画像を取得
                        var tiffImage = bitmaps.First();

                        // TIFF エンコーダの取得
                        var imageEncoders = ImageCodecInfo.GetImageEncoders();
                        static bool tiffEncoderPredicate(ImageCodecInfo inImageCodecInfo)
                        {
                            return inImageCodecInfo.FormatID == ImageFormat.Tiff.Guid;
                        }
                        var tiffEncoder = Array.Find(imageEncoders, tiffEncoderPredicate);

                        if (null == tiffEncoder)
                        {
                            throw new Exception("tiff encoder is null");
                        }

                        // エンコーダのパラメータ設定
                        var tiffEncoderParameters = new EncoderParameters(2);
                        tiffEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
                        tiffEncoderParameters.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, compressionFlag);

                        // メモリストリームへの書き出し
                        tiffImage.Save(tiffStream, tiffEncoder, tiffEncoderParameters);

                        // 2ページ以降の保存に使用するエンコーダのパラメータ
                        var appendEncoderParameters = new EncoderParameters(2);
                        appendEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
                        appendEncoderParameters.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, compressionFlag);

                        foreach (var i in Enumerable.Range(1, bitmaps.Count - 1))
                        {
                            // 追加画像の書き出し
                            tiffImage.SaveAdd(bitmaps[i], appendEncoderParameters);
                        }

                        // メモリストリームの内容をファイルに保存する。
                        File.WriteAllBytes(fileInfo.FullName, tiffStream.ToArray());
                        #endregion
                    }
                    // 都度ファイルに書き出す手法
                    else
                    {
                        #region 都度ファイルに書き出す手法
                        {
                            // 1ページ目の画像を取得
                            var tiffImage = bitmaps.First();

                            // TIFF エンコーダの取得
                            var imageEncoders = ImageCodecInfo.GetImageEncoders();

                            static bool tiffEncoderPredicate(ImageCodecInfo inImageCodecInfo)
                            {
                                return inImageCodecInfo.FormatID == ImageFormat.Tiff.Guid;
                            }
                            var tiffEncoder = Array.Find(imageEncoders, tiffEncoderPredicate);

                            if (null == tiffEncoder)
                            {
                                throw new Exception("tiff encoder is null");
                            }

                            // エンコーダのパラメータ設定
                            var tiffEncoderParameters = new EncoderParameters(2);
                            tiffEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
                            tiffEncoderParameters.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, compressionFlag);

                            // ファイルへの書き出し
                            tiffImage.Save(fileInfo.FullName, tiffEncoder, tiffEncoderParameters);

                            // 2ページ以降の保存に使用するエンコーダのパラメータ
                            var appendEncoderParameters = new EncoderParameters(2);
                            appendEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
                            appendEncoderParameters.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, compressionFlag);

                            foreach (var i in Enumerable.Range(1, bitmaps.Count - 1))
                            {
                                // 追加画像のファイルへの書き出し
                                tiffImage.SaveAdd(bitmaps[i], appendEncoderParameters);
                            }

                            var finishParams = new EncoderParameters(1);
                            finishParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.Flush);

                            // 画像ファイルクローズ
                            tiffImage.SaveAdd(finishParams);
                        }
                        #endregion
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                result = false;
                Debug.WriteLine(this, ex.Message);
            }

            return result;
        }

        /// <summary>
        /// マルチフレームTiffファイル保存
        /// </summary>
        /// <param name="fileInfo">拡張子tifを指定してください</param>
        /// <param name="bitmaps">1つの画像ファイルにまとめるビットマップファイルリスト</param>
        /// <param name="compression">圧縮形式</param>
        /// <returns></returns>
        public bool SaveMultiFrameTiffFile(
                                                        FileInfo fileInfo
                                                      , List<FileInfo> bitmapFiles
                                                      , MultiFrameTiffFileCompression compression = MultiFrameTiffFileCompression.Lzw
                                                      )
        {
            var result = true;

            try
            {
                if (
                    (null != bitmapFiles)
                && (0 < bitmapFiles.Count)
                )
                {
                    // ファイル情報が指定されていない場合
                    if (null == fileInfo)
                    {
                        throw new Exception("FileInfo is null");
                    }

                    // 保存先ディレクリの作成
                    if (fileInfo.Directory?.Exists ?? false)
                    {
                        fileInfo.Directory.Create();
                    }

                    #region 画像ファイルの保存

                    // 圧縮フラグ:圧縮無し
                    var compressionFlag = (long)EncoderValue.CompressionNone;

                    if (MultiFrameTiffFileCompression.Lzw == compression)
                    {
                        // 圧縮フラグ:圧縮有り
                        compressionFlag = (long)EncoderValue.CompressionLZW;
                    }

                    #region 都度ファイルに書き出す手法
                    {
                        var tiffImage = (Bitmap?)null;

                        // 1ページ目の画像を取得
                        using (var bmp = Renderer.LoadBitmapFromFile(bitmapFiles.First().FullName))
                        {
                            tiffImage = (Bitmap)bmp.Clone();
                        }

                        // TIFF エンコーダの取得
                        var imageEncoders = ImageCodecInfo.GetImageEncoders();
                        static bool tiffEncoderPredicate(ImageCodecInfo inImageCodecInfo)
                        {
                            return inImageCodecInfo.FormatID == ImageFormat.Tiff.Guid;
                        }
                        var tiffEncoder = Array.Find(imageEncoders, tiffEncoderPredicate);

                        if (null == tiffEncoder)
                        {
                            throw new Exception("tiff encoder is null");
                        }

                        // エンコーダのパラメータ設定
                        var tiffEncoderParameters = new EncoderParameters(2);
                        tiffEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
                        tiffEncoderParameters.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, compressionFlag);

                        // ファイルへの書き出し
                        tiffImage.Save(fileInfo.FullName, tiffEncoder, tiffEncoderParameters);

                        // 2ページ以降の保存に使用するエンコーダのパラメータ
                        var appendEncoderParameters = new EncoderParameters(2);
                        appendEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
                        appendEncoderParameters.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, compressionFlag);

                        foreach (var i in Enumerable.Range(1, bitmapFiles.Count - 1))
                        {
                            using var bmp = Renderer.LoadBitmapFromFile(bitmapFiles[i].FullName);

                            // 追加画像のファイルへの書き出し
                            tiffImage.SaveAdd(bmp, appendEncoderParameters);
                        }

                        var finishParams = new EncoderParameters(1);
                        finishParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.Flush);

                        // 画像ファイルクローズ
                        tiffImage.SaveAdd(finishParams);

                        tiffImage.Dispose();
                    }
                    #endregion

                    #endregion
                }
            }
            catch (Exception ex)
            {
                result = false;
                Debug.WriteLine(this, ex.Message);
            }

            return result;
        }


        #endregion
    }

    /// <summary>
    /// マルチフレームTiffファイル保存手法
    /// </summary>
    [Serializable]
    public enum MultiFrameTiffFileSaveMethod
    {
        UsingBufferMemory,
        WithoutUsingBufferMemory,
    }

    /// <summary>
    /// マルチフレームTiffファイル圧縮
    /// </summary>
    [Serializable]
    public enum MultiFrameTiffFileCompression
    {
        None,
        Lzw,
    }
}