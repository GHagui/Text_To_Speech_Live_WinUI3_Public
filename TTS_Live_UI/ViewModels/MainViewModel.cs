using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using TTS_Live_UI.Contracts.Services;
using TTS_Live_UI.Core.Contracts.Services;
using TTS_Live_UI.Core.Models;

namespace TTS_Live_UI.ViewModels;

/// <summary>
/// ViewModel for the main TTS page. Owns all speech logic, decoupled from the View.
/// Includes clipboard watch, audio ducking, word highlighting, and history integration.
/// </summary>
public partial class MainViewModel : ObservableRecipient, IDisposable
{
    private readonly ISpeechService _speechService;
    private readonly IHistoryService _historyService;
    private readonly IClipboardWatchService _clipboardService;
    private readonly ILogger<MainViewModel> _logger;

    private CancellationTokenSource? _typingCancelSource;
    private string _previousText = string.Empty;
    private int _lastSpokenPosition;
    private bool _disposed;

    // Regex to detect end-of-sentence punctuation
    private static readonly Regex SentenceEndPattern = new(@"[.!?;:\n]\s*$", RegexOptions.Compiled);

    // --- Observable Properties ---

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private bool _isLiveMode;

    [ObservableProperty]
    private bool _isSpeaking;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _statusText = "Converter em áudio";

    [ObservableProperty]
    private bool _canSpeak = true;

    [ObservableProperty]
    private bool _canCancel;

    [ObservableProperty]
    private bool _isProgressActive;

    [ObservableProperty]
    private int _volume = 100;

    [ObservableProperty]
    private int _rate;

    [ObservableProperty]
    private int _selectedFontSizeIndex = 2;

    [ObservableProperty]
    private double _currentFontSize = 16;

    [ObservableProperty]
    private VoiceDetail? _selectedVoice;

    [ObservableProperty]
    private int _debounceMs = 800;

    [ObservableProperty]
    private bool _duckOtherApps;

    [ObservableProperty]
    private bool _isClipboardWatchActive;

    // Word highlighting
    [ObservableProperty]
    private int _highlightStart;

    [ObservableProperty]
    private int _highlightLength;

    public ObservableCollection<VoiceDetail> InstalledVoices { get; } = new();
    public List<int> FontSizes { get; } = new() { 12, 14, 16, 18, 20, 24, 28, 32, 36, 40, 64 };

    public MainViewModel(
        ISpeechService speechService,
        IHistoryService historyService,
        IClipboardWatchService clipboardService,
        ILogger<MainViewModel> logger)
    {
        _speechService = speechService;
        _historyService = historyService;
        _clipboardService = clipboardService;
        _logger = logger;

        _speechService.StateChanged += OnSpeechStateChanged;
        _speechService.WordProgress += OnWordProgress;
        _clipboardService.TextCopied += OnClipboardTextCopied;

        LoadVoices();
    }

