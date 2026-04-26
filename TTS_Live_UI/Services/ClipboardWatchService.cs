using Microsoft.Extensions.Logging;
using TTS_Live_UI.Contracts.Services;
using Windows.ApplicationModel.DataTransfer;

namespace TTS_Live_UI.Services;

/// <summary>
/// Monitors the Windows clipboard for text content changes.
/// Uses the WinRT Clipboard.ContentChanged event.
/// </summary>
public class ClipboardWatchService : IClipboardWatchService
{
    private readonly ILogger<ClipboardWatchService> _logger;
    private bool _isWatching;
    private string _lastClipboardText = string.Empty;
    private bool _disposed;

    public event EventHandler<string>? TextCopied;

    public bool IsWatching
    {
        get => _isWatching;
        set
        {
            if (_isWatching == value) return;
            _isWatching = value;

            if (value)
            {
                Clipboard.ContentChanged += OnClipboardContentChanged;
                _logger.LogInformation("Clipboard watch started");
            }
            else
            {
                Clipboard.ContentChanged -= OnClipboardContentChanged;
                _logger.LogInformation("Clipboard watch stopped");
            }
        }
    }

    public ClipboardWatchService(ILogger<ClipboardWatchService> logger)
    {
        _logger = logger;
    }

    private async void OnClipboardContentChanged(object? sender, object e)
    {
        if (!_isWatching) return;

        try
        {
            var content = Clipboard.GetContent();
            if (content.Contains(StandardDataFormats.Text))
            {
                var text = await content.GetTextAsync();
                if (!string.IsNullOrWhiteSpace(text) && text != _lastClipboardText)
                {
                    _lastClipboardText = text;
                    _logger.LogDebug("Clipboard text detected: {Preview}",
                        text.Length > 50 ? text[..50] + "..." : text);
                    TextCopied?.Invoke(this, text);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read clipboard content");
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            IsWatching = false;
            _disposed = true;
        }
    }
}
