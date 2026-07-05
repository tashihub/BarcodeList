using BarcodeList.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using ZXing.Net.Maui;

namespace BarcodeList.Services.CreateServices
{
    /// <summary>
    /// GS1-128はGTINとロット番号の2値で検証するため、単一値のIBarcodeCreateServiceは実装しない。
    /// </summary>
    public class Gs1128CreateService
    {
        private readonly DatabaseService _databaseService;
        public Gs1128CreateService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public string Validate(string gtin, string lotNo)
        {
            if (string.IsNullOrWhiteSpace(gtin))
                return "GTINを入力してください";

            if (!gtin.All(char.IsDigit))
                return "GTINは数字のみ入力可能です";

            if (gtin.Length != 13 && gtin.Length != 14)
                return "GTINは13桁(JAN)または14桁(GTIN)で入力してください";

            if (string.IsNullOrWhiteSpace(lotNo))
                return "ロット番号を入力してください";

            return "";
        }

        public static string NormalizeGtin(string gtin)
        {
            // JAN(EAN13) → GTIN14
            if (gtin.Length == 13)
            {
                return gtin.PadLeft(14, '0');
            }

            return gtin;
        }

        public static string GenerateGs1Value(string gtin, DateTime expirationDate, string lotNo)
        {
            return $"01{gtin}17{expirationDate:yyMMdd}10{lotNo}";
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
