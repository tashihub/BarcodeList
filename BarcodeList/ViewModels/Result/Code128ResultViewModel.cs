using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarcodeList.ViewModels.Result
{
    public partial class Code128ResultViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private string code128Value = "";

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Code128Value", out object? value))
            {
                Code128Value = value?.ToString() ?? "";
            }
        }
    }
}
