using Hutzper.Library.ImageProcessing.Process.ImageCropper;
using System.Diagnostics;
using System.Drawing;

namespace Hutzper.Library.ImageProcessing.Process;

public interface IImageProcessor<TInput, TResult>
{
    public Stopwatch Stopwatch { get; }
    public TResult Execute(TInput input);
}