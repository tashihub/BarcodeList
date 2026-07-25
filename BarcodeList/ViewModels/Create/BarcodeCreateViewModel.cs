using BarcodeList.Models;
using BarcodeList.Resources.Strings;
using BarcodeList.Services;
using BarcodeList.Services.CreateServices;
using BarcodeList.Tool;
using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui;
using System;
using System.Collections.Generic;
using ZXing.Common;
using ZXing.Net.Maui;

namespace BarcodeList.ViewModels.Create;

/// <summary>
/// 単一値を入力するだけで作成できる全フォーマット(QR/Code39/Code128/EAN13/EAN8/UPC-A/ITF/Codabar/Code93/DataMatrix/PDF417/Aztec)
/// 共通の作成画面。作成メニューからは必ずフォーマットを指定して遷移してくるので、
/// その場合はフォーマット選択欄を隠し、そのフォーマット専用の画面のように見せる。
/// GS1-128は複数AI要素の入力が必要なため、Gs1128CreateViewModelを別途使う。
/// </summary>
public partial class BarcodeCreateViewModel : ObservableObject, IQueryAttributable
{
    public IReadOnlyList<BarcodeFormatDefinition> Formats { get; } = BarcodeFormatCatalog.All;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Placeholder))]
    [NotifyPropertyChangedFor(nameof(FormatHint))]
    [NotifyPropertyChangedFor(nameof(Keyboard))]
    [NotifyPropertyChangedFor(nameof(EntryMaxLength))]
    [NotifyPropertyChangedFor(nameof(Title))]
    private BarcodeFormatDefinition selectedFormat;

    [ObservableProperty]
    private string inputValue = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = "";

    /// <summary>作成メニューの特定フォーマットから遷移してきた場合はtrue。フォーマット選択欄を隠す。</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowFormatPicker))]
    private bool isFormatFixed;

    public bool ShowFormatPicker => !IsFormatFixed;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public string Placeholder => SelectedFormat?.Placeholder ?? "";
    public string FormatHint => SelectedFormat?.FormatHint ?? "";
    public Keyboard Keyboard => SelectedFormat?.NumericKeyboard == true ? Keyboard.Numeric : Keyboard.Default;
    public int EntryMaxLength => SelectedFormat?.MaxLength > 0 ? SelectedFormat.MaxLength : int.MaxValue;
    public string Title => SelectedFormat != null ? string.Format(AppResources.Create_TitleFormat, SelectedFormat.DisplayName) : AppResources.Create_DefaultTitle;

    private readonly DatabaseService _databaseService;
    private readonly AdFrequencyService _adFrequencyService;
    private readonly InterstitialAdService _interstitialAdService;

    public BarcodeCreateViewModel(
        DatabaseService databaseService,
        AdFrequencyService adFrequencyService,
        InterstitialAdService interstitialAdService)
    {
        _databaseService = databaseService;
        _adFrequencyService = adFrequencyService;
        _interstitialAdService = interstitialAdService;
        selectedFormat = Formats[0];
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Format", out var formatObj) && formatObj is BarcodeFormat format)
        {
            var definition = BarcodeFormatCatalog.Find(format);
            if (definition != null)
            {
                SelectedFormat = definition;
            }
        }

        IsFormatFixed = true;
        InputValue = "";
        ErrorMessage = "";
    }

    partial void OnSelectedFormatChanged(BarcodeFormatDefinition value)
    {
        ErrorMessage = "";
    }

    [RelayCommand]
    private async Task Create()
    {
        if (SelectedFormat == null)
        {
            ErrorMessage = AppResources.Create_SelectFormatError;
            return;
        }

        if (string.IsNullOrWhiteSpace(InputValue))
        {
            ErrorMessage = AppResources.Create_EnterValueError;
            return;
        }

        var normalized = SelectedFormat.Normalize(InputValue);

        var validationError = SelectedFormat.Validate(normalized);
        if (!string.IsNullOrEmpty(validationError))
        {
            ErrorMessage = validationError;
            return;
        }

        var finalValue = SelectedFormat.AppendCheckDigit != null
            ? SelectedFormat.AppendCheckDigit(normalized)
            : normalized;

        // 実際にバーコードとしてエンコードできるかをここで確認してから履歴に保存する。
        // 先に保存してしまうと、結果画面でのエンコード時にクラッシュした場合、
        // 同じ壊れたデータが履歴に残り、履歴から開こうとするたびに再クラッシュしてしまうため。
        try
        {
            var writer = new global::ZXing.BarcodeWriterGeneric
            {
                Format = ToZXingBarcodeFormat(SelectedFormat.Format),
                Options = new EncodingOptions { Margin = 10 }
            };
            writer.Encode(finalValue);
        }
        catch (Exception ex)
        {
            ErrorMessage = AppResources.Create_EncodeError;
            AppLogger.LogError("BarcodeCreateViewModel.Create: encode check failed", ex);
            return;
        }

        var savedBarcode = new SavedBarcode
        {
            BarcodeValue = finalValue,
            BarcodeType = SelectedFormat.Format.ToString(),
            FolderId = 0,
            CreatedAt = DateTime.Now,
        };
        await _databaseService.SaveBarcodeAsync(savedBarcode);

        InputValue = "";
        ErrorMessage = "";

        // バーコードを5回作成するごとに1回、インタースティシャル広告を表示する
        var shouldShowAd = _adFrequencyService.ShouldShowAd("barcode_created", every: 5);

        await Shell.Current.GoToAsync(
            nameof(BarcodeResultView),
            new Dictionary<string, object>
            {
                ["Value"] = finalValue,
                ["Format"] = SelectedFormat.Format
            });

        // 結果画面への遷移が完了してから広告を表示する(遷移中に表示すると画面遷移と広告表示が競合してクラッシュするため)
        if (shouldShowAd)
        {
            _interstitialAdService.LoadAndShow();
        }
    }

    /// <summary>
    /// 結果画面のzxing:BarcodeGeneratorViewが内部で使うのと同じZXing.BarcodeFormatへ変換する。
    /// BarcodeFormatCatalog.Allに登録されている形式のみ対応。
    /// </summary>
    private static global::ZXing.BarcodeFormat ToZXingBarcodeFormat(BarcodeFormat format) => format switch
    {
        BarcodeFormat.QrCode => global::ZXing.BarcodeFormat.QR_CODE,
        BarcodeFormat.Code128 => global::ZXing.BarcodeFormat.CODE_128,
        BarcodeFormat.Code39 => global::ZXing.BarcodeFormat.CODE_39,
        BarcodeFormat.Ean13 => global::ZXing.BarcodeFormat.EAN_13,
        BarcodeFormat.Ean8 => global::ZXing.BarcodeFormat.EAN_8,
        BarcodeFormat.UpcA => global::ZXing.BarcodeFormat.UPC_A,
        BarcodeFormat.Itf => global::ZXing.BarcodeFormat.ITF,
        BarcodeFormat.Codabar => global::ZXing.BarcodeFormat.CODABAR,
        BarcodeFormat.Code93 => global::ZXing.BarcodeFormat.CODE_93,
        BarcodeFormat.DataMatrix => global::ZXing.BarcodeFormat.DATA_MATRIX,
        BarcodeFormat.Pdf417 => global::ZXing.BarcodeFormat.PDF_417,
        BarcodeFormat.Aztec => global::ZXing.BarcodeFormat.AZTEC,
        _ => global::ZXing.BarcodeFormat.CODE_128,
    };
}
