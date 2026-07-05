using BarcodeList.ViewModels;

namespace BarcodeList.Views;

public partial class BarcodeCreateMenuView : ContentPage
{
	private readonly BarcodeCreateMenuViewModel _viewModel;
    public BarcodeCreateMenuView(BarcodeCreateMenuViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}