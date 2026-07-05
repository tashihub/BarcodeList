using BarcodeList.ViewModels.Create;

namespace BarcodeList.Views.Create;

public partial class Code128CreateView : ContentPage
{
	private readonly Code128CreateViewModel _viewModel;
    public Code128CreateView(Code128CreateViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}