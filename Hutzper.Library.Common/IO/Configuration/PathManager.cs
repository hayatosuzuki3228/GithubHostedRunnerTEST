using System.Diagnostics;
using System.Reflection;

namespace Hutzper.Library.Common.IO.Configuration
{
    /// <summary>
    /// IPathManager実装
    /// </summary>
    [Serializable]
    public record PathManager : IniFileCompatible<PathManager>, IPathManager
    {
        #region IPathManager

        /// <summary>
        /// ルートパス
        /// </summary>
        public virtual DirectoryInfo Root => new("..\\");

        /// <summary>
        /// 設定
        /// </summary>
        public DirectoryInfo Config => this[nameof(DirectoryNames.Config)];

        /// <summary>
        /// データ
        /// </summary>
        public DirectoryInfo Data => this[nameof(DirectoryNames.Data)];

        /// <summary>
        /// レシピ
        /// </summary>
        public DirectoryInfo Recipe => this[nameof(DirectoryNames.Recipe)];

        /// <summary>
        /// ログ
        /// </summary>
        public DirectoryInfo Log => this[nameof(DirectoryNames.Log)];

        /// <summary>
        /// テンポラリ
        /// </summary>
        public DirectoryInfo Temp => this[nameof(DirectoryNames.Temp)];

        /// <summary>
        /// インデクサ
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual DirectoryInfo this[string name]
        {
            get
            {
                var info = new System.IO.DirectoryInfo("..\\");

                if (true == this.dictionary.ContainsKey(name.ToLower()))
                {
                    info = new DirectoryInfo(this.dictionary[name.ToLower()]);
                }

                return info;
            }

            set
            {
                if (true == this.dictionary.ContainsKey(name.ToLower()) && null != value)
                {
                    this.dictionary[name.ToLower()] = value.FullName;
                }
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="values"></param>
        public virtual void Initialize(params KeyValuePair<string, string>[] additionalPath)
        {
            try
            {
                var valueList = new List<KeyValuePair<string, string>>();

                foreach (var f in typeof(DirectoryNames).GetFields(BindingFlags.Static | BindingFlags.Public).Where(x => x.FieldType == typeof(string)))
                {
                    var path = System.IO.Path.Combine(this.Root.FullName, f.GetValue(typeof(DirectoryNames)) as string ?? string.Empty);

                    valueList.Add(new KeyValuePair<string, string>(f.Name, path));
                }

                if (null != additionalPath)
                    valueList.AddRange(additionalPath);

                this.initialize(valueList.ToArray());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
        }

        /// <summary>
        /// ディレクトリ情報を取得する
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, DirectoryInfo> GetDirectoryInfo()
        {
            var clone = new Dictionary<string, DirectoryInfo>();

            try
            {
                foreach (var item in this.dictionary)
                {
                    clone.Add(item.Key, new System.IO.DirectoryInfo(item.Value));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }

            return clone;
        }

        #endregion

        #region IIniFileCompatible

        /// <summary>
        /// 読み込み
        /// </summary>
        /// <param name="iniFile"></param>
        /// <returns></returns>
        public override bool Read(IniFileReaderWriter iniFile)
        {
            var isSuccess = true;

            try
            {
                foreach (var item in this)
                {
                    var keyName = char.ToUpper(item.Key[0]) + item.Key[1..];

                    isSuccess &= iniFile.ReadValue(this.Section, keyName, item.Value, out DirectoryInfo? returnedValue);

                    if (true == isSuccess)
                    {
                        this[item.Key] = returnedValue ?? new DirectoryInfo(this.Root.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 書き込み
        /// </summary>
        /// <param name="iniFile"></param>
        /// <returns></returns>
        public override bool Write(IniFileReaderWriter iniFile)
        {
            var isSuccess = true;

            try
            {
                var default_root = new System.IO.DirectoryInfo("..");

                foreach (var item in this)
                {
                    var path = item.Value.FullName;
                    if (item.Value.Parent?.FullName?.Equals(default_root.FullName) ?? false)
                    {
                        path = $"..\\{item.Value.Name}\\";
                    }

                    var keyName = char.ToUpper(item.Key[0]) + item.Key[1..];

                    isSuccess &= iniFile.WriteValue(this.Section, keyName, path);
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
            ;
        }

        #endregion

        /// <summary>
        /// ディレクトリ名
        /// </summary>
        [Serializable]
        protected static class DirectoryNames
        {
            public static readonly string Config = "config";
            public static readonly string Data = "data";
            public static readonly string Log = "log";
            public static readonly string Temp = "temp";
            public static readonly string Recipe = "recipe";
        }

        #region フィールド

        /// <summary>
        /// パスリスト
        /// </summary>
        protected Dictionary<string, string> dictionary = new();

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PathManager() : base("path".ToUpper())
        {
            this.Initialize();
        }

        public virtual IEnumerator<KeyValuePair<string, DirectoryInfo>> GetEnumerator()
        {
            foreach (var item in this.dictionary)
            {
                yield return new KeyValuePair<string, DirectoryInfo>(item.Key, new System.IO.DirectoryInfo(item.Value));
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="path"></param>
        protected virtual void initialize(params KeyValuePair<string, string>[] values)
        {
            try
            {
                if (null == values)
                    throw new ArgumentNullException(nameof(values));

                this.dictionary = new Dictionary<string, string>();

                foreach (var v in values)
                {
                    try
                    {
                        var key = v.Key.Trim().ToLower();
                        if (false == string.IsNullOrEmpty(key) && false == this.dictionary.ContainsKey(key))
                        {
                            var directory = new DirectoryInfo(v.Value);

                            if (false == directory.Exists)
                                directory.Create();

                            this.dictionary.Add(key, directory.FullName);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(this, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(this, ex.Message);
            }
        }

        #region IEquatable

        public override bool Equals(IIniFileCompatible? compalison)
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

            if (compalison is IPathManager pm)
            {
                var info1 = this.GetDirectoryInfo();
                var info2 = pm.GetDirectoryInfo();

                if (info1.Count != info2.Count)
                {
                    return false;
                }

                foreach (var item in info1)
                {
                    if (false == info2.ContainsKey(item.Key))
                    {
                        return false;
                    }

                    if (false == item.Value.FullName.Equals(info2[item.Key].FullName))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return base.Equals(compalison);
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            foreach (var d in this)
            {
                hashCode.Add(d.Value.FullName.GetHashCode());
            }

            return hashCode.ToHashCode();
        }

        #endregion
    }
}