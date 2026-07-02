using BarcodeList.Services;
using BarcodeList.ViewModels.Result;

namespace BarcodeList.Views.Result;

public partial class Ean13ResultView : ContentPage
{
	private readonly Ean13ResultViewModel _viewModel;
    public Ean13ResultView(FolderService folderService)
	{
		InitializeComponent();
        _viewModel = new Ean13ResultViewModel(folderService);
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