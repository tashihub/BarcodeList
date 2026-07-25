using BarcodeList.Models;
using BarcodeList.Services;
using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

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
        private readonly AdFrequencyService _adFrequencyService;
        private readonly InterstitialAdService _interstitialAdService;

        public FolderDetailViewModel(
            DatabaseService databaseService,
            FolderService folderService,
            AdFrequencyService adFrequencyService,
            InterstitialAdService interstitialAdService)
        {
            _databaseService = databaseService;
            _folderService = folderService;
            _adFrequencyService = adFrequencyService;
            _interstitialAdService = interstitialAdService;
        }

        public async Task InitializeAsync()
        {
            if (Folder != null)
            {
                var barcodes = await _databaseService.GetBarcodesAsync(Folder.Id);
                Barcodes = new ObservableCollection<SavedBarcode>(barcodes);
            }

            // フォルダ詳細(フォルダ内のバーコード一覧)を5回開くごとに1回、インタースティシャル広告を表示する
            if (_adFrequencyService.ShouldShowAd("folder_detail_view", every: 5))
            {
                _interstitialAdService.LoadAndShow();
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

            // それ以外は共通の結果画面に遷移する
            await Shell.Current.GoToAsync(nameof(BarcodeResultView),
            new Dictionary<string, object>
            {
                ["Value"] = barcode.BarcodeValue,
                ["Format"] = barcode.BarcodeFormat
            });
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
