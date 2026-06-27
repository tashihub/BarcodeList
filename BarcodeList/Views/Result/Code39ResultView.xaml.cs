using BarcodeList.Services;
using BarcodeList.ViewModels.Result;

namespace BarcodeList.Views.Result;

public partial class Code39ResultView : ContentPage
{
    private readonly Code39ResultViewModel _vm;
    public Code39ResultView(DatabaseService databaseService, FolderService folderService)
	{
		InitializeComponent();
        _vm = new Code39ResultViewModel(databaseService, folderService);
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