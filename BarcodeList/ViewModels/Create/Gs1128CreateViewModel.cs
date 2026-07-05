using BarcodeList.Services.CreateServices;
using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BarcodeList.ViewModels.Create;

public partial class Gs1128CreateViewModel : ObservableObject
{
    [ObservableProperty]
    private string gtin = "";

    [ObservableProperty]
    private DateTime expirationDate = DateTime.Today;

    [ObservableProperty]
    private string lotNo = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = "";

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    private readonly Gs1128CreateService _gs1128Service;
    public Gs1128CreateViewModel(Gs1128CreateService gs1128Service)
    {
        _gs1128Service = gs1128Service;
    }

    [RelayCommand]
    private async Task Create()
    {
        ErrorMessage = _gs1128Service.Validate(Gtin, LotNo);

        if (HasError)
            return;

        // 13桁ならGTIN14へ変換
        var normalizedGtin = Gs1128CreateService.NormalizeGtin(Gtin);

        var gs1Value = Gs1128CreateService.GenerateGs1Value(normalizedGtin, ExpirationDate, LotNo);

        await Shell.Current.GoToAsync(
            nameof(Gs1128ResultView),
            new Dictionary<string, object>
            {
                ["Gs1Value"] = gs1Value,
                ["Gtin"] = normalizedGtin,
                ["ExpirationDate"] = ExpirationDate,
                ["LotNo"] = LotNo
            });
    }
}