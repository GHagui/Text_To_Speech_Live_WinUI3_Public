using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using TTS_Live_UI.Core.Contracts.Services;

namespace TTS_Live_UI.ViewModels;

/// <summary>
/// Represents a user-defined text shortcut for quick TTS playback.
/// </summary>
public partial class TextShortcut : ObservableObject
{
    [ObservableProperty]
    private string _label = string.Empty;

    [ObservableProperty]
    private string _text = string.Empty;
}

/// <summary>
/// ViewModel for the shortcuts editor page.
/// </summary>
public partial class EditorViewModel : ObservableRecipient
{
    private readonly ISpeechService _speechService;
    private readonly ILogger<EditorViewModel> _logger;

    public ObservableCollection<TextShortcut> Shortcuts { get; } = new();

    [ObservableProperty]
    private string _newLabel = string.Empty;

    [ObservableProperty]
    private string _newText = string.Empty;

    [ObservableProperty]
    private TextShortcut? _selectedShortcut;

    public EditorViewModel(ISpeechService speechService, ILogger<EditorViewModel> logger)
    {
        _speechService = speechService;
        _logger = logger;

        // Add some default examples
        Shortcuts.Add(new TextShortcut { Label = "Saudação", Text = "Olá, tudo bem? Como posso ajudar?" });
        Shortcuts.Add(new TextShortcut { Label = "Agradecimento", Text = "Muito obrigado pela sua atenção!" });
        Shortcuts.Add(new TextShortcut { Label = "Despedida", Text = "Foi um prazer conversar com você. Até mais!" });
    }

    [RelayCommand]
    private void AddShortcut()
    {
        if (string.IsNullOrWhiteSpace(NewLabel) || string.IsNullOrWhiteSpace(NewText))
        {
            return;
        }

        Shortcuts.Add(new TextShortcut { Label = NewLabel, Text = NewText });
        _logger.LogInformation("Added shortcut: {Label}", NewLabel);
        NewLabel = string.Empty;
        NewText = string.Empty;
    }

    [RelayCommand]
    private void RemoveShortcut(TextShortcut? shortcut)
    {
        if (shortcut != null)
        {
            Shortcuts.Remove(shortcut);
            _logger.LogInformation("Removed shortcut: {Label}", shortcut.Label);
        }
    }

    [RelayCommand]
    private async Task SpeakShortcut(TextShortcut? shortcut)
    {
        if (shortcut == null || string.IsNullOrWhiteSpace(shortcut.Text)) return;

        try
        {
            _logger.LogInformation("Speaking shortcut: {Label}", shortcut.Label);
            await _speechService.SpeakAsync(shortcut.Text);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to speak shortcut: {Label}", shortcut.Label);
        }
    }
}
