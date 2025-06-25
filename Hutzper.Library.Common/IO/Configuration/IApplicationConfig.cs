using Hutzper.Library.Common.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Hutzper.Library.Common.IO.Configuration
{
    public interface IApplicationConfig : ILoggable, IEquatable<IApplicationConfig>
    {
        /// <summary>
        /// 設定ファイル名
        /// </summary>
        public FileInfo FileInfo { get; init; }

        /// <summary>
        /// 設定項目
        /// </summary>
        public List<IIniFileCompatible> Items { get; init; }

        /// <summary>
        /// パス管理
        /// </summary>
        public IPathManager GetPathManager();

        public bool Load();

        public bool Save();

        public bool Load(FileInfo fileInfo);

        public bool Save(FileInfo fileInfo);

        public bool Load(DirectoryInfo directoryInfo);

        public bool Save(DirectoryInfo directoryInfo);
    }

    /// <summary>
    /// アプリケーション設定
    /// </summary>
    public abstract class ApplicationConfig : IApplicationConfig
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

        #region IApplicationConfig

        public FileInfo FileInfo { get; init; }

        public List<IIniFileCompatible> Items { get; init; } = new();


        public virtual bool Load() => this.Load(this.FileInfo);
        public virtual bool Save() => this.Save(this.FileInfo);
        public virtual bool Load(DirectoryInfo directoryInfo) => this.Load(new FileInfo(System.IO.Path.Combine(directoryInfo.FullName, this.FileInfo.Name)));
        public virtual bool Save(DirectoryInfo directoryInfo) => this.Save(new FileInfo(System.IO.Path.Combine(directoryInfo.FullName, this.FileInfo.Name)));

        public virtual bool Load(FileInfo fileInfo)
        {
            var file = new IniFileReaderWriter(fileInfo.FullName);

            var result = file.Load();

            foreach (var item in this.Items)
            {
                result &= item.Read(file);
            }

            return result;
        }

        public virtual bool Save(FileInfo fileInfo)
        {
            var result = true;

            var file = new IniFileReaderWriter(fileInfo.FullName);
            foreach (var item in this.Items)
            {
                result &= item.Write(file);
            }

            result &= file.Save();

            return result;
        }

        /// <summary>
        /// パス管理
        /// </summary>
        public IPathManager GetPathManager() => this.Path;

        #endregion

        #region IEquatable

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj is ApplicationConfig ap)
            {
                return this.Equals(ap);
            }
            else
            {
                return false;
            }

        }

        public bool Equals(IApplicationConfig? ap)
        {
            if (ap is null)
            {
                return false;
            }

            if (Object.ReferenceEquals(this, ap))
            {
                return true;
            }

            if (this.GetType() != ap.GetType())
            {
                return false;
            }

            if (this.Items.Count != ap.Items.Count)
            {
                return false;
            }

            foreach (var item in this.Items)
            {
                var index = this.Items.IndexOf(item);

                if (false == item.Equals(ap.Items[index]))
                {
                    return false;
                }

                if (item is IControllerParameter cp1 && ap.Items[index] is IControllerParameter cp2)
                {
                    if (false == cp1.Equals(cp2))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            this.Items.ForEach(item => hashCode.Add(item));

            return hashCode.ToHashCode();
        }

        public static bool operator ==(ApplicationConfig ap1, ApplicationConfig ap2)
        {
            if (ap1 is null)
            {
                if (ap2 is null)
                {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return ap1.Equals(ap2);
        }

        public static bool operator !=(ApplicationConfig ap1, ApplicationConfig ap2) => !(ap1 == ap2);

        #endregion

        #region プロパティ

        public IPathManager Path { get; init; }


        public LogSettingIniFile Log { get; init; }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="fullFileName"></param>
        public ApplicationConfig(IServiceCollectionSharing? serviceCollectionSharing, string fullFileName)
        {
            this.FileInfo = new FileInfo(fullFileName);

            this.nickname = this.FileInfo.Name;

            this.Path = serviceCollectionSharing?.ServiceProvider?.GetRequiredService<IPathManager>() ?? new PathManager();

            this.Items.Add(this.Path);
            this.Items.Add(this.Log = new());
        }

        #endregion

        #region スタティックメソッド

        public static FileInfo GetStandardConfigFileInfo()
        {
            var configFile = new FileInfo(System.IO.Path.ChangeExtension(Application.ExecutablePath, ".ini"));

            return new FileInfo(System.IO.Path.Combine(new PathManager().Config.FullName, configFile.Name));
        }

        #endregion
    }
}