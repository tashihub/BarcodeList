using BarcodeList.Services;
using BarcodeList.ViewModels.Details;

namespace BarcodeList.Views.Details;

public partial class FolderDetailView : ContentPage
{
	private FolderDetailViewModel _viewModel;
    public FolderDetailView(DatabaseService databaseService,FolderService folderService)
	{
		InitializeComponent();
		_viewModel = new FolderDetailViewModel(databaseService, folderService);
		BindingContext = _viewModel;
	}

    bool isInitialized = true;
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if(isInitialized)
        {
            await _viewModel.InitializeAsync();
            isInitialized = false;
        }
    }
}