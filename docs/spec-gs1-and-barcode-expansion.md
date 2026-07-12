# 仕様書: GS1バーコード作成 対応 / 対応バーコード種類の拡張

状態: 設計確定・実装準備中(2026-07-06時点、未実装。次回セッションで①から実装開始)

## 0. 確定事項

- GS1作成は **GS1-128のみ**を対象とする(GS1 DataBar/Rss系はライブラリ制約により対応しない。3.2節の通り確定)。
- バーコード種類は、3.2節で確認した「生成できる15種」のうち未対応の11種類から、**UPC-E / MSI / Plessey の3種を除いた8種類**を追加対象とする(Aztec, Codabar, Code93, DataMatrix, Ean8, Itf, Pdf417, UpcA)。
  - UPC-E / MSI / Plessey は、`ZXing.Net`側の挙動(チェックデジット自動付与の有無、正しい入力形式)が未確認でリスクがあるため、**読み取り・作成とも対象外**とする(2026-07-06決定)。読み取り側も `BarcodeReaderOptions.Formats` から明示的に除外する対応が必要(詳細は [barcode-format-specs.md](barcode-format-specs.md) 参照)。
- 各フォーマットの技術仕様(文字種・桁数・チェックデジット等)は [barcode-format-specs.md](barcode-format-specs.md) にまとめた。

## 1. 背景

- 現状、GS1-128の読み取りは `Gs1Parser` + ZXingのシンボロジ識別子(`]C1`等)により正しく機能することを実機で確認済み。
- 一方、GS1バーコードの**作成**は `Gs1128CreateService.GenerateGs1Value()` が `"01{gtin}17{date}10{lot}"` という単純文字列連結のみで、FNC1(GS1フラグ)もAI区切り文字(GS, `(char)29`)も埋め込んでおらず、本物のGS1-128になっていない。
- また、現在バーコード作成は QR / Code39 / Code128 / Ean13 / (疑似)Gs1128 の5種類のみ対応。
- アプリの用途は「スキャンした結果を履歴・フォルダに保存し、いつでも再現(再作成)できるようにする」ことなので、**読み取れる形式は原則すべて作成もできる**ことが目標になる。

## 2. スコープ

1. GS1バーコード作成を、本物のFNC1付きGS1-128として正しく生成できるようにする。AIコードは頻出のものを専用フォームで、それ以外もできる限り汎用的に対応する。
2. 対応バーコード種類を、現在の5種類から「読み取り可能な形式」に近づけて拡張する。

## 3. 制約事項(実機検証・アセンブリ調査により確定した事実)

### 3.1 GS1作成の実現方法

`ZXing.Net`(本アプリが依存する `ZXing.Net.Maui` の内部依存)には `ZXing.Common.EncodingOptions.GS1Format`(bool)が存在し、これを `true` にして `Code128` フォーマットでエンコードすると、先頭に本物のFNC1が付与される。可変長AIが末尾以外に来る場合は、値の中に `(char)29`(GS)を区切り文字として埋め込めば良い。これはテスト用SVGを実機でスキャンし、`]C1` 識別子・`HasGroupSeparator`/`IsReliable` が正しく立つことを確認済み。

**→ GS1作成は内部的には「Code128 + GS1Format=true」の特殊系として実装する。**

**追記(2026-07-06 実装時に判明): 表示に使う `zxing:BarcodeGeneratorView`(`ZXing.Net.Maui.Controls`)は `Format`/`Value`/`ForegroundColor`/`BackgroundColor`/`BarcodeMargin`/`CharacterSet` しか公開しておらず、`EncodingOptions`(GS1Format含む)を渡す手段がない**(アセンブリのメタデータを直接確認して判明)。そのためGS1-128の描画だけは `BarcodeGeneratorView` を使わず、`ZXing.BarcodeWriterGeneric`(`Format=CODE_128`, `Options.GS1Format=true`)で直接 `BitMatrix` を生成し、`GraphicsView` + 自前の `IDrawable`(`Tool/BitMatrixDrawable.cs`)でモジュールを黒四角として描画する方式で実装した。他の12フォーマット(GS1以外)はGS1Formatが不要なので、引き続き `BarcodeGeneratorView` をそのまま使える。

### 3.2 フォーマットごとの生成可否(実機ではなくライブラリの機械的検証)

`ZXing.Net.Maui.BarcodeFormat` の全21値について、実際に `ZXing.BarcodeWriterSvg` でエンコードを試した結果:

