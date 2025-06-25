using System.Drawing.Imaging;

namespace Hutzper.Library.Common.Serialization
{
    public static class Base64Caster
    {
        /// <summary>
        /// Base64データを画像に変換(デコード)
        /// </summary>
        /// <param name="DeCodeStr">Base64Stringデータ</param>
        /// <returns>Bitmap画像</returns>
        public static Bitmap DeCodeStrToBmp(string DeCodeStr)
        {
            Bitmap? bmp = null;

            byte[] DeCodeByte = Convert.FromBase64String(DeCodeStr);
            using (MemoryStream ms = new MemoryStream(DeCodeByte))
            {
                bmp = new Bitmap(ms);
            }

            return bmp;
        }

        /// <summary>
        /// 画像データをBase64に変換(エンコード)
        /// </summary>
        /// <param name="EncodeBmp">Bitmap画像</param>
        /// <returns>Base64Stringデータ</returns>
        public static string EnCodeBmpToStr(Bitmap EncodeBmp)
        {
            byte[]? image = null;

            using (MemoryStream ms = new MemoryStream())
            {

                Bitmap img = new Bitmap(EncodeBmp);
                img.Save(ms, ImageFormat.Bmp);
                image = ms.ToArray();
            }

            return Convert.ToBase64String(image);
        }
    }
}