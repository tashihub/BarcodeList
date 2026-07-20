using BarcodeList.ViewModels;

namespace BarcodeList.Views;

public partial class SettingsView : ContentPage
{
    private readonly SettingsViewModel _viewModel;

    public SettingsView(SettingsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}
