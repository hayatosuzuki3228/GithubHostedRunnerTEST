using System.Diagnostics;
using System.Reflection;

namespace Hutzper.Library.Common.Data
{
    /// <summary>
    /// CSV形式操作
    /// </summary>
    public class CsvParser
    {
        /// <summary>
        /// CSV文字列からプロパティ値を設定する
        /// </summary>
        /// <param name="convertible"></param>
        /// <param name="csvString"></param>
        public static void SetValues(ICsvConvertible convertible, string csvString)
        {
            CsvParser.SetValues(convertible, csvString.Split(","));
        }

        /// <summary>
        /// 文字列からプロパティ値を設定する
        /// </summary>
        /// <param name="convertible"></param>
        /// <param name="values"></param>
        public static void SetValues(ICsvConvertible convertible, string[] values)
        {
            try
            {
                if (values.Length == 0 || null == convertible)
                {
                    return;
                }

                foreach (var t in CsvParser.GetProperties(convertible))
                {
                    if (false == t.Item1.IsAvailable || t.Item1.ColumnIndex < 0 || t.Item1.ColumnIndex >= values.Length)
                    {
                        continue;
                    }

                    var prop = t.Item2;
                    var value = values[t.Item1.ColumnIndex];

                    try
                    {
                        if (typeof(string) == prop.PropertyType)
                        {
                            prop.SetValue(convertible, value, null);
                        }
                        else if (typeof(sbyte) == prop.PropertyType)
                        {
                            prop.SetValue(convertible, sbyte.Parse(value), null);
                        }
                        else if (typeof(byte) == prop.PropertyType)
                        {
                            prop.SetValue(convertible, byte.Parse(value), null);
                        }
                        else if (typeof(char) == prop.PropertyType)
                        {
                            prop.SetValue(convertible, char.Parse(value), null);
                        }
                        else if (typeof(short) == prop.PropertyType)
                        {
                            prop.SetValue(convertible, short.Parse(value), null);
                        }
                        else if (typeof(int) == prop.PropertyType)
                        {
                            prop.SetValue(convertible, int.Parse(value), null);
                        }
                        else if (typeof(uint) == prop.PropertyType)
                        {
                            prop.SetValue(convertible, uint.Parse(value), null);
                        }
                        else if (typeof(long) == prop.PropertyType)
                        {
                            prop.SetValue(convertible, long.Parse(value), null);
                        }
                        else if (typeof(ulong) == prop.PropertyType)
                        {
                            prop.SetValue(convertible, ulong.Parse(value), null);
                        }
                        else if (typeof(float) == prop.PropertyType)
                        {
                            prop.SetValue(convertible, float.Parse(value), null);
                        }
                        else if (typeof(double) == prop.PropertyType)
                        {
                            prop.SetValue(convertible, double.Parse(value), null);
                        }
                        else if (typeof(decimal) == prop.PropertyType)
                        {
                            prop.SetValue(convertible, decimal.Parse(value), null);
                        }
                        else if (typeof(bool) == prop.PropertyType)
                        {
                            var castValue = Convert.ToBoolean(int.Parse(value));

                            prop.SetValue(convertible, castValue, null);
                        }
                        else if (typeof(System.Drawing.Color) == prop.PropertyType)
                        {
                            var castValue = Color.Black;
                            castValue = castValue.FromHexadecimal(value);

                            prop.SetValue(convertible, castValue, null);
                        }
                        else if (prop.PropertyType.IsEnum)
                        {
                            var castValue = (Enum)Enum.Parse(typeof(Enum), value);

                            prop.SetValue(convertible, castValue, null);
                        }
                        else if (typeof(DirectoryInfo) == prop.PropertyType)
                        {
                            var castValue = new DirectoryInfo(value);

                            prop.SetValue(convertible, castValue, null);
                        }
                        else if (typeof(DateTime) == prop.PropertyType)
                        {
                            var castValue = DateTime.ParseExact(value, "yyyy/MM/dd HH:mm:ss.fff", null);

                            prop.SetValue(convertible, castValue, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// 項目値を取得する
        /// </summary>
        /// <returns></returns>
        public static string[] GetValues(ICsvConvertible convertible)
        {
            var values = Array.Empty<string>();

            try
            {
                var props = CsvParser.GetProperties(convertible);

                var maxIndex = System.Math.Max(-1, props.Max(p => p.Item1.ColumnIndex));

                values = Enumerable.Range(0, maxIndex + 1).Select(i => string.Empty).ToArray();

                foreach (var t in props)
                {
                    var prop = t.Item2;

                    var valueRaw = prop.GetValue(convertible, null);

                    if (null == valueRaw)
                    {
                        continue;
                    }

                    var valueText = valueRaw.ToString();
                    if (string.IsNullOrEmpty(valueText))
                    {
                        valueText = string.Empty;
                    }

                    if (typeof(bool) == prop.PropertyType)
                    {
                        var castValue = (bool)valueRaw;

                        valueText = Convert.ToInt32(castValue).ToString();
                    }
                    else if (typeof(System.Drawing.Color) == prop.PropertyType)
                    {
                        var castValue = (System.Drawing.Color)valueRaw;

                        valueText = castValue.ToHexadecimal();
                    }
                    else if (typeof(DirectoryInfo) == prop.PropertyType)
                    {
                        var castValue = (DirectoryInfo)valueRaw;

                        valueText = castValue.FullName;
                    }
                    else if (typeof(DateTime) == prop.PropertyType)
                    {
                        var castValue = (DateTime)valueRaw;

                        valueText = castValue.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    }

                    values[t.Item1.ColumnIndex] = valueText;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return values.ToArray();
        }

        /// <summary>
        /// 項目説明を取得する
        /// </summary>
        /// <returns></returns>
        public static string[] GetDescriptions(ICsvConvertible convertible)
        {
            return Array.ConvertAll(CsvParser.GetProperties(convertible), item => item.Item1.Description);
        }

        /// <summary>
        /// 属性値を取得する
        /// </summary>
        /// <returns></returns>
        public static Tuple<CsvColumnAttribute, PropertyInfo>[] GetProperties(ICsvConvertible convertible, bool isOnlyAvailable = true)
        {
            var attributes = new List<Tuple<CsvColumnAttribute, PropertyInfo>>();

            try
            {
                foreach (var prop in convertible.GetType().GetProperties())
                {
                    var item = prop.GetCustomAttribute<CsvColumnAttribute>();

                    if (
                        item != null
                    && item.ColumnIndex >= 0
                    && false == string.IsNullOrEmpty(item.Description)
                    && (item.IsAvailable == true || !isOnlyAvailable)
                    )
                    {
                        if (isOnlyAvailable == false)
                        {
                            attributes.Add(Tuple.Create(item, prop));
                        }
                        else
                        {
                            var isAvailable = (null != prop.GetValue(convertible, null));

                            if (typeof(string) == prop.PropertyType)
                            {
                            }
                            else if (typeof(sbyte) == prop.PropertyType)
                            {
                            }
                            else if (typeof(byte) == prop.PropertyType)
                            {
                            }
                            else if (typeof(char) == prop.PropertyType)
                            {
                            }
                            else if (typeof(short) == prop.PropertyType)
                            {
                            }
                            else if (typeof(int) == prop.PropertyType)
                            {
                            }
                            else if (typeof(uint) == prop.PropertyType)
                            {
                            }
                            else if (typeof(long) == prop.PropertyType)
                            {
                            }
                            else if (typeof(ulong) == prop.PropertyType)
                            {
                            }
                            else if (typeof(float) == prop.PropertyType)
                            {
                            }
                            else if (typeof(double) == prop.PropertyType)
                            {
                            }
                            else if (typeof(decimal) == prop.PropertyType)
                            {
                            }
                            else if (typeof(bool) == prop.PropertyType)
                            {
                            }
                            else if (typeof(System.Drawing.Color) == prop.PropertyType)
                            {
                            }
                            else if (prop.PropertyType.IsEnum)
                            {
                            }
                            else if (typeof(DirectoryInfo) == prop.PropertyType)
                            {
                            }
                            else if (typeof(DateTime) == prop.PropertyType)
                            {
                            }
                            else
                            {
                                isAvailable = false;
                            }

                            if (true == isAvailable)
                            {
                                attributes.Add(Tuple.Create(item, prop));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return attributes.ToArray();
        }
    }
}