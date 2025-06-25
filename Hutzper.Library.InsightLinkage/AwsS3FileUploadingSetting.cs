using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.IO;

namespace Hutzper.Library.InsightLinkage
{
    public record AwsS3FileUploadingSetting : IniFileCompatible<AwsS3FileUploadingSetting>
    {
        [IniKey(true, 0)]
        public bool IsUse { get; set; } = false;
        [IniKey(true, "")]
        public string AccessKeyID { get; set; } = String.Empty;
        [IniKey(true, "")]
        public string SecretKey { get; set; } = String.Empty;
        [IniKey(true, "")]
        public string ProjectUuid { get; set; } = String.Empty;
        [IniKey(true, "")]
        public string BucketName { get; set; } = "insight-image";
        [IniKey(true, "")]
        public string ProjectDataBucketName { get; set; } = "insight-ai-project-data";
        [IniKey(true, "")]
        public string ProjectTaskBucketName { get; set; } = "insight-ai-project-task";

        public AwsS3FileUploadingSetting() : base("AWS_S3_FILE_UPLOADING_01".ToUpper())
        {
        }
    }
}