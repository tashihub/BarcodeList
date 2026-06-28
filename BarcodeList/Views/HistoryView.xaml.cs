using BarcodeList.Services;
using BarcodeList.ViewModels;

namespace BarcodeList.Views;

public partial class HistoryView : ContentPage
{
	private readonly HistoryViewModel _viewModel;
    public HistoryView(DatabaseService databaseService,FolderService folderService)
	{
		InitializeComponent();
		_viewModel = new HistoryViewModel(databaseService, folderService);
		BindingContext = _viewModel;
	}

    bool isFirstLoad = true;
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (isFirstLoad)
        {
            await _viewModel.LoadHistoriesAsync();
            isFirstLoad = false;
        }   
    }
}