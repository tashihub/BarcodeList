using BarcodeList.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ZXing.Net.Maui;

namespace BarcodeList.Services
{
    public class FolderService
    {
        private readonly DatabaseService _databaseService;

        public FolderService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<BarcodeFolder?> CreateFolderAsync()
        {
            // フォルダ名を入力するためのプロンプトを表示
            var folderName = await Shell.Current.DisplayPromptAsync(
                "フォルダ作成",
                "フォルダ名を入力してください");

            if (string.IsNullOrWhiteSpace(folderName)) 
            {
                Console.WriteLine("キャンセルされました。");
                return null;
            }

            var folder = new BarcodeFolder
            {
                Name = folderName,
                CreatedAt = DateTime.Now
            };

            await _databaseService.SaveFolderAsync(folder);
            Console.WriteLine($"フォルダが作成されました: {folder.Name}");
            return folder;
        }
        public async Task<ObservableCollection<BarcodeFolder>> LoadFoldersAsync()
        {
            var folderList = await _databaseService.GetFoldersAsync();
            return new ObservableCollection<BarcodeFolder>(folderList);
        }


        public async Task<bool> SaveToFolderAsync(
            string barcodeValue,
            BarcodeFormat barcodeType,
            BarcodeFolder selectedFolder)
        {
            if (selectedFolder == null)
            {
                Console.WriteLine("Selected folder is null.");
                return false;
            }
            var savedBarcode = new SavedBarcode
            {
                BarcodeValue = barcodeValue,
                BarcodeType = barcodeType.ToString(),
                FolderId = selectedFolder.Id,
                CreatedAt = DateTime.Now,
            };
            Console.WriteLine($"Saving barcode: {savedBarcode.BarcodeValue} to folder: {selectedFolder.Name} BarcodeType: {savedBarcode.BarcodeType} CreatedAt: {savedBarcode.CreatedAt}");
            await _databaseService.SaveBarcodeAsync(savedBarcode);
            return true;
        }
    }
}
