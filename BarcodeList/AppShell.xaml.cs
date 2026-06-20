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
            Routing.RegisterRoute(nameof(QrCreateView), typeof(QrCreateView));
            Routing.RegisterRoute(nameof(QrResultView), typeof(QrResultView));
            Routing.RegisterRoute(nameof(Code39CreateView), typeof(Code39CreateView));
            Routing.RegisterRoute(nameof(Code39ResultView), typeof(Code39ResultView));
            Routing.RegisterRoute(nameof(Code128CreateView), typeof(Code128CreateView));
            Routing.RegisterRoute(nameof(Code128ResultView), typeof(Code128ResultView));
        }
    }
}
