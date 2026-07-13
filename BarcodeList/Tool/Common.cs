using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodeList.Tool
{
    internal static class Common
    {
        /// <summary>
        /// Checks if the given string contains only ASCII characters (characters with code points from 0 to 127).
        /// </summary>
        /// <param name="value"></param>
        /// <returns>日本語入ったらfalseを返す</returns>
        public static bool IsAsciiOnly(string value)
        {
            return value.All(c => c <= 0x7F);
        }

        /// <summary>
        /// EAN-13/EAN-8/UPC-Aで共通のモジュラス10チェックデジットを計算する。
        /// 右端のデータ桁から重み3,1を交互に掛けて合計し、10の補数を返す。
        /// </summary>
        public static int CalculateMod10CheckDigit(string dataDigits)
        {
            var sum = 0;
            for (var i = 0; i < dataDigits.Length; i++)
            {
                var digit = dataDigits[dataDigits.Length - 1 - i] - '0';
                var weight = i % 2 == 0 ? 3 : 1;
                sum += digit * weight;
            }
            return (10 - (sum % 10)) % 10;
        }

        /// <summary>
        /// 値がhttp(s)のURLかどうかを判定する。
        /// </summary>
        public static bool IsWebUrl(string value)
        {
            return Uri.TryCreate(value, UriKind.Absolute, out var uri)
                   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }

    public enum BarcodeType
    {
        Code128,
        Code39,
        QRCode,
        EAN13,
        EAN8,
        UPC_A,
        UPC_E,
        ITF,
        PDF417,
        DataMatrix,
        Aztec
    }
}
