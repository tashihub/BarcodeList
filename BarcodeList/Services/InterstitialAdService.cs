using BarcodeList.Tool;
using Plugin.MauiMtAdmob;

namespace BarcodeList.Services;

/// <summary>
/// インタースティシャル(全画面)広告の読み込み・表示をラップする。
/// 広告ユニットIDは現在Googleの公式テストIDを使用している。
/// 本番配信前に実際のAdMobインタースティシャル広告ユニットIDに差し替えること。
/// </summary>
public class InterstitialAdService
{
    private const string TestInterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";

    private readonly PurchaseService _purchaseService;

    public InterstitialAdService(PurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    /// <summary>
    /// 広告を読み込み、読み込み完了次第すぐに表示する。読み込みに失敗した場合は何もしない(ユーザー操作をブロックしない)。
    /// 広告削除を購入済みの場合は何もしない。
    /// </summary>
    public void LoadAndShow()
    {
        if (_purchaseService.IsAdsRemoved)
            return;

        void OnLoaded(object? sender, EventArgs e)
        {
            CrossMauiMTAdmob.Current.OnInterstitialLoaded -= OnLoaded;
            CrossMauiMTAdmob.Current.OnInterstitialFailedToLoad -= OnFailedToLoad;
            CrossMauiMTAdmob.Current.ShowInterstitial();
        }

        void OnFailedToLoad(object? sender, EventArgs e)
        {
            CrossMauiMTAdmob.Current.OnInterstitialLoaded -= OnLoaded;
            CrossMauiMTAdmob.Current.OnInterstitialFailedToLoad -= OnFailedToLoad;
            AppLogger.LogWarning("Interstitial ad failed to load.");
        }

        CrossMauiMTAdmob.Current.OnInterstitialLoaded += OnLoaded;
        CrossMauiMTAdmob.Current.OnInterstitialFailedToLoad += OnFailedToLoad;
        CrossMauiMTAdmob.Current.LoadInterstitial(TestInterstitialAdUnitId);
    }
}
