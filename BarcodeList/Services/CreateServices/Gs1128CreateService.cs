using BarcodeList.Models;
using BarcodeList.Tool;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZXing.Net.Maui;

namespace BarcodeList.Services.CreateServices
{
    /// <summary>
    /// GS1-128は複数のAI要素(AIコード+値)の組み合わせで作成するため、単一値のIBarcodeCreateServiceは実装しない。
    /// AIコードの検証・生データ組み立てロジック自体はBarcodeList.Core(Tool.Gs1AiTable/Gs1ValueBuilder)に集約されている。
    /// </summary>
    public class Gs1128CreateService
    {
        private readonly DatabaseService _databaseService;
        public Gs1128CreateService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public string ValidateAiCode(string aiCode)
        {
            return Gs1AiTable.ValidateAiCodeFormat(aiCode);
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
            return Gs1ValueBuilder.Build(elements);
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
