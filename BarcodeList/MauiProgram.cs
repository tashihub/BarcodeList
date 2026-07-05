using BarcodeList.Services;
using BarcodeList.Services.CreateServices;
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

            builder.Services.AddTransient<QrCreateView>();
            builder.Services.AddTransient<QrCreateViewModel>();
            builder.Services.AddTransient<QrResultView>();
            builder.Services.AddTransient<QrResultViewModel>();
            builder.Services.AddTransient<Code39ResultView>();
            builder.Services.AddTransient<Code39ResultViewModel>();
            builder.Services.AddTransient<Code39CreateView>();
            builder.Services.AddTransient<Code39CreateViewModel>();
            builder.Services.AddTransient<Code128CreateView>();
            builder.Services.AddTransient<Code128CreateViewModel>();
            builder.Services.AddTransient<Code128ResultView>();
            builder.Services.AddTransient<Code128ResultViewModel>();
            builder.Services.AddTransient<Ean13CreateView>();
            builder.Services.AddTransient<Ean13CreateViewModel>();
            builder.Services.AddTransient<Ean13ResultView>();
            builder.Services.AddTransient<Ean13ResultViewModel>();
            builder.Services.AddTransient<Gs1128CreateView>();
            builder.Services.AddTransient<Gs1128CreateViewModel>();
            builder.Services.AddTransient<Gs1128ResultView>();
            builder.Services.AddTransient<Gs1128ResultViewModel>();

            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<FolderService>();
            builder.Services.AddSingleton<Code39CreateService>();
            builder.Services.AddSingleton<Code128CreateService>();
            builder.Services.AddSingleton<Ean13CreateService>();
            builder.Services.AddSingleton<Gs1128CreateService>();
            builder.Services.AddSingleton<QrCreateService>();
            return builder.Build();
        }
    }
}
