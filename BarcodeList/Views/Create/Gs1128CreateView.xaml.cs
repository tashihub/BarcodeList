using BarcodeList.ViewModels.Create;

namespace BarcodeList.Views.Create;

public partial class Gs1128CreateView : ContentPage
{

	private readonly Gs1128CreateViewModel _viewModel;
    public Gs1128CreateView(Gs1128CreateViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}