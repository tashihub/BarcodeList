using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ZXing.PDF417.Internal;

namespace BarcodeList.ViewModels.Result;

public partial class Ean13ResultViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private string ean13Value = "";



    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Ean13Value", out var value))
        {
            Ean13Value = value?.ToString() ?? "";
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Ean13Value))
            return;
    }


}