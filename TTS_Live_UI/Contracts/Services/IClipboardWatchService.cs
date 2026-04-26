namespace TTS_Live_UI.Contracts.Services;

/// <summary>
/// Monitors the system clipboard and raises events when text content changes.
/// </summary>
public interface IClipboardWatchService : IDisposable
{
    /// <summary>
    /// Gets or sets whether clipboard monitoring is active.
    /// </summary>
    bool IsWatching { get; set; }

    /// <summary>
    /// Raised when new text is detected on the clipboard.
    /// </summary>
    event EventHandler<string>? TextCopied;
}
