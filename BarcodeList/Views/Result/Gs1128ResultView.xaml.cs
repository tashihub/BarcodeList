using BarcodeList.Tool;
using BarcodeList.ViewModels.Result;
using ZXing;
using ZXing.Common;

namespace BarcodeList.Views.Result;

public partial class Gs1128ResultView : ContentPage
{
	private readonly Gs1128ResultViewModel _viewModel;
    private readonly BitMatrixDrawable _drawable = new();

    public Gs1128ResultView(Gs1128ResultViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        gs1GraphicsView.Drawable = _drawable;
    }

    private bool _isInitialized = false;
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_isInitialized)
        {
            await _viewModel.InitializeAsync();
            _isInitialized = true;
        }

        RenderBarcode();
    }

    /// <summary>
    /// zxing:BarcodeGeneratorViewはGS1Formatを渡せないため、
    /// BarcodeWriterGenericで直接BitMatrixを生成してGraphicsViewに描画する。
    /// </summary>
    private void RenderBarcode()
    {
        if (string.IsNullOrEmpty(_viewModel.Gs1Value))
            return;

        var writer = new BarcodeWriterGeneric
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                GS1Format = true,
                Margin = 10,
            }
        };

        _drawable.Matrix = writer.Encode(_viewModel.Gs1Value);
        gs1GraphicsView.Invalidate();
    }
}