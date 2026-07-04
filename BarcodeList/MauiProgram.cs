using BarcodeList.Services;
using BarcodeList.Services.CreateServices;
using BarcodeList.ViewModels;
using BarcodeList.Views;
using BarcodeList.Views.Create;
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
            builder.Services.AddTransient<QrCreateView>();
            builder.Services.AddTransient<QrResultView>();
            builder.Services.AddTransient<Code39ResultView>();
            builder.Services.AddTransient<Code39CreateView>();
            builder.Services.AddTransient<Code128CreateView>();
            builder.Services.AddTransient<Code128ResultView>();
            builder.Services.AddTransient<Ean13CreateView>();
            builder.Services.AddTransient<Ean13ResultView>();
            builder.Services.AddTransient<Gs1128CreateView>();
            builder.Services.AddTransient<Gs1128ResultView>();



            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<FolderService>();
            builder.Services.AddSingleton<Code39CreateService>();
            return builder.Build();
        }
    }
}
