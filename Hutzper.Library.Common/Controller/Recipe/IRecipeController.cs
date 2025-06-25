using Hutzper.Library.Common.Data;
using Hutzper.Library.Common.Data.Recipe;
using Hutzper.Library.Common.IO;
using Hutzper.Library.Common.Runtime;

namespace Hutzper.Library.Common.Controller.Recipe
{
    /// <summary>
    /// レシピ管理インタフェース
    /// </summary>
    /// <remarks>レシピデータを管理するインタフェースです。</remarks>
    public interface IRecipeController : IController
    {
        /// <summary>
        /// 指定した名称のデータが存在するかどうか
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public bool Contains(string name);

        /// <summary>
        /// 一覧情報の取得
        /// </summary>
        /// <returns></returns>
        public List<IRecordable> GetRecords();

        /// <summary>
        /// 指定された名前のレシピデータを取得する
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IRecipeData? GetRecipeData(string name);

        /// <summary>
        /// 指定されたレコードのレシピデータを削除する
        /// </summary>
        /// <param name="record"></param>
        public bool DeleteRecipeData(IRecordable record);

        /// <summary>
        /// 指定されたレシピデータを追加する
        /// </summary>
        /// <param name="data"></param>
        public bool AddRecipeData(IRecipeData data);

        /// <summary>
        /// 指定されたレコードと同Nameを持つデータを置換する
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public bool Replace(IRecordable record);

        /// <summary>
        /// 現在のデータを保存する
        /// </summary>
        /// <returns></returns>
        public bool Save();
    }

    /// <summary>
    /// レシピ管理基本クラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>IRecipeControllerの実装です。基本的なデータの読み書き機能を提供します。</remarks>
    public abstract class RecipeControllerBase<T> : ControllerBase, IRecipeController where T : IRecipeData, new()
    {
        protected IRecipeControllerParameter? Parameter;

        protected List<IRecordable> Items = new();

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RecipeControllerBase() : base("レシピ管理")
        {

        }

        #endregion

        #region IController

