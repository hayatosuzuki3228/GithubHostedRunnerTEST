using Hutzper.Library.Common;
using Hutzper.Library.InsightLinkage.Connection;
using Hutzper.Library.InsightLinkage.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Hutzper.Library.InsightLinkage
{
    /// <summary>
    /// 受渡し用のコンテナクラス
    /// </summary>
    [Serializable]
    public class MekikiUnit
    {
        #region プロパティ

        /// <summary>
        /// データ日時
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// データ分類
        /// </summary>
        /// <remarks>all, camera1, camera2,...camerarN</remarks>
        public string DataCategory { get; set; } = string.Empty;

        /// <summary>
        /// クラス値
        /// </summary>
        public string Class { get; set; } = string.Empty;

        /// <summary>
        /// Option
        /// </summary>
        public List<string> Options { get; set; } = new();

        /// <summary>
        /// アップロード元画像ファイル情報
        /// </summary>
        public FileInfo? ImageFileInfo { get; set; }

        /// <summary>
        /// アップロード先のファイル名
        /// </summary>
        public string DestinationFileName { get; set; } = string.Empty;

        /// <summary>
        /// 汎用格納域
        /// </summary>
        public List<double> GeneralValues { get; init; } = new();

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dataCategory"></param>
        /// <param name="class"></param>
        /// <param name="imageFileInfo"></param>
        /// <param name="destinationFileName"></param>
        public MekikiUnit(string dataCategory, string @class, FileInfo? imageFileInfo, string destinationFileName = "") : this(DateTime.Now, dataCategory, @class, imageFileInfo, destinationFileName)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="dataCategory"></param>
        /// <param name="class"></param>
        /// <param name="imageFileInfo"></param>
        /// <param name="destinationFileName"></param>
        public MekikiUnit(DateTime dateTime, string dataCategory, string @class, FileInfo? imageFileInfo, string destinationFileName = "")
        {
            this.DateTime = dateTime;
            this.DataCategory = dataCategory;
            this.Class = @class;
            this.ImageFileInfo = imageFileInfo;
            this.DestinationFileName = destinationFileName;
        }

        #endregion
    }

    /// <summary>
    /// IInsightLingageRequestを生成するヘルパクラス
    /// </summary>
    public class MekikiHelper
    {
        // IServiceCollection
        protected IServiceCollectionSharing? Services;

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="serviceCollection"></param>
        public void Initialize(IServiceCollectionSharing? serviceCollection)
        {
            this.Services = serviceCollection;
        }

        /// <summary>
        /// 入力されたMekikiUnitからInsight連携リクエストを生成する
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="mekikiUnit"></param>
        /// <returns></returns>
        public IInsightLingageRequest[] CreateInsightLinkageRequest(string requestId, string uuid, InsightRequestType requestType, params MekikiUnit[] mekikiUnit) => this.CreateInsightLinkageRequest(requestId, uuid, mekikiUnit.First()?.DateTime ?? DateTime.Now, requestType, mekikiUnit);

        /// <summary>
        /// 入力されたMekikiUnitからInsight連携リクエストを生成する
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="dateTime"></param>
        /// <param name="mekikiUnit"></param>
        /// <returns></returns>
        public IInsightLingageRequest[] CreateInsightLinkageRequest(string requestId, string uuid, DateTime dateTime, InsightRequestType requestType, params MekikiUnit[] mekikiUnit)
        {
            var requests = new List<IInsightLingageRequest>();

            if (this.Services is not null)
            {
                var baseData = ServiceCollectionSharing.Instance.ServiceProvider?.GetRequiredService<IInsightMekikiData>();

                if (mekikiUnit is not null && baseData is not null)
                {
                    baseData.Initialize(uuid, dateTime);

                    foreach (var m in mekikiUnit)
                    {
                        var data = baseData.Clone();

                        if (data is null)
                        {
                            continue;
                        }

                        data.DataCategory = m.DataCategory;
                        data.Class = m.Class;
                        data.ImageUploaded = false;

                        if (0 < m.GeneralValues.Count)
                        {
                            foreach (var i in Enumerable.Range(0, System.Math.Min(data.Values.Length, m.GeneralValues.Count)))
                            {
                                data.Values[i] = m.GeneralValues[i];
                            }
                        }

                        if (0 < m.Options.Count)
                        {
                            foreach (var i in Enumerable.Range(0, System.Math.Min(data.Values.Length, m.Options.Count)))
                            {
                                if (false == data.Options.ContainsKey(m.Options[i]))
                                {
                                    data.Options.Add(m.Options[i], data.Values[i]);
                                }
                            }
                        }

                        var fileUploadRequest = (IFileUploadRequest?)null;

                        if (m.ImageFileInfo is not null)
                        {
                            data.ImageUploaded = true;

                            fileUploadRequest = new FileUploadRequest(m.ImageFileInfo, m.DestinationFileName, uuid);

                            if (data.DataCategory is not null && false == data.DataCategory.Equals("all"))
                            {
                                fileUploadRequest.DestinationFolderHierarchy.Add(data.DataCategory);
                            }
                        }

                        requests.Add(new InsightLingageRequest(requestId, data, fileUploadRequest, requestType));
                    }
                }
            }

            return requests.ToArray();
        }
        static public IInsightLingageRequest[] CreateInsightLinkageRequest_noDI(string requestId, string uuid, DateTime dateTime, InsightRequestType requestType, IInsightMekikiData baseData, params MekikiUnit[] mekikiUnit)
        {
            var requests = new List<IInsightLingageRequest>();

            if (mekikiUnit is not null && baseData is not null)
            {
                baseData.Initialize(uuid, dateTime);

                foreach (var m in mekikiUnit)
                {
                    var data = baseData.Clone();

                    if (data is null)
                    {
                        continue;
                    }

                    data.DataCategory = m.DataCategory;
                    data.Class = m.Class;
                    data.ImageUploaded = false;

                    if (0 < m.GeneralValues.Count)
                    {
                        foreach (var i in Enumerable.Range(0, System.Math.Min(data.Values.Length, m.GeneralValues.Count)))
                        {
                            data.Values[i] = m.GeneralValues[i];
                        }
                    }

                    if (0 < m.Options.Count)
                    {
                        foreach (var i in Enumerable.Range(0, System.Math.Min(data.Values.Length, m.Options.Count)))
                        {
                            if (m.Options[i] is not null && false == data.Options.ContainsKey(m.Options[i]))
                            {
                                data.Options.Add(m.Options[i], data.Values[i]);
                            }
                        }
                    }

                    var fileUploadRequest = (IFileUploadRequest?)null;

                    if (m.ImageFileInfo is not null)
                    {
                        data.ImageUploaded = true;

                        fileUploadRequest = new FileUploadRequest(m.ImageFileInfo, m.DestinationFileName, uuid);

                        if (data.DataCategory is not null && false == data.DataCategory.Equals("all"))
                        {
                            fileUploadRequest.DestinationFolderHierarchy.Add(data.DataCategory);
                        }
                    }

                    requests.Add(new InsightLingageRequest(requestId, data, fileUploadRequest, requestType));
                }
            }
            return requests.ToArray();
        }
    }
}