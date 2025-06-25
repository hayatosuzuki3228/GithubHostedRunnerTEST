namespace Hutzper.Library.Common
{
    /// <summary>
    /// 算術
    /// </summary>
    [Serializable]
    public static class Mathematics
    {
        /// <summary>
        /// ラジアン → 度
        /// </summary>
        public static double RadianToDegree(double radian)
        {
            /// ラジアンを度に変換してreturn
            return radian * 180.0 / System.Math.PI;
        }

        /// <summary>
        /// ラジアン → 度
        /// </summary>
        public static float RadianToDegree(float radian)
        {
            /// ラジアンを度に変換してreturn
            return radian * 180.0f / (float)System.Math.PI;
        }

        /// <summary>
        /// 度 → ラジアン
        /// </summary>
        public static double DegreeToRadian(double degree)
        {
            /// 度をラジアンに変換してreturn
            return degree * System.Math.PI / 180.0;
        }

        /// <summary>
        /// 度 → ラジアン
        /// </summary>
        public static float DegreeToRadian(float degree)
        {
            /// 度をラジアンに変換してreturn
            return degree * (float)System.Math.PI / 180.0f;
        }
    }
}