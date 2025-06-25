using System.Security.Cryptography;

namespace Hutzper.Library.Security.Aes;

public class AesEncryptionOptions
{
    public required byte[] Key { get; init; }     // 16, 24, or 32 byte
    public required byte[] IV { get; init; }      // 16 byte固定（AESブロックサイズ）

    /// <summary>
    /// 16 進文字列からインスタンスを生成
    /// </summary>
    public static AesEncryptionOptions FromHex(string hexKey, string hexIV)
    {
        var key = Convert.FromHexString(hexKey);
        var iv = Convert.FromHexString(hexIV);

        if (key.Length != 16 && key.Length != 24 && key.Length != 32)
        {
            throw new CryptographicException("Invalid AES key length. Must be 16, 24, or 32 bytes.");
        }

        if (iv.Length != 16)
        {
            throw new CryptographicException("Invalid AES IV length. Must be exactly 16 bytes.");
        }

        return new AesEncryptionOptions
        {
            Key = key,
            IV = iv
        };
    }

}

