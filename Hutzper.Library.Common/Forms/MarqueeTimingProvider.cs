namespace Hutzper.Library.Common.Forms
{
    public interface IMarqueeText
    {
        public bool MarqueeTextEnabled { get; }

        public void Attach(MarqueeTimingProvider customizer);

        public void Detach(MarqueeTimingProvider customizer);

        public void Invalidate();
    }

    /// <summary>
    /// Marquee表示用のタイミングを提供します
    /// </summary>
    public class MarqueeTimingProvider
    {
        #region プロパティ

        public bool MarqueeEnabled { get; set; } = true;

        public int MarqueeAnimationSpeedMs
        {
            get => this.marqueeAnimationSpeedMs;

            set
            {
                this.marqueeAnimationSpeedMs = value > 0 ? value : value * -1;

                this.intervalIimer.Change(0, this.marqueeAnimationSpeedMs);
            }
        }

        #endregion

        #region フィールド

        private readonly List<IMarqueeText> items = new();

        private readonly System.Threading.Timer intervalIimer;
        private int marqueeAnimationSpeedMs = 250;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MarqueeTimingProvider()
        {
            this.intervalIimer = new System.Threading.Timer((o) =>
            {
                #region 描画イベントを発生させる
                try
                {
                    if (true == this.MarqueeEnabled)
                    {
                        if (o is List<IMarqueeText> targets)
                        {
                            var items = Array.Empty<IMarqueeText>();
                            lock (targets)
                            {
                                items = targets.ToArray();
                            }

                            foreach (var item in items)
                            {
                                item.Invalidate();
                            }
                        }
                    }
                }
                catch
                {
                }
                #endregion

            }, this.items, this.marqueeAnimationSpeedMs, this.marqueeAnimationSpeedMs);
        }

        #endregion

        #region メソッド

        /// <summary>
        /// カスタム対象のラベルを追加する
        /// </summary>
        /// <param name="label">管理対象のラベル</param>
        /// <param name="isMarqueeText">Marqueeスタイルにするか</param>
        /// <param name="mouseOverEmphasize">マウスオーバー時に強調表示するか</param>
        public void Attach(IMarqueeText label)
        {
            lock (this.items)
            {
                if (false == this.items.Contains(label))
                {
                    this.items.Add(label);
                }
            }
        }

        /// <summary>
        /// カスタム対象から除外する
        /// </summary>
        /// <param name="label"></param>
        public void Detach(IMarqueeText label)
        {
            lock (this.items)
            {
                if (true == this.items.Contains(label))
                {
                    this.items.Remove(label);
                }
            }
        }

        #endregion
    }
}