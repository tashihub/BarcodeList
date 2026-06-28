using BarcodeList.Models;
using BarcodeList.Services;
using BarcodeList.Views.Details;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace BarcodeList.ViewModels
{
    public partial class FolderViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<BarcodeFolder> folders;

        private readonly DatabaseService _databaseService;
        private readonly FolderService _folderService;
        public FolderViewModel(DatabaseService databaseService, FolderService folderService)
        {
            _databaseService = databaseService;
            _folderService = folderService;
        }

        [RelayCommand]
        private async Task OpenFolder(BarcodeFolder folder)
        {
            // フォルダ一覧を取得する処理を実装する
            await Shell.Current.GoToAsync(nameof(FolderDetailView),
                new Dictionary<string, object> { ["folder"] = folder });
        }

        internal async Task InitializeAsync()
        {
            var folders = await _databaseService.GetFoldersAsync();
            Folders = new ObservableCollection<BarcodeFolder>(folders);
        }
    }
}
