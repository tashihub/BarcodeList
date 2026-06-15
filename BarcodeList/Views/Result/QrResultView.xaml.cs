using BarcodeList.ViewModels.Result;

namespace BarcodeList.Views.Result;

public partial class QrResultView : ContentPage
{
	private readonly QrResultViewModel _viewModel = new();
    public QrResultView()
	{
		InitializeComponent();
        BindingContext = _viewModel;
    }
}