using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Hutzper.Library.Common;
using Hutzper.Library.Common.Controller;
using Hutzper.Library.Common.Data;
using Hutzper.Library.InsightLinkage.Controller;
using Hutzper.Library.InsightLinkage.Data;

namespace Hutzper.Library.InsightLinkage.Connection.Aws.S3
{
    /// <summary>
    /// AWS S3通信
    /// </summary>
    [Serializable]
    public class AwsS3FileUploader : ControllerBase, IFileUploader
    {
        #region IController

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="serviceCollection"></param>
        public override void Initialize(IServiceCollectionSharing? serviceCollection)
        {
            try
            {
                base.Initialize(serviceCollection);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// パラメーター設定
        /// </summary>
        /// <param name="parameter"></param>
        public override void SetParameter(IControllerParameter? parameter)
        {
            try
            {
                base.SetParameter(parameter);
                if (parameter is IFileUploaderParameter p)
                {
                    this.Parameter = p;
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
        }

        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            try
            {
                if (this.Parameter is AwsS3FileUploaderParameter p)
                {
                    var credentials = new BasicAWSCredentials(p.AccessKeyID, p.SecretKey);

                    var config = new AmazonS3Config
                    {
                        RegionEndpoint = RegionEndpoint.APNortheast1,
                    };

                    this.S3Client = new AmazonS3Client(credentials, config);
                }
            }
            catch (Exception ex)
            {
                this.DisposeSafely(this.S3Client);
                this.S3Client = null;
                Serilog.Log.Warning(ex, ex.Message);
            }

            return this.Enabled;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            var isSuccess = true;

            try
            {
                this.DisposeSafely(this.S3Client);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.S3Client = null;
            }

            return isSuccess;
        }

        #endregion

        #region IConnection

        /// <summary>
        /// 有効かどうか
        /// </summary>
        /// <remarks>Open済みか</remarks>
        public bool Enabled => this.S3Client is not null;

        /// <summary>
        /// Insight連携タイプ
        /// </summary>
        public InsightLinkageType LinkageType => InsightLinkageType.FileUploading;

        #endregion

        #region IFileUploader

        /// <summary>
        /// 指定されたファイルをアップロードする
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public bool UploadFile(IFileUploadRequest[] requests, InsightRequestType insightRequestType)
        {
            var isSuccess = this.Enabled;

            try
            {
                if (this.S3Client is not null && this.Parameter is AwsS3FileUploaderParameter p && p.IsUse)
                {
                    foreach (var r in requests)
                    {
                        var subdirectory = string.Join("/", r.DestinationFolderHierarchy);
                        if (false == string.IsNullOrEmpty(subdirectory))
                        {
                            subdirectory += "/";
                        }

                        var filePath = r.SourceFileInfo.FullName;
                        var fileExtension = Path.GetExtension(filePath).ToLower();

                        PutObjectRequest putRequest = insightRequestType switch
                        {
                            InsightRequestType.Inspection => new PutObjectRequest
                            {
                                BucketName = p.BucketName,
                                Key = $"{r.ProjectUUID}/{subdirectory}{r.DestinationFileName}",
                                FilePath = filePath,
                                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
                            },
                            InsightRequestType.ImageCollection => new PutObjectRequest
                            {
                                BucketName = fileExtension == ".json" ? "insight-ai-project-task" : "insight-ai-project-data",
                                Key = $"{r.ProjectUUID}/{subdirectory}{r.DestinationFileName}{fileExtension}",
                                FilePath = filePath
                            },
                            _ => throw new NotImplementedException("This InsightRequestType is not yet implemented")
                        };

                        var response = this.S3Client.PutObjectAsync(putRequest);
                        response.Wait();
                        if (response.Result.HttpStatusCode != System.Net.HttpStatusCode.OK)
                        {
                            isSuccess = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }

            return isSuccess;
        }

        #endregion

        #region SafelyDisposable

        /// <summary>
        /// リソースの解放
        /// </summary>
        protected override void DisposeExplicit()
        {
            try
            {
                this.DisposeSafely(this.S3Client);
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, ex.Message);
            }
            finally
            {
                this.S3Client = null;
            }
        }

        #endregion

        #region フィールド

        /// <summary>
        /// パラメータ
        /// </summary>
        protected IControllerParameter? Parameter;

        protected AmazonS3Client? S3Client;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index"></param>
        public AwsS3FileUploader() : this(typeof(AwsS3FileUploader).Name, -1)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="index"></param>
        public AwsS3FileUploader(string nickname, int index = -1) : base(nickname, index)
        {
        }

        #endregion
    }
}