using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.MauiMtAdmob;
using Plugin.MauiMtAdmob.Extra;

namespace BarcodeList
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Plugin.MauiMTAdmobはCrossMauiMTAdmob.Current.Initを呼ぶまで広告機能が使えない。
            // 現在はGoogle公式のテストIDのみ使用(本番配信前に実際のAdMob IDへ差し替えること)。
            // GDPR同意(UMP)は無料版では自動対応されないため、initialiseConsentAtStartupはfalseにして未対応のままにしている。
            CrossMauiMTAdmob.Current.Init(
                activity: this,
                appId: "ca-app-pub-3940256099942544~3347511713",
                license: "",
                openAdsId: "",
                nativeAdsId: "",
                enableOpenAds: false,
                tagForUnderAgeOfConsent: false,
                testDeviceId: "",
                forceTesting: true,
                geography: DebugGeography.DEBUG_GEOGRAPHY_DISABLED,
                initialiseConsentAtStartup: false,
                debugMode: true);
        }
    }
}
