using Hutzper.Library.Security.Aes;
using System.Security.Cryptography;
using System.Text.Json;

namespace Hutzper.Library.Security.License;

/// <summary>
/// license.sig 読み込み→署名検証→AES サービス生成のファクトリ
/// </summary>
public static class LicenseLoader
{
    /// <summary>
    /// 指定されたライセンスを読み込み、署名の検証を行った上で
    /// AES暗号化サービスを生成して返す。
    /// </summary>
    /// <param name="licensePath"></param>
    /// <param name="publicKey"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public static AesEncryptionService LoadAesFromLicense(string licensePath, RSAParameters publicKey)
    {
        try
        {
            string json = File.ReadAllText(licensePath);

            var license = JsonSerializer.Deserialize<LicenseData>(json);
            if (license == null)
            {
                throw new InvalidDataException("Invalid license format.");
            }


            if (string.IsNullOrWhiteSpace(license.Key) || string.IsNullOrWhiteSpace(license.IV))
            {
                throw new InvalidDataException("Key or IV is empty.");
            }

            if (!LicenseVerifier.VerifyLicenseSignature(license, publicKey))
            {
                throw new UnauthorizedAccessException("License signature verification failed.");
            }

            var options = AesEncryptionOptions.FromHex(license.Key, license.IV);
            return new AesEncryptionService(options);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException("Invalid license format.", ex);
        }
    }
}
