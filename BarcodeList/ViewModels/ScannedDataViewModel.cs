using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using ZXing.Net.Maui;

namespace BarcodeList.ViewModels
{
    public partial class ScannedDataViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private BarcodeResult barcodeResult = new BarcodeResult();
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
        }

        private void CreateBarcode()
        {
            // ここでBarcodeResultを使用してバーコードを作成するロジックを実装

        }
    }
}
