using BarcodeList.Models;
using BarcodeList.Services;
using BarcodeList.Tool;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ZXing.Net.Maui;
using ZXing.PDF417.Internal;

namespace BarcodeList.ViewModels.Result;

public partial class Ean13ResultViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private string ean13Value = "";
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Ean13Value", out var value)) //なぜか大文字だと反応しない　→　小文字にする
        {
            Ean13Value = value?.ToString() ?? "";
        }
    }

    [ObservableProperty]
    private BarcodeFormat barcodeFormat = BarcodeFormat.Ean13;

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

    public Ean13ResultViewModel(FolderService folderService)
    {
        _folderService = folderService;
    }

    /// <summary>
    /// 初期化処理。フォルダ一覧を取得してViewModelに設定する
    /// </summary>
    /// <returns></returns>
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
            BarcodeValue = Ean13Value,
            BarcodeType = BarcodeFormat.Ean13.ToString(),
            FolderId = SelectedFolder?.Id ?? 0,
            CreatedAt = DateTime.Now,
        };
        bool success = await _folderService.SaveToFolderAsync(Ean13Value, BarcodeFormat.Ean13, SelectedFolder);
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