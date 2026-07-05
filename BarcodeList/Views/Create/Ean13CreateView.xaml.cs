using BarcodeList.ViewModels.Create;

namespace BarcodeList.Views.Create;

public partial class Ean13CreateView : ContentPage
{
	private readonly Ean13CreateViewModel _viewModel;
    public Ean13CreateView(Ean13CreateViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}