using BarcodeList.Tool;
using Xunit;

namespace BarcodeList.Tests.Tool;

public class Gs1ValueBuilderTests
{
    private const char GroupSeparator = (char)29;

    [Fact]
    public void Build_InsertsGroupSeparator_AfterVariableLengthAiNotAtEnd()
    {
        var elements = new List<Gs1Element>
        {
            new() { Ai = "10", Value = "LOT1" },          // 可変長・末尾でない → GS必要
            new() { Ai = "01", Value = "12345678901231" } // 固定長・末尾 → GS不要
        };

        var value = Gs1ValueBuilder.Build(elements);

        Assert.Equal("10LOT1" + GroupSeparator + "0112345678901231", value);
    }

    [Fact]
    public void Build_DoesNotAppendTrailingGroupSeparator_WhenVariableAiIsLast()
    {
        var elements = new List<Gs1Element>
        {
            new() { Ai = "01", Value = "12345678901231" },
            new() { Ai = "10", Value = "LOT1" }
        };

        var value = Gs1ValueBuilder.Build(elements);

        Assert.Equal("0112345678901231" + "10LOT1", value);
        Assert.DoesNotContain(GroupSeparator, value);
    }

    [Fact]
    public void Build_DoesNotInsertSeparator_BetweenFixedLengthAis()
    {
        var elements = new List<Gs1Element>
        {
            new() { Ai = "01", Value = "12345678901231" },
            new() { Ai = "17", Value = "251231" }
        };

        var value = Gs1ValueBuilder.Build(elements);

        Assert.Equal("0112345678901231" + "17251231", value);
    }
}
