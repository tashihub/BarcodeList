using BarcodeList.ViewModels;

namespace BarcodeList.Views;

public partial class HistoryView : ContentPage
{
	private readonly HistoryViewModel _viewModel;
    public HistoryView(HistoryViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
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