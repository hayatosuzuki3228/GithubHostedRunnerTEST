namespace Hutzper.Library.Common.Serialization
{
    /// <summary>
    /// シリアル化インタフェースう
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// オブジェクト、または先頭 (ルート) を指定したオブジェクト グラフを、指定したストリームにシリアル化します。
        /// </summary>
        /// <param name="serializationStream"></param>
        /// <param name="graph"></param>
        public void Serialize<T>(Stream serializationStream, T graph);

        /// <summary>
        /// オブジェクトのシリアライズ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="alignmentPackSize"></param>
        /// <returns></returns>
        public ReadOnlySpan<byte> Serialize<T>(T source, uint alignmentPackSize = 0);

        /// <summary>
        /// 指定したストリームをオブジェクト グラフに逆シリアル化します。
        /// </summary>
        /// <param name="serializationStream"></param>
        /// <returns></returns>
        public T? Deserialize<T>(Stream serializationStream);

        /// <summary>
        /// オブジェクトのデシリアライズ
        /// </summary>
        public T? Deserialize<T>(ReadOnlySpan<byte> buffer);

        /// <summary>
        /// オブジェクトのディープコピー
        /// </summary>
        public T? DeepCopy<T>(T src);
    }
}