using BarcodeList.Models;
using BarcodeList.Services;
using BarcodeList.Tool;
using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZXing.Net.Maui;

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

        private readonly DatabaseService _databaseService;
        public Code128CreateViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        partial void OnCode128ValueChanged(string? oldValue, string newValue)
        {
            Code128Value = newValue;
            if (Common.IsAsciiOnly(newValue) == false)
            {
                ErrorMessage = "Code128では日本語を生成できません。QRコードを使用してください。";
            }
            else
            {
                ErrorMessage = "";
            }
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

                if (Common.IsAsciiOnly(Code128Value) == false)
                {
                    await Shell.Current.DisplayAlertAsync(
                        "エラー",
                        "Code128では日本語を生成できません。QRコードを使用してください。",
                        "OK");
                    return;
                }

                var saveBarcode = new SavedBarcode
                {
                    BarcodeValue = Code128Value,
                    BarcodeType = BarcodeFormat.Code128.ToString(),
                    FolderId = 0,
                    CreatedAt = DateTime.Now,
                };
                await _databaseService.SaveBarcodeAsync(saveBarcode);


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