| 生成できる(15種) | 生成できない=読み取り専用(6種) |
|---|---|
| Aztec, Codabar, Code128, Code39, Code93, DataMatrix, Ean13, Ean8, Itf, Msi, Pdf417, Plessey, QrCode, UpcA, UpcE | Imb, MaxiCode, PharmaCode, **Rss14(GS1 DataBar)**, **RssExpanded(GS1 DataBar Expanded)**, UpcEanExtension |

**重要な制約:** GS1 DataBar(Rss14 / RssExpanded)は読み取りはできてもZXing.Netでは生成できない。そのため「GS1バーコードの作成」は事実上 **GS1-128(Code128ベース)のみ** が対象になる。DataBar系のGS1作成は今回のスコープ外(ライブラリの制約により実現不可)。

## 4. 設計方針

### 4.1 GS1 AI対応(①)

**方針転換(2026-07-06): 「専用フォーム+汎用モード」の二本立てをやめ、単一のAI要素リスト編集UIに一本化する。** 「頻出AIをいくつか決め打ちする」のではなく、**どのAIコードでも入力できる汎用UIを基本形にし、既知AIテーブルはあくまで利便性向上(表示名・固定長判定・入力補助)のためだけに使う**。これによりAIを追加するたびにUIを増やす必要がなくなり、「事実上すべてのAIに対応」を最も素直に実現できる。

- **UI**: AI要素(AIコード+値)を1件ずつ追加していくリスト編集UI。1件ごとに「AIコード」入力(既知AIは名前付きで選べるPickerを候補表示しつつ、そこに無いコードも自由入力できる)+「値」入力(既知AIが日付系なら日付入力、数値系なら数値キーボードなど、テーブル情報を使って入力しやすくするのは任意の改善)。追加した要素はリストに表示し、順序入れ替え・削除ができる。
- **既知AIテーブル**: 現行 `Gs1Parser.SupportedAis`(`01` GTIN, `10` ロット番号, `11` 製造日, `15` 賞味期限, `17` 有効期限, `21` シリアル番号, `30` 数量, `3100`-`3103` 重量)をベースに、`Tool/Gs1AiTable.cs`(新規)へ切り出す。テーブルにないAIコードは「名前=(不明なAI)」「可変長」として扱えば良いだけなので、対応AIを都度増やす必要はない。
- **エンコード時のGSセパレータ挿入規則**: 各要素について、「可変長 かつ リスト内で末尾要素ではない」場合にのみ後ろにGS(`(char)29`)を挿入する。固定長要素の後や、末尾の可変長要素の後には付けない(GS1仕様通り)。
- **`EncodingOptions.GS1Format = true`** を `BarcodeGeneratorView.Options`(または直接 `BarcodeWriterSvg`/writer)に設定し、真のFNC1付きGS1-128として生成する。
- 読み取り側 `Gs1Parser.SupportedAis` / `GetAiName` / `GetFixedLength` は、作成側のリストUIでも同じ `Gs1AiTable` を参照する(読み取り・作成で単一の情報源)。

### 4.1.1 GS1履歴詳細表示(オープンクエスチョン2の回答: AI毎の詳細表示まで行う)

- `SavedBarcode` はデータモデル変更なし(`IsGs1` フラグ + 生の `BarcodeValue` で十分。GSセパレータを含む生値をそのまま保存済みのため、表示時に `Gs1Parser.ParseRaw(barcode.BarcodeValue)` で再パースすればAI要素ごとの内訳が得られる)。
- `HistoryViewModel.OpenHistory` / `FolderDetailViewModel.OpenBarcode` で `barcode.IsGs1 == true` の場合、既存の `ScannedDataView` の「AIコード解析」カード(`CollectionView` + `Gs1Element` テンプレート)と同じUIを再利用した専用の結果画面に遷移する。具体的には、GS1用の結果表示は `ScannedDataView` 相当のAI一覧表示 + `Gs1128ResultView` 相当の「フォルダ保存」機能を1画面に統合する形にする(詳細は次回実装時に既存2画面のUIを比較して1つにまとめる)。

### 4.2 バーコード種類拡張(②)

対象は 0節で確定した8種: `Aztec, Codabar, Code93, DataMatrix, Ean8, Itf, Pdf417, UpcA`(UPC-E/MSI/Plesseyは対象外)。

実装効率化のため、性質でグルーピングする:

