using BarcodeList.Services.CreateServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BarcodeList.ViewModels.Create
{
    public partial class Ean13CreateViewModel : ObservableObject
    {

        [ObservableProperty]
        private string ean13Value;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        private string errorMessage = "";
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        private readonly Ean13CreateService _ean13Service;
        public Ean13CreateViewModel(Ean13CreateService ean13Service)
        {
            _ean13Service = ean13Service;
        }

        [RelayCommand]
        private async Task Create()
        {
            var validationMessage = _ean13Service.Validate(Ean13Value);
            if (!string.IsNullOrEmpty(validationMessage))
            {
                await Shell.Current.DisplayAlertAsync(
                    "エラー",
                    validationMessage,
                    "OK");
                return;
            }

            await _ean13Service.SaveBarcodeToHistory(Ean13Value, folderId: 0);

            await Shell.Current.GoToAsync(
                nameof(Views.Result.Ean13ResultView),
                new Dictionary<string, object>
                {
                    ["Ean13Value"] = Ean13Value
                });
        }
    }
}
