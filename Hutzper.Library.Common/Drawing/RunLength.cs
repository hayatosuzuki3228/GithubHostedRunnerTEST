namespace Hutzper.Library.Common.Drawing
{
    [Serializable]
    public record RunLength
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Length { get; set; }
    }
}