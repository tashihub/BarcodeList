using BarcodeList.Services.CreateServices;
using BarcodeList.Tool;
using BarcodeList.Views.Result;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui;
using System.Collections.ObjectModel;

namespace BarcodeList.ViewModels.Create;

public partial class Gs1128CreateViewModel : ObservableObject
{
    [ObservableProperty]
    private string newAiCode = "";

    [ObservableProperty]
    private string newAiValue = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = "";

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public ObservableCollection<Gs1Element> Elements { get; } = new();

    public bool HasElements => Elements.Count > 0;

    /// <summary>
    /// 対応AIコード一覧(参考表示用)。ここにないAIコードも作成はできるが、
    /// このアプリでの再読み込み時の内訳表示は保証されない。
    /// </summary>
    public IReadOnlyList<Gs1AiReferenceItem> KnownAiReference { get; } = Gs1AiTable.GetReferenceList();

    /// <summary>
    /// 入力中のAIコードが既知AIかどうかをリアルタイムで表示するためのヒント文言(名前+入力形式)。
    /// </summary>
    public string NewAiNameHint
    {
        get
        {
            if (string.IsNullOrWhiteSpace(NewAiCode))
                return "";

            var name = Gs1AiTable.GetAiName(NewAiCode);
            var formatHint = Gs1AiTable.GetFormatHint(NewAiCode);
            return string.IsNullOrEmpty(formatHint) ? $"→ {name}" : $"→ {name}({formatHint})";
        }
    }

    /// <summary>既知AIが数字のみの場合は数字キーボードを表示する。</summary>
    public Keyboard NewAiValueKeyboard => Gs1AiTable.IsNumericOnly(NewAiCode) ? Keyboard.Numeric : Keyboard.Default;

    /// <summary>既知AIの桁数に応じた入力欄のMaxLength(0以下は無制限)。</summary>
    public int NewAiValueMaxLength
    {
        get
        {
            var maxLength = Gs1AiTable.GetMaxLength(NewAiCode);
            return maxLength > 0 ? maxLength : int.MaxValue;
        }
    }

    private readonly Gs1128CreateService _gs1128Service;
    public Gs1128CreateViewModel(Gs1128CreateService gs1128Service)
    {
        _gs1128Service = gs1128Service;
        Elements.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasElements));
    }

    partial void OnNewAiCodeChanged(string value)
    {
        OnPropertyChanged(nameof(NewAiNameHint));
        OnPropertyChanged(nameof(NewAiValueKeyboard));
        OnPropertyChanged(nameof(NewAiValueMaxLength));
    }

    [RelayCommand]
    private void AddElement()
    {
        var aiError = _gs1128Service.ValidateAiCode(NewAiCode);
        if (!string.IsNullOrEmpty(aiError))
        {
            ErrorMessage = aiError;
            return;
        }

        var valueError = _gs1128Service.ValidateValue(NewAiCode, NewAiValue);
        if (!string.IsNullOrEmpty(valueError))
        {
            ErrorMessage = valueError;
            return;
        }

        Elements.Add(new Gs1Element
        {
            Ai = NewAiCode,
            Name = Gs1AiTable.GetAiName(NewAiCode),
            Value = NewAiValue
        });

        NewAiCode = "";
        NewAiValue = "";
        ErrorMessage = "";
    }

    [RelayCommand]
    private void RemoveElement(Gs1Element element)
    {
        if (element == null)
            return;

        Elements.Remove(element);
    }

    [RelayCommand]
    private async Task Create()
    {
        if (Elements.Count == 0)
        {
            ErrorMessage = "AI要素を1つ以上追加してください";
            return;
        }

        var gs1Value = Gs1128CreateService.BuildGs1Value(Elements);

        await _gs1128Service.SaveBarcodeToHistory(gs1Value, 0);

        await Shell.Current.GoToAsync(
            nameof(Gs1128ResultView),
            new Dictionary<string, object>
            {
                ["Gs1Value"] = gs1Value
            });
    }
}
