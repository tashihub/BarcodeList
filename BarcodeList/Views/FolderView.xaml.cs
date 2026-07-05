using BarcodeList.ViewModels;

namespace BarcodeList.Views;

public partial class FolderView : ContentPage
{
	private readonly FolderViewModel _viewModel;
    public FolderView(FolderViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    bool _isInitialized = false;
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