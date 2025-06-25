namespace Hutzper.Library.Common
{
    /// <summary>
    /// リング形式のカウンターです。
    /// </summary>
    /// <remarks>有限リソースの割当などに使用します。</remarks>
    [Serializable]

    public class CircularCounter
    {
        #region プロパティ

        /// <summary>
        /// 最小値
        /// </summary>
        public readonly int Minimum;

        /// <summary>
        /// 最大値
        /// </summary>
        public readonly int Maximum;

        /// <summary>
        /// ステップ方向
        /// </summary>
        public CircularCounterStepDirection StepDirection { get; set; }

        /// <summary>
        /// ラージステップ値
        /// </summary>
        public int LargeStepValue { get; set; }

        /// <summary>
        /// 同期用オブジェクト
        /// </summary>
        /// <remarks>アクセスを同期するために使用できるオブジェクト</remarks>
        public object SyncRoot { get; private set; }

        /// <summary>
        /// 汎用データオブジェクトタグ
        /// </summary>
        public object? Tag;

        /// <summary>
        /// カウンタ値
        /// </summary>
        public int Counter
        {
            get => this.counter;
            set
            {
                /// カウンタに指定値を代入する
                int input = value;

                /// カウンタが最小値未満の場合
                if (input < this.Minimum)
                {
                    //// カウンタに最小値に設定する
                    input = this.Minimum;
                }

                /// カウンタが最大値を超えている場合
                if (input > this.Maximum)
                {
                    //// カウンタに最大値を設定する
                    input = this.Maximum;
                }

                /// カウンタを更新する
                this.counter = input;
            }
        }

        #endregion

        #region フィールド

        /// <summary>
        /// 内部カウンタ
        /// </summary>
        private int counter;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="minimum">最小値</param>
        /// <param name="maximum">最大値</param>
        public CircularCounter(int minimum, int maximum) : this(minimum, maximum, minimum)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="minimum">最小値</param>
        /// <param name="maximum">最大値</param>
        /// <param name="initial">初期値</param>
        public CircularCounter(int minimum, int maximum, int initial)
        {
            /// 最小値と最大値を設定する
            this.Minimum = minimum;
            this.Maximum = maximum;

            /// 最小値が最大値より大きい場合
            if (this.Minimum > this.Maximum)
            {
                //// 最小値と最大値を入れ替える
                (this.Minimum, this.Maximum) = (this.Maximum, this.Minimum);
            }

            /// 同期用オブジェクトを生成する
            this.SyncRoot = new object();

            /// ラージステップ値を初期化する
            this.LargeStepValue = 1;

            /// 初期値を設定する
            this.counter = initial;

            /// カウンタが最小値未満の場合
            if (this.counter < this.Minimum)
            {
                //// カウンタを最小値に設定する
                this.counter = this.Minimum;
            }

            /// カウンタが最大値を超えている場合
            if (this.counter > this.Maximum)
            {
                //// カウンタを最大値に設定する
                this.counter = this.Maximum;
            }
        }

        #endregion

        #region メソッド

        /// <summary>
        /// カウンターをインクリメントします。
        /// </summary>
        /// <returns>カウンタ値</returns>
        /// <remarks>前置演算</remarks>
        public int PreIncrement()
        {
            /// カウンタをインクリメントする
            int prev = this.counter++;

            /// カウンタが最大値を超えている場合
            if (prev >= this.Maximum)
            {
                //// カウンタを最小値に設定する
                this.counter = this.Minimum;
            }

            /// カウンタを返す
            return (this.counter);
        }

        /// <summary>
        /// カウンターをデクリメントします。
        /// </summary>
        /// <returns>カウンタ値</returns>
        /// <remarks>前置演算</remarks>
        public int PreDecrement()
        {
            /// カウンタをデクリメントする
            int prev = this.counter--;

            /// カウンタが最小値未満の場合
            if (prev <= this.Minimum)
            {
                //// カウンタに最小値を設定する
                this.counter = this.Maximum;
            }

            /// カウンタを返す
            return (this.counter);
        }

        /// <summary>
        /// カウンターをインクリメントします。
        /// </summary>
        /// <returns>カウンタ値</returns>
        /// <remarks>後置演算</remarks>
        public int PostIncrement()
        {
            /// カウンタの現在値を取得してからインクリメントする
            int prev = this.counter++;

            /// カウンタが最大値を超えている場合
            if (prev >= this.Maximum)
            {
                //// カウンタに最大値を設定する
                this.counter = this.Minimum;
            }

            /// 変更前のカウンタを返す
            return (prev);
        }

        /// <summary>
        /// カウンターをデクリメントします。
        /// </summary>
        /// <returns>カウンタ値</returns>
        /// <remarks>後置演算</remarks>
        public int PostDecrement()
        {
            /// カウンタの現在値を取得してからデクリメントする
            int prev = this.counter--;

            /// カウンタが最小値未満の場合
            if (prev <= this.Minimum)
            {
                //// カウンタに最小値を設定する
                this.counter = this.Maximum;
            }

            /// 変更前のカウンタを返す
            return (prev);
        }

        /// <summary>
        /// カウンターを設定された方向に増減します。
        /// </summary>
        /// <returns>カウンタ値</returns>
        /// <remarks>前置演算</remarks>
        public int PreStep()
        {
            /// ステップ方向が加算の場合
            if (this.StepDirection == CircularCounterStepDirection.Positive)
            {
                //// カウンタをインクリメントする(前置演算)
                return (this.PreIncrement());
            }
            /// ステップ方向が減算の場合
            else
            {
                //// カウンタをデクリメントする(前置演算)
                return (this.PreDecrement());
            }
        }

        /// <summary>
        /// カウンターを設定された方向に増減します。
        /// </summary>
        /// <returns>カウンタ値</returns>
        /// <remarks>後置演算</remarks>
        public int PostStep()
        {
            /// ステップ方向が加算の場合
            if (this.StepDirection == CircularCounterStepDirection.Positive)
            {
                //// カウンタをインクリメントする(後置演算)
                return (this.PostIncrement());
            }
            /// ステップ方向が減算の場合
            else
            {
                //// カウンタをデクリメントする(後置演算)
                return (this.PostDecrement());
            }
        }

        /// <summary>
        /// カウンターを設定された方向にラージステップ設定値分増減します。
        /// </summary>
        /// <returns>カウンタ値</returns>
        /// <remarks>前置演算</remarks>
        public int LargePreStep()
        {
            var optimizedStepValue = this.LargeStepValue % (this.Maximum - this.Minimum + 1);

            /// ステップ方向が減算の場合
            if (this.StepDirection == CircularCounterStepDirection.Negative)
            {
                optimizedStepValue *= -1;
            }

            if (0 < optimizedStepValue)
            {
                var tryStepValue = this.counter + optimizedStepValue;

                if (this.Maximum < tryStepValue)
                {
                    this.counter = this.Minimum + (tryStepValue - this.Maximum - 1);
                }
                else
                {
                    this.counter = tryStepValue;
                }
            }
            else
            {
                var tryStepValue = this.counter + optimizedStepValue;

                if (this.Minimum > tryStepValue)
                {
                    this.counter = this.Maximum - (this.Minimum - tryStepValue - 1);
                }
                else
                {
                    this.counter = tryStepValue;
                }
            }

            return this.counter;
        }

        /// <summary>
        /// カウンターを設定された方向にラージステップ設定値分増減します。
        /// </summary>
        /// <returns>カウンタ値</returns>
        /// <remarks>後置演算</remarks>
        public int LargePostStep()
        {
            /// カウンタの現在値を取得する
            int prev = this.counter--;

            var optimizedStepValue = this.LargeStepValue % (this.Maximum - this.Minimum + 1);

            /// ステップ方向が減算の場合
            if (this.StepDirection == CircularCounterStepDirection.Negative)
            {
                optimizedStepValue *= -1;
            }

            if (0 < optimizedStepValue)
            {
                var tryStepValue = this.counter + optimizedStepValue;

                if (this.Maximum < tryStepValue)
                {
                    this.counter = this.Minimum + (tryStepValue - this.Maximum - 1);
                }
                else
                {
                    this.counter = tryStepValue;
                }
            }
            else
            {
                var tryStepValue = this.counter + optimizedStepValue;

                if (this.Minimum > tryStepValue)
                {
                    this.counter = this.Maximum - (this.Minimum - tryStepValue - 1);
                }
                else
                {
                    this.counter = tryStepValue;
                }
            }

            return prev;
        }

        #endregion

        #region サブクラス

        /// <summary>
        /// ステップ方向
        /// </summary>
        /// <remarks>ステップ処理が加算か減算かの設定</remarks>
        public enum CircularCounterStepDirection
        {
            Positive,   //  加算
            Negative,   //  減算
        }

        #endregion
    }
}