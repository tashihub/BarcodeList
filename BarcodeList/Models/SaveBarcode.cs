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
        public int Id { get; set; }

        /// <summary>
        /// どのフォルダに保存するかを示すためのFolderId。BarcodeFolderのIdと紐づける。nullableにしておくと、フォルダに入れないで保存することもできる。（通常の履歴）
        /// </summary>
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

        public DateTime CreatedAt { get; set; }

    }
}
