using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZXing.PDF417.Internal;

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

    [RelayCommand]
    private async Task Create()
    {
        ErrorMessage = Validate(BarcodeValue);

        if (HasError || string.IsNullOrWhiteSpace(BarcodeValue))
            return;

        // TODO: 結果画面へ遷移
        await Shell.Current.GoToAsync(nameof(Code39ResultView),
                        new Dictionary<string, object>
                        {
                            ["code39Value"] = BarcodeValue
                        });
    }
}