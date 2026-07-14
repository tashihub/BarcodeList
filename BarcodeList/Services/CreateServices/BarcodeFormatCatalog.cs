using BarcodeList.Resources.Strings;
using BarcodeList.Tool;
using ZXing.Net.Maui;


namespace BarcodeList.Services.CreateServices
{
    /// <summary>
    /// 単一値を入力するだけで作成できる全バーコード形式の定義一覧。
    /// フォーマットを追加したいときは、ここに1エントリ追加するだけでよい。
    /// </summary>
    public static class BarcodeFormatCatalog
    {
        private const string Code39AllowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ -. $/+%";
        private const string CodabarAllowedChars = "0123456789-$:./+ABCD";

        public static IReadOnlyList<BarcodeFormatDefinition> All { get; } = new List<BarcodeFormatDefinition>
        {
            new(
                Format: BarcodeFormat.QrCode,
                DisplayName: AppResources.Format_QrCode_Name,
                Placeholder: AppResources.Format_QrCode_Placeholder,
                FormatHint: AppResources.Format_FreeText_Hint,
                NumericKeyboard: false,
                MaxLength: 0,
                Normalize: v => v,
                Validate: v => ""),

            new(
                Format: BarcodeFormat.Code128,
                DisplayName: AppResources.Format_Code128_Name,
                Placeholder: AppResources.Format_AlphaNumPlaceholder,
                FormatHint: AppResources.Format_AsciiOnly_Hint,
                NumericKeyboard: false,
                MaxLength: 0,
                Normalize: v => v,
                Validate: ValidateAsciiOnly),

            new(
                Format: BarcodeFormat.Code39,
                DisplayName: AppResources.Format_Code39_Name,
                Placeholder: AppResources.Format_Code39_Placeholder,
                FormatHint: AppResources.Format_Code39_Hint,
                NumericKeyboard: false,
                MaxLength: 0,
                Normalize: v => v.ToUpperInvariant(),
                Validate: ValidateCode39),

            new(
                Format: BarcodeFormat.Ean13,
                DisplayName: AppResources.Format_Ean13_Name,
                Placeholder: AppResources.Format_Ean13_Placeholder,
                FormatHint: AppResources.Format_Ean13_Hint,
                NumericKeyboard: true,
                MaxLength: 12,
                Normalize: v => v,
                Validate: v => ValidateNumericLength(v, 12, "EAN-13"),
                AppendCheckDigit: v => v + Common.CalculateMod10CheckDigit(v)),

            new(
                Format: BarcodeFormat.Ean8,
                DisplayName: AppResources.Format_Ean8_Name,
                Placeholder: AppResources.Format_Ean8_Placeholder,
                FormatHint: AppResources.Format_Ean8_Hint,
                NumericKeyboard: true,
                MaxLength: 7,
                Normalize: v => v,
                Validate: v => ValidateNumericLength(v, 7, "EAN-8"),
                AppendCheckDigit: v => v + Common.CalculateMod10CheckDigit(v)),

            new(
                Format: BarcodeFormat.UpcA,
                DisplayName: AppResources.Format_UpcA_Name,
                Placeholder: AppResources.Format_UpcA_Placeholder,
                FormatHint: AppResources.Format_UpcA_Hint,
                NumericKeyboard: true,
                MaxLength: 11,
                Normalize: v => v,
                Validate: v => ValidateNumericLength(v, 11, "UPC-A"),
                AppendCheckDigit: v => v + Common.CalculateMod10CheckDigit(v)),

            new(
                Format: BarcodeFormat.Itf,
                DisplayName: AppResources.Format_Itf_Name,
                Placeholder: AppResources.Format_Itf_Placeholder,
                FormatHint: AppResources.Format_Itf_Hint,
                NumericKeyboard: true,
                MaxLength: 0,
                Normalize: v => v,
                Validate: ValidateItf),

            new(
                Format: BarcodeFormat.Codabar,
                DisplayName: AppResources.Format_Codabar_Name,
                Placeholder: AppResources.Format_Codabar_Placeholder,
                FormatHint: AppResources.Format_Codabar_Hint,
                NumericKeyboard: false,
                MaxLength: 0,
                Normalize: v => v.ToUpperInvariant(),
                Validate: ValidateCodabar),

            new(
                Format: BarcodeFormat.Code93,
                DisplayName: AppResources.Format_Code93_Name,
                Placeholder: AppResources.Format_AlphaNumPlaceholder,
                FormatHint: AppResources.Format_AsciiOnly_Hint,
                NumericKeyboard: false,
                MaxLength: 0,
                Normalize: v => v,
                Validate: ValidateAsciiOnly),

            new(
                Format: BarcodeFormat.DataMatrix,
                DisplayName: AppResources.Format_DataMatrix_Name,
                Placeholder: AppResources.Format_DataMatrix_Placeholder,
                FormatHint: AppResources.Format_FreeText_Hint,
                NumericKeyboard: false,
                MaxLength: 0,
                Normalize: v => v,
                Validate: v => ""),

            new(
                Format: BarcodeFormat.Pdf417,
                DisplayName: AppResources.Format_Pdf417_Name,
                Placeholder: AppResources.Format_DataMatrix_Placeholder,
                FormatHint: AppResources.Format_FreeText_Hint,
                NumericKeyboard: false,
                MaxLength: 0,
                Normalize: v => v,
                Validate: v => ""),

            new(
                Format: BarcodeFormat.Aztec,
                DisplayName: AppResources.Format_Aztec_Name,
                Placeholder: AppResources.Format_DataMatrix_Placeholder,
                FormatHint: AppResources.Format_FreeText_Hint,
                NumericKeyboard: false,
                MaxLength: 0,
                Normalize: v => v,
                Validate: v => ""),
        };

        /// <summary>
        /// 作成対応している全フォーマットをOR結合したもの。読み取り側(BarcodeReaderView)で
        /// 「作成もできる形式だけ読み取る」ように絞り込むのに使う。
        /// GS1-128は物理的にはCode128として検出されるため、別途足す必要はない。
        /// </summary>
        public static BarcodeFormat SupportedFormats { get; } =
            All.Select(f => f.Format).Aggregate((BarcodeFormat)0, (acc, f) => acc | f);

        public static BarcodeFormatDefinition? Find(BarcodeFormat format)
        {
            return All.FirstOrDefault(f => f.Format == format);
        }

        private static string ValidateAsciiOnly(string value)
        {
            return Common.IsAsciiOnly(value) ? "" : AppResources.FormatValidate_NonAscii;
        }

        private static string ValidateCode39(string value)
        {
            return value.All(c => Code39AllowedChars.Contains(c))
                ? ""
                : AppResources.FormatValidate_Code39;
        }

        private static string ValidateCodabar(string value)
        {
            return value.All(c => CodabarAllowedChars.Contains(c))
                ? ""
                : AppResources.FormatValidate_Codabar;
        }

        private static string ValidateItf(string value)
        {
            if (!value.All(char.IsDigit))
                return AppResources.FormatValidate_ItfDigitsOnly;

            return value.Length % 2 == 0 ? "" : AppResources.FormatValidate_ItfEvenLength;
        }

        private static string ValidateNumericLength(string value, int length, string formatName)
        {
            if (!value.All(char.IsDigit))
                return string.Format(AppResources.FormatValidate_NumericOnly, formatName);

            return value.Length == length ? "" : string.Format(AppResources.FormatValidate_LengthMismatch, formatName, length);
        }
    }
}
