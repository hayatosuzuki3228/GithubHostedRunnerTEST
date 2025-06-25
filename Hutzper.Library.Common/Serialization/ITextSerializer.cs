namespace Hutzper.Library.Common.Serialization
{
    public interface ITextSerializer : ISerializer
    {
        /// <summary>
        /// 文字列へシリアライズ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ToString<T>(T value);

        /// <summary>
        /// 文字列からデシリアライズ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public T? FromString<T>(string value);

        /// <summary>
        /// 文字列からデシリアライズ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<T> ToListFromString<T>(string value);
    }
}