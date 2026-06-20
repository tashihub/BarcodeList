using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using ZXing.PDF417.Internal;

namespace BarcodeList.ViewModels.Result
{
    internal partial class Code39ResultViewModel : ObservableObject,IQueryAttributable
    {
        [ObservableProperty]
        private string code39Value = "";

        [ObservableProperty]
        public string displayValue = "";

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("code39Value", out var code39Value))
            {
                Code39Value = code39Value?.ToString() ?? "";
                DisplayValue = $"*{Code39Value}*";
            }
        }
    }
}
