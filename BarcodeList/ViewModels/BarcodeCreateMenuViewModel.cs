using BarcodeList.Views.Create;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ZXing.Net.Maui;
namespace BarcodeList.ViewModels
{
    public partial class BarcodeCreateMenuViewModel : ObservableObject
    {
        [ObservableProperty]
        private string barcodeValue = "";

        [ObservableProperty]
        private BarcodeFormat selectedBarcodeFormat = BarcodeFormat.Code128;

        [ObservableProperty]
        public ObservableCollection<BarcodeFormat> barcodeFormats;

        public BarcodeCreateMenuViewModel()
        {
            BarcodeFormats = new(Enum.GetValues<BarcodeFormat>());
        }


        [RelayCommand]
        private async Task OpenQrCreate()
        {
            await Shell.Current.GoToAsync(nameof(QrCreateMenuView));
        }

        [RelayCommand]
        private async Task OpenCode128Create()
        {

        }

        [RelayCommand]
        private async Task OpenCode39CreateCommand()
        {

        }

        [RelayCommand]
        private async Task OpenEan13CreateCommand()
        {

        }

        [RelayCommand]
        private async Task OpenGs1128CreateCommand()
        {

        }

    }
}
