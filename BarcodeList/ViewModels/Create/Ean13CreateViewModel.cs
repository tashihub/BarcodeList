using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodeList.ViewModels.Create
{
    public partial class Ean13CreateViewModel : ObservableObject
    {

        [ObservableProperty]
        private string ean13Value;


        private static string Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "バーコード値を入力してください";

            if (!value.All(char.IsDigit))
                return "EAN13は数字のみ使用できます";

            if (value.Length != 13)
                return "EAN13は13桁で入力してください";

            if (!IsValidEan13CheckDigit(value))
                return "チェックデジットが正しくありません";

            return "";
        }

        private static bool IsValidEan13CheckDigit(string value)
        {
            var expected = CalculateEan13CheckDigit(value[..12]);
            var actual = value[12] - '0';

            return expected == actual;
        }

        private static int CalculateEan13CheckDigit(string first12Digits)
        {
            var sum = 0;

            for (var i = 0; i < 12; i++)
            {
                var digit = first12Digits[i] - '0';
                sum += i % 2 == 0 ? digit : digit * 3;
            }

            return (10 - (sum % 10)) % 10;
        }

        [RelayCommand]
        private async Task Create()
        {
            var validationMessage = Validate(Ean13Value);
            if (!string.IsNullOrEmpty(validationMessage))
            {
                await Shell.Current.DisplayAlertAsync(
                    "エラー",
                    validationMessage,
                    "OK");
                return;
            }
            await Shell.Current.GoToAsync(
                nameof(Views.Result.Ean13ResultView),
                new Dictionary<string, object>
                {
                    ["Ean13Value"] = Ean13Value
                });
        }
    }
}
