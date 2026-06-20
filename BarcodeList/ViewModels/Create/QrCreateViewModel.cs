using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BarcodeList.ViewModels.Create
{
    public partial class QrCreateViewModel : ObservableObject
    {
        [ObservableProperty]
        private string qrValue;

        [RelayCommand]
        private async Task Create() 
        {
            await Shell.Current.GoToAsync(nameof(QrResultView),
            new Dictionary<string, object>
            {
                ["qrValue"] = QrValue
            });
        }
    }
}
