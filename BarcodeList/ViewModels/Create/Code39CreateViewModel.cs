using BarcodeList.Models;
using BarcodeList.Services;
using BarcodeList.Tool;
using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class Code39CreateViewModel : ObservableObject
{
    private const string AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ -. $/+%";

    [ObservableProperty]
    private string title = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasBarcodeValue))]
    [NotifyPropertyChangedFor(nameof(NormalizedBarcodeValue))]
    private string barcodeValue = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = "";

    public bool HasBarcodeValue => !string.IsNullOrWhiteSpace(BarcodeValue);

    public string NormalizedBarcodeValue => BarcodeValue?.ToUpperInvariant() ?? "";

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);


    private readonly DatabaseService _databaseService;
    public Code39CreateViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    partial void OnBarcodeValueChanged(string value)
    {
        ErrorMessage = Validate(value);
    }

    private static string Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        var upper = value.ToUpperInvariant();

        foreach (var c in upper)
        {
            if (!AllowedChars.Contains(c))
                return $"Code39で使用できない文字があります: {c}";
        }

        return "";
    }

    private string NormalizeCode39(string value)
    {
        return value.Trim().ToUpperInvariant();
    }
    private static bool IsValidCode39(string value)
    {
        return !string.IsNullOrWhiteSpace(value)
               && value.All(c => AllowedChars.Contains(c));
    }

    [RelayCommand]
    private async Task Create()
    {
        ErrorMessage = Validate(BarcodeValue);

        if (HasError || string.IsNullOrWhiteSpace(BarcodeValue))
            return;

        var value = NormalizeCode39(BarcodeValue);
        if (!IsValidCode39(value))
        {
            ErrorMessage = "Code39では大文字英数字と - . スペース $ / + % のみ使用できます。";
            return;
        }
        try
        {
            //履歴DBに保存
            await _databaseService.SaveBarcodeAsync(new SavedBarcode
            {
                BarcodeValue = value,
                BarcodeType = BarcodeType.Code39.ToString(),
                CreatedAt = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving barcode: {ex.Message}");
        }

        try
        {
            // TODO: 結果画面へ遷移
            await Shell.Current.GoToAsync(nameof(Code39ResultView),
                            new Dictionary<string, object>
                            {
                                ["code39Value"] = value
                            });
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"Error navigating to result view: {ex.Message}"); Console.WriteLine(ex.ToString());
        }

    }


}