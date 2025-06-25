using Hutzper.Library.Common;
using System.Drawing;

namespace Hutzper.Library.ImageProcessing.Process;

public interface IImageProcessorInput : ISafelyDisposable
{
    public Bitmap Image { get; set; } // 処理対象の画像
}