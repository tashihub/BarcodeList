using BarcodeList.ViewModels.Create;

namespace BarcodeList.Views.Create;

public partial class Gs1128CreateView : ContentPage
{

	private readonly Gs1128CreateViewModel _viewModel;
    public Gs1128CreateView(Gs1128CreateViewModel viewModel)
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
        aiCodeEntry.Unfocus();
        aiValueEntry.Unfocus();
    }
}