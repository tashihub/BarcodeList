using BarcodeList.Services.CreateServices;
using Xunit;
using ZXing.Net.Maui;

namespace BarcodeList.Tests.Services.CreateServices;

public class BarcodeFormatCatalogTests
{
    [Fact]
    public void Find_ReturnsDefinition_ForEveryCatalogFormat()
    {
        foreach (var definition in BarcodeFormatCatalog.All)
        {
            var found = BarcodeFormatCatalog.Find(definition.Format);

            Assert.NotNull(found);
            Assert.Equal(definition.Format, found!.Format);
        }
    }

    [Fact]
    public void Find_ReturnsNull_ForUnregisteredFormat()
    {
        // UpcEはこのアプリでは意図的に非対応
        Assert.Null(BarcodeFormatCatalog.Find(BarcodeFormat.UpcE));
    }

    [Fact]
    public void SupportedFormats_IncludesAllCatalogFormats()
    {
        foreach (var definition in BarcodeFormatCatalog.All)
        {
            Assert.True(BarcodeFormatCatalog.SupportedFormats.HasFlag(definition.Format));
        }
    }

    [Fact]
    public void QrCode_Validate_AlwaysSucceeds()
    {
        var qrCode = BarcodeFormatCatalog.Find(BarcodeFormat.QrCode)!;

        Assert.Equal("", qrCode.Validate("any text including 日本語"));
    }

    [Theory]
    [InlineData("490123456789", true)]  // 12桁の数字
    [InlineData("49012345678", false)]  // 桁数不足
    [InlineData("49012345678A", false)] // 数字以外を含む
    public void Ean13_Validate_RequiresTwelveDigits(string value, bool expectedValid)
    {
        var ean13 = BarcodeFormatCatalog.Find(BarcodeFormat.Ean13)!;

        var error = ean13.Validate(value);

        Assert.Equal(expectedValid, string.IsNullOrEmpty(error));
    }

    [Fact]
    public void Ean13_AppendCheckDigit_AppendsCorrectDigit()
    {
        var ean13 = BarcodeFormatCatalog.Find(BarcodeFormat.Ean13)!;

        var finalValue = ean13.AppendCheckDigit!("400638133393");

        Assert.Equal("4006381333931", finalValue);
    }

    [Theory]
    [InlineData("ABC-123", true)]
    [InlineData("abc-123", false)] // Code39は大文字のみ許可(小文字は不可)
    [InlineData("ABC@123", false)] // 未対応の記号
    public void Code39_Validate_ChecksAllowedCharacterSet(string value, bool expectedValid)
    {
        var code39 = BarcodeFormatCatalog.Find(BarcodeFormat.Code39)!;

        var error = code39.Validate(value);

        Assert.Equal(expectedValid, string.IsNullOrEmpty(error));
    }

    [Theory]
    [InlineData("A123456A", true)]
    [InlineData("A123456Z", false)] // Zはスタート/ストップ文字に使えない
    public void Codabar_Validate_ChecksAllowedCharacterSet(string value, bool expectedValid)
    {
        var codabar = BarcodeFormatCatalog.Find(BarcodeFormat.Codabar)!;

        var error = codabar.Validate(value);

        Assert.Equal(expectedValid, string.IsNullOrEmpty(error));
    }

    [Theory]
    [InlineData("12345678", true)]  // 偶数桁
    [InlineData("1234567", false)]  // 奇数桁
    [InlineData("1234567A", false)] // 数字以外を含む
    public void Itf_Validate_RequiresEvenLengthDigitsOnly(string value, bool expectedValid)
    {
        var itf = BarcodeFormatCatalog.Find(BarcodeFormat.Itf)!;

        var error = itf.Validate(value);

        Assert.Equal(expectedValid, string.IsNullOrEmpty(error));
    }

    [Fact]
    public void Code128_Validate_RejectsNonAsciiCharacters()
    {
        var code128 = BarcodeFormatCatalog.Find(BarcodeFormat.Code128)!;

        Assert.Equal("", code128.Validate("ABC123"));
        Assert.NotEqual("", code128.Validate("日本語"));
    }
}
