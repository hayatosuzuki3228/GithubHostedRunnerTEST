namespace Hutzper.Library.Forms.ImageView
{
    /// <summary>
    /// 描画を更新するタイミングを外部から注入可能
    /// </summary>
    public interface IDrawingUpdateTimingInjectable
    {
        /// <summary>
        /// 注入されたタイミングを使うかどうか
        /// </summary>
        public bool UseInjectedTiming { get; set; }

        /// <summary>
        /// 更新タイミングを注入する
        /// </summary>
        public void InjectUpdateTiming();
    }
}
