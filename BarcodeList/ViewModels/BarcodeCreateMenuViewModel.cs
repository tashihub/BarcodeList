using BarcodeList.Services.CreateServices;
using BarcodeList.Views.Create;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace BarcodeList.ViewModels
{
    public partial class BarcodeCreateMenuViewModel : ObservableObject
    {
        /// <summary>
        /// フォーマットごとのメニュー項目一覧。BarcodeFormatCatalogにエントリを足すだけで
        /// このメニューにも自動的に反映される。
        /// </summary>
        public IReadOnlyList<BarcodeFormatDefinition> Formats { get; } = BarcodeFormatCatalog.All;

        [RelayCommand]
        private async Task OpenFormatCreate(BarcodeFormatDefinition format)
        {
            if (format == null)
                return;

            await Shell.Current.GoToAsync(nameof(BarcodeCreateView),
                new Dictionary<string, object>
                {
                    ["Format"] = format.Format
                });
        }

        [RelayCommand]
        private async Task OpenGs1128Create()
        {
            await Shell.Current.GoToAsync(nameof(Gs1128CreateView));
        }
    }
}
