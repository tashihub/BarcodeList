using BarcodeList.ViewModels.Result;

namespace BarcodeList.Views.Result;

public partial class Ean13ResultView : ContentPage
{
	private readonly Ean13ResultViewModel _viewModel;
    public Ean13ResultView()
	{
		InitializeComponent();
        _viewModel = new Ean13ResultViewModel();
        BindingContext = _viewModel;
	}
}