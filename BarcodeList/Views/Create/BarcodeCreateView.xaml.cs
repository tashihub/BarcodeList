using BarcodeList.ViewModels.Create;

namespace BarcodeList.Views.Create;

public partial class BarcodeCreateView : ContentPage
{
    private readonly BarcodeCreateViewModel _viewModel;
    public BarcodeCreateView(BarcodeCreateViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}
