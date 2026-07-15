using BarcodeList.Tool;
using Xunit;

namespace BarcodeList.Tests.Tool;

public class CommonTests
{
    [Theory]
    [InlineData("400638133393", 1)] // 実在するEAN-13(4006381333931)のデータ桁での検証
    [InlineData("000000000000", 0)]
    public void CalculateMod10CheckDigit_ReturnsExpectedDigit(string dataDigits, int expected)
    {
        var result = Common.CalculateMod10CheckDigit(dataDigits);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("ABC123")]
    [InlineData("")]
    [InlineData("!@#$%^&*()")]
    public void IsAsciiOnly_ReturnsTrue_ForAsciiOnlyInput(string value)
    {
        Assert.True(Common.IsAsciiOnly(value));
    }

    [Theory]
    [InlineData("日本語")]
    [InlineData("ABC日本語")]
    public void IsAsciiOnly_ReturnsFalse_ForNonAsciiInput(string value)
    {
        Assert.False(Common.IsAsciiOnly(value));
    }

    [Theory]
    [InlineData("https://example.com", true)]
    [InlineData("http://example.com/path?query=1", true)]
    [InlineData("ftp://example.com", false)]
    [InlineData("not a url", false)]
    [InlineData("", false)]
    public void IsWebUrl_DetectsHttpAndHttpsOnly(string value, bool expected)
    {
        Assert.Equal(expected, Common.IsWebUrl(value));
    }
}
