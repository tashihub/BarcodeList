using BarcodeList.Models;
using BarcodeList.Services;
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
        private bool isDetecting = true;

        private readonly DatabaseService _databaseService;
        private readonly AdFrequencyService _adFrequencyService;
        private readonly InterstitialAdService _interstitialAdService;

        public BarcodeReaderViewModel(
            DatabaseService databaseService,
            AdFrequencyService adFrequencyService,
            InterstitialAdService interstitialAdService)
        {
            _databaseService = databaseService;
            _adFrequencyService = adFrequencyService;
            _interstitialAdService = interstitialAdService;
        }

        [RelayCommand]
        private async Task BarcodeDetected(BarcodeDetectionEventArgs e)
        {
            if (!IsDetecting)
                return;

            var result = e.Results.FirstOrDefault();
            if (result == null)
                return;


            IsDetecting = false;

            // バーコードを3回読み込むごとに1回、インタースティシャル広告を表示する
            var shouldShowAd = _adFrequencyService.ShouldShowAd("barcode_scanned", every: 3);

            //MetaData内で"]C1"となっているのでGS1-128と判定はできているのでAIコードの認識ができる。
            //var metaData = result.Metadata;

            if (Common.IsWebUrl(result.Value))
            {
                //webページだったらアクセスする
                await SaveToHistoryAsync(result, isGs1: false);
                await NormalBarcodeOperation(result, Common.IsWebUrl(result.Value));
            }
            else
            {

                var gs1 = Gs1Parser.Parse(result);
                if (gs1 != null && gs1.IsGs1)
                {
                    await SaveToHistoryAsync(result, isGs1: true);
                    await Gs1BarcodeOperation(gs1, result);
                }
                else
                {
                    await SaveToHistoryAsync(result, isGs1: false);
                    await NormalBarcodeOperation(result, Common.IsWebUrl(result.Value));
                }
            }

            // 結果画面への遷移が完了してから広告を表示する(遷移中に表示すると画面遷移と広告表示が競合してクラッシュするため)
            if (shouldShowAd)
            {
                _interstitialAdService.LoadAndShow();
            }

            await Task.Delay(3000);
            IsDetecting = true;
        }

        /// <summary>
        /// スキャンした結果を履歴(フォルダ未指定)に保存する。
        /// </summary>
        private async Task SaveToHistoryAsync(BarcodeResult result, bool isGs1)
        {
            var savedBarcode = new SavedBarcode
            {
                BarcodeValue = result.Value,
                BarcodeType = result.Format.ToString(),
                FolderId = 0,
                IsScanned = true,
                IsGs1 = isGs1,
                CreatedAt = DateTime.Now,
            };
            await _databaseService.SaveBarcodeAsync(savedBarcode);
        }

        private async Task NormalBarcodeOperation(BarcodeResult result,bool isWebUrl)
        {
            //通常のバーコードの場合は、読み取ったテキストを表示する。
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.GoToAsync(
                    nameof(ScannedDataView),
                    new Dictionary<string, object>
                    {
                        ["barcodeResult"] = result,
                        ["isWebUrl"] = isWebUrl
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
