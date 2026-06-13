using BarcodeList.ViewModels;

namespace BarcodeList.Views;

public partial class BarcodeReaderView : ContentPage
{
    private readonly BarcodeReaderViewModel viewModel;
	public BarcodeReaderView(BarcodeReaderViewModel viewModel)
	{
		InitializeComponent();
		this.viewModel = viewModel;
        BindingContext = viewModel;
	}

    /// <summary>
    /// Event handler for when barcodes are detected by the ZXing.Net.Maui barcode reader.
    /// This method will be called whenever the barcode reader detects one or more barcodes in the camera feed.
    /// You can use this method to process the detected barcodes, such as displaying their information or adding them to a list.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        if (BindingContext is BarcodeReaderViewModel vm)
        {
            vm.BarcodeDetectedCommand.Execute(e);
        }
    }
}