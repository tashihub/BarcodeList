using BarcodeList.Services.CreateServices;
using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BarcodeList.ViewModels.Create
{
    public partial class QrCreateViewModel : ObservableObject
    {
        [ObservableProperty]
        private string qrValue;

        private readonly QrCreateService _qrService;
        public QrCreateViewModel(QrCreateService qrService)
        {
            _qrService = qrService;
        }

        [RelayCommand]
        private async Task Create()
        {
            await _qrService.SaveBarcodeToHistory(QrValue, folderId: 0);

            await Shell.Current.GoToAsync(nameof(QrResultView),
            new Dictionary<string, object>
            {
                ["qrValue"] = QrValue
            });
        }
    }
}
