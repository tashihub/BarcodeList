using BarcodeList.Tool;
using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using ZXing.PDF417.Internal;

namespace BarcodeList.ViewModels.Create
{
    public partial class Code128CreateViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = "";

        [ObservableProperty]
        private string code128Value = "";



        [RelayCommand]
        private async Task CreateAsync()
        {
            if (string.IsNullOrWhiteSpace(Code128Value))
            {
                await Shell.Current.DisplayAlertAsync(
                    "エラー",
                    "バーコード値を入力してください",
                    "OK");
                return;
            }

            if(Common.IsAsciiOnly(Code128Value) == false)
            {
                await Shell.Current.DisplayAlertAsync(
                    "エラー",
                    "Code128では日本語を生成できません。QRコードを使用してください。",
                    "OK");
                return;
            }

            await Shell.Current.GoToAsync(
                nameof(Code128ResultView),
                new Dictionary<string, object>
                {
                    ["Code128Value"] = Code128Value
                });
        }
    }
}