| グループ | フォーマット | 特徴 | 実装方針 |
|---|---|---|---|
| 数字+チェックデジット系(Ean13と同系統) | Ean8, UpcA | 固定桁数の数字+モジュラス10チェックデジット | `Ean13CreateService` と同様の「桁数+チェックデジット検証」ロジックを一般化して使い回す |
| 数字のみ・可変長系 | Itf | 数字のみ、偶数桁必須 | `Code39CreateService` に近い「許容文字種チェック」パターンを流用 |
| 英数記号系 | Codabar, Code93 | Code128に近い文字種制限 | `Code128CreateService` に近いパターンを流用 |
| 2次元・自由入力系 | DataMatrix, Aztec, Pdf417 | 文字種制限が緩く、QRに近い | `QrCreateService` に近い(ほぼ無検証)パターンを流用 |

### 4.3 アーキテクチャ(決定: 汎用Create/Result画面に統合)

**決定(2026-07-06): 「フォーマットごとに専用View+ViewModel+CreateService+ResultView」パターンをやめ、汎用画面に寄せる。** GS1-128だけはAI複数入力という構造的に異なるUIが必要なため、専用画面のまま残す(4.1参照)。それ以外の全「単一値を入力するだけ」のフォーマット(既存の QR, Code39, Code128, Ean13 + 新規8種 = 計12種)は、1つの汎用Create画面 + 1つの汎用Result画面に統合する。

**新しい構成案:**

- `Services/CreateServices/BarcodeFormatDefinition.cs`(新規): 1フォーマット分の定義をレコードで表現する。

  ```csharp
  public record BarcodeFormatDefinition(
      BarcodeFormat Format,
      string DisplayName,
      Func<string, string> Normalize,           // 例: Code39/Codabarは大文字化
      Func<string, string> Validate,            // ""なら正常、それ以外はエラーメッセージ
      Func<string, string>? AppendCheckDigit = null // Ean13/Ean8/UpcAなど、入力にチェックデジットを自動付加する場合に使う
  );
  ```

- `Services/CreateServices/BarcodeFormatCatalog.cs`(新規): 上記12フォーマット分の定義を1箇所にまとめる静的テーブル。4.2のグルーピング(数字+チェックデジット系/数字のみ系/英数記号系/2次元自由入力系)ごとに検証ロジックをまとめる。
- `ViewModels/BarcodeCreateViewModel.cs`(新規、既存の `Code39CreateViewModel` 等を置き換え): `SelectedFormat`(`BarcodeFormatCatalog.All` から選択)、`InputValue`、`ErrorMessage` を持ち、`CreateCommand` で `SelectedFormat.Validate/Normalize/AppendCheckDigit` を呼んで最終値を確定 → 履歴保存 → 汎用Result画面へ遷移。
- `ViewModels/Result/BarcodeResultViewModel.cs`(新規、既存の `Code128ResultViewModel`/`Ean13ResultViewModel`/`QrResultViewModel` 等を置き換え): `Value` + `Format` を受け取り、`zxing:BarcodeGeneratorView` 表示・フォルダ選択・保存を行う(この部分は現状すでに4フォーマット分でほぼ同一コードなので、そのまま一般化できる)。
- 既存の `Views/Create/{Code39,Code128,Ean13,Qr}CreateView.*`、`Views/Result/{Code39,Code128,Ean13,Qr}ResultView.*` および対応する `CreateService`/`ViewModel` は、汎用画面への移行が完了次第削除する(重複コードを残さない)。
- `Views/BarcodeCreateMenuView`(フォーマット選択メニュー)は、新しい汎用Create画面 + GS1専用画面への導線に整理する。

この移行により、8フォーマット追加は「カタログにエントリを追加するだけ」で完結し、新規ファイルはほぼ増えない。

## 5. データモデルへの影響

- `SavedBarcode.BarcodeType` は既存通り `ZXing.Net.Maui.BarcodeFormat` の文字列表現を保存する方式を維持できる(全フォーマット共通の型なので変更不要)。
- GS1(複数AI要素)を履歴に保存し再現する場合、現状の `SavedBarcode` は単一の `BarcodeValue` 文字列のみ保持する設計なので、**GSセパレータを含んだ生の値をそのまま保存すれば再現は可能**(AI要素ごとに分解して保存する必要はない)。ただし履歴一覧やフォルダ詳細でGS1の中身(AI要素)を人間が読める形で再表示したい場合は、保存時に `Gs1Parser.ParseRaw()` で再パースするか、AI要素のJSON等を別途保存するかの検討が必要。

