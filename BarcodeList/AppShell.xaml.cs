using BarcodeList.Views;
using BarcodeList.Views.Create;
using BarcodeList.Views.Details;
using BarcodeList.Views.Result;

namespace BarcodeList
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ScannedDataView), typeof(ScannedDataView));
            Routing.RegisterRoute(nameof(BarcodeCreateView), typeof(BarcodeCreateView));
            Routing.RegisterRoute(nameof(BarcodeResultView), typeof(BarcodeResultView));
            Routing.RegisterRoute(nameof(Gs1128CreateView), typeof(Gs1128CreateView));
            Routing.RegisterRoute(nameof(Gs1128ResultView), typeof(Gs1128ResultView));
            Routing.RegisterRoute(nameof(FolderDetailView), typeof(FolderDetailView));
        }
    }
}
