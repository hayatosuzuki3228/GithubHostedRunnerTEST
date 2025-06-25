namespace Hutzper.Library.Common.IO
{
    /// <summary>
    /// INIファイル読み書き
    /// </summary>
    [Serializable]
    public class IniFileReaderWriter
    {
        #region プロパティ

        public string FileName { get; init; }

        #endregion

        #region フィールド

        protected ManagedIniFile IniFile;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fileName">.ini ファイルの名前</param>
        public IniFileReaderWriter(string fileName)
        {
            this.FileName = fileName;
            this.IniFile = new ManagedIniFile(this.FileName);
        }

        #endregion

        #region メソッド

        /// <summary>
        /// Readを行う前に呼び出してファイルからメモリに読み込みます。
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            var result = true;

            try
            {
                result &= this.IniFile.Load();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Writeを行った後に呼び出してファイルに保存します。
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            var result = true;

            try
            {
                result &= this.IniFile.Save();
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public bool ReadValue(string appName, string keyName, string defaultValue, out string returnedValue)
        {
            return this.ReadString(appName, keyName, defaultValue, out returnedValue);
        }

        public bool ReadValue(string appName, string keyName, int defaultValue, out int returnedValue) => this.ReadValue<int>(appName, keyName, defaultValue, out returnedValue);
        public bool ReadValue(string appName, string keyName, double defaultValue, out double returnedValue) => this.ReadValue<double>(appName, keyName, defaultValue, out returnedValue);
        public bool ReadValue(string appName, string keyName, short defaultValue, out short returnedValue) => this.ReadValue<short>(appName, keyName, defaultValue, out returnedValue);

        public bool ReadValue(string appName, string keyName, bool defaultValue, out bool returnedValue)
        {
            returnedValue = defaultValue;

            if (false == this.ReadValue<int>(appName, keyName, Convert.ToInt32(defaultValue), out int returnedInt))
            {
                return false;
            }

            returnedValue = Convert.ToBoolean(returnedInt);

            return true;
        }

        public bool ReadValue(string appName, string keyName, System.Drawing.Color defaultValue, out System.Drawing.Color returnedValue)
        {
            returnedValue = defaultValue;

            if (false == this.ReadString(appName, keyName, defaultValue.ToHexadecimal(), out string returnedString))
            {
                return false;
            }

            returnedValue = returnedValue.FromHexadecimal(returnedString);

            return true;
        }


        public bool ReadValue(string appName, string keyName, Enum defaultValue, out Enum returnedValue)
        {
            returnedValue = defaultValue;

            if (false == this.ReadString(appName, keyName, defaultValue.ToString(), out string returnedString))
            {
                return false;
            }

            try
            {
                returnedValue = (Enum)Enum.Parse(returnedValue.GetType(), returnedString);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool ReadValue(string appName, string keyName, DirectoryInfo? defaultValue, out DirectoryInfo? returnedValue)
        {
            if (null != defaultValue && false == string.IsNullOrEmpty(defaultValue.FullName))
            {
                returnedValue = new DirectoryInfo(defaultValue.FullName);
            }
            else
            {
                returnedValue = null;
            }

            if (false == ReadString(appName, keyName, defaultValue?.FullName ?? string.Empty, out string returnedString))
            {
                return false;
            }

            if (false == string.IsNullOrEmpty(returnedString))
            {
                returnedValue = new DirectoryInfo(returnedString);
            }

            return true;
        }

        /// <summary>
        /// 指定したキーの値を指定した型で取得します。
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="appName">セクション</param>
        /// <param name="keyName">キー</param>
        /// <param name="defaultValue">初期値</param>
        /// <param name="returnedValue">取得値</param>
        /// <returns>true:成功 false:失敗</returns>
        public bool ReadValue<T>(string appName, string keyName, T defaultValue, out T returnedValue) where T : struct
        {
            returnedValue = this.IniFile.GetValue<T>(appName, keyName, defaultValue);
            return true;
        }

        public bool ReadString(string appName, string keyName, string defaultValue, out string returnedValue)
        {
            returnedValue = this.IniFile.GetValue<string>(appName, keyName, defaultValue) ?? string.Empty;
            return true;
        }

        /// <summary>
        /// 指定したキーの値を指定した型で書き込みます。
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <param name="appName">セクション</param>
        /// <param name="keyName">キー</param>
        /// <param name="value">値</param>
        public bool WriteValue<T>(string appName, string keyName, T value) where T : struct
        {
            return this.WriteValue(appName, keyName, value.ToString());
        }
        public bool WriteValue(string appName, string keyname, bool value)
        {
            return this.WriteValue(appName, keyname, Convert.ToInt32(value).ToString());
        }

        public bool WriteValue(string appName, string keyname, Enum value)
        {
            return this.WriteValue(appName, keyname, value.ToString());
        }

        public bool WriteValue(string appName, string keyname, System.Drawing.Color value)
        {
            return this.WriteValue(appName, keyname, value.ToHexadecimal());
        }

        public bool WriteValue(string appName, string keyname, DirectoryInfo? value)
        {
            return this.WriteValue(appName, keyname, value?.FullName ?? string.Empty);
        }

        /// <summary>
        /// 指定したキーの値を書き込みます。
        /// </summary>
        /// <param name="appName">セクション</param>
        /// <param name="keyName">キー</param>
        /// <param name="string">文字列</param>
        /// <returns>true:成功 false:失敗</returns>
        public bool WriteValue(string appName, string keyName, string? @string)
        {
            this.IniFile.SetValue<string>(appName, keyName, @string);

            return true;
        }

        /// <summary>
        /// 指定したセクションに存在するすべてのキーの値を取得します。
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public string[] GetKeys(string appName)
        {
            return this.IniFile.GetKeys(appName).ToArray();
        }

        /// <summary>
        /// 指定したセクションが存在しているかどうかを取得します。
        /// </summary>
        /// <param name="appName">セクション名</param>
        /// <param name="existence">true:存在する false:存在しない</param>
        /// <returns>true:成功 false:失敗</returns>
        public bool IsSectionExists(string appName)
        {
            return this.IniFile.IsSectionExists(appName);
        }

        /// <summary>
        /// 指定したキーが存在しているかどうかを取得します。
        /// </summary>
        /// <param name="appName">セクション名</param>
        /// <param name="keyName">キー名</param>
        public bool IsKeyExists(string appName, string keyName)
        {
            return this.IniFile.IsKeyExists(appName, keyName);
        }

        /// <summary>
        /// すべてのセクションの名前を取得します。
        /// </summary>
        /// <returns>すべてのセクションの名前</returns>
        public string[] GetSections()
        {
            return this.IniFile.GetSections().ToArray();
        }

        #endregion
    }
}