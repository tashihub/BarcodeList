using BarcodeList.Models;
using BarcodeList.Services;
using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

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
            var histories = await _databaseService.GetBarcodesForHistoryAsync();
            Histories = new ObservableCollection<SavedBarcode>(histories);
        }

        [RelayCommand]
        private async Task Delete(SavedBarcode barcode) 
        {
            if(barcode == null)
            {
                return;
            }


            var result = await _databaseService.DeleteHistoryAsync(barcode);
            Histories.Remove(barcode);
        }

        [RelayCommand]
        private async Task DeleteAll()
        {
            var result = await _databaseService.DeleteAllHistoryAsync();
            Histories.Clear();
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
    }
}
