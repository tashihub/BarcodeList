using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BarcodeList.ViewModels.Result;

public partial class Gs1128ResultViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private string gs1Value = "";

    [ObservableProperty]
    private string gtin = "";

    [ObservableProperty]
    private DateTime expirationDate = DateTime.Today;

    [ObservableProperty]
    private string lotNo = "";



    public string DisplayValue => $"(01){Gtin}(17){ExpirationDate:yyMMdd}(10){LotNo}";

    public string ExpirationDateText => ExpirationDate.ToString("yyyy/MM/dd");

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Gs1Value", out var gs1Value))
            Gs1Value = gs1Value?.ToString() ?? "";

        if (query.TryGetValue("Gtin", out var gtin))
            Gtin = gtin?.ToString() ?? "";

        if (query.TryGetValue("ExpirationDate", out var expirationDate)
            && expirationDate is DateTime date)
            ExpirationDate = date;

        if (query.TryGetValue("LotNo", out var lotNo))
            LotNo = lotNo?.ToString() ?? "";

        OnPropertyChanged(nameof(DisplayValue));
        OnPropertyChanged(nameof(ExpirationDateText));
    }

}