using TTS_Live_UI.Core.Models;

namespace TTS_Live_UI.Core.Contracts.Services;

/// <summary>
/// Abstracts text-to-speech operations for testability and decoupling.
/// </summary>
public interface ISpeechService : IDisposable
{
    /// <summary>
    /// Speaks the given text asynchronously through the default audio device.
    /// </summary>
    Task SpeakAsync(string text, CancellationToken ct = default);

    /// <summary>
    /// Saves spoken text to a WAV file at the specified path.
    /// </summary>
    Task SaveToFileAsync(string text, string filePath, CancellationToken ct = default);

    /// <summary>
    /// Saves spoken text to an MP3 file at the specified path.
    /// Internally generates WAV then converts to MP3.
    /// </summary>
    Task SaveToMp3FileAsync(string text, string filePath, CancellationToken ct = default);

    /// <summary>
    /// Cancels all pending and current speech operations.
    /// </summary>
    void CancelAll();

    /// <summary>
    /// Gets the list of installed TTS voices on the system.
    /// </summary>
    IReadOnlyList<VoiceDetail> GetInstalledVoices();

    /// <summary>
    /// Selects a voice by name.
    /// </summary>
    void SelectVoice(string voiceName);

    /// <summary>
    /// Gets or sets the speech rate (-10 to 10).
    /// </summary>
    int Rate { get; set; }

    /// <summary>
    /// Gets or sets the speech volume (0 to 100).
    /// </summary>
    int Volume { get; set; }

    /// <summary>
    /// Gets the name of the currently selected voice.
    /// </summary>
    string CurrentVoiceName { get; }

    /// <summary>
    /// Gets or sets whether to duck (lower volume of) other applications while speaking.
    /// </summary>
    bool DuckOtherApps { get; set; }

    /// <summary>
    /// Raised when the speech state changes (idle, speaking, error, etc.).
    /// </summary>
    event EventHandler<SpeechStateChangedEventArgs>? StateChanged;

    /// <summary>
    /// Raised for each word as it is being spoken. Used for word highlighting.
    /// </summary>
    event EventHandler<WordProgressEventArgs>? WordProgress;
}
