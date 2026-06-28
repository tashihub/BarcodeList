using BarcodeList.Models;
using SQLite;

namespace BarcodeList.Services;

public class DatabaseService 
{
    private SQLiteAsyncConnection _database;

    private const string AllHistoryFolderName = "全履歴";

    private bool _isInitialized = false;
    public async Task InitAsync()
    {
        if (_database != null)
            return;

        var dbPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "barcode.db");

        // デバッグ時にDBを削除して初期化する
        if (File.Exists(dbPath) && _isInitialized)
        {
            File.Delete(dbPath);
            _isInitialized = true;
        }

        _database = new SQLiteAsyncConnection(dbPath);

        await _database.CreateTableAsync<BarcodeFolder>();
        await _database.CreateTableAsync<SavedBarcode>();

        Console.WriteLine($"DB exists: {File.Exists(dbPath)}");
    }


    public async Task<int> SaveBarcodeAsync(SavedBarcode barcode)
    {
        await InitAsync();
        return await _database.InsertAsync(barcode);
    }


    public async Task<List<SavedBarcode>> GetBarcodesAsync()
    {
        await InitAsync();
        return await _database.Table<SavedBarcode>().ToListAsync();
    }

    public async Task<List<SavedBarcode>> GetBarcodesAsync(int folderId)
    {
        await InitAsync();
        return await _database.Table<SavedBarcode>().Where(b => b.FolderId == folderId).ToListAsync();
    }


    public async Task<int> SaveFolderAsync(BarcodeFolder folder)
    {
        await InitAsync();
        return await _database.InsertAsync(folder);
    }

    public async Task<List<BarcodeFolder>> GetFoldersAsync()
    {
        try
        {
            await InitAsync();
            var folders = await _database.Table<BarcodeFolder>().ToListAsync();
            return folders;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return new List<BarcodeFolder>();
    }


}