using BarcodeList.Models;
using BarcodeList.Services;
using BarcodeList.Tool;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ZXing.Net.Maui;

namespace BarcodeList.ViewModels.Result
{
    public partial class Code128ResultViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private string code128Value = "";

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Code128Value", out object? value))
            {
                Code128Value = value?.ToString() ?? "";
            }
        }

        [ObservableProperty]
        private BarcodeFormat barcodeFormat = BarcodeFormat.Code128;

        [ObservableProperty]
        public string displayValue = "";

        [ObservableProperty]
        private BarcodeFolder? selectedFolder;

        [ObservableProperty]
        private string name = "";

        [ObservableProperty]
        private SavedBarcode? savedBarcode;

        [ObservableProperty]
        private ObservableCollection<BarcodeFolder> folders = new ObservableCollection<BarcodeFolder>();
        
        private readonly FolderService _folderService;
        private readonly BarcodeResultService _barcodeResultService;
        public Code128ResultViewModel(FolderService folderService, BarcodeResultService barcodeResultService)
        {
            _folderService = folderService;
            _barcodeResultService = barcodeResultService;
        }

        /// <summary>
        /// 初期化処理。フォルダ一覧を取得してViewModelに設定する
        /// </summary>
        /// <returns></returns>
        internal async Task InitializeAsync()
        {
            try
            {
                Folders = await _barcodeResultService.LoadFoldersAsync();
                Name = Folders.Count > 0 ? Folders[0].Name : "";
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
            if (newFolder == null)
            {
                Console.WriteLine("フォルダの作成がキャンセルされました。");
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
            if (SelectedFolder == null)
            {
                await Shell.Current.DisplayAlertAsync("フォルダ未選択", "保存するフォルダを選択してください。", "OK");
                return;
            }

            SavedBarcode = new SavedBarcode
            {
                BarcodeValue = Code128Value,
                BarcodeType = BarcodeFormat.Code128.ToString(),
                FolderId = SelectedFolder?.Id ?? 0,
                CreatedAt = DateTime.Now,
            };
            bool success = await _barcodeResultService.SaveToFolderAsync(Code128Value, BarcodeFormat.Code128, SelectedFolder);
            if (success)
            {
                await Shell.Current.DisplayAlertAsync("保存完了", $"バーコードをフォルダ「{SelectedFolder.Name}」に保存しました。", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("保存失敗", "バーコードの保存に失敗しました。", "OK");
                Console.WriteLine("Failed to save barcode.");
            }
        }
    }
}
