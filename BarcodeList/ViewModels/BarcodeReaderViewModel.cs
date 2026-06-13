using BarcodeList.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Net.Maui;

namespace BarcodeList.ViewModels
{
    public partial class BarcodeReaderViewModel : ObservableObject
    {

        [ObservableProperty]
        private string scannedText = "読み取り待ち";


        [ObservableProperty]
        private bool isDetecting = true;
        [RelayCommand]
        private async Task BarcodeDetected(BarcodeDetectionEventArgs e)
        {
            if (!IsDetecting)
                return;

            var result = e.Results.FirstOrDefault();
            if (result == null)
                return;


            IsDetecting = false;

            if (IsWebUrl(result.Value))
            {
                //webページだったらアクセスする
                await Launcher.OpenAsync(result.Value);
            }
            else
            {
                ///テキストだったら、読み取ったテキストを渡して、別のページに遷移する
                /////メインスレッドで遷移する必要があるため、MainThread.InvokeOnMainThreadAsyncを使用する
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.GoToAsync(
                        nameof(ScannedDataView),
                        new Dictionary<string, object>
                        {
                            ["barcodeResult"] = result
                        });
                });
            }
            await Task.Delay(1000);
            IsDetecting = true;
        }

        /// <summary>
        /// 読み込んだテキストがURLかどうかを判定するヘルパーメソッド。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsWebUrl(string value)
        {
            return Uri.TryCreate(value, UriKind.Absolute, out var uri)
                   && (uri.Scheme == Uri.UriSchemeHttp
                       || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