## 6. 決定事項ログ(2026-07-06)

| # | 論点 | 決定 |
|---|---|---|
| 1 | 4.3のアーキテクチャ | 汎用Create/Result画面に統合する(GS1-128のみ専用画面を維持) |
| 2 | GS1履歴の詳細表示 | AI要素ごとの内訳まで表示する(4.1.1参照) |
| 3 | 専用フォーム対象AIの追加 | 「よく使うAIを決め打ちで増やす」のではなく、**どのAIコードでも入力できる単一のリスト編集UIに一本化**する方針に転換(4.1参照)。既知AIテーブルは利便性向上のためだけに使う |
| 4 | 実装順序 | ①GS1作成の修正 → ②8フォーマット追加、の順で進める |

## 7. 今回のスコープ外

- GS1 DataBar(Rss14 / RssExpanded)の作成 — ZXing.Netに書き込み対応がないため不可。
- Imb, MaxiCode, PharmaCode, UpcEanExtension の作成 — 同上の理由で不可。
- UPC-E, MSI, Plessey の読み取り・作成 — 0節の通り、ライブラリ挙動未確認によるリスクを理由に対象外(2026-07-06決定)。

## 8. 実装タスク分解

### ①GS1-128作成の修正 — 実装済み・実機確認済み(2026-07-06)

1. ✅ `Tool/Gs1AiTable.cs` を新設し、`Gs1Parser` の AIテーブル(`SupportedAis`/`GetFixedLength`/`GetAiName`)をそこに集約。読み取り・作成の両方から参照する単一の情報源にした。
2. ✅ `Gs1128CreateService.BuildGs1Value(elements)` で、複数AI要素からGSセパレータを正しく挿入しながらエンコードする文字列を組み立てるようにした。**表示側の注意点**: `zxing:BarcodeGeneratorView` は `EncodingOptions`(GS1Format含む)を渡す手段がないことが判明したため、`Gs1128ResultView` では `ZXing.BarcodeWriterGeneric`(`Options.GS1Format=true`)で直接 `BitMatrix` を生成し、`GraphicsView` + `Tool/BitMatrixDrawable.cs` で自前描画する方式に変更した(3.1参照)。
3. ✅ `Gs1128CreateViewModel`/`Gs1128CreateView` を、AI要素(AIコード+値)を1件ずつ追加できるリスト編集UIに書き換えた。
4. ✅ `HistoryViewModel`/`FolderDetailViewModel` から、`IsGs1` なバーコードを開いたときに `Gs1128ResultView`(AI内訳表示)に遷移するよう変更した。あわせて `Gs1128ResultViewModel` に(バグで動いていなかった)フォルダ保存機能も実装し、`FolderService.SaveToFolderAsync` に `isGs1` パラメータを追加した。
5. ✅ 実機で確認済み(ユーザー確認: 複数AI要素からの生成・再スキャンが正しく動作)。
6. ✅ **AIコードごとの入力仕様バリデーション**(実機確認後の追加要望): `Gs1AiTable.ValidateValue(ai, value)` で、既知AIごとに桁数・数字のみ・YYMMDD日付妥当性をチェックするようにした(例: `01`=14桁数字必須、`11/15/17`=YYMMDD6桁、`3100-3103`=6桁数字)。あわせて入力欄の `Keyboard`/`MaxLength` も選択中のAIに応じて動的に切り替わるようにした(`Gs1128CreateViewModel.NewAiValueKeyboard`/`NewAiValueMaxLength`)。未知AIは従来通り空文字チェックのみ。
7. **GS1公式の全AI(約200種)対応は見送り**(2026-07-06決定)。switch文ベースの現行実装では現実的に10〜20件が限度で、全AI対応にはデータ駆動のテーブル化+外部の公式データ取得が必要になり、コストに見合わないと判断。代わりに、対応AIコード一覧を可視化することにした:
   - ✅ `Gs1AiTable.GetReferenceList()` / `Gs1AiReferenceItem` を追加。
   - ✅ `Gs1128CreateView` に「対応AIコード一覧」カードを追加し、対応AI(コード・名前・入力形式)と「一覧以外も作成はできるが読み返しの内訳表示は保証されない」旨を明記した。
   - この一覧・注意書きは `Gs1AiTable.SupportedAis` を増やすたびに自動で追従する。
