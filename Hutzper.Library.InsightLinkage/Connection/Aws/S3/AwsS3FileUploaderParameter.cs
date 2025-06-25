using Hutzper.Library.Common.Attribute;
using Hutzper.Library.Common.Data;
using System.Security.Cryptography;

namespace Hutzper.Library.InsightLinkage.Connection.Aws.S3;

/// <summary>
/// ファイルアップローダー（AWS S3用）
/// </summary>
[Serializable]
public record AwsS3FileUploaderParameter : ControllerParameterBaseRecord, IFileUploaderParameter
{
    /// <summary>
    /// 使用するかどうか
    /// </summary>
    [IniKey(true, true)]
    public bool IsUse { get; set; } = true;

    /// <summary>
    /// アクセスキー ID
    /// </summary>
    public string AccessKeyID { get; set; } = "";

    /// <summary>
    /// シークレットアクセスキー
    /// </summary>
    public string SecretKey { get; set; } = "";

    /// <summary>
    /// プロジェクトUUID
    /// </summary>
    public string ProjectUuid { get; set; } = "";

    /// <summary>
    /// バケット名
    /// </summary>
    [IniKey(true, "insight-image")]
    public string BucketName { get; set; } = "insight-image"; // 判定画像：insight-image、画像収集：｛画像用バケット：insight-ai-project-data、JSON用バケット：insight-ai-project-task｝

    #region コンストラクタ

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="directoryName"></param>
    public AwsS3FileUploaderParameter(string accessKeyIDPath, string secretKeyPath) : this(accessKeyIDPath, secretKeyPath, 0)
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="directoryName"></param>
    public AwsS3FileUploaderParameter(string accessKeyIDPath, string secretKeyPath, int index) : this(accessKeyIDPath, secretKeyPath, index, typeof(AwsS3FileUploaderParameter).Name)
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="directoryName"></param>
    public AwsS3FileUploaderParameter(string accessKeyIDPath, string secretKeyPath, int index, string fileNameWithoutExtension) : base($"Aws_S3_File_Uploading_{((index < 0) ? 0 : index) + 1:D2}", "AwsS3FileUploaderParameter", $"{fileNameWithoutExtension}_{((index < 0) ? 0 : index) + 1:D2}.ini")
    {
        this.IsHierarchy = false;
        this.AccessKeyID = this.LoadCredentials(accessKeyIDPath);
        this.SecretKey = this.LoadCredentials(secretKeyPath);
    }

    #endregion

    #region メソッド

    /// <summary>
    /// 認証情報を読み込む
    /// </summary>
    public string LoadCredentials(string encryptedFilePath)
    {
        try
        {
            using (FileStream fs = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read))
            using (Aes aes = Aes.Create())
            {
                aes.Key = HexStringToByteArray("f257abbc26f482df9ba574acb648a4169f57c8a1ad0c67ffa0e212ba1fb653f3");
                aes.IV = HexStringToByteArray("3e9070989c68bf66b7f342788e266bad");

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(fs, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        cryptoStream.CopyTo(ms);
                    }

                    ms.Position = 0;
                    using (StreamReader reader = new StreamReader(ms))
                    {
                        return reader.ReadToEnd().Trim();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Serilog.Log.Warning(ex, ex.Message);
            return String.Empty;
        }
    }

    /// <summary>
    /// 16進数文字列をバイト配列に変換
    /// </summary>
    private static byte[] HexStringToByteArray(string hex)
    {
        int length = hex.Length;
        byte[] bytes = new byte[length / 2];
        for (int i = 0; i < length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }
    #endregion
}