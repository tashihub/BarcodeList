using CommunityToolkit.Mvvm.ComponentModel;

namespace BarcodeList.ViewModels.Result
{
    public partial class QrResultViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private string qrValue;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("qrValue", out var value))
            {
                QrValue = (string)value;
            }
        }
    }
}
