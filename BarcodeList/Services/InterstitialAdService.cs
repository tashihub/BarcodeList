using BarcodeList.Tool;
using Plugin.MauiMtAdmob;

namespace BarcodeList.Services;

/// <summary>
/// インタースティシャル(全画面)広告の読み込み・表示をラップする。
/// デバッグ作業のため一時的にGoogle公式テストIDに戻している。本番に戻す際はInterstitialAdUnitIdを本番用に戻すこと。
/// </summary>
public class InterstitialAdService
{
    // private const string InterstitialAdUnitId = "ca-app-pub-1283307746031730/3498927696"; // 本番用
    private const string InterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712"; // テスト用(Google公式テスト広告ユニットID)

    private readonly PurchaseService _purchaseService;

    public InterstitialAdService(PurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    /// <summary>
    /// 広告を読み込み、読み込み完了次第すぐに表示する。読み込みに失敗した場合は何もしない(ユーザー操作をブロックしない)。
    /// 広告削除を購入済みの場合は何もしない。
    /// AdMobのネイティブ呼び出し(読み込み・表示とも)はメインUIスレッドでの実行が必須のため、
    /// バックグラウンドスレッド(例: カメラ解析スレッドから呼ばれるスキャン検出時)から呼ばれても
    /// 安全なよう、このメソッド内でメインスレッドにディスパッチする。
    /// </summary>
    public void LoadAndShow()
    {
        if (_purchaseService.IsAdsRemoved)
            return;

        void OnLoaded(object? sender, EventArgs e)
        {
            CrossMauiMTAdmob.Current.OnInterstitialLoaded -= OnLoaded;
            CrossMauiMTAdmob.Current.OnInterstitialFailedToLoad -= OnFailedToLoad;
            MainThread.BeginInvokeOnMainThread(() => CrossMauiMTAdmob.Current.ShowInterstitial());
        }

        void OnFailedToLoad(object? sender, EventArgs e)
        {
            CrossMauiMTAdmob.Current.OnInterstitialLoaded -= OnLoaded;
            CrossMauiMTAdmob.Current.OnInterstitialFailedToLoad -= OnFailedToLoad;
            AppLogger.LogWarning("Interstitial ad failed to load.");
        }

        CrossMauiMTAdmob.Current.OnInterstitialLoaded += OnLoaded;
        CrossMauiMTAdmob.Current.OnInterstitialFailedToLoad += OnFailedToLoad;
        MainThread.BeginInvokeOnMainThread(() => CrossMauiMTAdmob.Current.LoadInterstitial(InterstitialAdUnitId));
    }
}
