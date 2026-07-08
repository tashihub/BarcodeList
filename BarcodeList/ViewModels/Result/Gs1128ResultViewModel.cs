using BarcodeList.Models;
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

    public Gs1128ResultViewModel(FolderService folderService)
    {
        _folderService = folderService;
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
            await Shell.Current.DisplayAlertAsync("フォルダ未選択", "保存するフォルダを選択してください。", "OK");
            return;
        }

        bool success = await _folderService.SaveToFolderAsync(Gs1Value, BarcodeFormat.Code128, SelectedFolder, isGs1: true);
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
