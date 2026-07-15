using BarcodeList.Tool;
using Xunit;

namespace BarcodeList.Tests.Tool;

public class Gs1ParserTests
{
    private const char GroupSeparator = (char)29;

    [Fact]
    public void ParseRaw_ParsesFixedThenVariableThenFixedAi_WithGroupSeparator()
    {
        // AI01(固定14桁) + AI10(可変, GS区切り) + AI17(固定6桁)
        var raw = "01" + "12345678901231" + "10" + "LOT123" + GroupSeparator + "17" + "251231";

        var result = Gs1Parser.ParseRaw(raw);

        Assert.True(result.HasGroupSeparator);
        Assert.True(result.IsReliable);
        Assert.Equal(3, result.Elements.Count);

        Assert.Equal("01", result.Elements[0].Ai);
        Assert.Equal("12345678901231", result.Elements[0].Value);

        Assert.Equal("10", result.Elements[1].Ai);
        Assert.Equal("LOT123", result.Elements[1].Value);

        Assert.Equal("17", result.Elements[2].Ai);
        Assert.Equal("251231", result.Elements[2].Value);
    }

    [Fact]
    public void ParseRaw_VariableAiAsLastElement_DoesNotRequireTrailingGroupSeparator()
    {
        var raw = "01" + "12345678901231" + "10" + "LOT1";

        var result = Gs1Parser.ParseRaw(raw);

        Assert.Equal(2, result.Elements.Count);
        Assert.Equal("LOT1", result.Elements[1].Value);
    }

    [Fact]
    public void ParseRaw_ReturnsEmptyResult_ForEmptyInput()
    {
        var result = Gs1Parser.ParseRaw("");

        Assert.Empty(result.Elements);
    }

    [Fact]
    public void ParseRaw_MarksUnreliable_WhenUnknownAiEncountered()
    {
        var raw = "9999" + "somevalue";

        var result = Gs1Parser.ParseRaw(raw);

        Assert.False(result.IsReliable);
        Assert.Empty(result.Elements);
    }
}
