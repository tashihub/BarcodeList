using BarcodeList.Services;
using BarcodeList.ViewModels.Result;

namespace BarcodeList.Views.Result;

public partial class QrResultView : ContentPage
{
    private readonly QrResultViewModel _viewModel;
    public QrResultView(FolderService folderService)
	{
		InitializeComponent();
        _viewModel = new QrResultViewModel(folderService);
        BindingContext = _viewModel;
    }

    bool _isInitialized = false;
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