using BarcodeList.ViewModels.Create;

namespace BarcodeList.Views.Create;

public partial class QrCreateView : ContentPage
{
	private readonly QrCreateViewModel _viewModel;
    public QrCreateView(QrCreateViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}