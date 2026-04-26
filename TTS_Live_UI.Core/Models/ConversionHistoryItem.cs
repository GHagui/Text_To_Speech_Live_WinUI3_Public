namespace TTS_Live_UI.Core.Models;

/// <summary>
/// Represents a single TTS conversion entry in the history log.
/// </summary>
public class ConversionHistoryItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Text { get; set; } = string.Empty;
    public string VoiceName { get; set; } = string.Empty;
    public int Rate { get; set; }
    public int Volume { get; set; }
    public bool WasLiveMode { get; set; }
    public string? SavedFilePath { get; set; }

    /// <summary>
    /// Short preview of the text for display in lists.
    /// </summary>
    public string TextPreview => Text.Length > 80 ? Text[..80] + "…" : Text;

    public string TimestampFormatted => Timestamp.ToString("dd/MM/yyyy HH:mm:ss");
}
