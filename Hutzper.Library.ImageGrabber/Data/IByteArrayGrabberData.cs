namespace Hutzper.Library.ImageGrabber.Data
{
    public interface IByteArrayGrabberData : IGrabberData
    {
        /// <summary>
        /// 画像
        /// </summary>
        public byte[] Image { get; set; }

        /// <summary>
        /// ストライド
        /// </summary>
        public int Stride { get; set; }
    }
}