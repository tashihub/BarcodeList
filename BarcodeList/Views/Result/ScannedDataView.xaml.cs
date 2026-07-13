using BarcodeList.ViewModels;

namespace BarcodeList.Views;

public partial class ScannedDataView : ContentPage
{
    private readonly ScannedDataViewModel _viewModel;
    public ScannedDataView(ScannedDataViewModel viewModel)
	{
        _viewModel = viewModel;
		InitializeComponent();
        BindingContext = _viewModel;
    }

    private bool _isInitialized = false;
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_isInitialized)
        {
            await _viewModel.InitializeAsync();
            _isInitialized = true;
        }
    }
}