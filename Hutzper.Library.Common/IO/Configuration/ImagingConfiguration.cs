using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.Imaging;

namespace Hutzper.Library.Common.IO.Configuration
{
    /// <summary>
    /// 画像に関する設定
    /// </summary>
    [Serializable]
    public record ImagingConfiguration : IniFileCompatible<ImagingConfiguration>
    {
        #region プロパティ

        /// <summary>
        /// 設定項目
        /// </summary>
        public List<IImageProperties> Settings { get; } = new();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ImagingConfiguration() : base($"Imaging_Configuration".ToUpper())
        {
            var indexY = 0;
            foreach (var n in Enumerable.Range(0, 2))
            {
                foreach (var x in Enumerable.Range(0, n))
                {
                    this.Settings.Add(new ImagingSetting(new Library.Common.Drawing.Point(x, indexY)));
                }
                indexY++;
            }
        }

        #endregion

        #region publicメソッド

        /// <summary>
        /// location値から対象のIImagePropertiesを取得する
        /// </summary>
        /// <param name="location">検索値</param>
        /// <returns>対象のIImagePropertiesへの参照</returns>
        public IImageProperties? FindByLocation(Library.Common.Drawing.Point location) => this.Settings.FirstOrDefault(item => item.Location.Equals(location));

        /// <summary>
        /// location値から対象のIImagePropertiesを取得する
        /// </summary>
        /// <param name="location">検索値</param>
        /// <returns>対象のIImagePropertiesへの参照</returns>
        /// <remarks>存在しない場合は新規作成する</remarks>
        public IImageProperties GetOrAdd(Library.Common.Drawing.Point location)
        {
            var item = this.FindByLocation(location);

            if (item is null)
            {
                var newItem = new ImagingSetting(location);
                this.Settings.Add(newItem);

                this.Settings.Sort((s1, s2) =>
                {
                    var result = s1.Location.Y.CompareTo(s2.Location.Y);
                    return result == 0 ? s1.Location.X.CompareTo(s2.Location.X) : result;
                });

                return newItem;
            }
            else
            {
                return item;
            }
        }

        /// <summary>
        /// IIniFileCompatibleリストを取得する
        /// </summary>
        public List<IIniFileCompatible> GetItems() => this.Settings.ConvertAll(p => (IIniFileCompatible)p);

        #endregion

        #region IIniFileCompatible

        /// <summary>
        /// 読み込み
        /// </summary>
        /// <returns>成功した場合はtrue</returns>
        public override bool Read(IniFileReaderWriter iniFile)
        {
            var isSuccess = base.Read(iniFile);

            this.Settings.Clear();

            var sections = iniFile.GetSections();
            Array.Sort(sections, (s1, s2) => s1.CompareTo(s2));

            var indexY = 0;
            var indexX = 0;
            var nextSetting = new ImagingSetting(new Library.Common.Drawing.Point(indexX, indexY));

            var sectionSample = nextSetting.Section;
            var targetKeyword = "_Y";
            targetKeyword = sectionSample.Substring(0, sectionSample.IndexOf(targetKeyword) + targetKeyword.Length);
            var targetSections = sections.Where(s => s.StartsWith(targetKeyword)).ToArray();

            for (indexY = 0; indexY < 2; indexY++)
            {
                indexX = 0;
                nextSetting = new ImagingSetting(new Library.Common.Drawing.Point(indexX, indexY));
                foreach (var s in targetSections)
                {
                    if (false == s.Equals(nextSetting.Section) || false == nextSetting.Read(iniFile))
                    {
                        break;
                    }

                    this.Settings.Add(nextSetting);
                    nextSetting = new ImagingSetting(new Library.Common.Drawing.Point(++indexX, indexY));
                }
            }

            foreach (var i in this.GetItems())
            {
                isSuccess &= i?.Read(iniFile) ?? true;
            }

            return isSuccess;
        }

        /// <summary>
        /// 書込み
        /// </summary>
        /// <param name="iniFile">INIファイル読み書き</param>
        /// <returns>成功した場合はtrue</returns>
        public override bool Write(IniFileReaderWriter iniFile)
        {
            var isSuccess = base.Write(iniFile);

            foreach (var i in this.GetItems())
            {
                isSuccess &= i?.Write(iniFile) ?? true;
            }

            return isSuccess;
        }

        #endregion

        #region IEquatable

        /// <summary>
        /// IEquatable.Equals実装
        /// </summary>
        /// <param name="compalison">比較対象</param>
        /// <returns>同一と判断された場合はtrue</returns>
        public virtual bool Equals(IControllerParameter? compalison)
        {
            var items1 = this.GetItems();
            var items2 = compalison?.GetItems() ?? new();

            if (items1.Count != items2.Count)
            {
                return false;
            }

            foreach (var i in Enumerable.Range(0, items1.Count))
            {
                if (false == items1[i].Equals(items2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// HashCode取得
        /// </summary>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            hashCode.Add(base.GetHashCode());

            this.GetItems().ForEach(item => hashCode.Add(item));

            return hashCode.ToHashCode();
        }

        #endregion
    }
}

