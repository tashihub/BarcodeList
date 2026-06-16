using BarcodeList.ViewModels;

namespace BarcodeList.Views;

public partial class ScannedDataView : ContentPage
{
    private readonly ScannedDataViewModel _viewModel;
    public ScannedDataView(ScannedDataViewModel viewModel)
	{
        _viewModel = viewModel;
		InitializeComponent();
        BindingContext = _viewModel;
    }
}