using BarcodeList.ViewModels.Create;

namespace BarcodeList.Views.Create;

public partial class Ean13CreateView : ContentPage
{
	private readonly Ean13CreateViewModel _viewModel;
    public Ean13CreateView()
	{
		InitializeComponent();
        _viewModel = new Ean13CreateViewModel();
        BindingContext = _viewModel;
    }
}