8. ⬜ 6・7は未実機確認。次回、(a) 既知AI(特に `01`/日付系)で意図的に桁数違反・不正日付を入力してエラーメッセージが正しく出るか、(b) GS1作成画面に「対応AIコード一覧」が正しく表示されるか、を確認する。あわせて `ScannedDataView` の一時的デバッグ表示(黄色いカード)は確認完了後に削除して良い。

### ②8フォーマット追加 — 実装済み(2026-07-06、実機確認は未実施)

1. ✅ `Services/CreateServices/BarcodeFormatDefinition.cs`(record)/`BarcodeFormatCatalog.cs` を新設。4.2のグルーピングに沿って12フォーマット(既存4: QR/Code39/Code128/EAN-13 + 新規8: EAN-8/UPC-A/ITF/Codabar/Code93/DataMatrix/PDF417/Aztec)分の `Normalize`/`Validate`/`AppendCheckDigit`(EAN-8・EAN-13・UPC-Aのみ)を実装。チェックデジット計算は `Tool/Common.CalculateMod10CheckDigit` に共通化(右端データ桁から重み3,1を交互)。EAN系はデータ桁のみ入力させ、チェックデジットは自動付加する方式にした(既存Ean13の「13桁全部入力必須」から仕様変更)。
2. ✅ `ViewModels/Create/BarcodeCreateViewModel.cs` + `Views/Create/BarcodeCreateView.xaml(.cs)`(汎用Create画面: フォーマットPicker+値Entry、選択フォーマットに応じてKeyboard/MaxLength/ヒントが切り替わる)、`ViewModels/Result/BarcodeResultViewModel.cs` + `Views/Result/BarcodeResultView.xaml(.cs)`(汎用Result画面: `zxing:BarcodeGeneratorView`+フォルダ保存)を新設。
3. ✅ `MauiProgram.cs`・`AppShell.xaml.cs` を新しい汎用画面(`BarcodeCreateView`/`BarcodeResultView`)に向けて更新。
4. ✅ 旧 `Code39/Code128/Ean13/Qr` の View/ViewModel/CreateService/ResultView 一式(16ファイル)と、使われなくなった `Interface/IBarcodeCreateService.cs` を削除。`.csproj` の該当 `MauiXaml`/`Compile` Update エントリも整理。`HistoryViewModel`/`FolderDetailViewModel` の遷移分岐(4形式ぶんのif/else)も、共通の `BarcodeResultView` へ1本化して大幅に簡素化した。
5. ✅ `BarcodeReaderView` の `BarcodeReaderOptions.Formats` から `BarcodeFormats.All & ~(UpcE | Msi | Plessey)` で除外。
6. ✅ **UX修正(2026-07-06、ユーザー指摘)**: 「コード上ではまとめてよいが、作成画面ではまとめないでほしい」との指摘を受け、`BarcodeCreateMenuView` を1つの汎用入口ではなく**フォーマットごとに個別のメニュー項目**(`BarcodeFormatCatalog.All`から自動生成、GS1-128は別枠)に戻した。各項目をタップすると `BarcodeCreateView` に `Format` を渡して遷移し、その画面ではフォーマット選択欄(Picker)を隠して**そのフォーマット専用の画面に見える**ようにした(`BarcodeCreateViewModel.IsFormatFixed`/`ShowFormatPicker`)。裏側の検証ロジック(`BarcodeFormatCatalog`)自体は1つのテーブルのまま共有しており、UIの入口だけをフォーマットごとに分けている。
7. ✅ **さらなるUX統一(2026-07-06)**: GS1-128のメニュー項目だけ大きいアイコン枠付きの見た目で浮いていたため、他フォーマットと同じ「タイトル+ヒント+シェブロン」のシンプルなカードに統一(`BarcodeCreateMenuView.xaml`)。「その他のバーコード」の見出しも不要になったため削除し、GS1-128を先頭にした1本のリストにした。
8. ⬜ **実機での確認が未実施**。次回、(a) メニューから各フォーマット(GS1-128含む)を個別に開けるか・遷移後にPickerが隠れて専用画面に見えるか、(b) 追加した8フォーマット(特にチェックデジット自動付加のEAN-8/EAN-13/UPC-A、偶数桁必須のITF、スタート/ストップ文字を含むCodabar)それぞれで作成→スキャンの往復確認を行うこと。
