using BarcodeList.Tool;
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

            //MetaData内で"]C1"となっているのでGS1-128と判定はできているのでAIコードの認識ができる。
            //var metaData = result.Metadata;
            
            if (IsWebUrl(result.Value))
            {
                //webページだったらアクセスする
                await Launcher.OpenAsync(result.Value);
            }
            else
            {

                var gs1 = Gs1Parser.Parse(result);
                if (gs1 != null && gs1.IsGs1)
                {
                    await Gs1BarcodeOperation(gs1, result);
                }
                else
                {
                    await NormalBarcodeOperation(result);
                }
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

        private async Task NormalBarcodeOperation(BarcodeResult result)
        {
            //通常のバーコードの場合は、読み取ったテキストを表示する。
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

        private async Task Gs1BarcodeOperation(Gs1ParseResult gs1, BarcodeResult result)
        {
            Console.WriteLine($"GS1シンボル識別子: {gs1.SymbologyIdentifier}");
            if (gs1.IsReliable)
            {
                Console.WriteLine("FNC1区切りあり：正確解析");
            }
            else
            {
                Console.WriteLine("FNC1区切りなし：簡易解析");
            }
            ///GS1の場合は、AIコードを認識して表示する。
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.GoToAsync(
                    nameof(ScannedDataView),
                    new Dictionary<string, object>
                    {
                        ["barcodeResult"] = result,
                        ["gs1ParseResult"] = gs1
                    });
            });
        }
    }
}
