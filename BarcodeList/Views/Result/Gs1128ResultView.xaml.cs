using BarcodeList.ViewModels.Result;

namespace BarcodeList.Views.Result;

public partial class Gs1128ResultView : ContentPage
{
	private readonly Gs1128ResultViewModel _viewModel;
    public Gs1128ResultView()
	{
		InitializeComponent();
        _viewModel = new Gs1128ResultViewModel();
        BindingContext = _viewModel;
    }
}