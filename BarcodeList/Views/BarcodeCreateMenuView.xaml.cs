using BarcodeList.ViewModels;

namespace BarcodeList.Views;

public partial class BarcodeCreateMenuView : ContentPage
{
	private readonly BarcodeCreateMenuViewModel _viewModel = new();
    public BarcodeCreateMenuView()
	{
		InitializeComponent();
        BindingContext = _viewModel;
    }
}