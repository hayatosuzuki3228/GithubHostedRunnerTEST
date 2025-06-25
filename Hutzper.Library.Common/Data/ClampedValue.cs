namespace Hutzper.Library.Common.Data
{
    /// <summary>
    /// クランプ値
    /// </summary>
    /// <remarks>上下限の範囲を持つ値</remarks>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public record ClampedValue<T> : IClampedValue<T> where T : struct, IEquatable<T>, IComparable<T>
    {
        #region IClampedValue<T>

        /// <summary>
        /// 値
        /// </summary>
        public virtual T Value
        {
            get => this.value;
            set => this.SetValue(value);
        }

        /// <summary>
        /// 最大値
        /// </summary>
        public virtual T Maximum
        {
            get => this.maximum;
            set
            {
                if (0 != this.maximum.CompareTo(value))
                {
                    this.maximum = value;
                    if (0 > this.maximum.CompareTo(this.minimum))
                    {
                        this.maximum = this.minimum;
                    }
                    this.SetValue(this.value);
                }
            }
        }

        /// <summary>
        /// 最小値
        /// </summary>
        public virtual T Minimum
        {
            get => this.minimum;
            set
            {
                if (0 != this.minimum.CompareTo(value))
                {
                    this.minimum = value;
                    if (0 < this.minimum.CompareTo(this.maximum))
                    {
                        this.minimum = this.maximum;
                    }
                    this.SetValue(this.value);
                }
            }
        }


        /// <summary>
        /// 値を設定する
        /// </summary>
        /// <param name="value"></param>
        /// <returns>クランプされた結果値</returns>
        public virtual T SetValue(T value)
        {
            if (0 < this.Minimum.CompareTo(value))
            {
                value = this.Minimum;
            }
            else if (0 > this.Maximum.CompareTo(value))
            {
                value = this.Maximum;
            }

            var previous = this.value;

            this.value = value;

            if (0 != this.value.CompareTo(previous))
            {
                this.OnValueChanged();
            }

            return this.value;
        }

        /// <summary>
        /// 値変更
        /// </summary>
        public event Action<object>? ValueChanged;

        #endregion

        protected T value = default;
        protected T maximum = default;
        protected T minimum = default;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maximum"></param>
        /// <param name="minimum"></param>
        public ClampedValue(T value, T minimum, T maximum)
        {
            this.Maximum = maximum;
            this.Minimum = minimum;
            this.SetValue(value);
        }

        /// <summary>
        /// 値変更イベント通知
        /// </summary>
        protected virtual void OnValueChanged() => this.ValueChanged?.Invoke(this);
    }
}