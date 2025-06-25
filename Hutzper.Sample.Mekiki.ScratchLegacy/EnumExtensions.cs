using Hutzper.Library.Common;

namespace Hutzper.Sample.Mekiki.ScratchLegacy
{
    [Serializable]
    public static class EnumExtensions
    {
        #region DeviceStatusKind

        /// <summary>
        /// 指定された値を含むかどうか
        /// </summary>
        /// <returns></returns>
        public static bool Contains(this DeviceStatusKind value, DeviceStatusKind flag) => (value & flag) == flag;

        #endregion

        #region AvailableImageFormat

        public static string GetFileExtension(this AvailableImageFormat format, bool includeDot = true)
        {
            // エイリアス名を取得
            var alias = format.StringValueOf();

            // ドットを含めるかどうかを判断
            return includeDot ? "." + alias : alias;
        }

        #endregion
    }
}