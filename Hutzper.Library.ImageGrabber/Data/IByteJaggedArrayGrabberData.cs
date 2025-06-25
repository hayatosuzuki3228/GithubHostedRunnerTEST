namespace Hutzper.Library.ImageGrabber.Data
{
    public interface IByteJaggedArrayGrabberData : IGrabberData
    {
        /// <summary>
        /// 画像
        /// </summary>
        public byte[][] Image { get; set; }

        /// <summary>
        /// 画像の連結
        /// </summary>
        /// <param name="data"></param>
        public void ConcatImage(params IByteJaggedArrayGrabberData[] data);

        /// <summary>
        /// 配列化
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray();
    }
}