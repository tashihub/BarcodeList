using BarcodeList.ViewModels;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;

namespace BarcodeList.Views;

public partial class BarcodeReaderView : ContentPage
{
    private readonly BarcodeReaderViewModel viewModel;
	public BarcodeReaderView(BarcodeReaderViewModel viewModel)
	{
		InitializeComponent();
		this.viewModel = viewModel;
        BindingContext = viewModel;

        //GS1の読み込みに対応できない。FNC1に対応できていないため
        cameraBarcodeReaderView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.All,
            AutoRotate = true,
            Multiple = true,
            DelayBetweenAnalyzingFrames = 150,
            InitialDelayBeforeAnalyzingFrames = 300,
            DelayBetweenContinuousScans = 1000,
            CameraResolutionSelector = availableResolutions =>
              availableResolutions
                .OrderBy(resolution => Math.Abs((resolution.Width * resolution.Height) - (1280 * 720)))
                .ThenBy(resolution => Math.Abs(resolution.Width - 1280) + Math.Abs(resolution.Height - 720))
                .First()
        };
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