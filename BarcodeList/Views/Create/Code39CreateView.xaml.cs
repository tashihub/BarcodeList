using BarcodeList.Services;

namespace BarcodeList.Views.Create;

public partial class Code39CreateView : ContentPage
{
    private readonly Code39CreateViewModel _viewModel;
    public Code39CreateView(DatabaseService databaseService)
	{
		InitializeComponent();
        _viewModel = new Code39CreateViewModel(databaseService);
        BindingContext = _viewModel;
    }
}