        /// <summary>
        /// パラメーター設定
        /// </summary>
        /// <param name="parameter"></param>
        public override void SetParameter(IControllerParameter? parameter)
        {
            try
            {
                base.SetParameter(parameter);

                this.Parameter = (IRecipeControllerParameter?)parameter;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update()
        {
            this.Items.Clear();

            try
            {
                if (this.PathManager != null && this.Parameter != null)
                {
                    #region 一覧データを読み込む

                    var directoryInfo = this.PathManager[this.Parameter.DirectoryKey];

                    if (null != directoryInfo)
                    {
                        var data = new T();

                        var fileInfo = new FileInfo(System.IO.Path.Combine(directoryInfo.FullName, this.Parameter.FileNames.First()));

                        var reader = new CsvFileReaderWriter(fileInfo.FullName);

                        if (true == reader.ReadAllLine(out string[][] lines))
                        {
                            // ヘッダスキップ
                            foreach (var line in lines.Skip(1))
                            {
                                try
                                {
                                    var item = new RecordableBaseRecord();
                                    item.FromCsvString(string.Join(",", line));

                                    this.Items.Add(item);
                                }
                                catch (Exception ex)
                                {
                                    Serilog.Log.Warning(ex, ex.Message);
                                }
                            }
                        }
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// 現在のデータを保存する
        /// </summary>
        public virtual bool Save()
        {
            var isSuccess = false;

            try
            {
                if (this.PathManager != null && this.Parameter != null)
                {
                    #region 一覧データを書き込む

                    var directoryInfo = this.PathManager[this.Parameter.DirectoryKey];

                    if (null != directoryInfo)
                    {
                        var fileInfo = new FileInfo(System.IO.Path.Combine(directoryInfo.FullName, this.Parameter.FileNames.First()));

                        var writer = new CsvFileReaderWriter(fileInfo.FullName);

                        // ヘッダ書き込み
                        writer.WriteLine(new RecordableBaseRecord().GetDescriptions());

                        // データ書き込み
                        foreach (ICsvConvertible item in Items.Cast<ICsvConvertible>())
                        {
                            writer.AppedLine(item.ToCsvString());
                        }
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        #endregion

        #region IRecipeController

        /// <summary>
        /// 指定した名称のデータが存在するかどうか
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public virtual bool Contains(string name)
        {
            var isExists = false;

            try
            {
                if (null != this.Items.Find(item => item.Name == name))
                {
                    isExists = true;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isExists;
        }

        /// <summary>
        /// 一覧情報の取得
        /// </summary>
        /// <returns></returns>
        public virtual List<IRecordable> GetRecords()
        {
            var records = new List<IRecordable>();

            try
            {
                records.AddRange(this.Items.ConvertAll(item => PropertyCopier.CopyTo(item, new RecordableBaseRecord())));
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return records;
        }

        /// <summary>
        /// 指定された名前のレシピデータを取得する
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IRecipeData? GetRecipeData(string name)
        {
            IRecipeData? data = null;

            try
            {
                var selected = this.Items.Find(item => item.Name == name);

                if (null != selected && this.PathManager != null && this.Parameter != null)
                {
                    #region データ取得
                    var directoryInfo = this.PathManager[this.Parameter.DirectoryKey];

                    if (null != directoryInfo)
                    {
                        var temp = new T();

                        if (this.Parameter.IsHierarchy)
                        {
                            directoryInfo = new DirectoryInfo(System.IO.Path.Combine(directoryInfo.FullName, name.ToLower()));

                            if (true == temp.Load(directoryInfo))
                            {
                                data = temp;
                            }
                            else
                            {
                                temp.Dispose();
                            }
                        }
                        else
                        {
                            var fileExt = System.IO.Path.GetExtension(temp.GetFileName());
                            var fileName = System.IO.Path.ChangeExtension(name.ToLower(), fileExt);

                            var fileInfo = new FileInfo(System.IO.Path.Combine(directoryInfo.FullName, fileName));

                            if (data is IFileCompatible fileCompatible)
                            {
                                fileCompatible.Load(fileInfo);
                            }
                            else
                            {
                                if (true == temp.Load(directoryInfo, fileInfo.Name))
                                {
                                    data = temp;
                                }
                                else
                                {
                                    temp.Dispose();
                                }
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return data;
        }


        /// <summary>
        /// 指定されたレコードのレシピデータを削除する
        /// </summary>
        /// <remarks>この操作はもとに戻せません</remarks>
        /// <param name="record"></param>
        public virtual bool DeleteRecipeData(IRecordable record)
        {
            var isSuccess = true;

            try
            {
                var selected = this.Items.Find(item => item.Name == record.Name);

                if (null != selected && this.PathManager != null && this.Parameter != null)
                {
                    // 一覧から除外
                    this.Items.Remove(selected);

                    #region ファイル削除

                    var directoryInfo = this.PathManager[this.Parameter.DirectoryKey];

                    if (null != directoryInfo)
                    {
                        if (this.Parameter.IsHierarchy)
                        {
                            directoryInfo = new DirectoryInfo(System.IO.Path.Combine(directoryInfo.FullName, selected.Name.ToLower()));

                            isSuccess = FileUtilities.DeleteAndVerify(directoryInfo);
                        }
                        else
                        {
                            using var temp = new T()
                            {
                                Name = record.Name
                            };

                            var fileExt = System.IO.Path.GetExtension(temp.GetFileName());
                            var fileName = System.IO.Path.ChangeExtension(record.Name.ToLower(), fileExt);

                            var fileInfo = new FileInfo(System.IO.Path.Combine(directoryInfo.FullName, fileName));

                            isSuccess = FileUtilities.DeleteAndVerify(fileInfo);
                        }
                    }

                    #endregion

                    isSuccess = this.Save();
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 指定されたレシピデータを追加する
        /// </summary>
        /// <param name="data"></param>
        public virtual bool AddRecipeData(IRecipeData data)
        {
            var isSuccess = false;

            try
            {
                var selected = this.Items.Find(item => item.Name == data.Name);

                if (null == selected && this.PathManager != null && this.Parameter != null && !string.IsNullOrEmpty(data.Name))
                {
                    #region ファイル保存

                    var directoryInfo = this.PathManager[this.Parameter.DirectoryKey];

                    var saveDateTime = DateTime.Now;

                    if (null != directoryInfo)
                    {
                        data.DateTime = saveDateTime;

                        if (this.Parameter.IsHierarchy)
                        {
                            directoryInfo = new DirectoryInfo(System.IO.Path.Combine(directoryInfo.FullName, data.Name.ToLower()));
                            isSuccess = data.Save(directoryInfo);
                        }
                        else
                        {
                            var fileExt = System.IO.Path.GetExtension(data.GetFileName());
                            var fileName = System.IO.Path.ChangeExtension(data.Name.ToLower(), fileExt);

                            var fileInfo = new FileInfo(System.IO.Path.Combine(directoryInfo.FullName, fileName));

                            if (data is IFileCompatible fileCompatible)
                            {
                                isSuccess = fileCompatible.Save(fileInfo);
                            }
                            else
                            {
                                isSuccess = data.Save(directoryInfo, fileInfo.Name);
                            }
                        }
                    }

                    #endregion

                    // 一覧に追加
                    if (true == isSuccess)
                    {
                        var item = new RecordableBaseRecord();

                        PropertyCopier.CopyTo(data, item);

                        this.Items.Add(item);

                        isSuccess = this.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        /// <summary>
        /// 指定されたレコードと同Nameを持つデータを置換する
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public virtual bool Replace(IRecordable record)
        {
            var isSuccess = false;

            try
            {
                var selected = this.Items.Find(item => item.Name == record.Name);

                if (null != selected && this.PathManager != null && this.Parameter != null)
                {
                    // プロパティコピー
                    PropertyCopier.CopyTo(record, selected);

                    // 保存
                    isSuccess = this.Save();
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        #endregion
    }
}