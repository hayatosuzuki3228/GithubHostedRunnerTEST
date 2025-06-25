using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Drawing;
using System.ComponentModel;

namespace Hutzper.Library.Common
{
    /// <summary>
    /// Enum拡張
    /// </summary>
    [Serializable]
    public static class EnumExtensions
    {
        public static string StringValueOf(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            if (fi == null)
            {
                throw new ArgumentException("not fields");
            }

            var attributes = (AliasNameAttribute[])fi.GetCustomAttributes(typeof(AliasNameAttribute), false);

            if (attributes.Length > 0)
            {
                return attributes.First().AliasName;
            }
            else
            {
                return value.ToString();
            }
        }

        public static object EnumValueOf(string value, Type enumType)
        {
            foreach (string name in Enum.GetNames(enumType))
            {
                if (EnumExtensions.StringValueOf((Enum)Enum.Parse(enumType, name)).Equals(value))
                {
                    return Enum.Parse(enumType, name);
                }
            }

            throw new ArgumentException("not exists");
        }

        public static string DescriptionOf(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            if (fi == null)
            {
                throw new ArgumentException("not fields");
            }

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                return attributes.First().Description;
            }
            else
            {
                return value.ToString();
            }
        }

        #region RoundedCorner

        /// <summary>
        /// 指定された値を含むかどうか
        /// </summary>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool Contains(this RoundedCorner value, RoundedCorner flag) => (value & flag) == flag;

        #endregion

        #region System.Drawing.Color

        public static string ToHexadecimal(this System.Drawing.Color value)
        {
            return $"0x{Convert.ToString(value.ToArgb(), 16)}";
        }

        public static System.Drawing.Color FromHexadecimal(this System.Drawing.Color value, string hexadecimal)
        {
            try
            {
                value = System.Drawing.Color.FromArgb(System.Convert.ToInt32(hexadecimal, 16));
            }
            catch
            {
                value = System.Drawing.Color.Black;
            }

            return value;
        }

        #endregion
    }
}