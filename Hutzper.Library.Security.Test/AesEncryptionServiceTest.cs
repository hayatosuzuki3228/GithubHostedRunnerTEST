using Hutzper.Library.Security.Aes;

namespace Hutzper.Library.Security.Test;

/// <summary>
/// AesEncryptionOptionsは共通かつ状態を持たないので、fixtureで共通化
/// </summary>
public class AesFixture
{
    public AesEncryptionService Service { get; }

    public AesFixture()
    {
        var options = new AesEncryptionOptions
        {
            Key = Convert.FromHexString("00112233445566778899AABBCCDDEEFF"),
            IV = Convert.FromHexString("0102030405060708090A0B0C0D0E0F10")
        };
        this.Service = new AesEncryptionService(options);
    }
}

/// <summary>
/// テストの並列実行を制限
/// </summary>
[CollectionDefinition("AesFileTests", DisableParallelization = true)]
public class AesFileTestsCollection : IClassFixture<AesFixture> { }

/// <summary>
/// AesEncryptionServiceの単体テスト
/// </summary
[Collection("AesFileTests")]
public class AesEncryptionServiceTests
{
    private readonly AesEncryptionService _aesFixtureService;

    public AesEncryptionServiceTests(AesFixture fixture)
    {
        _aesFixtureService = fixture.Service;
    }


    [Fact(DisplayName = "AES-01 小規模データを暗号化・復号できること(正常系)")]
    public void EncryptDecrypt_SmallData_ShouldReturnOriginal()
    {
        var encrypted = _aesFixtureService.Encrypt(new byte[] { 1, 2, 3 });
        var decrypted = _aesFixtureService.Decrypt(encrypted);
        Assert.Equal(new byte[] { 1, 2, 3 }, decrypted);
    }

    [Fact(DisplayName = "AES-02 空データを暗号化・復号できること(正常系)")]
    public void EncryptDecrypt_EmptyData_ShouldReturnEmpty()
    {
        var decrypted = _aesFixtureService.Decrypt(_aesFixtureService.Encrypt(Array.Empty<byte>()));
        Assert.Empty(decrypted);
    }

    [Fact(DisplayName = "AES-03 大容量データを暗号化・復号できること(正常系)")]
    public void EncryptDecrypt_LargeData_ShouldReturnOriginal()
    {
        var data = new byte[5 * 1024 * 1024];
        new Random(0).NextBytes(data);
        var result = _aesFixtureService.Decrypt(_aesFixtureService.Encrypt(data));
        Assert.Equal(data, result);
    }

    [Fact(DisplayName = "AES-04 ファイルの暗号化と復号結果が一致すること(正常系)")]
    public void EncryptFileDecryptFile_FileContents_ShouldMatch()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            Directory.CreateDirectory(tempDir);
            var src = Path.Combine(tempDir, "plain.txt");
            var enc = Path.Combine(tempDir, "enc.bin");
            var dec = Path.Combine(tempDir, "dec.txt");
            File.WriteAllText(src, "Hello");
            _aesFixtureService.EncryptFile(src, enc);
            _aesFixtureService.DecryptFile(enc, dec);
            Assert.Equal(File.ReadAllText(src), File.ReadAllText(dec));
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact(DisplayName = "AES-05 入力ファイルが存在しない場合に例外が発生すること(異常系)")]
    public void EncryptFile_NonexistentInput_ShouldThrowFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() => _aesFixtureService.EncryptFile("noexist.txt", "out.enc"));
    }

    [Fact(DisplayName = "AES-06 出力先に書き込み権限がない場合に例外が発生すること(異常系)")]
    public void EncryptFile_NoWritePermission_ShouldThrowIOException()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        try
        {
            var inputPath = Path.Combine(tempDir, "input.txt");
            var outputPath = Path.Combine(tempDir, "locked_output.enc");

            File.WriteAllText(inputPath, "test");

            // ファイルを開いたままロック
            using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                var exception = Record.Exception(() =>
                    _aesFixtureService.EncryptFile(inputPath, outputPath)
                );

                Assert.NotNull(exception);
                Assert.IsAssignableFrom<IOException>(exception);
            }
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Fact(DisplayName = "AES-07 暗号化後のデータが元データと異なること(正常系)")]
    public void Encrypt_WithPlainData_ShouldProduceDifferentCiphertext()
    {
        var data = new byte[] { 1, 2, 3, 4, 5 };

        var cipher = _aesFixtureService.Encrypt(data);

        Assert.NotEqual(data, cipher);
    }

    [Fact(DisplayName = "AES-08 同じ入力から異なる暗号が生成されること(正常系)")]
    public void Encrypt_SamePlaintext_ShouldProduceDifferentCiphertexts()
    {
        var plain = new byte[] { 10, 20, 30, 40, 50 };

        var cipher1 = _aesFixtureService.Encrypt(plain);
        var cipher2 = _aesFixtureService.Encrypt(plain);

        Assert.NotEqual(cipher1, cipher2);
    }
}

