using BarcodeList.Tool;
using Plugin.MauiMtAdmob;

namespace BarcodeList.Services;

/// <summary>
/// インタースティシャル(全画面)広告の読み込み・表示をラップする。
/// 広告ユニットIDは実際のAdMobアカウントのものに差し替え済み。
/// MainActivity.OnCreateでforceTesting:trueのまま初期化しているため、
/// このIDを使っていても実際にはテスト広告が表示される(本番リリース前にforceTestingをfalseへ変更すること)。
/// </summary>
public class InterstitialAdService
{
    private const string InterstitialAdUnitId = "ca-app-pub-1283307746031730/3498927696";

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
        CrossMauiMTAdmob.Current.LoadInterstitial(InterstitialAdUnitId);
    }
}
