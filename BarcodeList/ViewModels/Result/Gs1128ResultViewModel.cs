using BarcodeList.Models;
using BarcodeList.Resources.Strings;
using BarcodeList.Services;
using BarcodeList.Tool;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ZXing.Net.Maui;

namespace BarcodeList.ViewModels.Result;

public partial class Gs1128ResultViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private string gs1Value = "";

    [ObservableProperty]
    private ObservableCollection<Gs1Element> elements = new();

    [ObservableProperty]
    private BarcodeFolder? selectedFolder;

    [ObservableProperty]
    private string name = "";

    [ObservableProperty]
    private ObservableCollection<BarcodeFolder> folders = new();

    private readonly FolderService _folderService;
    private readonly AdFrequencyService _adFrequencyService;
    private readonly InterstitialAdService _interstitialAdService;

    public Gs1128ResultViewModel(
        FolderService folderService,
        AdFrequencyService adFrequencyService,
        InterstitialAdService interstitialAdService)
    {
        _folderService = folderService;
        _adFrequencyService = adFrequencyService;
        _interstitialAdService = interstitialAdService;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Gs1Value", out var value))
        {
            Gs1Value = value?.ToString() ?? "";
            var parsed = Gs1Parser.ParseRaw(Gs1Value);
            Elements = new ObservableCollection<Gs1Element>(parsed.Elements);
        }
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
            AppLogger.LogError("Gs1128ResultViewModel.InitializeAsync failed", ex);
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

        bool success = await _folderService.SaveToFolderAsync(Gs1Value, BarcodeFormat.Code128, SelectedFolder, isGs1: true);
        if (success)
        {
            await Shell.Current.DisplayAlertAsync(AppResources.Common_SaveSuccessTitle, string.Format(AppResources.Common_SaveSuccessMessage, SelectedFolder.Name), AppResources.Common_OK);

            // バーコードを5回保存するごとに1回、インタースティシャル広告を表示する
            if (_adFrequencyService.ShouldShowAd("barcode_saved", every: 5))
            {
                _interstitialAdService.LoadAndShow();
            }
        }
        else
        {
            await Shell.Current.DisplayAlertAsync(AppResources.Common_SaveFailureTitle, AppResources.Common_SaveFailureMessage, AppResources.Common_OK);
            AppLogger.LogWarning("Gs1128ResultViewModel.Save: SaveToFolderAsync returned false");
        }
    }
}
