using BarcodeList.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
