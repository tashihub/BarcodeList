namespace BarcodeList.Views.Create;

public partial class Code39CreateView : ContentPage
{
	private readonly Code39CreateViewModel _viewModel = new();
    public Code39CreateView()
	{
		InitializeComponent();
        BindingContext = _viewModel;
    }
}