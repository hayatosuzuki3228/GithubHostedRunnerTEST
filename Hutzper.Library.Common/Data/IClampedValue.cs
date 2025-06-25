namespace Hutzper.Library.Common.Data
{
    public interface IClampedValue<T> where T : struct, IEquatable<T>, IComparable<T>
    {
        /// <summary>
        /// 値
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// 最大値
        /// </summary>
        public T Maximum { get; set; }

        /// <summary>
        /// 最小値
        /// </summary>
        public T Minimum { get; set; }

        /// <summary>
        /// 値を設定する
        /// </summary>
        /// <param name="value"></param>
        /// <returns>クランプされた結果値</returns>
        public T SetValue(T value);

        /// <summary>
        /// 値変更
        /// </summary>
        public event Action<object>? ValueChanged;
    }
}