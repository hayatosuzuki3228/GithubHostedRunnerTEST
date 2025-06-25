namespace Hutzper.Library.Common.Serialization
{
    /// <summary>
	/// ISerializer実装:JsonSerializer
    /// </summary>
    [Serializable]
    public class JsonSerializer : ITextSerializer
    {
        #region ISerializer

        /// <summary>
        /// オブジェクト、または先頭 (ルート) を指定したオブジェクト グラフを、指定したストリームにシリアル化します。
        /// </summary>
        /// <param name="serializationStream"></param>
        /// <param name="graph"></param>
        public void Serialize<T>(Stream serializationStream, T graph)
        {
            ReadOnlySpan<byte> tempBytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes<T>(graph);
            serializationStream.Write(tempBytes);
            serializationStream.Flush();
        }

        /// <summary>
        /// オブジェクトのシリアライズ
        /// </summary>
        /// <param name="serializeObject">シリアライズするオブジェクト</param>
        /// <param name="alignmentPackSize">アライメントサイズ</param>
        /// <returns>バイト配列</returns>
        public ReadOnlySpan<byte> Serialize<T>(T source, uint alignmentPackSize = 0)
        {
            ReadOnlySpan<byte> buffer;

            using (var stream = new MemoryStream())
            {
                ReadOnlySpan<byte> serializeBytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes<T>(source);
                stream.Write(serializeBytes);

                byte[] alignmentedBytes;
                if (alignmentPackSize > 0)
                {
                    alignmentedBytes = new byte[stream.Length + alignmentPackSize - stream.Length % alignmentPackSize];
                }
                else
                {
                    alignmentedBytes = new byte[stream.Length];
                }

                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(alignmentedBytes, 0, (int)stream.Length);
                stream.Close();

                buffer = alignmentedBytes;
            }

            return buffer;
        }

        /// <summary>
        /// 指定したストリームをオブジェクト グラフに逆シリアル化します。
        /// </summary>
        /// <param name="serializationStream"></param>
        /// <returns></returns>
        public T? Deserialize<T>(Stream serializationStream)
        {
            T? deserializedObject = System.Text.Json.JsonSerializer.Deserialize<T>(serializationStream);

            if (deserializedObject is T t)
            {
                return t;
            }
            return default;
        }

        /// <summary>
        /// オブジェクトのデシリアライズ
        /// </summary>
        /// <typeparam name="T">取得するClassのType</typeparam>
        /// <param name="buffer">デシリアライズするバイト配列</param>
        /// <returns>デシリアライズしたオブジェクト</returns>
        public T? Deserialize<T>(ReadOnlySpan<byte> buffer)
        {
            if (buffer != null && buffer.Length > 0)
            {
                T? deserializedObject = System.Text.Json.JsonSerializer.Deserialize<T>(buffer);

                if (deserializedObject is T t)
                {
                    return t;
                }
            }
            return default;
        }

        /// <summary>
        /// オブジェクトのディープコピー
        /// </summary>
        /// <typeparam name="T">シリアライズ可能なオブジェクトの型</typeparam>
        /// <param name="source">ディープコピーするオブジェクト</param>
        /// <returns>ディープコピーしたオブジェクト</returns>
        public T? DeepCopy<T>(T src)
        {
            ReadOnlySpan<byte> b = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes<T>(src);
            return System.Text.Json.JsonSerializer.Deserialize<T>(b);
        }

        #endregion

        #region ITextSerializer

        /// <summary>
        /// 文字列へシリアライズ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ToString<T>(T value) => System.Text.Json.JsonSerializer.Serialize<T>(value);

        /// <summary>
        /// 文字列からデシリアライズ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public T? FromString<T>(string value) => System.Text.Json.JsonSerializer.Deserialize<T>(value);

        /// <summary>
        /// 文字列からデシリアライズ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<T> ToListFromString<T>(string value) => System.Text.Json.JsonSerializer.Deserialize<List<T>>(value) ?? new List<T>();

        #endregion
    }
}