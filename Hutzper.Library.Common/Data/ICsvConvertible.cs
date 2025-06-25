using System.Reflection;

namespace Hutzper.Library.Common.Data
{
    /// <summary>
    /// Csv互換インタフェース
    /// </summary>
    /// <remarks>プロパティとCSVの相互変換を行います。</remarks>
    public interface ICsvConvertible
    {
        /// <summary>
        /// CSV文字列に変換する
        /// </summary>
        /// <returns></returns>
        public string ToCsvString();

        /// <summary>
        /// CSV文字列からプロパティ値を設定する
        /// </summary>
        /// <param name="csvString"></param>
        public void FromCsvString(string csvString);

        /// <summary>
        /// 項目説明を取得する
        /// </summary>
        /// <returns></returns>
        public string[] GetDescriptions();

        /// <summary>
        /// プロパティを取得する
        /// </summary>
        /// <returns></returns>
        public Tuple<CsvColumnAttribute, PropertyInfo>[] GetProperties(bool isOnlyAvailable = true);
    }

    /// <summary>
    /// CSV列項目属性
    /// </summary>
    [Serializable]
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class CsvColumnAttribute : System.Attribute
    {
        public bool IsAvailable { get; init; }

        public int ColumnIndex { get; init; }

        public string Description { get; init; }

        public CsvColumnAttribute(bool isAvailable, int columnIndex, string description)
        {
            this.ColumnIndex = columnIndex;
            this.IsAvailable = isAvailable;
            this.Description = description;
        }
    }
}