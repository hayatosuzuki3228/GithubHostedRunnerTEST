namespace Hutzper.Library.Onnx.Data
{
    public interface IOnnxDataOutputClassIndexedInstance : IOnnxDataOutputClassIndexed
    {
        /// <summary>
        /// クラス別検出個数
        /// </summary>
        public List<int> ClassCounts { get; }
    }
}