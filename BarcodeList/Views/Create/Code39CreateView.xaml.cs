using BarcodeList.Services.CreateServices;

namespace BarcodeList.Views.Create;

public partial class Code39CreateView : ContentPage
{
    private readonly Code39CreateViewModel _viewModel;
    public Code39CreateView(Code39CreateService code39Service)
	{
		InitializeComponent();
        _viewModel = new Code39CreateViewModel(code39Service);
        BindingContext = _viewModel;
    }
}