using BarcodeList.ViewModels.Result;

namespace BarcodeList.Views.Result;

public partial class Code39ResultView : ContentPage
{
	private Code39ResultViewModel ViewModel;
    public Code39ResultView()
	{
		InitializeComponent();
        ViewModel = new Code39ResultViewModel();
        BindingContext = ViewModel;
    }
}