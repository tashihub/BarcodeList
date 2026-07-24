using BarcodeList.Resources.Strings;
using System.Collections.Generic;
using System.Linq;

namespace BarcodeList.Tool
{
    /// <summary>
    /// 対応AIコード一覧の1件を表す(UI表示・ドキュメント化用)。
    /// </summary>
    public class Gs1AiReferenceItem
    {
        public string Ai { get; init; } = "";
        public string Name { get; init; } = "";
        public string FormatHint { get; init; } = "";
    }

    /// <summary>
    /// GS1のAI(Application Identifier)に関する既知情報。読み取り(Gs1Parser)・作成(Gs1128CreateService)の両方から参照する単一の情報源。
    /// テーブルにないAIコードも作成はできるが、このアプリ自身での読み取り(内訳表示)は保証されない。
    /// 対応AIコードは docs/spec-gs1-and-barcode-expansion.md にも一覧を記載している。
    /// </summary>
    public static class Gs1AiTable
    {
        private static readonly string[] KnownAis =
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

        public static IReadOnlyList<string> SupportedAis => KnownAis;

        public static bool IsKnown(string ai) => KnownAis.Contains(ai);

        /// <summary>
        /// 対応AIコードの一覧(コード・名前・入力形式)。UI表示やドキュメント確認用。
        /// </summary>
        public static IReadOnlyList<Gs1AiReferenceItem> GetReferenceList()
        {
            return KnownAis
                .Select(ai => new Gs1AiReferenceItem
                {
                    Ai = ai,
                    Name = GetAiName(ai),
                    FormatHint = GetFormatHint(ai)
                })
                .ToList();
        }

        /// <summary>
        /// 生データ中の指定位置から、既知AIのうち最長一致するものを検出する。見つからなければnull。
        /// </summary>
        public static string? DetectAi(string raw, int index)
        {
            return KnownAis
                .OrderByDescending(x => x.Length)
                .FirstOrDefault(ai =>
                    raw.Length >= index + ai.Length &&
                    raw.Substring(index, ai.Length) == ai);
        }

        public static bool IsFixedLength(string ai) => GetFixedLength(ai) > 0;

        /// <summary>固定長AIの桁数。可変長・未知AIの場合は-1。</summary>
        public static int GetFixedLength(string ai)
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

        public static string GetAiName(string ai)
        {
            return ai switch
            {
                "01" => AppResources.AiName_01,
                "10" => AppResources.AiName_10,
                "11" => AppResources.AiName_11,
                "15" => AppResources.AiName_15,
                "17" => AppResources.AiName_17,
                "21" => AppResources.AiName_21,
                "30" => AppResources.AiName_30,
                "3100" => AppResources.AiName_3100,
                "3101" => AppResources.AiName_3101,
                "3102" => AppResources.AiName_3102,
                "3103" => AppResources.AiName_3103,
                _ => AppResources.AiName_Unknown
            };
        }

        /// <summary>入力欄で数字キーボードを出すべきかどうか。</summary>
        public static bool IsNumericOnly(string ai)
        {
            return ai switch
            {
                "01" or "11" or "15" or "17" or "30" or "3100" or "3101" or "3102" or "3103" => true,
                _ => false
            };
        }

        /// <summary>入力欄のMaxLengthの目安。未知AIは特に制限しない(0=無制限)。</summary>
        public static int GetMaxLength(string ai)
        {
            var fixedLength = GetFixedLength(ai);
            if (fixedLength > 0)
                return fixedLength;

            return ai switch
            {
                "10" or "21" => 20,
                _ => 0
            };
        }

        /// <summary>入力欄の下に表示する、既知AIの入力形式ヒント文言。未知AIは空文字。</summary>
        public static string GetFormatHint(string ai)
        {
            return ai switch
            {
                "01" => AppResources.AiHint_01,
                "10" => AppResources.AiHint_TextMax20,
                "11" or "15" or "17" => AppResources.AiHint_Date,
                "21" => AppResources.AiHint_TextMax20,
                "30" => AppResources.AiHint_30,
                "3100" or "3101" or "3102" or "3103" => AppResources.AiHint_Weight,
                _ => ""
            };
        }

        /// <summary>
        /// AIコードごとの入力仕様に沿って値を検証する。未知AIは空文字チェックのみ。
        /// </summary>
        public static string ValidateValue(string ai, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return AppResources.AiValidate_EnterValue;

            // GS1-128はCode128としてエンコードするため、ASCII以外の文字(日本語など)が
            // 入るとバーコード生成時にクラッシュする。AIの種類によらず共通で弾く。
            if (!Common.IsAsciiOnly(value))
                return AppResources.AiValidate_NonAscii;

            switch (ai)
            {
                case "01":
                    if (value.Length != 14 || !value.All(char.IsDigit))
                        return AppResources.AiValidate_01;
                    break;

                case "11":
                case "15":
                case "17":
                    if (value.Length != 6 || !value.All(char.IsDigit) || !IsValidYyMmDd(value))
                        return string.Format(AppResources.AiValidate_DateFormat, GetAiName(ai), ai);
                    break;

                case "30":
                    if (value.Length != 8 || !value.All(char.IsDigit))
                        return AppResources.AiValidate_30;
                    break;

                case "3100":
                case "3101":
                case "3102":
                case "3103":
                    if (value.Length != 6 || !value.All(char.IsDigit))
                        return string.Format(AppResources.AiValidate_Weight, GetAiName(ai), ai);
                    break;

                case "10":
                case "21":
                    if (value.Length > 20)
                        return string.Format(AppResources.AiValidate_MaxLength20, GetAiName(ai), ai);
                    break;
            }

            return "";
        }

        /// <summary>
        /// AIコード自体(2〜4桁の数字であるべき)の形式を検証する。値の検証はValidateValueが担当する。
        /// </summary>
        public static string ValidateAiCodeFormat(string aiCode)
        {
            if (string.IsNullOrWhiteSpace(aiCode))
                return AppResources.AiCode_EnterCode;

            if (aiCode.Length < 2 || aiCode.Length > 4 || !aiCode.All(char.IsDigit))
                return AppResources.AiCode_InvalidFormat;

            return "";
        }

        private static bool IsValidYyMmDd(string value)
        {
            if (!int.TryParse(value.Substring(2, 2), out var month) || month < 1 || month > 12)
                return false;

            var dayText = value.Substring(4, 2);
            if (dayText == "00")
                return true; // GS1では日不明を00で表すことが許容されている

            return int.TryParse(dayText, out var day) && day >= 1 && day <= 31;
        }
    }
}
