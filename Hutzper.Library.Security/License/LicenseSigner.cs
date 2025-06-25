using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Hutzper.Library.Security.License;

/// <summary>
/// ライセンスデータに対する署名生成ユーティリティ
/// </summary>
public class LicenseSigner
{
    /// <summary>
    /// 指定された RSA プライベートキーを用いてライセンスに署名を行う
    /// </summary>
    /// <param name="license">署名対象のライセンスデータ</param>
    /// <param name="privateKey">署名に使用する RSA プライベートキー</param>
    /// <returns>Base64 形式の署名文字列</returns>
    public static string CreateSignature(LicenseData license, RSA privateKey)
    {
        var payload = JsonSerializer.Serialize(new
        {
            license.Key,
            license.IV
        });

        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        byte[] signature = privateKey.SignData(payloadBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        return Convert.ToBase64String(signature);
    }
}
