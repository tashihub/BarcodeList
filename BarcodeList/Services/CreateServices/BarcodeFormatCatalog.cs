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
                DisplayName: "QRコード",
                Placeholder: "例：https://example.com",
                FormatHint: "任意の文字列",
                NumericKeyboard: false,
                MaxLength: 0,
                Normalize: v => v,
                Validate: v => ""),

            new(
                Format: BarcodeFormat.Code128,
                DisplayName: "Code128",
                Placeholder: "例：ABC123",
                FormatHint: "ASCII文字(日本語不可)",
                NumericKeyboard: false,
                MaxLength: 0,
                Normalize: v => v,
                Validate: ValidateAsciiOnly),

            new(
                Format: BarcodeFormat.Code39,
                DisplayName: "Code39",
                Placeholder: "例：ABC-123",
                FormatHint: "英数字(大文字)と - . スペース $ / + %",
                NumericKeyboard: false,
                MaxLength: 0,
                Normalize: v => v.ToUpperInvariant(),
                Validate: ValidateCode39),

            new(
                Format: BarcodeFormat.Ean13,
                DisplayName: "EAN-13(JAN)",
                Placeholder: "例：490123456789",
                FormatHint: "12桁の数字(チェックデジットは自動計算)",
                NumericKeyboard: true,
                MaxLength: 12,
                Normalize: v => v,
                Validate: v => ValidateNumericLength(v, 12, "EAN-13"),
                AppendCheckDigit: v => v + Common.CalculateMod10CheckDigit(v)),

            new(
                Format: BarcodeFormat.Ean8,
                DisplayName: "EAN-8",
                Placeholder: "例：4901234",
                FormatHint: "7桁の数字(チェックデジットは自動計算)",
                NumericKeyboard: true,
                MaxLength: 7,
                Normalize: v => v,
                Validate: v => ValidateNumericLength(v, 7, "EAN-8"),
                AppendCheckDigit: v => v + Common.CalculateMod10CheckDigit(v)),

            new(
                Format: BarcodeFormat.UpcA,
                DisplayName: "UPC-A",
                Placeholder: "例：03600029145",
                FormatHint: "11桁の数字(チェックデジットは自動計算)",
                NumericKeyboard: true,
                MaxLength: 11,
                Normalize: v => v,
                Validate: v => ValidateNumericLength(v, 11, "UPC-A"),
                AppendCheckDigit: v => v + Common.CalculateMod10CheckDigit(v)),

            new(
                Format: BarcodeFormat.Itf,
                DisplayName: "ITF",
                Placeholder: "例：12345678",
                FormatHint: "偶数桁の数字",
                NumericKeyboard: true,
                MaxLength: 0,
                Normalize: v => v,
                Validate: ValidateItf),

            new(
                Format: BarcodeFormat.Codabar,
                DisplayName: "Codabar",
                Placeholder: "例：A123456A",
                FormatHint: "数字と - $ : / . + 、前後にA〜Dのスタート/ストップ文字(省略可)",
                NumericKeyboard: false,
                MaxLength: 0,
                Normalize: v => v.ToUpperInvariant(),
                Validate: ValidateCodabar),

            new(
                Format: BarcodeFormat.Code93,
                DisplayName: "Code93",
                Placeholder: "例：ABC123",
                FormatHint: "ASCII文字(日本語不可)",
                NumericKeyboard: false,
                MaxLength: 0,
                Normalize: v => v,
                Validate: ValidateAsciiOnly),

            new(
                Format: BarcodeFormat.DataMatrix,
                DisplayName: "Data Matrix",
                Placeholder: "例：任意のテキスト",
                FormatHint: "任意の文字列",
                NumericKeyboard: false,
                MaxLength: 0,
                Normalize: v => v,
                Validate: v => ""),

            new(
                Format: BarcodeFormat.Pdf417,
                DisplayName: "PDF417",
                Placeholder: "例：任意のテキスト",
                FormatHint: "任意の文字列",
                NumericKeyboard: false,
                MaxLength: 0,
                Normalize: v => v,
                Validate: v => ""),

            new(
                Format: BarcodeFormat.Aztec,
                DisplayName: "Aztec",
                Placeholder: "例：任意のテキスト",
                FormatHint: "任意の文字列",
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
            return Common.IsAsciiOnly(value) ? "" : "日本語は使用できません。QRコードを使用してください。";
        }

        private static string ValidateCode39(string value)
        {
            return value.All(c => Code39AllowedChars.Contains(c))
                ? ""
                : "Code39では大文字英数字と - . スペース $ / + % のみ使用できます。";
        }

        private static string ValidateCodabar(string value)
        {
            return value.All(c => CodabarAllowedChars.Contains(c))
                ? ""
                : "Codabarでは数字と - $ : / . + 、A〜Dのみ使用できます。";
        }

        private static string ValidateItf(string value)
        {
            if (!value.All(char.IsDigit))
                return "ITFでは数字のみ使用できます。";

            return value.Length % 2 == 0 ? "" : "ITFは偶数桁で入力してください。";
        }

        private static string ValidateNumericLength(string value, int length, string formatName)
        {
            if (!value.All(char.IsDigit))
                return $"{formatName}は数字のみ使用できます。";

            return value.Length == length ? "" : $"{formatName}は{length}桁(チェックデジットを除く)で入力してください。";
        }
    }
}
