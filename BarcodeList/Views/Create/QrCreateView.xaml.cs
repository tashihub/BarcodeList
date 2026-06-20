using BarcodeList.ViewModels.Create;

namespace BarcodeList.Views.Create;

public partial class QrCreateView : ContentPage
{
	private readonly QrCreateViewModel _viewModel;
    public QrCreateView()
	{
		InitializeComponent();
        _viewModel = new QrCreateViewModel();
        BindingContext = _viewModel;
    }
}