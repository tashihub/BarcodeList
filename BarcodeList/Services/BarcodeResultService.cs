using BarcodeList.Models;
using BarcodeList.Tool;
using System.Collections.ObjectModel;

namespace BarcodeList.Services;

public class BarcodeResultService
{
    private readonly DatabaseService _databaseService;
    private readonly FolderService _folderService;

    public BarcodeResultService(
        DatabaseService databaseService,
        FolderService folderService)
    {
        _databaseService = databaseService;
        _folderService = folderService;
    }

    public async Task<ObservableCollection<BarcodeFolder>> LoadFoldersAsync()
    {
        var folderList = await _databaseService.GetFoldersAsync();
        return new ObservableCollection<BarcodeFolder>(folderList);
    }


    public async Task<bool> SaveToFolderAsync(
        string barcodeValue,
        BarcodeType barcodeType,
        BarcodeFolder selectedFolder)
    {
        if(selectedFolder == null)
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
