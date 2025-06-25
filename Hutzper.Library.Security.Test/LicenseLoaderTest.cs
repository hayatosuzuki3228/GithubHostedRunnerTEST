using Hutzper.Library.Security.License;
using System.Security.Cryptography;
using System.Text.Json;

namespace Hutzper.Library.Security.Test;

public class LicenseLoaderFixture : IDisposable
{
    public string TempDirectory { get; }

    public RSA Rsa { get; }

    public RSAParameters PublicKey { get; }

    public LicenseLoaderFixture()
    {
        this.TempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(this.TempDirectory);

        this.Rsa = RSA.Create(2048);
        this.PublicKey = this.Rsa.ExportParameters(false);
    }

    /// <summary>
    /// Dispose 時にディレクトリ以下をすべて削除
    /// </summary>
    public void Dispose()
    {
        if (Directory.Exists(this.TempDirectory))
        {
            Directory.Delete(this.TempDirectory, recursive: true);
        }
        this.Rsa.Dispose();
    }

    /// <summary>
    /// 指定オブジェクトを JSON 化して一時ファイルに書き込むヘルパーメソッド
    /// </summary>
    /// <param name="licenseObj"></param>
    /// <returns></returns>
    public string WriteTempLicense(LicenseData licenseObj)
    {
        var path = Path.Combine(this.TempDirectory, Guid.NewGuid() + ".sig");
        File.WriteAllText(path, JsonSerializer.Serialize(licenseObj));
        return path;
    }

    /// <summary>
    /// 指定オブジェクトを JSON 化して一時ファイルに書き込むヘルパーメソッド
    /// 異常テスト用
    /// </summary>
    /// <param name="licenseObj"></param>
    /// <returns></returns>
    public string WriteTempErrLicense(object licenseObj)
    {
        var path = Path.Combine(this.TempDirectory, Guid.NewGuid() + ".sig");
        File.WriteAllText(path, JsonSerializer.Serialize(licenseObj));
        return path;
    }
}

public class LicenseLoaderTest : IClassFixture<LicenseLoaderFixture>
{
    private readonly LicenseLoaderFixture _licenseLoaderFixture;

    public LicenseLoaderTest(LicenseLoaderFixture licenseLoaderFixture)
    {
        _licenseLoaderFixture = licenseLoaderFixture;
    }

    [Fact(DisplayName = "LL-01 正常なライセンスファイルからAESサービスが生成されること(正常系)")]
    public void LoadAesFromLicense_ValidLicense_ShouldReturnService()
    {
        LicenseData licenseData = new LicenseData { Key = "00112233445566778899AABBCCDDEEFF", IV = "0102030405060708090A0B0C0D0E0F10" };
        licenseData.Signature = LicenseSigner.CreateSignature(licenseData, _licenseLoaderFixture.Rsa);

        var path = _licenseLoaderFixture.WriteTempLicense(licenseData);

        var service = LicenseLoader.LoadAesFromLicense(path, _licenseLoaderFixture.PublicKey);
        var result = service.Decrypt(service.Encrypt(new byte[] { 1, 2, 3 }));
        Assert.Equal(new byte[] { 1, 2, 3 }, result);
    }

    [Fact(DisplayName = "LL-02 ライセンスファイルがJSON形式でない場合に例外が発生すること(異常系)")]
    public void LoadAesFromLicense_InvalidFormat_ShouldThrowInvalidDataException()
    {
        string path = _licenseLoaderFixture.WriteTempErrLicense("not json");

        Assert.Throws<InvalidDataException>(() => LicenseLoader.LoadAesFromLicense(path, _licenseLoaderFixture.PublicKey));
    }

    [Fact(DisplayName = "LL-03 署名が不正な場合に検証が失敗すること(異常系)")]
    public void LoadAesFromLicense_InvalidSignature_ShouldThrowUnauthorizedAccessException()
    {
        LicenseData licenseData = new LicenseData { Key = "AB", IV = "CD", Signature = "invalid" };
        string path = _licenseLoaderFixture.WriteTempLicense(licenseData);
        Assert.Throws<UnauthorizedAccessException>(() => LicenseLoader.LoadAesFromLicense(path, _licenseLoaderFixture.PublicKey));
    }

    [Fact(DisplayName = "LL-04 ライセンスファイルが存在しない場合に例外が発生すること(異常系)")]
    public void LoadAesFromLicense_WithMissingFile_ShouldThrowFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() => LicenseLoader.LoadAesFromLicense("noexist.sig", _licenseLoaderFixture.PublicKey));
    }

    [Fact(DisplayName = "LL-05 Key または IV が空文字の場合に例外が発生すること(異常系)")]
    public void LoadAesFromLicense_WithEmptyKeyOrIV_ShouldThrowInvalidDataException()
    {
        LicenseData lic = new LicenseData { Key = "", IV = "", Signature = "dummy" };
        string path = _licenseLoaderFixture.WriteTempLicense(lic);
        Assert.Throws<InvalidDataException>(() => LicenseLoader.LoadAesFromLicense(path, _licenseLoaderFixture.PublicKey));
    }

    [Fact(DisplayName = "LL-06 AES鍵の長さが不正な場合に例外が発生すること(異常系)")]
    public void LoadAesFromLicense_WithInvalidKeyLength_ShouldThrowCryptographicException()
    {
        //自分で署名し、自分で検証する用の公開鍵を渡す
        LicenseData payload = new LicenseData { Key = "AABBCC", IV = "0102030405060708090A0B0C0D0E0F10" };
        string signature = LicenseSigner.CreateSignature(payload, _licenseLoaderFixture.Rsa);

        var license = new LicenseData
        {
            Key = payload.Key,
            IV = payload.IV,
            Signature = signature
        };

        var path = _licenseLoaderFixture.WriteTempLicense(license);

        Assert.Throws<CryptographicException>(() =>
        LicenseLoader.LoadAesFromLicense(path, _licenseLoaderFixture.PublicKey));
    }

    [Fact(DisplayName = "LL-07 Key または IV のHex文字列が不正な場合に例外が発生すること(異常系)")]
    public void LoadAesFromLicense_WithInvalidHexInKeyOrIV_ShouldThrowFormatException()
    {
        LicenseData lic = new LicenseData
        {
            Key = "ZZZZ", // Hexとして不正
            IV = "GGGG"   // Hexとして不正
        };

        lic.Signature = LicenseSigner.CreateSignature(lic, _licenseLoaderFixture.Rsa);

        string path = _licenseLoaderFixture.WriteTempLicense(lic);

        Assert.Throws<FormatException>(() => LicenseLoader.LoadAesFromLicense(path, _licenseLoaderFixture.Rsa.ExportParameters(false)));
    }


    [Fact(DisplayName = "LL-08 必須フィールドが欠落している場合に例外が発生すること(異常系)")]
    public void LoadAesFromLicense_WithMissingFields_ShouldThrowInvalidDataException()
    {
        var path = Path.Combine(_licenseLoaderFixture.TempDirectory, Guid.NewGuid() + ".sig");
        //Jsonではなくtextで書き込み
        File.WriteAllText(path, "{}");

        Assert.Throws<InvalidDataException>(() => LicenseLoader.LoadAesFromLicense(path, _licenseLoaderFixture.PublicKey));
    }
}

