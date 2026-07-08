using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodeList.Tool
{
    using ZXing.Net.Maui;

    public class Gs1Element
    {
        public string Ai { get; set; } = "";
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
    }

    public class Gs1ParseResult
    {
        public bool IsGs1 { get; set; }
        public string SymbologyIdentifier { get; set; } = "";
        public bool HasGroupSeparator { get; set; }
        public bool IsReliable { get; set; }
        public string RawValue { get; set; } = "";
        public List<Gs1Element> Elements { get; set; } = new();
    }

    public static class Gs1Parser
    {
        private const char GroupSeparator = (char)29;

        public static Gs1ParseResult Parse(BarcodeResult barcodeResult)
        {
            var raw = barcodeResult.Value ?? "";

            var symbologyIdentifier = GetSymbologyIdentifier(barcodeResult);

            var result = ParseRaw(raw);

            result.RawValue = raw;
            result.SymbologyIdentifier = symbologyIdentifier;
            result.IsGs1 = IsGs1Symbology(symbologyIdentifier);

            // ]C1 なら GS1-128 と判断できる
            if (result.IsGs1 && result.Elements.Count > 0)
            {
                result.IsReliable = result.HasGroupSeparator;
            }

            return result;
        }

        public static Gs1ParseResult ParseRaw(string raw)
        {
            var result = new Gs1ParseResult
            {
                RawValue = raw
            };

            if (string.IsNullOrWhiteSpace(raw))
                return result;

            raw = raw.Trim();

            // 可読文字から来た場合の保険
            raw = raw.Replace("(", "").Replace(")", "");

            result.HasGroupSeparator = raw.Contains(GroupSeparator);
            result.IsReliable = result.HasGroupSeparator;

            int index = 0;

            while (index < raw.Length)
            {
                var ai = Gs1AiTable.DetectAi(raw, index);

                if (ai == null)
                {
                    result.IsReliable = false;
                    break;
                }

                index += ai.Length;

                string value;

                if (Gs1AiTable.IsFixedLength(ai))
                {
                    value = ReadFixedValue(raw, ref index, ai);
                }
                else
                {
                    value = result.HasGroupSeparator
                        ? ReadVariableValueByGroupSeparator(raw, ref index)
                        : ReadVariableValueByGuess(raw, ref index);

                    if (!result.HasGroupSeparator)
                        result.IsReliable = false;
                }

                result.Elements.Add(new Gs1Element
                {
                    Ai = ai,
                    Name = Gs1AiTable.GetAiName(ai),
                    Value = value
                });
            }

            return result;
        }

        private static string GetSymbologyIdentifier(BarcodeResult result)
        {
            if (result.Metadata == null)
                return "";

            foreach (var item in result.Metadata)
            {
                var value = item.Value?.ToString();

                if (value == "]C1" || value == "]Q3" || value == "]d2")
                    return value;
            }

            return "";
        }

        private static bool IsGs1Symbology(string value)
        {
            return value is "]C1" or "]Q3" or "]d2";
        }

        private static string ReadFixedValue(string raw, ref int index, string ai)
        {
            var length = Gs1AiTable.GetFixedLength(ai);

            if (raw.Length < index + length)
            {
                var remaining = raw.Substring(index);
                index = raw.Length;
                return remaining;
            }

            var value = raw.Substring(index, length);
            index += length;

            if (index < raw.Length && raw[index] == GroupSeparator)
                index++;

            return value;
        }

        private static string ReadVariableValueByGroupSeparator(string raw, ref int index)
        {
            int start = index;

            while (index < raw.Length && raw[index] != GroupSeparator)
            {
                index++;
            }

            var value = raw.Substring(start, index - start);

            if (index < raw.Length && raw[index] == GroupSeparator)
                index++;

            return value;
        }

        private static string ReadVariableValueByGuess(string raw, ref int index)
        {
            int start = index;

            while (index < raw.Length)
            {
                var nextAi = Gs1AiTable.DetectAi(raw, index);

                if (index > start && nextAi != null)
                {
                    // 次AIの後ろに必要な固定長データが残っている場合だけAIとみなす
                    if (CanReadAiValue(raw, index, nextAi))
                        break;
                }

                index++;
            }

            return raw.Substring(start, index - start);
        }

        private static bool CanReadAiValue(string raw, int aiIndex, string ai)
        {
            var valueStart = aiIndex + ai.Length;
            var fixedLength = Gs1AiTable.GetFixedLength(ai);

            if (fixedLength > 0)
            {
                return raw.Length >= valueStart + fixedLength;
            }

            // 可変長AIを次AIとして推定するのは危険なので、
            // FNC1なしの場合は次AI候補にしない
            return false;
        }
    }
}
