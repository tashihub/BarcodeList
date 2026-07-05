namespace BarcodeList.Views.Create;

public partial class Code39CreateView : ContentPage
{
    private readonly Code39CreateViewModel _viewModel;
    public Code39CreateView(Code39CreateViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}