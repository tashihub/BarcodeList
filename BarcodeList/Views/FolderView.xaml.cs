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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}