namespace Hutzper.Library.Security.Aes;

/// <summary>
/// 暗号化／復号サービスのインターフェース
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// バイト配列を暗号化します。
    /// </summary>
    /// <param name="data">暗号化対象の平文バイト配列</param>
    /// <returns>暗号化後のバイト配列</returns>
    byte[] Encrypt(byte[] data);

    /// <summary>
    /// バイト配列を復号します。
    /// </summary>
    /// <param name="encryptedData">復号対象の暗号化バイト配列</param>
    /// <returns>復号後のバイト配列</returns>
    byte[] Decrypt(byte[] encryptedData);

    /// <summary>
    /// ファイルを読み込み、暗号化して別ファイルへ出力します。
    /// </summary>
    /// <param name="inputPath">平文ファイルのパス</param>
    /// <param name="outputPath">暗号化ファイルの出力先パス</param>
    void EncryptFile(string inputPath, string outputPath);
    /// <summary>
    /// 暗号化ファイルを読み込み、復号して別ファイルへ出力します。
    /// </summary>
    /// <param name="inputPath">暗号化ファイルのパス</param>
    /// <param name="outputPath">復号後ファイルの出力先パス</param>
    void DecryptFile(string inputPath, string outputPath);
}
