using Hutzper.Library.Common.Attribute;
using System.Reflection;

namespace Hutzper.Library.Common.IO
{
    /// <summary>
    /// INIファイル用インタフェース
    /// </summary>
    /// <remarks>INIファイルと対応させたいプロパティにIniKeyAttributeを付与してください</remarks>
    public interface IIniFileCompatible : IEquatable<IIniFileCompatible>
    {
        public string Section { get; init; }

        public bool Read(IniFileReaderWriter iniFile);

        public bool Write(IniFileReaderWriter iniFile);

        public List<PropertyInfo> GetIniKeyProperties();
    }

    /// <summary>
    /// IIniFileCompatible 実装
    /// </summary>
    [Serializable]
    public abstract record IniFileCompatible<T> : ILoggable, IIniFileCompatible where T : IniFileCompatible<T>
    {
        #region ILoggable

        /// <summary>
        /// インスタンスを識別する名前
        /// </summary>
        /// <remarks>NicknameとIndexプロパティから成る文字列</remarks>
        public string UniqueName
        {
            #region 取得
            get
            {
                if (0 <= this.Index)
                {
                    return string.Format("{0}{1:D2}", this.Nickname, this.Index + 1);
                }
                else
                {
                    return string.Format("{0}", this.Nickname);
                }
            }
            #endregion
        }

        /// <summary>
        /// 通称を取得、設定する
        /// </summary>
        public string Nickname
        {
            #region 取得
            get
            {
                var value = base.ToString() ?? string.Empty;

                if (false == string.IsNullOrEmpty(this.nickname))
                {
                    value = this.nickname;
                }

                return value;
            }
            #endregion

            #region 更新
            set
            {
                this.nickname = value;
            }
            #endregion
        }
        protected string nickname = String.Empty;

        /// <summary>
        /// 整数インデックス
        /// </summary>
        public int Index { get; set; } = -1;

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="logger"></param>
        public void Attach(string nickname, int index)
        {
            try
            {
                this.nickname = nickname;
                this.Index = index;
            }
            catch
            {
            }
        }

        #endregion

        #region IIniFileCompatible

        /// <summary>
        /// セクション名
        /// </summary>
        public string Section { get; init; }

        /// <summary>
        /// 読み込み
        /// </summary>
        /// <param name="iniFile"></param>
        /// <returns></returns>
        public virtual bool Read(IniFileReaderWriter iniFile)
        {
            try
            {
                foreach (var prop in this.GetType().GetProperties())
                {
                    var iniItem = prop.GetCustomAttribute<IniKeyAttribute>();
                    if (iniItem == null || iniItem.DefaultValue == null || iniItem.IsAvailable == false)
                    {
                        continue;
                    }

                    var iniKey = prop.Name;

                    if (typeof(string) == prop.PropertyType)
                    {
                        var iniDef = iniItem.DefaultValue.ToString() ?? string.Empty;

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out string returnedValue))
                        {
                            return false;
                        }

                        prop.SetValue(this, returnedValue, null);
                    }
                    else if (typeof(sbyte) == prop.PropertyType)
                    {
                        var iniDef = Convert.ToSByte(iniItem.DefaultValue);

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out sbyte returnedValue))
                        {
                            return false;
                        }

