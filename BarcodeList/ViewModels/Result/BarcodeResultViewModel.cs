using BarcodeList.Models;
using BarcodeList.Resources.Strings;
using BarcodeList.Services;
using BarcodeList.Services.CreateServices;
using BarcodeList.Tool;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ZXing.Net.Maui;

namespace BarcodeList.ViewModels.Result;

/// <summary>
/// QR/Code39/Code128/EAN13/EAN8/UPC-A/ITF/Codabar/Code93/DataMatrix/PDF417/Aztec共通の結果画面。
/// GS1-128はAI内訳表示が必要なため、Gs1128ResultViewModelを別途使う。
/// </summary>
public partial class BarcodeResultViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private string barcodeValue = "";

    [ObservableProperty]
    private BarcodeFormat format;

    [ObservableProperty]
    private BarcodeFolder? selectedFolder;

    [ObservableProperty]
    private string name = "";

    [ObservableProperty]
    private ObservableCollection<BarcodeFolder> folders = new();

    public string DisplayName => BarcodeFormatCatalog.Find(Format)?.DisplayName ?? Format.ToString();

    public bool IsWebUrl => Common.IsWebUrl(BarcodeValue);

    private readonly FolderService _folderService;

    public BarcodeResultViewModel(FolderService folderService)
    {
        _folderService = folderService;
    }

    partial void OnBarcodeValueChanged(string value)
    {
        OnPropertyChanged(nameof(IsWebUrl));
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Value", out var valueObj))
        {
            BarcodeValue = valueObj?.ToString() ?? "";
        }

        if (query.TryGetValue("Format", out var formatObj) && formatObj is BarcodeFormat formatValue)
        {
            Format = formatValue;
            OnPropertyChanged(nameof(DisplayName));
        }
    }

    [RelayCommand]
    private async Task OpenWebUrl()
    {
        if (!IsWebUrl)
            return;

        await Launcher.OpenAsync(BarcodeValue);
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

    [RelayCommand]
    private async Task Save()
    {
        if (SelectedFolder == null)
        {
            await Shell.Current.DisplayAlertAsync(AppResources.Common_FolderNotSelectedTitle, AppResources.Common_FolderNotSelectedMessage, AppResources.Common_OK);
            return;
        }

        bool success = await _folderService.SaveToFolderAsync(BarcodeValue, Format, SelectedFolder);
        if (success)
        {
            await Shell.Current.DisplayAlertAsync(AppResources.Common_SaveSuccessTitle, string.Format(AppResources.Common_SaveSuccessMessage, SelectedFolder.Name), AppResources.Common_OK);
        }
        else
        {
            await Shell.Current.DisplayAlertAsync(AppResources.Common_SaveFailureTitle, AppResources.Common_SaveFailureMessage, AppResources.Common_OK);
            Console.WriteLine("Failed to save barcode.");
        }
    }
}
