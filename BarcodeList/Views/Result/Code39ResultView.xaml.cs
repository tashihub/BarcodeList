using BarcodeList.ViewModels.Result;

namespace BarcodeList.Views.Result;

public partial class Code39ResultView : ContentPage
{
    private readonly Code39ResultViewModel _vm;
    public Code39ResultView(Code39ResultViewModel viewModel)
	{
		InitializeComponent();
        _vm = viewModel;
        BindingContext = _vm;
    }

    private bool _isInitialized = false;
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // フォルダ一覧を取得してViewModelに設定
        if (!_isInitialized)
        {
            await _vm.InitializeAsync();
            _isInitialized = true;
        }
    }
}