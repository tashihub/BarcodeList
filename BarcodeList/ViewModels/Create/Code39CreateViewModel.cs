using BarcodeList.Models;
using BarcodeList.Services.CreateServices;
using BarcodeList.Tool;
using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZXing.Net.Maui;

public partial class Code39CreateViewModel : ObservableObject
{
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


    private readonly Code39CreateService _code39Service;
    public Code39CreateViewModel(Code39CreateService code39Service)
    {
        _code39Service = code39Service;
    }

    partial void OnBarcodeValueChanged(string value)
    {
        ErrorMessage = _code39Service.Validate(value);
    }

    [RelayCommand]
    private async Task Create()
    {
        ErrorMessage = _code39Service.Validate(BarcodeValue);

        if (HasError || string.IsNullOrWhiteSpace(BarcodeValue))
            return;

        var value = _code39Service.Normalize(BarcodeValue);

        var(isValid, validationError) = _code39Service.IsValid(value);
        if (!isValid)
        {
            ErrorMessage = validationError;
            return;
        }

        try
        {
            //履歴DBに保存
            await _code39Service.SaveBarcodeToHistory(value, folderId: 0);

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