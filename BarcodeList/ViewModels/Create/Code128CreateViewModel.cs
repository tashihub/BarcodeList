using BarcodeList.Services.CreateServices;
using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BarcodeList.ViewModels.Create
{
    public partial class Code128CreateViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        private string errorMessage = "";
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
        [ObservableProperty]
        private string code128Value = "";

        private readonly Code128CreateService _code128Service;
        public Code128CreateViewModel(Code128CreateService code128Service)
        {
            _code128Service = code128Service;
        }

        partial void OnCode128ValueChanged(string? oldValue, string newValue)
        {
            ErrorMessage = _code128Service.Validate(newValue);
        }

        [RelayCommand]
        private async Task CreateAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Code128Value))
                {
                    await Shell.Current.DisplayAlertAsync(
                        "エラー",
                        "バーコード値を入力してください",
                        "OK");
                    return;
                }

                ErrorMessage = _code128Service.Validate(Code128Value);

                if (HasError)
                {
                    await Shell.Current.DisplayAlertAsync(
                        "エラー",
                        ErrorMessage,
                        "OK");
                    return;
                }

                await _code128Service.SaveBarcodeToHistory(Code128Value, folderId: 0);

                await Shell.Current.GoToAsync(
                    nameof(Code128ResultView),
                    new Dictionary<string, object>
                    {
                        ["Code128Value"] = Code128Value
                    });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
