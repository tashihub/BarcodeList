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
    }
}
