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

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseBarcodeReader()
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
            return builder.Build();
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
