using CommunityToolkit.Mvvm.ComponentModel;
using TTS_Live_UI.Contracts.ViewModels;

namespace TTS_Live_UI.ViewModels;

public class EditorDetailViewModel : ObservableRecipient, INavigationAware
{
    private string? _shortcutLabel;
    private string? _shortcutText;

    public string? ShortcutLabel
    {
        get => _shortcutLabel;
        set => SetProperty(ref _shortcutLabel, value);
    }

    public string? ShortcutText
    {
        get => _shortcutText;
        set => SetProperty(ref _shortcutText, value);
    }

    public EditorDetailViewModel()
    {
    }

    public void OnNavigatedTo(object parameter)
    {
        if (parameter is TextShortcut shortcut)
        {
            ShortcutLabel = shortcut.Label;
            ShortcutText = shortcut.Text;
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
