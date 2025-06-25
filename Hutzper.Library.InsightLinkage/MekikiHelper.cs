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
    /// IInsightLinkageRequestを生成するヘルパクラス
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
        /// <param name="dateTime"></param>
        /// <param name="mekikiUnit"></param>
        /// <returns></returns>
        static public IInsightLinkageRequest[] CreateInsightLinkageRequest(string requestId, string deviceUuid, DateTime dateTime, InsightRequestType requestType, IInsightMekikiData baseData, params MekikiUnit[] mekikiUnits)
        {
            var requests = new List<IInsightLinkageRequest>();

            if (mekikiUnits is not null && baseData is not null)
            {
                baseData.Initialize(deviceUuid, dateTime);

                foreach (var unit in mekikiUnits)
                {
                    var data = baseData.Clone();

                    if (data is null)
                    {
                        continue;
                    }

                    data.DataCategory = unit.DataCategory;
                    data.Class = unit.Class;
                    data.ImageUploaded = false;

                    if (0 < unit.GeneralValues.Count)
                    {
                        foreach (var i in Enumerable.Range(0, System.Math.Min(data.Values.Length, unit.GeneralValues.Count)))
                        {
                            data.Values[i] = unit.GeneralValues[i];
                        }
                    }

                    if (0 < unit.Options.Count)
                    {
                        foreach (var i in Enumerable.Range(0, System.Math.Min(data.Values.Length, unit.Options.Count)))
                        {
                            if (unit.Options[i] is not null && false == data.Options.ContainsKey(unit.Options[i]))
                            {
                                data.Options.Add(unit.Options[i], data.Values[i]);
                            }
                        }
                    }

                    var fileUploadRequest = (IFileUploadRequest?)null;

                    if (unit.ImageFileInfo is not null)
                    {
                        data.ImageUploaded = true;

                        fileUploadRequest = new FileUploadRequest(unit.ImageFileInfo, unit.DestinationFileName, deviceUuid);

                        if (data.DataCategory is not null && false == data.DataCategory.Equals("all"))
                        {
                            fileUploadRequest.DestinationFolderHierarchy.Add(data.DataCategory);
                        }
                    }

                    requests.Add(new InsightLinkageRequest(requestId, data, fileUploadRequest, requestType));
                }
            }
            return requests.ToArray();
        }
    }
}