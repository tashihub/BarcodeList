using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using ZXing;
using ZXing.Net.Maui;

namespace BarcodeList.Models
{

    /// <summary>
    /// バーコード情報を保存しておくためのモデルクラス
    /// BarcodeResultでも行けるか？
    /// </summary>
    public class SavedBarcode
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// どのフォルダに保存するかを示すためのFolderId。BarcodeFolderのIdと紐づける。nullableにしておくと、フォルダに入れないで保存することもできる。（通常の履歴）
        /// </summary>
        [Indexed(Name = "IX_SavedBarcode_FolderId_CreatedAt", Order = 1)]
        public int FolderId { get; set; }
        public string BarcodeValue { get; set; } = "";

        /// <summary>
        /// BarcodeFormatを文字列で保存する。BarcodeResultのFormatは列挙型なので、保存する際に文字列に変換して保存する。
        /// </summary>
        public string BarcodeType { get; set; } = "";

        /// <summary>
        /// スキャンされて取得したデータかどうかを示すフラグ。スキャンされたデータはIsScanned=true、手入力されたデータはIsScanned=falseとする。
        /// </summary>
        public bool IsScanned { get; set; }


        public bool IsGs1 { get; set; }

        [Indexed(Name = "IX_SavedBarcode_FolderId_CreatedAt", Order = 2)]
        public DateTime CreatedAt { get; set; }

        [Ignore]
        public string CreatedAtText => CreatedAt.ToString("yyyy/MM/dd HH:mm:ss");
        [Ignore]
        public ZXing.Net.Maui.BarcodeFormat BarcodeFormat => (ZXing.Net.Maui.BarcodeFormat)Enum.Parse(typeof(ZXing.Net.Maui.BarcodeFormat), BarcodeType);

        /// <summary>
        /// 履歴・フォルダ一覧のアイコン枠に表示する、バーコード形式ごとの画像ファイル名(Resources/Images配下)。
        /// GS1-128は物理的にはCode128として保存されるため、code128.pngが使われる。
        /// </summary>
        [Ignore]
        public string BarcodeTypeIconImage
        {
            get
            {
                if (!Enum.TryParse<ZXing.Net.Maui.BarcodeFormat>(BarcodeType, out var format))
                    return "qrcode.png";

                return format switch
                {
                    ZXing.Net.Maui.BarcodeFormat.QrCode => "qrcode.png",
                    ZXing.Net.Maui.BarcodeFormat.Code128 => "code128.png",
                    ZXing.Net.Maui.BarcodeFormat.Code39 => "code39.png",
                    ZXing.Net.Maui.BarcodeFormat.Code93 => "code93.png",
                    ZXing.Net.Maui.BarcodeFormat.Ean13 => "ean13.png",
                    ZXing.Net.Maui.BarcodeFormat.Ean8 => "ean8.png",
                    ZXing.Net.Maui.BarcodeFormat.UpcA => "upca.png",
                    ZXing.Net.Maui.BarcodeFormat.Itf => "itf.png",
                    ZXing.Net.Maui.BarcodeFormat.Codabar => "codabar.png",
                    ZXing.Net.Maui.BarcodeFormat.DataMatrix => "datamatrix.png",
                    ZXing.Net.Maui.BarcodeFormat.Pdf417 => "pdf417.png",
                    ZXing.Net.Maui.BarcodeFormat.Aztec => "aztec.png",
                    _ => "qrcode.png",
                };
            }
        }
    }
}
