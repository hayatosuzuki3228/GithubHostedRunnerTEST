using System.Reflection;

namespace Hutzper.Library.Common.Data
{
    /// <summary>
    /// 記録インタフェース
    /// </summary>
    /// <remarks>データを識別するためのプロパティを提供します。</remarks>
    public interface IRecordable
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 日時
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// IRecordable実装:record
    /// </summary>
    [Serializable]
    public class RecordableBase : IRecordable, ICsvConvertible
    {
        #region IRecordable

        /// <summary>
        /// 名称
        /// </summary>
        [CsvColumn(true, 0, "名称")]
        public virtual string Name { get; set; } = string.Empty;

        /// <summary>
        /// 日時
        /// </summary>
        [CsvColumn(true, 1, "日時")]
        public virtual DateTime DateTime { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        [CsvColumn(true, 2, "説明")]
        public virtual string Description { get; set; } = string.Empty;

        #endregion

        #region ICsvConvertible

        /// <summary>
        /// CSV文字列に変換する
        /// </summary>
        /// <returns></returns>
        public virtual string ToCsvString() => string.Join(",", CsvParser.GetValues(this));

        /// <summary>
        /// CSV文字列からプロパティ値を設定する
        /// </summary>
        /// <param name="csvString"></param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void FromCsvString(string csvString) => CsvParser.SetValues(this, csvString);

        /// <summary>
        /// 項目説明を取得する
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDescriptions() => CsvParser.GetDescriptions(this);

        /// <summary>
        /// プロパティを取得する
        /// </summary>
        /// <returns></returns>
        public virtual Tuple<CsvColumnAttribute, PropertyInfo>[] GetProperties(bool isOnlyAvailable = true) => CsvParser.GetProperties(this, isOnlyAvailable);

        #endregion
    }

    /// <summary>
    /// IRecordable実装:record
    /// </summary>
    [Serializable]
    public record RecordableBaseRecord : IRecordable, ICsvConvertible
    {
        #region IRecordable

        /// <summary>
        /// 名称
        /// </summary>
        [CsvColumn(true, 0, "名称")]
        public virtual string Name { get; set; } = string.Empty;

        /// <summary>
        /// 日時
        /// </summary>
        [CsvColumn(true, 1, "日時")]
        public virtual DateTime DateTime { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        [CsvColumn(true, 2, "説明")]
        public virtual string Description { get; set; } = string.Empty;

        #endregion

        #region ICsvConvertible

        /// <summary>
        /// CSV文字列に変換する
        /// </summary>
        /// <returns></returns>
        public virtual string ToCsvString() => string.Join(",", CsvParser.GetValues(this));

        /// <summary>
        /// CSV文字列からプロパティ値を設定する
        /// </summary>
        /// <param name="csvString"></param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void FromCsvString(string csvString) => CsvParser.SetValues(this, csvString);

        /// <summary>
        /// 項目説明を取得する
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetDescriptions() => CsvParser.GetDescriptions(this);

        /// <summary>
        /// プロパティを取得する
        /// </summary>
        /// <returns></returns>
        public virtual Tuple<CsvColumnAttribute, PropertyInfo>[] GetProperties(bool isOnlyAvailable = true) => CsvParser.GetProperties(this, isOnlyAvailable);

        #endregion
    }
}