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
    /// バー1本あたりの最小幅(端末非依存ピクセル)。これを下回るとカメラでの読み取りが困難になる。
    /// </summary>
    private const double MinModuleWidth = 1.5;

    /// <summary>
    /// 回転しない場合のバーコードの太さ(厚み)方向のサイズ。
    /// </summary>
    private const double BarcodeThickness = 140;

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

        var matrix = writer.Encode(_viewModel.Gs1Value);
        _drawable.Matrix = matrix;

        // AIコードが増えてデータが長くなるほどバーコードのモジュール数が増え、横幅に収めようとすると
        // バー1本が細くなりすぎてスキャンできなくなる。画面の横幅に収まらない場合は、
        // 縦方向のほうがスペースに余裕があることを利用し、90度回転して縦長に表示することで
        // 最小バー幅を確保する(画面内に全体が収まらないとカメラで一度に読み取れないため、
        // スクロールでは解決できない)。
        var requiredLength = matrix.Width * MinModuleWidth;
        var availableWidth = DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density
            - 2 * (20 + 22); // ページPadding(20)とカードBorderPadding(22)を左右分差し引く

        var rotated = requiredLength > availableWidth;
        _drawable.Rotated = rotated;

        gs1GraphicsView.WidthRequest = rotated ? BarcodeThickness : requiredLength;
        gs1GraphicsView.HeightRequest = rotated ? requiredLength : BarcodeThickness;

        gs1GraphicsView.Invalidate();
    }
}