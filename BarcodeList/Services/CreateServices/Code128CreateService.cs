using BarcodeList.Interface;
using BarcodeList.Models;
using BarcodeList.Tool;
using System;
using System.Threading.Tasks;
using ZXing.Net.Maui;

namespace BarcodeList.Services.CreateServices
{
    public class Code128CreateService : IBarcodeCreateService
    {
        private const string AsciiOnlyError = "Code128では日本語を生成できません。QRコードを使用してください。";

        private readonly DatabaseService _databaseService;
        public Code128CreateService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // 空文字はASCII判定上「日本語を含まない」ため許容し、必須チェックは呼び出し側(Create時)で行う。
        public string Validate(string value)
        {
            return Common.IsAsciiOnly(value) ? "" : AsciiOnlyError;
        }

        public (bool isValid, string error) IsValid(string value)
        {
            var result = Common.IsAsciiOnly(value);
            return (result, result ? "" : AsciiOnlyError);
        }

        public async Task SaveBarcodeToHistory(string barcodeValue, int folderId)
        {
            var savedBarcode = new SavedBarcode
            {
                BarcodeValue = barcodeValue,
                FolderId = folderId,
                BarcodeType = BarcodeFormat.Code128.ToString(),
                CreatedAt = DateTime.Now,
            };
            await _databaseService.SaveBarcodeAsync(savedBarcode);
        }
    }
}
