using BarcodeList.Resources.Strings;
using BarcodeList.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BarcodeList.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private const string LanguagePreferenceKey = "app_language";

    // インデックス0=端末既定、1=日本語、2=英語。MauiProgram.ApplySavedLanguagePreferenceの空文字判定と対応させている。
    private static readonly string[] LanguageCodes = { string.Empty, "ja", "en" };

    private readonly PurchaseService _purchaseService;

    public string AppVersionText => string.Format(AppResources.Settings_AppVersionFormat, AppInfo.Current.VersionString);

    [ObservableProperty]
    private bool isAdsRemoved;

    [ObservableProperty]
    private int selectedLanguageIndex;

    public SettingsViewModel(PurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
        _purchaseService.AdsRemovedStateChanged += OnAdsRemovedStateChanged;
        isAdsRemoved = _purchaseService.IsAdsRemoved;

        var savedLanguage = Preferences.Default.Get(LanguagePreferenceKey, string.Empty);
        var index = Array.IndexOf(LanguageCodes, savedLanguage);
        selectedLanguageIndex = index >= 0 ? index : 0;
    }

    private void OnAdsRemovedStateChanged(object? sender, EventArgs e)
    {
        IsAdsRemoved = _purchaseService.IsAdsRemoved;
    }

    partial void OnSelectedLanguageIndexChanged(int value)
    {
        if (value < 0 || value >= LanguageCodes.Length)
            return;

        var code = LanguageCodes[value];
        if (Preferences.Default.Get(LanguagePreferenceKey, string.Empty) == code)
            return;

        Preferences.Default.Set(LanguagePreferenceKey, code);
        _ = Shell.Current.DisplayAlertAsync(
            AppResources.Settings_LanguageRestartTitle,
            AppResources.Settings_LanguageRestartMessage,
            AppResources.Common_OK);
    }

    [RelayCommand]
    private async Task RemoveAds()
    {
        var result = await _purchaseService.PurchaseRemoveAdsAsync();

        switch (result)
        {
            case PurchaseResult.Success:
                await Shell.Current.DisplayAlertAsync(AppResources.Settings_PurchaseSuccessTitle, AppResources.Settings_PurchaseSuccessMessage, AppResources.Common_OK);
                break;
            case PurchaseResult.Cancelled:
                break;
            case PurchaseResult.NotSupported:
                await Shell.Current.DisplayAlertAsync(AppResources.Settings_ComingSoonTitle, AppResources.Settings_PurchaseNotSupportedMessage, AppResources.Common_OK);
                break;
            default:
                await Shell.Current.DisplayAlertAsync(AppResources.Settings_PurchaseFailedTitle, AppResources.Settings_PurchaseFailedMessage, AppResources.Common_OK);
                break;
        }
    }

    [RelayCommand]
    private async Task RestorePurchases()
    {
        var result = await _purchaseService.RestorePurchasesAsync();

        switch (result)
        {
            case PurchaseResult.Success:
                await Shell.Current.DisplayAlertAsync(AppResources.Settings_RestoreSuccessTitle, AppResources.Settings_RestoreSuccessMessage, AppResources.Common_OK);
                break;
            case PurchaseResult.NothingToRestore:
                await Shell.Current.DisplayAlertAsync(AppResources.Settings_RestoreNothingTitle, AppResources.Settings_RestoreNothingMessage, AppResources.Common_OK);
                break;
            case PurchaseResult.NotSupported:
                await Shell.Current.DisplayAlertAsync(AppResources.Settings_ComingSoonTitle, AppResources.Settings_PurchaseNotSupportedMessage, AppResources.Common_OK);
                break;
            default:
                await Shell.Current.DisplayAlertAsync(AppResources.Settings_PurchaseFailedTitle, AppResources.Settings_PurchaseFailedMessage, AppResources.Common_OK);
                break;
        }
    }

    [RelayCommand]
    private async Task ShowPrivacyPolicy()
    {
        await Shell.Current.GoToAsync(nameof(Views.PrivacyPolicyView));
    }
}
