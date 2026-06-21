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

    [RelayCommand]
    private async Task Create()
    {
        ErrorMessage = Validate();

        if (HasError)
            return;

        // 13桁ならGTIN14へ変換
        var normalizedGtin = NormalizeGtin(Gtin);

        var gs1Value = GenerateGs1Value(normalizedGtin);

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

    private string GenerateGs1Value(string gtin)
    {
        return $"01{gtin}17{ExpirationDate:yyMMdd}10{LotNo}";
    }

    private static string NormalizeGtin(string gtin)
    {
        // JAN(EAN13) → GTIN14
        if (gtin.Length == 13)
        {
            return gtin.PadLeft(14, '0');
        }

        return gtin;
    }

    private string Validate()
    {
        if (string.IsNullOrWhiteSpace(Gtin))
            return "GTINを入力してください";

        if (!Gtin.All(char.IsDigit))
            return "GTINは数字のみ入力可能です";

        if (Gtin.Length != 13 && Gtin.Length != 14)
            return "GTINは13桁(JAN)または14桁(GTIN)で入力してください";

        if (string.IsNullOrWhiteSpace(LotNo))
            return "ロット番号を入力してください";

        return "";
    }
}