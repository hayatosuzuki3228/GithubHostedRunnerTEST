namespace Hutzper.Library.Common.Attribute
{
    /// <summary>
    /// INIファイル項目属性
    /// </summary>
    [Serializable]
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class IniKeyAttribute : System.Attribute
    {
        public bool IsAvailable { get; init; }

        public object? DefaultValue { get; init; }
        public IniKeyAttribute(bool isAvailable, string values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, sbyte values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, byte values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, char values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, short values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, int values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, uint values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, long values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, ulong values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, float values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, double values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, decimal values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, bool values) : this(isAvailable, (object?)values) { }


        public IniKeyAttribute(bool isAvailable, params string[] values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, params sbyte[] values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, params byte[] values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, params char[] values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, params short[] values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, params int[] values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, params uint[] values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, params long[] values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, params ulong[] values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, params float[] values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, params double[] values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, params decimal[] values) : this(isAvailable, (object?)values) { }
        public IniKeyAttribute(bool isAvailable, params bool[] values) : this(isAvailable, (object?)values) { }

        public IniKeyAttribute(bool isAvailable, object? defautlValue)
        {
            this.DefaultValue = defautlValue;
            this.IsAvailable = isAvailable;
        }
    }
}