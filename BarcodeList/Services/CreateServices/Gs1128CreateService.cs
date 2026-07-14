using BarcodeList.Models;
using BarcodeList.Resources.Strings;
using BarcodeList.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.Net.Maui;

namespace BarcodeList.Services.CreateServices
{
    /// <summary>
    /// GS1-128は複数のAI要素(AIコード+値)の組み合わせで作成するため、単一値のIBarcodeCreateServiceは実装しない。
    /// </summary>
    public class Gs1128CreateService
    {
        private const char GroupSeparator = (char)29;

        private readonly DatabaseService _databaseService;
        public Gs1128CreateService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public string ValidateAiCode(string aiCode)
        {
            if (string.IsNullOrWhiteSpace(aiCode))
                return AppResources.AiCode_EnterCode;

            if (aiCode.Length < 2 || aiCode.Length > 4 || !aiCode.All(char.IsDigit))
                return AppResources.AiCode_InvalidFormat;

            return "";
        }

        public string ValidateValue(string aiCode, string value)
        {
            return Gs1AiTable.ValidateValue(aiCode, value);
        }

        /// <summary>
        /// AI要素のリストから、GS1-128としてエンコードすべき生データ文字列を組み立てる。
        /// 可変長AIが末尾以外に来る場合のみ、後ろにGS(区切り文字)を挿入する。
        /// </summary>
        public static string BuildGs1Value(IReadOnlyList<Gs1Element> elements)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                sb.Append(element.Ai).Append(element.Value);

                var isLast = i == elements.Count - 1;
                if (!Gs1AiTable.IsFixedLength(element.Ai) && !isLast)
                {
                    sb.Append(GroupSeparator);
                }
            }

            return sb.ToString();
        }

        public async Task SaveBarcodeToHistory(string barcodeValue, int folderId)
        {
            var savedBarcode = new SavedBarcode
            {
                BarcodeValue = barcodeValue,
                FolderId = folderId,
                BarcodeType = BarcodeFormat.Code128.ToString(),
                IsGs1 = true,
                CreatedAt = DateTime.Now,
            };
            await _databaseService.SaveBarcodeAsync(savedBarcode);
        }
    }
}
