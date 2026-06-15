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

        private static readonly string[] SupportedAis =
        {
        "01",
        "10",
        "11",
        "15",
        "17",
        "21",
        "30",
        "3100", "3101", "3102", "3103"
    };

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
                var ai = DetectAi(raw, index);

                if (ai == null)
                {
                    result.IsReliable = false;
                    break;
                }

                index += ai.Length;

                string value;

                if (IsFixedLength(ai))
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
                    Name = GetAiName(ai),
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

        private static string? DetectAi(string raw, int index)
        {
            return SupportedAis
                .OrderByDescending(x => x.Length)
                .FirstOrDefault(ai =>
                    raw.Length >= index + ai.Length &&
                    raw.Substring(index, ai.Length) == ai);
        }

        private static bool IsFixedLength(string ai)
        {
            return GetFixedLength(ai) > 0;
        }

        private static int GetFixedLength(string ai)
        {
            return ai switch
            {
                "01" => 14,
                "11" => 6,
                "15" => 6,
                "17" => 6,
                "30" => 8,
                "3100" => 6,
                "3101" => 6,
                "3102" => 6,
                "3103" => 6,
                _ => -1
            };
        }

        private static string ReadFixedValue(string raw, ref int index, string ai)
        {
            var length = GetFixedLength(ai);

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
                var nextAi = DetectAi(raw, index);

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
            var fixedLength = GetFixedLength(ai);

            if (fixedLength > 0)
            {
                return raw.Length >= valueStart + fixedLength;
            }

            // 可変長AIを次AIとして推定するのは危険なので、
            // FNC1なしの場合は次AI候補にしない
            return false;
        }

        private static string GetAiName(string ai)
        {
            return ai switch
            {
                "01" => "GTIN",
                "10" => "ロット番号",
                "11" => "製造日",
                "15" => "賞味期限",
                "17" => "有効期限",
                "21" => "シリアル番号",
                "30" => "数量",
                "3100" => "重量kg 小数0桁",
                "3101" => "重量kg 小数1桁",
                "3102" => "重量kg 小数2桁",
                "3103" => "重量kg 小数3桁",
                _ => "不明"
            };
        }
    }
}