    private void LoadVoices()
    {
        try
        {
            var voices = _speechService.GetInstalledVoices();
            InstalledVoices.Clear();
            foreach (var voice in voices)
            {
                InstalledVoices.Add(voice);
            }

            var currentName = _speechService.CurrentVoiceName;
            SelectedVoice = InstalledVoices.FirstOrDefault(v => v.Name == currentName)
                            ?? InstalledVoices.FirstOrDefault();

            _logger.LogInformation("Loaded {Count} voices, selected: {Voice}", InstalledVoices.Count, SelectedVoice?.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load voices");
        }
    }

    // --- Property Change Handlers ---

    partial void OnIsLiveModeChanged(bool value)
    {
        if (value)
        {
            StatusText = "Ao vivo";
            CanSpeak = false;
            _previousText = InputText;
            _lastSpokenPosition = InputText.Length;
            _logger.LogInformation("Live mode activated");
        }
        else
        {
            StatusText = "Converter em áudio";
            CanSpeak = true;
            _logger.LogInformation("Live mode deactivated");
        }
    }

    partial void OnVolumeChanged(int value) => _speechService.Volume = value;
    partial void OnRateChanged(int value) => _speechService.Rate = value;

    partial void OnSelectedVoiceChanged(VoiceDetail? value)
    {
        if (value != null) _speechService.SelectVoice(value.Name);
    }

    partial void OnSelectedFontSizeIndexChanged(int value)
    {
        if (value >= 0 && value < FontSizes.Count)
            CurrentFontSize = FontSizes[value];
    }

    partial void OnDuckOtherAppsChanged(bool value)
    {
        _speechService.DuckOtherApps = value;
    }

    partial void OnIsClipboardWatchActiveChanged(bool value)
    {
        _clipboardService.IsWatching = value;
        _logger.LogInformation("Clipboard watch {State}", value ? "enabled" : "disabled");
    }

    // --- Clipboard Watch Handler ---

    private async void OnClipboardTextCopied(object? sender, string text)
    {
        if (!IsClipboardWatchActive || string.IsNullOrWhiteSpace(text)) return;

        try
        {
            _logger.LogInformation("Auto-speaking clipboard text");
            await _speechService.SpeakAsync(text);

            await _historyService.AddAsync(new ConversionHistoryItem
            {
                Text = text,
                VoiceName = _speechService.CurrentVoiceName,
                Rate = _speechService.Rate,
                Volume = _speechService.Volume,
                WasLiveMode = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to speak clipboard text");
        }
    }

    // --- Word Highlight Handler ---

    private void OnWordProgress(object? sender, WordProgressEventArgs e)
    {
        HighlightStart = e.CharacterPosition;
        HighlightLength = e.CharacterCount;
    }

    // --- Commands ---

    [RelayCommand]
    private async Task SpeakAsync()
    {
        if (string.IsNullOrWhiteSpace(InputText)) return;

        try
        {
            HasError = false;
            CanCancel = true;
            CanSpeak = false;
            IsProgressActive = true;
            StatusText = "Reproduzindo";

            await _speechService.SpeakAsync(InputText);

            // Add to history
            await _historyService.AddAsync(new ConversionHistoryItem
            {
                Text = InputText,
                VoiceName = _speechService.CurrentVoiceName,
                Rate = _speechService.Rate,
                Volume = _speechService.Volume,
                WasLiveMode = false
            });
        }
        catch (OperationCanceledException)
        {
            _logger.LogDebug("Speech was cancelled by user");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Speech failed");
            HasError = true;
        }
        finally
        {
            if (!IsLiveMode)
            {
                StatusText = "Converter em áudio";
                CanSpeak = true;
            }
            CanCancel = false;
            IsProgressActive = false;
            HighlightStart = 0;
            HighlightLength = 0;
        }
    }

    [RelayCommand]
    private void CancelSpeech()
    {
        _speechService.CancelAll();
        HighlightStart = 0;
        HighlightLength = 0;
        _logger.LogDebug("User cancelled speech");
    }

    [RelayCommand]
    private async Task SaveToFileAsync(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(InputText) || string.IsNullOrEmpty(filePath)) return;

        try
        {
            HasError = false;
            IsProgressActive = true;
            StatusText = "Salvando...";

            if (filePath.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
            {
                await _speechService.SaveToMp3FileAsync(InputText, filePath);
            }
            else
            {
                await _speechService.SaveToFileAsync(InputText, filePath);
            }

            // Add to history with file path
            await _historyService.AddAsync(new ConversionHistoryItem
            {
                Text = InputText,
                VoiceName = _speechService.CurrentVoiceName,
                Rate = _speechService.Rate,
                Volume = _speechService.Volume,
                SavedFilePath = filePath
            });

            StatusText = IsLiveMode ? "Ao vivo" : "Converter em áudio";
            _logger.LogInformation("File saved successfully: {Path}", filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save audio file");
            HasError = true;
            StatusText = "Erro ao salvar";
        }
        finally
        {
            IsProgressActive = false;
        }
    }

    /// <summary>
    /// Called by the View when the text input changes, to support Live mode.
    /// </summary>
    public async Task OnTextChangedAsync()
    {
        if (IsLiveMode)
        {
            _typingCancelSource?.Cancel();
            _typingCancelSource?.Dispose();
            _typingCancelSource = new CancellationTokenSource();
            var token = _typingCancelSource.Token;

            try
            {
                await Task.Delay(DebounceMs, token);

                var currentText = InputText;
                var newContent = GetNewContent(currentText, _lastSpokenPosition);

                if (!string.IsNullOrWhiteSpace(newContent) && SentenceEndPattern.IsMatch(newContent))
                {
                    HasError = false;
                    CanCancel = true;
                    IsProgressActive = true;

                    _logger.LogDebug("Live mode speaking: {Text}", newContent.Length > 50 ? newContent[..50] + "..." : newContent);

                    _ = _speechService.SpeakAsync(newContent, token);

                    // Add to history
                    _ = _historyService.AddAsync(new ConversionHistoryItem
                    {
                        Text = newContent,
                        VoiceName = _speechService.CurrentVoiceName,
                        Rate = _speechService.Rate,
                        Volume = _speechService.Volume,
                        WasLiveMode = true
                    });

                    _lastSpokenPosition = currentText.Length;
                    _previousText = currentText;
                }
            }
            catch (TaskCanceledException)
            {
                // User continued typing — expected
            }
        }
        else
        {
            _previousText = InputText;
            _lastSpokenPosition = InputText.Length;
        }
    }

    private static string GetNewContent(string currentText, int lastPosition)
    {
        if (currentText.Length > lastPosition)
            return currentText[lastPosition..].TrimStart();
        if (currentText.Length < lastPosition)
            return currentText.TrimStart();
        return string.Empty;
    }

    private void OnSpeechStateChanged(object? sender, SpeechStateChangedEventArgs e)
    {
        switch (e.State)
        {
            case SpeechState.Idle:
                IsSpeaking = false;
                IsProgressActive = false;
                CanCancel = false;
                HighlightStart = 0;
                HighlightLength = 0;
                if (!IsLiveMode) { CanSpeak = true; StatusText = "Converter em áudio"; }
                break;
            case SpeechState.Speaking:
                IsSpeaking = true;
                break;
            case SpeechState.Error:
                IsSpeaking = false;
                HasError = true;
                IsProgressActive = false;
                CanCancel = false;
                HighlightStart = 0;
                HighlightLength = 0;
                if (!IsLiveMode) { CanSpeak = true; StatusText = "Converter em áudio"; }
                _logger.LogWarning("Speech error: {Error}", e.ErrorMessage);
                break;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _speechService.StateChanged -= OnSpeechStateChanged;
            _speechService.WordProgress -= OnWordProgress;
            _clipboardService.TextCopied -= OnClipboardTextCopied;
            _typingCancelSource?.Cancel();
            _typingCancelSource?.Dispose();
            _disposed = true;
        }
    }
}
