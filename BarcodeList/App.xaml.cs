using BarcodeList.Services;
using BarcodeList.Tool;

namespace BarcodeList
{
    public partial class App : Application
    {
        private readonly PurchaseService _purchaseService;

        public App(PurchaseService purchaseService)
        {
            InitializeComponent();
            _purchaseService = purchaseService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override void OnStart()
        {
            base.OnStart();

            // 機種変更・再インストールでPreferencesの購入状態が消えても、
            // Google Play側の購入履歴から自動的に復元する。ユーザーへの通知は行わず、
            // 失敗しても通常の起動を妨げないよう、バックグラウンドで静かに実行する。
            _ = RestorePurchasesSilentlyAsync();
        }

        private async Task RestorePurchasesSilentlyAsync()
        {
            try
            {
                await _purchaseService.RestorePurchasesAsync();
            }
            catch (Exception ex)
            {
                AppLogger.LogWarning($"RestorePurchasesSilentlyAsync failed: {ex.Message}");
            }
        }
    }
}