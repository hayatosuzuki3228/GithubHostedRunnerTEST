using System.Security.Cryptography;

namespace Hutzper.Library.Security.Aes;

/// <summary>
/// Aes.Create() を使ったファイル・バイト配列の暗号化／復号サービス
/// </summary>
public class AesEncryptionService : IEncryptionService
{
    private readonly AesEncryptionOptions _options;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="options"></param>
    public AesEncryptionService(AesEncryptionOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// 与えられたバイト配列を AES で暗号化します。(ランダムIVを使用)
    /// </summary>
    public byte[] Encrypt(byte[] data)
    {
        using var aes = System.Security.Cryptography.Aes.Create();
        aes.Key = _options.Key;
        aes.GenerateIV(); // ランダムIV

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();

        // 先頭にIVを書き込む
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
            cs.Write(data, 0, data.Length);
        }

        return ms.ToArray();
    }

    /// <summary>
    /// 与えられた暗号化済みバイト配列を AES で復号します。
    /// </summary>
    /// <param name="encryptedData">復号対象の暗号化バイト配列</param>
    /// <returns>復号後の元データバイト配列</returns>
    public byte[] Decrypt(byte[] encryptedData)
    {
        using var aes = System.Security.Cryptography.Aes.Create();
        aes.Key = _options.Key;

        byte[] iv = new byte[16];
        Array.Copy(encryptedData, 0, iv, 0, iv.Length);
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        using var input = new MemoryStream(encryptedData, iv.Length, encryptedData.Length - iv.Length);
        using var cs = new CryptoStream(input, decryptor, CryptoStreamMode.Read);
        using var output = new MemoryStream();

        cs.CopyTo(output);
        return output.ToArray();
    }

    /// <summary>
    /// 指定したファイルを読み込み、AES で暗号化して別ファイルに書き出します。
    /// </summary>
    /// <param name="inputPath">元ファイルのパス</param>
    /// <param name="outputPath">暗号化ファイルの出力先パス</param>
    public void EncryptFile(string inputPath, string outputPath)
    {
        File.WriteAllBytes(outputPath, this.Encrypt(File.ReadAllBytes(inputPath)));
    }

    /// <summary>
    /// 指定した暗号化ファイルを読み込み、AES で復号して別ファイルに書き出します。
    /// </summary>
    /// <param name="inputPath">暗号化済みファイルのパス</param>
    /// <param name="outputPath">復号後ファイルの出力先パス</param>
    public void DecryptFile(string inputPath, string outputPath)
    {
        File.WriteAllBytes(outputPath, this.Decrypt(File.ReadAllBytes(inputPath)));
    }
}