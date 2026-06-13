using BarcodeList.Views;
using Microsoft.Extensions.DependencyInjection;

namespace BarcodeList
{
    public partial class App : Application
    {
        private readonly BarcodeReaderView _readerView;
        public App(BarcodeReaderView readerView)
        {
            InitializeComponent();
            _readerView = readerView;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}