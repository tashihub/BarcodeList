namespace BarcodeList.Tool;

/// <summary>
/// クラッシュ・エラー発生時に原因調査ができるよう、端末のアプリ専用ストレージにログをファイル保存する。
/// 外部送信は行わない(端末内のみ)。取得するには adb pull などで端末から直接ログファイルを取り出す。
/// </summary>
public static class AppLogger
{
    private const long MaxLogSizeBytes = 1 * 1024 * 1024; // 1MB

    private static readonly object _lock = new();

    private static string LogFilePath => Path.Combine(FileSystem.AppDataDirectory, "applog.txt");

    public static void LogInfo(string message) => Write("INFO", message);

    public static void LogWarning(string message) => Write("WARN", message);

    public static void LogError(string message, Exception? exception = null) =>
        Write("ERROR", exception == null ? message : $"{message}{Environment.NewLine}{exception}");

    private static void Write(string level, string message)
    {
        try
        {
            lock (_lock)
            {
                TrimIfTooLarge();
                var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}{Environment.NewLine}";
                File.AppendAllText(LogFilePath, line);
            }
        }
        catch
        {
            // ロギング自体が失敗してもアプリを落とさない
        }
    }

    private static void TrimIfTooLarge()
    {
        if (!File.Exists(LogFilePath))
            return;

        var info = new FileInfo(LogFilePath);
        if (info.Length <= MaxLogSizeBytes)
            return;

        // 肥大化を防ぐため、古い前半を捨てて残す簡易ローテーション
        var lines = File.ReadAllLines(LogFilePath);
        File.WriteAllLines(LogFilePath, lines.Skip(lines.Length / 2));
    }
}
