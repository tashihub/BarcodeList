using BarcodeList.Models;
using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Net.Maui;

namespace BarcodeList.Services.CreateServices
{
    public class Code39CreateService : IBarcodeCreateService
    {
        private const string Code39AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ -. $/+%";

        private readonly DatabaseService _databaseService;
        public Code39CreateService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public string Normalize(string value)
        {
            return value.Trim().ToUpperInvariant();
        }

        public (bool isValid, string error) IsValid(string value)
        {
            var result = !string.IsNullOrWhiteSpace(value) && value.All(c => Code39AllowedChars.Contains(c));
            return (result, result ? "" : "Code39では大文字英数字と - . スペース $ / + % のみ使用できます。");
        }


        public string Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "";

            var upper = value.ToUpperInvariant();

            foreach (var c in upper)
            {
                if (!Code39AllowedChars.Contains(c))
                    return $"Code39で使用できない文字があります: {c}";
            }

            return "";
        }

        public async Task SaveBarcodeToHistory(string barcodeValue, int folderId)
        {
            var savedBarcode = new SavedBarcode
            {
                BarcodeValue = barcodeValue,
                FolderId = folderId,
                BarcodeType = BarcodeFormat.Code39.ToString(),
                CreatedAt = DateTime.UtcNow,
            };
            await _databaseService.SaveBarcodeAsync(savedBarcode);
        }


    }
}
