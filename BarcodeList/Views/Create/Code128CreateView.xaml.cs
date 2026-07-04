using BarcodeList.Services;
using BarcodeList.ViewModels.Create;

namespace BarcodeList.Views.Create;

public partial class Code128CreateView : ContentPage
{
	private readonly Code128CreateViewModel _viewModel;
    public Code128CreateView(DatabaseService databaseService)
	{
		InitializeComponent();
        _viewModel = new Code128CreateViewModel(databaseService);
        BindingContext = _viewModel;
    }
}