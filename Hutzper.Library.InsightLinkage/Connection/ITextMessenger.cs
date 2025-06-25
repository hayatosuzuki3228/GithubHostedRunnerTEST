namespace Hutzper.Library.InsightLinkage.Connection
{
    /// <summary>
    /// テキストメッセンジャー
    /// </summary>
    public interface ITextMessenger : IConnection
    {
        /// <summary>
        /// テキストを送信する
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool SendText(string text);
    }
}