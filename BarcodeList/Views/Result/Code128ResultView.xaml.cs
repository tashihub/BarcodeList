using BarcodeList.ViewModels.Result;

namespace BarcodeList.Views.Result;

public partial class Code128ResultView : ContentPage
{
	private Code128ResultViewModel ViewModel;
    public Code128ResultView()
	{
		InitializeComponent();
        ViewModel = new Code128ResultViewModel();
        BindingContext = ViewModel;
    }
}