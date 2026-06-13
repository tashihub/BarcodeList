using BarcodeList.Views;

namespace BarcodeList
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ScannedDataView), typeof(ScannedDataView));
        }
    }
}
