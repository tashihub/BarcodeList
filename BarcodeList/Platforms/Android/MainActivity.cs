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
            // アプリID/広告ユニットIDは実際のAdMobアカウントのものに差し替え済み。
            // forceTesting/debugModeは開発中の誤クリック(ポリシー違反リスク)を防ぐため意図的にtrueのまま。
            // 本番リリースビルドの直前に、この2つをfalseへ変更すること。
            // GDPR同意(UMP)は無料版では自動対応されないため、initialiseConsentAtStartupはfalseにして未対応のままにしている。
            CrossMauiMTAdmob.Current.Init(
                activity: this,
                appId: "ca-app-pub-1283307746031730~6619915051",// "ca-app-pub-3940256099942544/1033173712",テスト用 //ca-app-pub-1283307746031730~6619915051 本番用
                license: "",
                openAdsId: "",
                nativeAdsId: "",
                enableOpenAds: false,
                tagForUnderAgeOfConsent: false,
                testDeviceId: "",
                forceTesting: false,
                geography: DebugGeography.DEBUG_GEOGRAPHY_DISABLED,
                initialiseConsentAtStartup: false,
                debugMode: true);
        }
    }
}
