using System.Collections.Generic;
using System.Linq;

namespace BarcodeList.Tool
{
    /// <summary>
    /// GS1のAI(Application Identifier)に関する既知情報。読み取り(Gs1Parser)・作成(Gs1128CreateService)の両方から参照する単一の情報源。
    /// テーブルにないAIコードは「不明なAI」「可変長」として扱えばよく、対応AIを都度増やす必要はない。
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
                _ => "不明なAI"
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
                "01" => "14桁の数字(GTIN)",
                "10" => "20文字以内",
                "11" or "15" or "17" => "YYMMDD形式の6桁の数字",
                "21" => "20文字以内",
                "30" => "8桁の数字",
                "3100" or "3101" or "3102" or "3103" => "6桁の数字",
                _ => ""
            };
        }

        /// <summary>
        /// AIコードごとの入力仕様に沿って値を検証する。未知AIは空文字チェックのみ。
        /// </summary>
        public static string ValidateValue(string ai, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "値を入力してください";

            switch (ai)
            {
                case "01":
                    if (value.Length != 14 || !value.All(char.IsDigit))
                        return "GTIN(AI:01)は14桁の数字で入力してください";
                    break;

                case "11":
                case "15":
                case "17":
                    if (value.Length != 6 || !value.All(char.IsDigit) || !IsValidYyMmDd(value))
                        return $"{GetAiName(ai)}(AI:{ai})はYYMMDD形式の6桁の数字で入力してください";
                    break;

                case "30":
                    if (value.Length != 8 || !value.All(char.IsDigit))
                        return "数量(AI:30)は8桁の数字で入力してください";
                    break;

                case "3100":
                case "3101":
                case "3102":
                case "3103":
                    if (value.Length != 6 || !value.All(char.IsDigit))
                        return $"{GetAiName(ai)}(AI:{ai})は6桁の数字で入力してください";
                    break;

                case "10":
                case "21":
                    if (value.Length > 20)
                        return $"{GetAiName(ai)}(AI:{ai})は20文字以内で入力してください";
                    break;
            }

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
