using BarcodeList.ViewModels.Create;

namespace BarcodeList.Views.Create;

public partial class QrCreateMenuView : ContentPage
{
	private readonly QrCreateMenuViewModel _viewModel;
    public QrCreateMenuView()
	{
		InitializeComponent();
        _viewModel = new QrCreateMenuViewModel();
        BindingContext = _viewModel;
    }
}