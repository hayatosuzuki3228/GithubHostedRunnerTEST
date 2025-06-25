namespace Hutzper.Library.Common.Threading
{
    /// <summary>
    /// スレッドインタフェース
    /// </summary>
    public interface IThread : ISafelyDisposable
    {
        /// <summary>
        /// スレッド優先順位
        /// </summary>
        public ThreadPriority Priority { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// タグ
        /// </summary>
        public object? Tag { get; set; }
    }
}