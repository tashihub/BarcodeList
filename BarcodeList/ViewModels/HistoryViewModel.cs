using BarcodeList.Models;
using BarcodeList.Services;
using BarcodeList.Views;
using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ZXing.Net.Maui;

namespace BarcodeList.ViewModels
{
    public partial class HistoryViewModel : ObservableObject
    {

        [ObservableProperty]
        public ObservableCollection<SavedBarcode> histories;

        private readonly DatabaseService _databaseService;
        private readonly FolderService _folderService;
        public HistoryViewModel(DatabaseService databaseService, FolderService folderService)
        {
            _databaseService = databaseService;
            _folderService = folderService;
        }

        public async Task LoadHistoriesAsync()
        {
            var histories = await _databaseService.GetBarcodesAsync(0);/// 0は履歴のフォルダIDを意味する。履歴はフォルダに属さないため、0を指定している
            Histories = new ObservableCollection<SavedBarcode>(histories);
        }

        /// <summary>
        /// 履歴の1つをタップしたときに呼ばれるコマンド
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task OpenHistory(SavedBarcode barcode)
        {
            if (barcode == null)
            {
                return;
            }
            //一旦GS1のことは無視して、通常のバーコードとして扱う

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
            else if(barcodeFormat == BarcodeFormat.Code128)
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
                    ["Ean13Value"] = barcode.BarcodeValue
                });
            }
        }
    }
}
