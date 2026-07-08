using BarcodeList.Models;
using BarcodeList.Services;
using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ZXing.Net.Maui;

namespace BarcodeList.ViewModels.Details
{
    public partial class FolderDetailViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private BarcodeFolder? folder;

        [ObservableProperty]
        private ObservableCollection<SavedBarcode> barcodes;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("folder", out object folderObj) && folderObj is Models.BarcodeFolder folder)
            {
                Folder = folder;
            }
        }
        private readonly DatabaseService _databaseService;
        private readonly FolderService _folderService;
        public FolderDetailViewModel(DatabaseService databaseService, FolderService folderService)
        {
            _databaseService = databaseService;
            _folderService = folderService;
        }

        public async Task InitializeAsync()
        {
            if (Folder != null)
            {
                var barcodes = await _databaseService.GetBarcodesAsync(Folder.Id);
                Barcodes = new ObservableCollection<SavedBarcode>(barcodes);
            }
        }

        /// <summary>
        /// 選択したバーコードを開くためのコマンド。バーコードの詳細ページに遷移する。
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        [RelayCommand]
        private async Task OpenBarcode(SavedBarcode barcode)
        {
            if (barcode == null)
            {
                return;
            }

            if (barcode.IsGs1)
            {
                // GS1バーコードの場合は、AI内訳を表示できるGS1専用の結果画面に遷移する
                await Shell.Current.GoToAsync(nameof(Gs1128ResultView),
                new Dictionary<string, object>
                {
                    ["Gs1Value"] = barcode.BarcodeValue
                });
                return;
            }

            var barcodeFormat = barcode.BarcodeFormat;
            if (barcodeFormat == BarcodeFormat.QrCode)
            {
                // QRコードの場合は、QRコードの内容を表示するページに遷移する
                await Shell.Current.GoToAsync(nameof(QrResultView),
                new Dictionary<string, object>
                {
                    ["qrValue"] = barcode.BarcodeValue
                });
            }
            else if (barcodeFormat == BarcodeFormat.Code39)
            {
                // Code39コードの場合は、Code39コードの内容を表示するページに遷移する
                await Shell.Current.GoToAsync(nameof(Code39ResultView),
                new Dictionary<string, object>
                {
                    ["code39Value"] = barcode.BarcodeValue,
                });
            }
            else if (barcodeFormat == BarcodeFormat.Code128)
            {
                // Code128コードの場合は、Code128コードの内容を表示するページに遷移する
                await Shell.Current.GoToAsync(nameof(Code128ResultView),
                new Dictionary<string, object>
                {
                    ["code128Value"] = barcode.BarcodeValue
                });
            }
            else if (barcodeFormat == BarcodeFormat.Ean13)
            {
                // EAN-13コードの場合は、EAN-13コードの内容を表示するページに遷移する
                await Shell.Current.GoToAsync(nameof(Ean13ResultView),
                new Dictionary<string, object>
                {
                    ["ean13Value"] = barcode.BarcodeValue
                });
            }
        }

        [RelayCommand]
        private async Task DeleteBarcode(SavedBarcode barcode)
        {
            if (barcode == null)
            {
                return;
            }
            var result = await _databaseService.DeleteHistoryAsync(barcode);
            if (result > 0)
            {
                Barcodes.Remove(barcode);
            }
        }
    }
}
