using BarcodeList.Models;
using BarcodeList.Services;
using BarcodeList.Tool;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ZXing.Net.Maui;

namespace BarcodeList.ViewModels.Result
{
    /// <summary>
    /// windowsだと、Code39の作成がバグる　→　デバッグはアンドロイド実機で　
    /// </summary>
    public partial class Code39ResultViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private string code39Value = "";

        [ObservableProperty]
        private BarcodeFormat barcodeFormat = BarcodeFormat.Code39;

        [ObservableProperty]
        public string displayValue = "";

        [ObservableProperty]
        private BarcodeFolder? selectedFolder;

        [ObservableProperty]
        private string name = "";

        [ObservableProperty]
        private SavedBarcode? savedBarcode;

        [ObservableProperty]
        private ObservableCollection<BarcodeFolder?> folders = new ObservableCollection<BarcodeFolder?>();

        private readonly DatabaseService _databaseService;
        private readonly FolderService _folderService;

        public Code39ResultViewModel(DatabaseService databaseService, FolderService folderService)
        {
            _databaseService = databaseService;
            _folderService = folderService;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("code39Value", out var code39Value))
            {
                Code39Value = code39Value?.ToString()?.Trim() ?? "";
                DisplayValue = $"*{Code39Value}*";
            }
            
        }
        /// <summary>
        /// 初期化処理。フォルダ一覧を取得してViewModelに設定する
        /// </summary>
        /// <returns></returns>
        internal async Task InitializeAsync()
        {
            try
            {
                // フォルダ一覧を取得してViewModelに設定
                var folderList = await _databaseService.GetFoldersAsync();
                if(folderList == null)
                {
                    Folders = new ObservableCollection<BarcodeFolder?>();
                    return;
                }
                else 
                {
                    Folders = new ObservableCollection<BarcodeFolder?>(folderList);
                }
                Name = Folders.Count > 0 ? Folders[0].Name : "新しいフォルダ";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while initializing: {ex.Message}");
            }

        }

        [RelayCommand]
        private async Task CreateFolder()
        {
            var newFolder = await _folderService.CreateFolderAsync();
            if(newFolder == null)
            {
                await Shell.Current.DisplayAlertAsync("フォルダ作成失敗", "新しいフォルダの作成に失敗しました。", "OK");
                return;
            }
            Folders.Add(newFolder);
            SelectedFolder = newFolder;
        }

        /// <summary>
        /// フォルダ込みの履歴に保存する。★通常履歴にはCrearteで登録する
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task Save() 
        {
            if(SelectedFolder == null) 
            {
                await Shell.Current.DisplayAlertAsync("フォルダ未選択", "保存するフォルダを選択してください。", "OK");
                return;
            }

            SavedBarcode = new SavedBarcode
            {
                BarcodeValue = Code39Value,
                BarcodeType = BarcodeType.Code39.ToString(),
                //FolderId = SelectedFolder?.Id ?? 0,
                CreatedAt = DateTime.Now,
            };

            await _databaseService.SaveBarcodeAsync(SavedBarcode);
            await Shell.Current.DisplayAlertAsync("保存完了", $"バーコードをフォルダ「{SelectedFolder.Name}」に保存しました。", "OK");
        }
    }
}
