using BarcodeList.Models;
using BarcodeList.Services;
using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZXing.Net.Maui;

namespace BarcodeList.ViewModels.Create
{
    public partial class QrCreateViewModel : ObservableObject
    {
        [ObservableProperty]
        private string qrValue;

        private readonly DatabaseService _databaseService;
        public QrCreateViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [RelayCommand]
        private async Task Create() 
        {
            var savedBarcode = new SavedBarcode
            {
                BarcodeValue = QrValue,
                BarcodeType = BarcodeFormat.QrCode.ToString(),
                CreatedAt = DateTime.Now,
                FolderId = 0
            };
            await _databaseService.SaveBarcodeAsync(savedBarcode);

            await Shell.Current.GoToAsync(nameof(QrResultView),
            new Dictionary<string, object>
            {
                ["qrValue"] = QrValue
            });
        }
    }
}
