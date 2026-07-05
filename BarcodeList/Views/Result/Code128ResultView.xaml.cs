using BarcodeList.ViewModels.Result;

namespace BarcodeList.Views.Result;

public partial class Code128ResultView : ContentPage
{
	private readonly Code128ResultViewModel ViewModel;
    public Code128ResultView(Code128ResultViewModel viewModel)
	{
		InitializeComponent();
        ViewModel = viewModel;
        BindingContext = ViewModel;
    }

    bool _isInitialized = false;
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // フォルダ一覧を取得してViewModelに設定
        if (!_isInitialized)
        {
            await ViewModel.InitializeAsync();
            _isInitialized = true;
        }
    }
}