using BarcodeList.Tool;
#if ANDROID || IOS || MACCATALYST
using Plugin.InAppBilling;
#endif

namespace BarcodeList.Services;

public enum PurchaseResult
{
    Success,
    Cancelled,
    Failed,
    NothingToRestore,
    NotSupported,
}

/// <summary>
/// 「広告削除」の非消耗型アプリ内課金を扱う。
/// 購入状態はPreferencesに永続化し、InterstitialAdServiceがこれを見て広告表示をスキップする。
/// 商品ID(RemoveAdsProductId)はPlay Console側で同じIDの非消耗型アイテムを登録すること。
/// Windowsはストア連携が未整備のため、PurchaseAsync/RestorePurchasesAsyncは常にNotSupportedを返す。
/// </summary>
public class PurchaseService
{
    public const string RemoveAdsProductId = "remove_ads";
    private const string AdsRemovedPreferenceKey = "iap_ads_removed";

    public bool IsAdsRemoved => Preferences.Default.Get(AdsRemovedPreferenceKey, false);

    public event EventHandler? AdsRemovedStateChanged;

    public async Task<PurchaseResult> PurchaseRemoveAdsAsync()
    {
#if ANDROID || IOS || MACCATALYST
        var billing = CrossInAppBilling.Current;
        try
        {
            var connected = await billing.ConnectAsync();
            if (!connected)
                return PurchaseResult.Failed;

            var purchase = await billing.PurchaseAsync(RemoveAdsProductId, ItemType.InAppPurchase);
            if (purchase == null)
                return PurchaseResult.Cancelled;

            if (purchase.State != PurchaseState.Purchased)
                return PurchaseResult.Failed;

            await billing.FinalizePurchaseAsync(new[] { purchase.TransactionIdentifier });
            SetAdsRemoved(true);
            return PurchaseResult.Success;
        }
        catch (InAppBillingPurchaseException ex)
        {
            AppLogger.LogWarning($"PurchaseRemoveAdsAsync: {ex.PurchaseError}");
            return ex.PurchaseError == PurchaseError.UserCancelled ? PurchaseResult.Cancelled : PurchaseResult.Failed;
        }
        catch (Exception ex)
        {
            AppLogger.LogError("PurchaseRemoveAdsAsync failed", ex);
            return PurchaseResult.Failed;
        }
        finally
        {
            await billing.DisconnectAsync();
        }
#else
        await Task.CompletedTask;
        return PurchaseResult.NotSupported;
#endif
    }

    public async Task<PurchaseResult> RestorePurchasesAsync()
    {
#if ANDROID || IOS || MACCATALYST
        var billing = CrossInAppBilling.Current;
        try
        {
            var connected = await billing.ConnectAsync();
            if (!connected)
                return PurchaseResult.Failed;

            var purchases = await billing.GetPurchasesAsync(ItemType.InAppPurchase);
            var owned = purchases != null && purchases.Any(p => p.ProductId == RemoveAdsProductId);
            SetAdsRemoved(owned);
            return owned ? PurchaseResult.Success : PurchaseResult.NothingToRestore;
        }
        catch (Exception ex)
        {
            AppLogger.LogError("RestorePurchasesAsync failed", ex);
            return PurchaseResult.Failed;
        }
        finally
        {
            await billing.DisconnectAsync();
        }
#else
        await Task.CompletedTask;
        return PurchaseResult.NotSupported;
#endif
    }

    private void SetAdsRemoved(bool removed)
    {
        Preferences.Default.Set(AdsRemovedPreferenceKey, removed);
        AdsRemovedStateChanged?.Invoke(this, EventArgs.Empty);
    }
}
