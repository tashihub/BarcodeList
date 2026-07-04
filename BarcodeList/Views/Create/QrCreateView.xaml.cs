using BarcodeList.Services;
using BarcodeList.ViewModels.Create;

namespace BarcodeList.Views.Create;

public partial class QrCreateView : ContentPage
{
	private readonly QrCreateViewModel _viewModel;
    public QrCreateView(DatabaseService　databaseService)
	{
		InitializeComponent();
        _viewModel = new QrCreateViewModel(databaseService);
        BindingContext = _viewModel;
    }
}