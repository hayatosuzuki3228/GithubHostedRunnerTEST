using Hutzper.Library.Common;
using System.Drawing;

namespace Hutzper.Library.ImageProcessing.Process;

public interface IImageProcessorResult : ISafelyDisposable
{
    public bool IsSuccessful { get; set; } // 処理が成功したかどうか
    public Bitmap? ProcessedImage { get; set; } // 処理後の画像
}