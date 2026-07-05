using BarcodeList.Interface;
using BarcodeList.Models;
using System;
using System.Threading.Tasks;
using ZXing.Net.Maui;

namespace BarcodeList.Services.CreateServices
{
    public class QrCreateService : IBarcodeCreateService
    {
        private readonly DatabaseService _databaseService;
        public QrCreateService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // QRコードは任意の文字列を許容するため検証は行わない(現状の挙動を維持)。
        public string Validate(string value) => "";

        public (bool isValid, string error) IsValid(string value) => (true, "");

        public async Task SaveBarcodeToHistory(string barcodeValue, int folderId)
        {
            var savedBarcode = new SavedBarcode
            {
                BarcodeValue = barcodeValue,
                FolderId = folderId,
                BarcodeType = BarcodeFormat.QrCode.ToString(),
                CreatedAt = DateTime.Now,
            };
            await _databaseService.SaveBarcodeAsync(savedBarcode);
        }
    }
}
