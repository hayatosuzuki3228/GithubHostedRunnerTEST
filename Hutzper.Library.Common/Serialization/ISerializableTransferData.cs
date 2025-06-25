namespace Hutzper.Library.Common.Serialization
{
    /// <summary>
    /// シリアル転送ヘッダ
    /// </summary>
    public class SerializableTransferHeader
    {
        public int Command { get; init; }

        public int DataBytes { get; init; }

        public SerializableTransferHeader(int command, int dataBytes)
        {
            this.Command = command;
            this.DataBytes = dataBytes;
        }

        public SerializableTransferHeader(SerializableTransferHeader? source)
        {
            this.Command = source?.Command ?? 0;
            this.DataBytes = source?.DataBytes ?? 0;
        }

        public SerializableTransferHeader(ReadOnlySpan<byte> source)
        {
            this.Command = BitConverter.ToInt32(source.Slice(0 * sizeof(int), sizeof(int)));
            this.DataBytes = BitConverter.ToInt32(source.Slice(1 * sizeof(int), sizeof(int)));
        }

        public byte[] ToArray()
        {
            var byteArray = new byte[0];

            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(BitConverter.GetBytes(this.Command));
                memoryStream.Write(BitConverter.GetBytes(this.DataBytes));

                byteArray = memoryStream.ToArray();
            }

            return byteArray;
        }
    }

    /// <summary>
    /// シリアル転送データインタフェース
    /// </summary>
    public interface ISerializableTransferData
    {
        /// <summary>
        /// ヘッダー
        /// </summary>
        public SerializableTransferHeader Header { get; set; }

        /// <summary>
        /// データ
        /// </summary>
        public byte[] SerializedBytes { get; set; }

        /// <summary>
        /// タグ
        /// </summary>
        /// <remarks>シリアル化には含まれません。</remarks>
        public object? Tag { get; set; }
    }

    /// <summary>
    /// ISerializableTransferData実装
    /// </summary>
    [Serializable]
    public class SerializableTransferData : ISerializableTransferData
    {
        #region ISerializableTransferData

        /// <summary>
        /// ヘッダー
        /// </summary>
        public SerializableTransferHeader Header { get; set; }

        /// <summary>
        /// データ
        /// </summary>
        public byte[] SerializedBytes { get; set; }

        /// <summary>
        /// タグ
        /// </summary>
        /// <remarks>シリアル化には含まれません。</remarks>
        public object? Tag { get => this.tag; set => this.tag = value; }

        #endregion

        /// <summary>
        /// タグ
        /// </summary>
#pragma warning disable CS0169
        [NonSerialized]
        private object? tag;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SerializableTransferData() : this(0, Array.Empty<byte>()) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="command"></param>
        /// <param name="body"></param>
        /// <param name="tag"></param>
        public SerializableTransferData(int command, byte[] body, object? tag = null)
        {
            this.SerializedBytes = body;
            this.Tag = tag;

            this.Header = new SerializableTransferHeader(command, this.SerializedBytes.Length);
        }

        public SerializableTransferData(SerializableTransferHeader? header, byte[] body, object? tag = null)
        {
            this.Header = new SerializableTransferHeader(header);

            this.SerializedBytes = body;
            this.Tag = tag;
        }
    }
}