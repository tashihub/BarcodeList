using BarcodeList.ViewModels.Result;

namespace BarcodeList.Views.Result;

public partial class Ean13ResultView : ContentPage
{
	private readonly Ean13ResultViewModel _viewModel;
    public Ean13ResultView(Ean13ResultViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
	}

    private bool _isInitialized = false;
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // フォルダ一覧を取得してViewModelに設定
        if (!_isInitialized)
        {
            await _viewModel.InitializeAsync();
            _isInitialized = true;
        }
    }
}