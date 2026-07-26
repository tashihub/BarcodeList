using BarcodeList.Models;
using BarcodeList.Resources.Strings;
using BarcodeList.Services;
using BarcodeList.Tool;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ZXing.Net.Maui;

namespace BarcodeList.ViewModels
{
    public partial class ScannedDataViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private BarcodeResult barcodeResult;
        [ObservableProperty]
        private Gs1ParseResult gs1ParseResult = new Gs1ParseResult { IsGs1 = false };
        [ObservableProperty]
        private bool isWebUrl;

        [ObservableProperty]
        private BarcodeFolder? selectedFolder;

        [ObservableProperty]
        private string name = "";

        [ObservableProperty]
        private ObservableCollection<BarcodeFolder> folders = new();

        public string BarcodeKindText => Gs1ParseResult?.IsGs1 == true ? AppResources.Scan_KindGs1 : AppResources.Scan_KindNormal;
        public Color BarcodeKindColor => Gs1ParseResult?.IsGs1 == true ? Colors.MediumPurple : Colors.DodgerBlue;
        public string Gs1ReliabilityText => AppResources.Scan_Gs1ReliabilityText;

        private readonly FolderService _folderService;
        private readonly AdFrequencyService _adFrequencyService;
        private readonly InterstitialAdService _interstitialAdService;

        public ScannedDataViewModel(
            FolderService folderService,
            AdFrequencyService adFrequencyService,
            InterstitialAdService interstitialAdService)
        {
            _folderService = folderService;
            _adFrequencyService = adFrequencyService;
            _interstitialAdService = interstitialAdService;
        }

        /// <summary>
        /// スキャンしたデータが受け渡される
        /// </summary>
        /// <param name="query"></param>
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("barcodeResult", out var value))
            {
                BarcodeResult = (BarcodeResult)value;
            }
            if (query.TryGetValue("gs1ParseResult", out var gs1Value))
            {
                Gs1ParseResult = (Gs1ParseResult)gs1Value;
            }
            if (query.TryGetValue("isWebUrl", out var isWebUrlValue) && isWebUrlValue is bool isWebUrl)
            {
                IsWebUrl = isWebUrl;
            }
        }
        [RelayCommand]
        private async Task OpenWebUrl()
        {
            if(!IsWebUrl) { return;}
            await Launcher.OpenAsync(BarcodeResult.Value);
        }

        /// <summary>
        /// 初期化処理。フォルダ一覧を取得してViewModelに設定する
        /// </summary>
        internal async Task InitializeAsync()
        {
            try
            {
                Folders = await _folderService.LoadFoldersAsync();
                Name = Folders.Count > 0 ? Folders[0].Name : "";
            }
            catch (Exception ex)
            {
                AppLogger.LogError("ScannedDataViewModel.InitializeAsync failed", ex);
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

        [RelayCommand]
        private async Task Save()
        {
            if (SelectedFolder == null)
            {
                await Shell.Current.DisplayAlertAsync(AppResources.Common_FolderNotSelectedTitle, AppResources.Common_FolderNotSelectedMessage, AppResources.Common_OK);
                return;
            }

            bool success = await _folderService.SaveToFolderAsync(
                BarcodeResult.Value,
                BarcodeResult.Format,
                SelectedFolder,
                isGs1: Gs1ParseResult?.IsGs1 ?? false);

            if (success)
            {
                await Shell.Current.DisplayAlertAsync(AppResources.Common_SaveSuccessTitle, string.Format(AppResources.Common_SaveSuccessMessage, SelectedFolder.Name), AppResources.Common_OK);

                // バーコードを3回保存するごとに1回、インタースティシャル広告を表示する
                if (_adFrequencyService.ShouldShowAd("barcode_saved", every: 3))
                {
                    _interstitialAdService.LoadAndShow();
                }
            }
            else
            {
                await Shell.Current.DisplayAlertAsync(AppResources.Common_SaveFailureTitle, AppResources.Common_SaveFailureMessage, AppResources.Common_OK);
                AppLogger.LogWarning("ScannedDataViewModel.Save: SaveToFolderAsync returned false");
            }
        }
    }
}
