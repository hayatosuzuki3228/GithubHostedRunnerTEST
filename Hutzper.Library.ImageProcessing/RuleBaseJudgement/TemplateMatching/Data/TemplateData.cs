using Hutzper.Library.Common;
using Hutzper.Library.Common.IO;
using System.Drawing;
using Point = Hutzper.Library.Common.Drawing.Point;
using Rectangle = Hutzper.Library.Common.Drawing.Rectangle;
using Size = Hutzper.Library.Common.Drawing.Size;

namespace Hutzper.Library.ImageProcessing.RuleBaseJudgement.TemplateMatching.Data;

/// <summary>
/// テンプレートデータ
/// </summary>
[Serializable]
public class TemplateData : SafelyDisposable
{
    #region プロパティ

    public Point Center
    {
        get
        {
            this.TemplateRectangle.GetCenterPoint(out Point center);

            return center;
        }
    }

    public Bitmap? Image { get; set; }

    public Rectangle TemplateRectangle { get; set; } = new();

    public List<TemplateDataItem> Items { get; set; } = new();

    public List<Rectangle> Rectangles { get; set; } = new();

    public List<TemplateMatchingResult> Results { get; set; } = new();

    public object? Tag { get; set; }

    #endregion

    #region リソースの解放

    /// <summary>
    /// リソースの解放
    /// </summary>
    protected override void DisposeExplicit()
    {
        this.Clear();
    }

    #endregion

    #region publicメソッド

    public void Clear()
    {
        this.Items.ForEach(item => item.Dispose());
        this.Items.Clear();
        this.Rectangles.Clear();
        this.Results.Clear();
        this.DisposeSafely(this.Image);
        this.Image = null;
    }

    public void CreateMatrix()
    {
        this.Results.Clear();
        foreach (var item in this.Items)
        {
            item.CreateMatrix();

            this.Results.Add(new TemplateMatchingResult());
        }
    }

    public void ResetResults()
    {
        foreach (var r in this.Results)
        {
            r.Score = 0;
            r.Template = null;
        }
    }

    /// <summary>
    /// ファイル読み込み
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <returns></returns>
    public bool Load(DirectoryInfo directoryInfo)
    {
        var result = true;

        this.Clear();

        var rawImageFile = new FileInfo(Path.Combine(directoryInfo.FullName, "image.png"));
        this.Image = BitmapProcessor.Read(rawImageFile);

        // 画像ファイル列挙
        var files = directoryInfo.GetFiles("*.png").Where(f => f.Name != $"{rawImageFile.Name}").ToArray();

        // ファイル名（角度）でソート
        Array.Sort(files, (x, y) =>
        {
            var xAngle = double.Parse(Path.GetFileNameWithoutExtension(x.Name));
            var yAngle = double.Parse(Path.GetFileNameWithoutExtension(y.Name));

            return xAngle.CompareTo(yAngle);
        });

        // テンプレート読み込み
        foreach (var file in files)
        {
            var item = new TemplateDataItem();

            if (true == item.Load(file))
            {
                this.Items.Add(item);
            }
            else
            {
                result = false;
            }
        }

        #region 過去に作成されたデータの互換性維持のための処理
        if (this.Items.FirstOrDefault() is TemplateDataItem itemTop)
        {
            if (this.Items.LastOrDefault() is TemplateDataItem itemEnd)
            {
                if (itemTop != itemEnd && Math.Abs(itemTop.AngleDegree) == Math.Abs(itemEnd.AngleDegree))
                {
                    this.Items.RemoveAt(this.Items.Count - 1);
                }
            }
        }
        #endregion

        // 矩形情報読み込み
        var csvFile = new FileInfo($"{Path.Combine(directoryInfo.FullName, "template")}.csv");

        // ファイル読み込み
        var csvReader = new CsvFileReaderWriter(csvFile.FullName);
        csvReader.ReadAllLine(out string[][] readData);

        if (readData.Length > 0)
        {
            if (readData[0].Length == 4)
            {
                var values = readData[0];

                this.TemplateRectangle.Location.X = int.Parse(values[0]);
                this.TemplateRectangle.Location.Y = int.Parse(values[1]);
                this.TemplateRectangle.Size.Width = int.Parse(values[2]);
                this.TemplateRectangle.Size.Height = int.Parse(values[3]);
            }

            foreach (var record in readData.Skip(1))
            {
                if (record.Length != 4)
                {
                    continue;
                }

                var x = int.Parse(record[0]);
                var y = int.Parse(record[1]);
                var width = int.Parse(record[2]);
                var height = int.Parse(record[3]);

                var rect = new Rectangle(new Point(x, y), new Size(width, height));
                this.Rectangles.Add(rect);
            }
        }

        return result;
    }

    /// <summary>
    /// ファイル書き込み
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public bool Save(DirectoryInfo directoryInfo)
    {
        if (true == directoryInfo.Exists)
        {
            directoryInfo.Delete(true);
        }

        if (false == directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        var rawImageFile = new FileInfo(Path.Combine(directoryInfo.FullName, "image.png"));
        var result = BitmapProcessor.Write(rawImageFile, this.Image);

        // テンプレート書き込み
        foreach (var item in this.Items)
        {
            result &= item.Save(directoryInfo);
        }

        // 情報書き込み
        var csvFile = new FileInfo($"{Path.Combine(directoryInfo.FullName, "template")}.csv");

        // ファイル書き込み
        var csvWriter = new CsvFileReaderWriter(csvFile.FullName);

        csvWriter.WriteLine($"{this.TemplateRectangle.Location.X},{this.TemplateRectangle.Location.Y},{this.TemplateRectangle.Size.Width},{this.TemplateRectangle.Size.Height}");

        foreach (var r in this.Rectangles)
        {
            var row = new List<string>();
            row.Add(r.Location.X.ToString());
            row.Add(r.Location.Y.ToString());
            row.Add(r.Size.Width.ToString());
            row.Add(r.Size.Height.ToString());

            csvWriter.AppedLine(string.Join(",", row));
        }

        return result;
    }

    #endregion
}
