using BarcodeList.Interface;
using BarcodeList.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using ZXing.Net.Maui;

namespace BarcodeList.Services.CreateServices
{
    public class Ean13CreateService : IBarcodeCreateService
    {
        private readonly DatabaseService _databaseService;
        public Ean13CreateService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public string Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "バーコード値を入力してください";

            if (!value.All(char.IsDigit))
                return "EAN13は数字のみ使用できます";

            if (value.Length != 13)
                return "EAN13は13桁で入力してください";

            if (!IsValidEan13CheckDigit(value))
                return "チェックデジットが正しくありません";

            return "";
        }

        public (bool isValid, string error) IsValid(string value)
        {
            var error = Validate(value);
            return (string.IsNullOrEmpty(error), error);
        }

        private static bool IsValidEan13CheckDigit(string value)
        {
            var expected = CalculateEan13CheckDigit(value[..12]);
            var actual = value[12] - '0';

            return expected == actual;
        }

        private static int CalculateEan13CheckDigit(string first12Digits)
        {
            var sum = 0;

            for (var i = 0; i < 12; i++)
            {
                var digit = first12Digits[i] - '0';
                sum += i % 2 == 0 ? digit : digit * 3;
            }

            return (10 - (sum % 10)) % 10;
        }

        public async Task SaveBarcodeToHistory(string barcodeValue, int folderId)
        {
            var savedBarcode = new SavedBarcode
            {
                BarcodeValue = barcodeValue,
                FolderId = folderId,
                BarcodeType = BarcodeFormat.Ean13.ToString(),
                CreatedAt = DateTime.UtcNow,
            };
            await _databaseService.SaveBarcodeAsync(savedBarcode);
        }
    }
}
