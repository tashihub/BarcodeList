namespace BarcodeList.Services;

/// <summary>
/// 「特定の画面をN回表示したら1回」のような頻度で広告を出すための、
/// キーごとの表示回数カウンター。端末に永続化されるため、アプリを再起動しても引き継がれる。
/// </summary>
public class AdFrequencyService
{
    private const string PreferenceKeyPrefix = "ad_frequency_count_";

    /// <summary>
    /// 呼ばれるたびにキーごとのカウントを1増やし、ちょうどevery回に達したタイミングでtrueを返す
    /// (その後カウントは0にリセットされ、次のevery回でまたtrueになる)。
    /// </summary>
    public bool ShouldShowAd(string key, int every)
    {
        var prefKey = PreferenceKeyPrefix + key;
        var count = Preferences.Default.Get(prefKey, 0) + 1;

        if (count >= every)
        {
            Preferences.Default.Set(prefKey, 0);
            return true;
        }

        Preferences.Default.Set(prefKey, count);
        return false;
    }
}