                        prop.SetValue(this, returnedValue, null);
                    }
                    else if (typeof(byte) == prop.PropertyType)
                    {
                        var iniDef = Convert.ToByte(iniItem.DefaultValue);

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out byte returnedValue))
                        {
                            return false;
                        }

                        prop.SetValue(this, returnedValue, null);
                    }
                    else if (typeof(char) == prop.PropertyType)
                    {
                        var iniDef = Convert.ToChar(iniItem.DefaultValue);

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out char returnedValue))
                        {
                            return false;
                        }

                        prop.SetValue(this, returnedValue, null);
                    }
                    else if (typeof(short) == prop.PropertyType)
                    {
                        var iniDef = Convert.ToInt16(iniItem.DefaultValue);

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out short returnedValue))
                        {
                            return false;
                        }

                        prop.SetValue(this, returnedValue, null);
                    }
                    else if (typeof(int) == prop.PropertyType)
                    {
                        var iniDef = Convert.ToInt32(iniItem.DefaultValue);

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out int returnedValue))
                        {
                            return false;
                        }

                        prop.SetValue(this, returnedValue, null);
                    }
                    else if (typeof(uint) == prop.PropertyType)
                    {
                        var iniDef = Convert.ToUInt32(iniItem.DefaultValue);

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out uint returnedValue))
                        {
                            return false;
                        }

                        prop.SetValue(this, returnedValue, null);
                    }
                    else if (typeof(long) == prop.PropertyType)
                    {
                        var iniDef = Convert.ToInt64(iniItem.DefaultValue);

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out long returnedValue))
                        {
                            return false;
                        }

                        prop.SetValue(this, returnedValue, null);
                    }
                    else if (typeof(ulong) == prop.PropertyType)
                    {
                        var iniDef = Convert.ToUInt64(iniItem.DefaultValue);

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out ulong returnedValue))
                        {
                            return false;
                        }

                        prop.SetValue(this, returnedValue, null);
                    }
                    else if (typeof(float) == prop.PropertyType)
                    {
                        var iniDef = Convert.ToSingle(iniItem.DefaultValue);

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out float returnedValue))
                        {
                            return false;
                        }

                        prop.SetValue(this, returnedValue, null);
                    }
                    else if (typeof(double) == prop.PropertyType)
                    {
                        var iniDef = Convert.ToDouble(iniItem.DefaultValue);

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out double returnedValue))
                        {
                            return false;
                        }

                        prop.SetValue(this, returnedValue, null);
                    }
                    else if (typeof(decimal) == prop.PropertyType)
                    {
                        var iniDef = Convert.ToDecimal(iniItem.DefaultValue);

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out decimal returnedValue))
                        {
                            return false;
                        }

                        prop.SetValue(this, returnedValue, null);
                    }
                    else if (typeof(bool) == prop.PropertyType)
                    {
                        var iniDef = Convert.ToBoolean(iniItem.DefaultValue);

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out bool returnedValue))
                        {
                            return false;
                        }

                        prop.SetValue(this, returnedValue, null);
                    }
                    else if (typeof(string[]) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is string[] array)
                        {
                            iniDef = string.Join(",", (string[])iniItem.DefaultValue);
                        }
                        else
                        {
                            iniDef = iniItem.DefaultValue.ToString();
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef ?? string.Empty, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<string>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = returnedValue.Split(",");
                        }

                        prop.SetValue(this, values, null);
                    }
                    else if (typeof(sbyte[]) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is sbyte[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((sbyte[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is sbyte value)
                        {
                            iniDef = iniItem.DefaultValue.ToString();
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef ?? string.Empty, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<sbyte>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => sbyte.Parse(s)));
                        }

                        prop.SetValue(this, values, null);
                    }
                    else if (typeof(byte[]) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is byte[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((byte[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is byte value)
                        {
                            iniDef = iniItem.DefaultValue.ToString();
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef ?? string.Empty, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<byte>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => byte.Parse(s)));
                        }

                        prop.SetValue(this, values, null);
                    }
                    else if (typeof(char[]) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is char[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((char[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is char value)
                        {
                            iniDef = iniItem.DefaultValue.ToString();
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef ?? string.Empty, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<char>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => char.Parse(s)));
                        }

                        prop.SetValue(this, values, null);
                    }
                    else if (typeof(short[]) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is short[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((short[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is short value)
                        {
                            iniDef = iniItem.DefaultValue.ToString();
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef ?? string.Empty, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<short>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => short.Parse(s)));
                        }

                        prop.SetValue(this, values, null);
                    }
                    else if (typeof(int[]) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is int[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((int[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is int value)
                        {
                            iniDef = iniItem.DefaultValue.ToString();
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef ?? string.Empty, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<int>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => int.Parse(s)));
                        }

                        prop.SetValue(this, values, null);
                    }
                    else if (typeof(uint[]) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is uint[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((uint[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is uint value)
                        {
                            iniDef = iniItem.DefaultValue.ToString();
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef ?? string.Empty, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<uint>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => uint.Parse(s)));
                        }

                        prop.SetValue(this, values, null);
                    }
                    else if (typeof(long[]) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is long[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((long[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is long value)
                        {
                            iniDef = iniItem.DefaultValue.ToString();
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef ?? string.Empty, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<long>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => long.Parse(s)));
                        }

                        prop.SetValue(this, values, null);
                    }
                    else if (typeof(ulong[]) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is ulong[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((ulong[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is ulong value)
                        {
                            iniDef = iniItem.DefaultValue.ToString();
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef ?? string.Empty, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<ulong>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => ulong.Parse(s)));
                        }

                        prop.SetValue(this, values, null);
                    }
                    else if (typeof(float[]) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is float[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((float[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is float value)
                        {
                            iniDef = iniItem.DefaultValue.ToString();
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef ?? string.Empty, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<float>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => float.Parse(s)));
                        }

                        prop.SetValue(this, values, null);
                    }
                    else if (typeof(double[]) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is double[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((double[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is double value)
                        {
                            iniDef = iniItem.DefaultValue.ToString();
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef ?? string.Empty, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<double>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => double.Parse(s)));
                        }

                        prop.SetValue(this, values, null);
                    }
                    else if (typeof(decimal[]) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is decimal[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((decimal[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is decimal value)
                        {
                            iniDef = iniItem.DefaultValue.ToString();
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef ?? string.Empty, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<decimal>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => decimal.Parse(s)));
                        }

                        prop.SetValue(this, values, null);
                    }
                    else if (typeof(bool[]) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is bool[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((bool[])iniItem.DefaultValue, v => Convert.ToInt32(v).ToString()));
                        }
                        else if (iniItem.DefaultValue is bool value)
                        {
                            iniDef = Convert.ToInt32(iniItem.DefaultValue).ToString();
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<bool>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => Convert.ToBoolean(s)));
                        }

                        prop.SetValue(this, values, null);
                    }
                    else if (typeof(List<string>) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is string[] array)
                        {
                            iniDef = string.Join(",", (string[])iniItem.DefaultValue);
                        }
                        else
                        {
                            iniDef = iniItem.DefaultValue.ToString();
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef ?? string.Empty, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<string>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = returnedValue.Split(",");
                        }

                        prop.SetValue(this, values, null);
                    }
                    else if (typeof(List<sbyte>) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is sbyte[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((sbyte[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is sbyte value)
                        {
                            iniDef = iniItem.DefaultValue.ToString() ?? string.Empty;
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<sbyte>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => sbyte.Parse(s)));
                        }

                        prop.SetValue(this, values.ToList(), null);
                    }
                    else if (typeof(List<byte>) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is byte[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((byte[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is byte value)
                        {
                            iniDef = iniItem.DefaultValue.ToString() ?? string.Empty;
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<byte>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => byte.Parse(s)));
                        }

                        prop.SetValue(this, values.ToList(), null);
                    }
                    else if (typeof(List<char>) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is char[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((char[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is char value)
                        {
                            iniDef = iniItem.DefaultValue.ToString() ?? string.Empty;
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<char>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => char.Parse(s)));
                        }

                        prop.SetValue(this, values.ToList(), null);
                    }
                    else if (typeof(List<short>) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is short[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((short[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is short value)
                        {
                            iniDef = iniItem.DefaultValue.ToString() ?? string.Empty;
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<short>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => short.Parse(s)));
                        }

                        prop.SetValue(this, values.ToList(), null);
                    }
                    else if (typeof(List<int[]>) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is int[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((int[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is int value)
                        {
                            iniDef = iniItem.DefaultValue.ToString() ?? string.Empty;
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<int>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => int.Parse(s)));
                        }

                        prop.SetValue(this, values.ToList(), null);
                    }
                    else if (typeof(List<uint[]>) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is uint[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((uint[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is uint value)
                        {
                            iniDef = iniItem.DefaultValue.ToString() ?? string.Empty;
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<uint>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => uint.Parse(s)));
                        }

                        prop.SetValue(this, values.ToList(), null);
                    }
                    else if (typeof(List<long[]>) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is long[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((long[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is long value)
                        {
                            iniDef = iniItem.DefaultValue.ToString() ?? string.Empty;
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<long>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => long.Parse(s)));
                        }

                        prop.SetValue(this, values.ToList(), null);
                    }
                    else if (typeof(List<ulong[]>) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is ulong[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((ulong[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is ulong value)
                        {
                            iniDef = iniItem.DefaultValue.ToString() ?? string.Empty;
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<ulong>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => ulong.Parse(s)));
                        }

                        prop.SetValue(this, values.ToList(), null);
                    }
                    else if (typeof(List<float[]>) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is float[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((float[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is float value)
                        {
                            iniDef = iniItem.DefaultValue.ToString() ?? string.Empty;
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<float>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => float.Parse(s)));
                        }

                        prop.SetValue(this, values.ToList(), null);
                    }
                    else if (typeof(List<double[]>) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is double[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((double[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is double value)
                        {
                            iniDef = iniItem.DefaultValue.ToString() ?? string.Empty;
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<double>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => double.Parse(s)));
                        }

                        prop.SetValue(this, values.ToList(), null);
                    }
                    else if (typeof(List<decimal[]>) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is decimal[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((decimal[])iniItem.DefaultValue, v => v.ToString()));
                        }
                        else if (iniItem.DefaultValue is decimal value)
                        {
                            iniDef = iniItem.DefaultValue.ToString() ?? string.Empty;
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<decimal>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => decimal.Parse(s)));
                        }

                        prop.SetValue(this, values, null);
                    }
                    else if (typeof(List<bool[]>) == prop.PropertyType)
                    {
                        var iniDef = string.Empty;

                        if (iniItem.DefaultValue is bool[] array)
                        {
                            iniDef = string.Join(",", Array.ConvertAll((bool[])iniItem.DefaultValue, v => Convert.ToInt32(v).ToString()));
                        }
                        else if (iniItem.DefaultValue is bool value)
                        {
                            iniDef = Convert.ToInt32(iniItem.DefaultValue).ToString();
                        }
                        else
                        {
                            iniDef = (string)iniItem.DefaultValue;
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out string returnedValue))
                        {
                            return false;
                        }

                        var values = Array.Empty<bool>();
                        if (false == string.IsNullOrEmpty(returnedValue))
                        {
                            values = Array.ConvertAll(returnedValue.Split(","), (s => Convert.ToBoolean(s)));
                        }

                        prop.SetValue(this, values.ToList(), null);
                    }
                    else if (typeof(System.Drawing.Color) == prop.PropertyType)
                    {
                        var iniDef = uint.Parse(iniItem.DefaultValue.ToString() ?? string.Empty);

                        if (false == iniFile.ReadValue(this.Section, iniKey, Color.FromArgb((int)iniDef), out System.Drawing.Color returnedValue))
                        {
                            return false;
                        }

                        prop.SetValue(this, returnedValue, null);
                    }
                    else if (prop.PropertyType.IsEnum)
                    {
                        var iniDef = (Enum)iniItem.DefaultValue;

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out Enum returnedValue))
                        {
                            return false;
                        }

                        prop.SetValue(this, returnedValue, null);
                    }
                    else if (typeof(DirectoryInfo) == prop.PropertyType)
                    {
                        var strDef = (string)iniItem.DefaultValue;

                        var iniDef = (DirectoryInfo?)null;

                        if (false == string.IsNullOrEmpty(strDef))
                        {
                            try
                            {
                                iniDef = new DirectoryInfo((string)iniItem.DefaultValue);
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                        }

                        if (false == iniFile.ReadValue(this.Section, iniKey, iniDef, out DirectoryInfo? returnedValue))
                        {
                            return false;
                        }

                        prop.SetValue(this, returnedValue, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return true;
        }

        /// <summary>
        /// 書き込み
        /// </summary>
        /// <param name="iniFile"></param>
        /// <returns></returns>
        public virtual bool Write(IniFileReaderWriter iniFile)
        {
            try
            {
                foreach (var prop in this.GetType().GetProperties())
                {
                    var iniItem = prop.GetCustomAttribute<IniKeyAttribute>();
                    if (iniItem == null || iniItem.DefaultValue == null || iniItem.IsAvailable == false)
                    {
                        continue;
                    }

                    var iniKey = prop.Name;
                    var iniObj = prop.GetValue(this, null);

                    if (iniObj == null)
                    {
                        iniFile.WriteValue(this.Section, iniKey, string.Empty);
                        continue;
                    }

                    if (typeof(string) == prop.PropertyType)
                    {
                        var iniValue = (string)iniObj;

                        if (false == iniFile.WriteValue(this.Section, iniKey, iniValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(sbyte) == prop.PropertyType)
                    {
                        var iniValue = (sbyte)iniObj;

                        if (false == iniFile.WriteValue(this.Section, iniKey, iniValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(byte) == prop.PropertyType)
                    {
                        var iniValue = (byte)iniObj;

                        if (false == iniFile.WriteValue(this.Section, iniKey, iniValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(char) == prop.PropertyType)
                    {
                        var iniValue = (char)iniObj;

                        if (false == iniFile.WriteValue(this.Section, iniKey, iniValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(short) == prop.PropertyType)
                    {
                        var iniValue = (short)iniObj;

                        if (false == iniFile.WriteValue(this.Section, iniKey, iniValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(int) == prop.PropertyType)
                    {
                        var iniValue = (int)iniObj;

                        if (false == iniFile.WriteValue(this.Section, iniKey, iniValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(uint) == prop.PropertyType)
                    {
                        var iniValue = (uint)iniObj;

                        if (false == iniFile.WriteValue(this.Section, iniKey, iniValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(long) == prop.PropertyType)
                    {
                        var iniValue = (long)iniObj;

                        if (false == iniFile.WriteValue(this.Section, iniKey, iniValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(ulong) == prop.PropertyType)
                    {
                        var iniValue = (ulong)iniObj;

                        if (false == iniFile.WriteValue(this.Section, iniKey, iniValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(float) == prop.PropertyType)
                    {
                        var iniValue = (float)iniObj;

                        if (false == iniFile.WriteValue(this.Section, iniKey, iniValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(double) == prop.PropertyType)
                    {
                        var iniValue = (double)iniObj;

                        if (false == iniFile.WriteValue(this.Section, iniKey, iniValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(decimal) == prop.PropertyType)
                    {
                        var iniValue = (decimal)iniObj;

                        if (false == iniFile.WriteValue(this.Section, iniKey, iniValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(bool) == prop.PropertyType)
                    {
                        var iniValue = (bool)iniObj;

                        if (false == iniFile.WriteValue(this.Section, iniKey, iniValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(string[]) == prop.PropertyType)
                    {
                        var iniValue = (string[])iniObj;

                        var strValue = string.Join(",", iniValue);

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(sbyte[]) == prop.PropertyType)
                    {
                        var iniValue = (sbyte[])iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(byte[]) == prop.PropertyType)
                    {
                        var iniValue = (byte[])iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(char[]) == prop.PropertyType)
                    {
                        var iniValue = (char[])iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(short[]) == prop.PropertyType)
                    {
                        var iniValue = (short[])iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(int[]) == prop.PropertyType)
                    {
                        var iniValue = (int[])iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(uint[]) == prop.PropertyType)
                    {
                        var iniValue = (uint[])iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(long[]) == prop.PropertyType)
                    {
                        var iniValue = (long[])iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(ulong[]) == prop.PropertyType)
                    {
                        var iniValue = (ulong[])iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(float[]) == prop.PropertyType)
                    {
                        var iniValue = (float[])iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(double[]) == prop.PropertyType)
                    {
                        var iniValue = (double[])iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(decimal[]) == prop.PropertyType)
                    {
                        var iniValue = (decimal[])iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(bool[]) == prop.PropertyType)
                    {
                        var iniValue = (bool[])iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => Convert.ToInt32(v).ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(List<string>) == prop.PropertyType)
                    {
                        var iniValue = (List<string>)iniObj;

                        var strValue = string.Join(",", iniValue);

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(List<sbyte>) == prop.PropertyType)
                    {
                        var iniValue = (List<sbyte>)iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(List<byte>) == prop.PropertyType)
                    {
                        var iniValue = (List<byte>)iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(List<char>) == prop.PropertyType)
                    {
                        var iniValue = (List<char>)iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(List<short>) == prop.PropertyType)
                    {
                        var iniValue = (List<short>)iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(List<int>) == prop.PropertyType)
                    {
                        var iniValue = (List<int>)iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(List<uint>) == prop.PropertyType)
                    {
                        var iniValue = (List<uint>)iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(List<long>) == prop.PropertyType)
                    {
                        var iniValue = (List<long>)iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(List<ulong>) == prop.PropertyType)
                    {
                        var iniValue = (List<ulong>)iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(List<float>) == prop.PropertyType)
                    {
                        var iniValue = (List<float>)iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(List<double>) == prop.PropertyType)
                    {
                        var iniValue = (List<double>)iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(List<decimal>) == prop.PropertyType)
                    {
                        var iniValue = (List<decimal>)iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => v.ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(List<bool>) == prop.PropertyType)
                    {
                        var iniValue = (List<bool>)iniObj;

                        var strValue = string.Join(",", iniValue.ToList().ConvertAll(v => Convert.ToInt32(v).ToString()));

                        if (false == iniFile.WriteValue(this.Section, iniKey, strValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(System.Drawing.Color) == prop.PropertyType)
                    {
                        var iniValue = (System.Drawing.Color)iniObj;

                        if (false == iniFile.WriteValue(this.Section, iniKey, iniValue))
                        {
                            return false;
                        }
                    }
                    else if (prop.PropertyType.IsEnum)
                    {
                        var iniValue = (Enum)iniObj;

                        if (false == iniFile.WriteValue(this.Section, iniKey, iniValue))
                        {
                            return false;
                        }
                    }
                    else if (typeof(DirectoryInfo) == prop.PropertyType)
                    {
                        var strValue = iniObj?.ToString();

                        var iniValue = (DirectoryInfo?)null;

                        if (false == string.IsNullOrEmpty(strValue))
                        {
                            try
                            {
                                iniValue = new DirectoryInfo(strValue);
                            }
                            catch (Exception ex)
                            {
                                Serilog.Log.Warning(ex, ex.Message);
                            }
                        }

                        if (false == iniFile.WriteValue(this.Section, iniKey, iniValue))
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return true;
        }

        #endregion

        #region プロパティ

        /// <summary>
        /// タグ
        /// </summary>
        [IniKey(false, (object?)null)]
        public object? Tag { get; set; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="section"></param>
        public IniFileCompatible(string section)
        {
            this.Section = section;

            this.nickname = $"{this.Section}";
        }

        #endregion

        #region IEquatable

        public virtual bool Equals(IIniFileCompatible? compalison)
        {
            if (compalison is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, compalison))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != compalison.GetType())
            {
                return false;
            }

            var prop1 = this.GetIniKeyProperties();
            var prop2 = compalison.GetIniKeyProperties();

            if (prop1.Count != prop2.Count)
            {
                return false;
            }

            foreach (var item in prop1)
            {
                var index = prop1.IndexOf(item);

                var value1 = item.GetValue(this, null);
                var value2 = prop2[index].GetValue(compalison, null);

                if (value1 is null || value2 is null)
                {
                    return false;
                }

                if (value1.GetType().IsArray)
                {
                    var array1 = (Array)value1;
                    var array2 = (Array)value2;

                    if (array1.Length != array2.Length)
                    {
                        return false;
                    }

                    foreach (var i in Enumerable.Range(0, array1.Length))
                    {
                        if (false == (array1.GetValue(i)?.ToString()?.Equals(array2.GetValue(i)?.ToString()) ?? false))
                        {
                            return false;
                        }
                    }
                }
                else if (false == (value1.ToString()?.Equals(value2.ToString()) ?? false))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            this.GetIniKeyProperties().ForEach(item => hashCode.Add(item));

            return hashCode.ToHashCode();
        }

        #endregion

        public virtual List<PropertyInfo> GetIniKeyProperties()
        {
            return this.GetType().GetProperties().Where(prop =>
            {
                var iniItem = prop.GetCustomAttribute<IniKeyAttribute>();
                if (iniItem == null || iniItem.DefaultValue == null || iniItem.IsAvailable == false)
                {
                    return false;
                }

                return true;
            }).ToList();
        }
    }
}