using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Hutzper.Library.Security.License;

/// <summary>
/// RSA 署名検証による改ざんチェックを行うユーティリティ
/// </summary>
public static class LicenseVerifier
{
    /// <summary>
    /// LicenseData の RSA 署名が有効であるかを検証する
    /// </summary>
    public static bool VerifyLicenseSignature(LicenseData license, RSAParameters publicKey)
    {
        if (string.IsNullOrWhiteSpace(license.Signature))
        {
            return false;
        }

        try
        {
            var payload = JsonSerializer.Serialize(new { license.Key, license.IV });
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
            byte[] signatureBytes = Convert.FromBase64String(license.Signature);

            using var rsa = RSA.Create();
            rsa.ImportParameters(publicKey);

            return rsa.VerifyData(
                payloadBytes,
                signatureBytes,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);
        }
        catch
        {
            // 不正な署名（形式・鍵エラーなど）は false 扱い
            return false;
        }
    }
}
