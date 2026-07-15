using BarcodeList.ViewModels.Create;

namespace BarcodeList.Views.Create;

public partial class BarcodeCreateView : ContentPage
{
    private readonly BarcodeCreateViewModel _viewModel;
    public BarcodeCreateView(BarcodeCreateViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    /// <summary>
    /// 入力欄にフォーカス(ソフトキーボード表示)が残ったまま作成コマンドで画面遷移すると、
    /// 端末によってはクラッシュするため、遷移前に明示的にフォーカスを外す。
    /// </summary>
    private void OnCreateClicked(object sender, EventArgs e)
    {
        valueEntry.Unfocus();
    }
}
