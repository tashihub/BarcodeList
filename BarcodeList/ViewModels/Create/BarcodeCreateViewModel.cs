using BarcodeList.Models;
using BarcodeList.Resources.Strings;
using BarcodeList.Services;
using BarcodeList.Services.CreateServices;
using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui;
using System.Collections.Generic;
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

    public BarcodeCreateViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
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

        await Shell.Current.GoToAsync(
            nameof(BarcodeResultView),
            new Dictionary<string, object>
            {
                ["Value"] = finalValue,
                ["Format"] = SelectedFormat.Format
            });
    }
}
