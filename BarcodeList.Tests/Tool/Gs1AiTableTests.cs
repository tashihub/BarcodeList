using BarcodeList.Tool;
using Xunit;

namespace BarcodeList.Tests.Tool;

public class Gs1AiTableTests
{
    [Theory]
    [InlineData("01", 14)]
    [InlineData("11", 6)]
    [InlineData("15", 6)]
    [InlineData("17", 6)]
    [InlineData("30", 8)]
    [InlineData("3100", 6)]
    [InlineData("3103", 6)]
    public void GetFixedLength_ReturnsExpectedLength_ForFixedLengthAis(string ai, int expectedLength)
    {
        Assert.Equal(expectedLength, Gs1AiTable.GetFixedLength(ai));
        Assert.True(Gs1AiTable.IsFixedLength(ai));
    }

    [Theory]
    [InlineData("10")]
    [InlineData("21")]
    [InlineData("9999")]
    public void GetFixedLength_ReturnsMinusOne_ForVariableOrUnknownAis(string ai)
    {
        Assert.Equal(-1, Gs1AiTable.GetFixedLength(ai));
        Assert.False(Gs1AiTable.IsFixedLength(ai));
    }

    [Fact]
    public void GetAiName_ReturnsGtin_ForAi01()
    {
        // GTINは日本語/英語どちらのリソースでも同一表記のため、カルチャに依存せず検証できる
        Assert.Equal("GTIN", Gs1AiTable.GetAiName("01"));
    }

    [Fact]
    public void GetAiName_ReturnsNonEmpty_ForKnownAndUnknownAis()
    {
        Assert.False(string.IsNullOrWhiteSpace(Gs1AiTable.GetAiName("10")));
        Assert.False(string.IsNullOrWhiteSpace(Gs1AiTable.GetAiName("9999")));
    }

    [Fact]
    public void DetectAi_FindsLongestMatchingKnownAi()
    {
        // "3101"は"31"始まりの4桁AIなので、2桁だけの誤検出をしないことを確認する
        var detected = Gs1AiTable.DetectAi("3101000123", 0);

        Assert.Equal("3101", detected);
    }

    [Fact]
    public void DetectAi_ReturnsNull_WhenNoKnownAiMatches()
    {
        Assert.Null(Gs1AiTable.DetectAi("999999", 0));
    }

    [Fact]
    public void ValidateValue_ReturnsError_ForEmptyValue()
    {
        var error = Gs1AiTable.ValidateValue("01", "");

        Assert.False(string.IsNullOrEmpty(error));
    }

    [Theory]
    [InlineData("12345678901234", true)]  // 14桁の数字
    [InlineData("1234567890123", false)]  // 13桁(桁数不足)
    [InlineData("1234567890123A", false)] // 数字以外を含む
    public void ValidateValue_ValidatesGtinLengthAndDigitsOnly(string value, bool expectedValid)
    {
        var error = Gs1AiTable.ValidateValue("01", value);

        Assert.Equal(expectedValid, string.IsNullOrEmpty(error));
    }

    [Theory]
    [InlineData("251231", true)]  // 2025年12月31日
    [InlineData("250100", true)]  // 日不明(00)は許容される
    [InlineData("251331", false)] // 13月は存在しない
    [InlineData("25123", false)]  // 桁数不足
    public void ValidateValue_ValidatesYyMmDdFormat_ForDateAis(string value, bool expectedValid)
    {
        var error = Gs1AiTable.ValidateValue("17", value);

        Assert.Equal(expectedValid, string.IsNullOrEmpty(error));
    }

    [Theory]
    [InlineData("12345678", true)]  // 8桁の数字
    [InlineData("1234567", false)]  // 桁数不足
    public void ValidateValue_ValidatesQuantityAi(string value, bool expectedValid)
    {
        var error = Gs1AiTable.ValidateValue("30", value);

        Assert.Equal(expectedValid, string.IsNullOrEmpty(error));
    }

    [Fact]
    public void ValidateValue_AllowsUpToTwentyCharacters_ForLotNumberAi()
    {
        var exactlyTwenty = new string('A', 20);
        var twentyOne = new string('A', 21);

        Assert.Equal("", Gs1AiTable.ValidateValue("10", exactlyTwenty));
        Assert.NotEqual("", Gs1AiTable.ValidateValue("10", twentyOne));
    }

    [Fact]
    public void ValidateValue_OnlyChecksNonEmpty_ForUnknownAi()
    {
        Assert.Equal("", Gs1AiTable.ValidateValue("9999", "任意の値"));
        Assert.NotEqual("", Gs1AiTable.ValidateValue("9999", ""));
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("1", false)]     // 1桁は短すぎる
    [InlineData("01", true)]
    [InlineData("3100", true)]
    [InlineData("12345", false)] // 5桁は長すぎる
    [InlineData("AB", false)]    // 数字以外
    public void ValidateAiCodeFormat_ChecksTwoToFourDigits(string aiCode, bool expectedValid)
    {
        var error = Gs1AiTable.ValidateAiCodeFormat(aiCode);

        Assert.Equal(expectedValid, string.IsNullOrEmpty(error));
    }
}
