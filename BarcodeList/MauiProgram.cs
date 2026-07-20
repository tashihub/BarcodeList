using BarcodeList.Services;
using BarcodeList.Services.CreateServices;
using BarcodeList.Tool;
using BarcodeList.ViewModels;
using BarcodeList.ViewModels.Create;
using BarcodeList.ViewModels.Details;
using BarcodeList.ViewModels.Result;
using BarcodeList.Views;
using BarcodeList.Views.Create;
using BarcodeList.Views.Details;
using BarcodeList.Views.Result;
using Microsoft.Extensions.Logging;
using Plugin.MauiMtAdmob;
using ZXing.Net.Maui.Controls;
namespace BarcodeList
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            RegisterGlobalExceptionLogging();

            // ZXingのQRエンコーダーが漢字モードでShift_JISを使用するため、
            // .NET標準では未登録のレガシーエンコーディングを利用可能にする。
            // 未登録だと該当パターンの文字列でQRコード生成時にクラッシュする。
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            ApplySavedLanguagePreference();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseBarcodeReader()
                .UseMauiMTAdmob()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddTransient<BarcodeReaderView>();
            builder.Services.AddTransient<BarcodeReaderViewModel>();
            builder.Services.AddTransient<ScannedDataView>();
            builder.Services.AddTransient<ScannedDataViewModel>();

            builder.Services.AddTransient<BarcodeCreateMenuView>();
            builder.Services.AddTransient<BarcodeCreateMenuViewModel>();
            builder.Services.AddTransient<FolderView>();
            builder.Services.AddTransient<FolderViewModel>();
            builder.Services.AddTransient<HistoryView>();
            builder.Services.AddTransient<HistoryViewModel>();
            builder.Services.AddTransient<FolderDetailView>();
            builder.Services.AddTransient<FolderDetailViewModel>();
            builder.Services.AddTransient<SettingsView>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<PrivacyPolicyView>();

            builder.Services.AddTransient<BarcodeCreateView>();
            builder.Services.AddTransient<BarcodeCreateViewModel>();
            builder.Services.AddTransient<BarcodeResultView>();
            builder.Services.AddTransient<BarcodeResultViewModel>();
            builder.Services.AddTransient<Gs1128CreateView>();
            builder.Services.AddTransient<Gs1128CreateViewModel>();
            builder.Services.AddTransient<Gs1128ResultView>();
            builder.Services.AddTransient<Gs1128ResultViewModel>();

            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<FolderService>();
            builder.Services.AddSingleton<Gs1128CreateService>();
            builder.Services.AddSingleton<AdFrequencyService>();
            builder.Services.AddSingleton<PurchaseService>();
            builder.Services.AddSingleton<InterstitialAdService>();
            return builder.Build();
        }

        private const string LanguagePreferenceKey = "app_language";

        /// <summary>
        /// 設定画面で選択した表示言語(空文字は端末既定)を、UI構築前に反映する。
        /// x:Staticでの文字列参照はページ生成時に一度だけ評価されるため、
        /// 実行中の切り替えではなく次回起動時に反映される。
        /// </summary>
        private static void ApplySavedLanguagePreference()
        {
            var languageCode = Preferences.Default.Get(LanguagePreferenceKey, string.Empty);
            if (string.IsNullOrEmpty(languageCode))
                return;

            try
            {
                var culture = new System.Globalization.CultureInfo(languageCode);
                BarcodeList.Resources.Strings.AppResources.Culture = culture;
                System.Globalization.CultureInfo.CurrentUICulture = culture;
            }
            catch (Exception ex)
            {
                AppLogger.LogWarning($"ApplySavedLanguagePreference failed for '{languageCode}': {ex.Message}");
            }
        }

        private static bool _globalExceptionLoggingRegistered;

        /// <summary>
        /// 未処理例外(クラッシュ)発生時に原因調査ができるよう、端末内のログファイルに記録する。
        /// try/catchで拾えていない例外(ネイティブ由来のものを除く)を捕捉するための最終防衛ライン。
        /// </summary>
        private static void RegisterGlobalExceptionLogging()
        {
            if (_globalExceptionLoggingRegistered)
                return;
            _globalExceptionLoggingRegistered = true;

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                AppLogger.LogError("UnhandledException (IsTerminating=" + e.IsTerminating + ")", e.ExceptionObject as Exception);
            };

            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                AppLogger.LogError("UnobservedTaskException", e.Exception);
                e.SetObserved();
            };
        }
    }
}
