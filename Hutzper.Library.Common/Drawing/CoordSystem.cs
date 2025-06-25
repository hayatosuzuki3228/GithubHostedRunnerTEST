namespace Hutzper.Library.Common.Drawing
{
    /// <summary>
    /// 座標系
    /// </summary>
    [Serializable]
    public static class CoordSystem
    {
        /// <summary>
        /// 軸
        /// </summary>
        [Serializable]
        public enum Axis
        {
            X,
            Y,
        }

        /// <summary>
        /// 軸の方向
        /// </summary>
        [Serializable]
        public enum AxisOrientation
        {
            Positive,
            Negative,
        }
    }
}