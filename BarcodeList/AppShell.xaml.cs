using BarcodeList.Views;
using BarcodeList.Views.Create;
using BarcodeList.Views.Result;

namespace BarcodeList
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ScannedDataView), typeof(ScannedDataView));
            Routing.RegisterRoute(nameof(QrCreateMenuView), typeof(QrCreateMenuView));
            Routing.RegisterRoute(nameof(QrResultView), typeof(QrResultView));

        }
    }
}
