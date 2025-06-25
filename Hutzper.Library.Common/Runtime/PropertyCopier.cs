namespace Hutzper.Library.Common.Runtime
{
    /// <summary>
    /// 同一プロパティをコピーします。
    /// </summary>
    public static class PropertyCopier
    {
        public static T CopyTo<T>(object src, T dest)
        {
            if (src == null || dest == null) return dest;

            var srcProperties = src.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite);
            var destProperties = dest.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite);

            var properties = srcProperties.Join(destProperties, p => new { p.Name, p.PropertyType }, p => new { p.Name, p.PropertyType }, (p1, p2) => new { p1, p2 });

            foreach (var property in properties)
            {
                property.p2.SetValue(dest, property.p1.GetValue(src));
            }

            return dest;
        }
    }
}