using Hutzper.Library.Security.License;
using System.Security.Cryptography;

namespace Hutzper.Library.Security.Test;

/// <summary>
/// RSA公開鍵およびライセンス署名共通化用Fixture
/// </summary>
public class LicenseVerifierFixture : IDisposable
{
    public RSA Rsa { get; }
    public RSAParameters PublicKey { get; }

    public LicenseVerifierFixture()
    {
        this.Rsa = RSA.Create(2048);
        this.PublicKey = this.Rsa.ExportParameters(false);
    }

    public void Dispose()
    {
        this.Rsa.Dispose();
    }
}

/// <summary>
/// ライセンス署名単体テスト
/// </summary>
public class LicenseVerifierTest : IClassFixture<LicenseVerifierFixture>
{
    private readonly LicenseVerifierFixture _fixture;

    public LicenseVerifierTest(LicenseVerifierFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "LV-01 正しい署名で検証が成功すること(正常系)")]
    public void VerifyLicenseSignature_ValidLicense_ShouldReturnTrue()
    {
        LicenseData data = new LicenseData { Key = "0011", IV = "2233" };
        var signature = LicenseSigner.CreateSignature(data, _fixture.Rsa);
        var license = new LicenseData { Key = data.Key, IV = data.IV, Signature = signature };

        Assert.True(LicenseVerifier.VerifyLicenseSignature(license, _fixture.PublicKey));
    }

    [Fact(DisplayName = "LV-02 改ざんされたデータで検証が失敗すること(異常系)")]
    public void VerifyLicenseSignature_InvalidLicense_ShouldReturnFalse()
    {
        LicenseData data = new LicenseData { Key = "AA", IV = "BB" };
        var signature = LicenseSigner.CreateSignature(data, _fixture.Rsa);

        // 改ざん
        var license = new LicenseData { Key = data.Key, IV = "CC", Signature = signature };

        Assert.False(LicenseVerifier.VerifyLicenseSignature(license, _fixture.PublicKey));
    }

    [Fact(DisplayName = "LV-03 署名がnullの場合に検証が失敗すること(異常系)")]
    public void VerifyLicenseSignature_WithNullSignature_ShouldReturnFalse()
    {
        var license = new LicenseData { Key = "AA", IV = "BB", Signature = null! };

        Assert.False(LicenseVerifier.VerifyLicenseSignature(license, _fixture.PublicKey));
    }
}
