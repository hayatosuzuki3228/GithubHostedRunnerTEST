using Hutzper.Library.Common;
using OpenCvSharp;
using System.Drawing;

namespace Hutzper.Library.ImageProcessing.RuleBaseJudgement.TemplateMatching.Data;

[Serializable]
public class TemplateDataItem : SafelyDisposable
{
    #region プロパティ

    public Bitmap? Image { get; set; }

    /// <summary>
    /// テンプレート画像
    /// </summary>
    public Mat? Matrix { get; set; }

    /// <summary>
    /// テンプレート角度
    /// </summary>
    public double AngleDegree { get; set; }

    #endregion

    #region リソースの解放

    /// <summary>
    /// リソースの解放
    /// </summary>
    protected override void DisposeExplicit()
    {
        this.DisposeSafely(this.Image);
        this.DisposeSafely(this.Matrix);
    }

    #endregion

    #region publicメソッド

    public void CreateMatrix()
    {
        this.DisposeSafely(this.Matrix);
        this.Matrix = null;

        if (this.Image is not null)
        {
            this.Matrix = OpenCvSharp.Extensions.BitmapConverter.ToMat(this.Image);
        }
    }

    /// <summary>
    /// テンプレート生成
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    public bool CreateMatrixFrom(Bitmap image, double angleDegree)
    {
        this.DisposeSafely(this.Image);
        this.DisposeSafely(this.Matrix);
        this.Image = null;
        this.Matrix = null;

        this.Image = (Bitmap)image.Clone();
        this.Matrix = OpenCvSharp.Extensions.BitmapConverter.ToMat(this.Image);
        this.AngleDegree = angleDegree;

        return this.Matrix is not null;
    }

    /// <summary>
    /// ファイル読み込み
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <returns></returns>
    public bool Load(FileInfo fileInfo)
    {
        var result = false;

        this.DisposeSafely(this.Image);
        this.DisposeSafely(this.Matrix);
        this.Image = null;
        this.Matrix = null;

        if (BitmapProcessor.Read(fileInfo) is Bitmap bitmap)
        {
            var angleText = Path.GetFileNameWithoutExtension(fileInfo.FullName);

            var angleValue = double.Parse(angleText);

            result = this.CreateMatrixFrom(bitmap, angleValue);
            this.DisposeSafely(bitmap);
        }

        return result;
    }

    /// <summary>
    /// ファイル書き込み
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public bool Save(DirectoryInfo directory)
    {
        var result = false;

        if (this.Image is not null)
        {
            if (false == directory.Exists)
            {
                directory.Create();
            }

            var fileInfo = new FileInfo(Path.Combine(directory.FullName, $"{this.AngleDegree:F1}.png"));

            result = BitmapProcessor.Write(fileInfo, this.Image);
        }

        return result;
    }

    #endregion
}
