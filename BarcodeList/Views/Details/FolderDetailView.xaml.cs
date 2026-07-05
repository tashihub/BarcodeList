using BarcodeList.ViewModels.Details;

namespace BarcodeList.Views.Details;

public partial class FolderDetailView : ContentPage
{
	private readonly FolderDetailViewModel _viewModel;
    public FolderDetailView(FolderDetailViewModel viewModel)
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