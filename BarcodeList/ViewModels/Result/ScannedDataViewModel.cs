using BarcodeList.Tool;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Net.Maui;

namespace BarcodeList.ViewModels
{
    public partial class ScannedDataViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private BarcodeResult barcodeResult;
        [ObservableProperty]
        private Gs1ParseResult gs1ParseResult = new Gs1ParseResult { IsGs1 = false };
        [ObservableProperty]
        private bool isWebUrl;

        public string BarcodeKindText => Gs1ParseResult?.IsGs1 == true ? "GS1バーコード" : "通常バーコード";
        public Color BarcodeKindColor => Gs1ParseResult?.IsGs1 == true ? Colors.MediumPurple : Colors.DodgerBlue;
        public string Gs1ReliabilityText => "簡易解析:正確な解析ではない場合があります。";

        // --- GS1読み取り検証用の一時的なデバッグ表示。確認後に削除する ---
        public string RawValueDebugText => (Gs1ParseResult?.RawValue ?? "").Replace((char)29, '␟');

        public string MetadataDebugText => BarcodeResult?.Metadata == null || BarcodeResult.Metadata.Count == 0
            ? "(メタデータなし)"
            : string.Join("\n", BarcodeResult.Metadata.Select(kv => $"{kv.Key}: {kv.Value}"));

        partial void OnBarcodeResultChanged(BarcodeResult value)
        {
            OnPropertyChanged(nameof(MetadataDebugText));
        }

        partial void OnGs1ParseResultChanged(Gs1ParseResult value)
        {
            OnPropertyChanged(nameof(RawValueDebugText));
        }
        // --- ここまで ---

        /// <summary>
        /// スキャンしたデータが受け渡される
        /// </summary>
        /// <param name="query"></param>
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("barcodeResult", out var value))
            {
                BarcodeResult = (BarcodeResult)value;
            }
            if (query.TryGetValue("gs1ParseResult", out var gs1Value))
            {
                Gs1ParseResult = (Gs1ParseResult)gs1Value;
            }
            if (query.TryGetValue("isWebUrl", out var isWebUrlValue) && isWebUrlValue is bool isWebUrl)
            {
                IsWebUrl = isWebUrl;
            }
        }
        [RelayCommand]
        private async Task OpenWebUrl()
        {
            if(!IsWebUrl) { return;}
            await Launcher.OpenAsync(BarcodeResult.Value);
        }
    }
}
