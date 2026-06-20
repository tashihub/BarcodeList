using BarcodeList.ViewModels.Create;

namespace BarcodeList.Views.Create;

public partial class Code128CreateView : ContentPage
{
	private readonly Code128CreateViewModel _viewModel;
    public Code128CreateView()
	{
		InitializeComponent();
        _viewModel = new Code128CreateViewModel();
        BindingContext = _viewModel;
    }
}