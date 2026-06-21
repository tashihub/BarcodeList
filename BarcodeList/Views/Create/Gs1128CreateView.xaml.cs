using BarcodeList.ViewModels.Create;

namespace BarcodeList.Views.Create;

public partial class Gs1128CreateView : ContentPage
{

	private readonly Gs1128CreateViewModel _viewModel;
    public Gs1128CreateView()
	{
		InitializeComponent();
        _viewModel = new Gs1128CreateViewModel();
        BindingContext = _viewModel;
    }
}