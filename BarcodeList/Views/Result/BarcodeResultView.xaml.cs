using BarcodeList.ViewModels.Result;

namespace BarcodeList.Views.Result;

public partial class BarcodeResultView : ContentPage
{
    private readonly BarcodeResultViewModel _viewModel;
    public BarcodeResultView(BarcodeResultViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
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
