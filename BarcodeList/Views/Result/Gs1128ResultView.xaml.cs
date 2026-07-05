using BarcodeList.ViewModels.Result;

namespace BarcodeList.Views.Result;

public partial class Gs1128ResultView : ContentPage
{
	private readonly Gs1128ResultViewModel _viewModel;
    public Gs1128ResultView(Gs1128ResultViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}