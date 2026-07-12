using System;
using ZXing.Net.Maui;

namespace BarcodeList.Services.CreateServices
{
    /// <summary>
    /// 単一値を入力するだけで作成できるバーコード1形式分の仕様。
    /// GS1-128は複数AI要素の組み合わせで構造が異なるため、専用画面(Gs1128CreateView)のまま別管理する。
    /// </summary>
    public record BarcodeFormatDefinition(
        BarcodeFormat Format,
        string DisplayName,
        string Placeholder,
        string FormatHint,
        bool NumericKeyboard,
        int MaxLength,
        Func<string, string> Normalize,
        Func<string, string> Validate,
        Func<string, string>? AppendCheckDigit = null);
}
