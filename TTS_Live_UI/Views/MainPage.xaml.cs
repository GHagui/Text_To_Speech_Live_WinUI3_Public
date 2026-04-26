using Microsoft.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using TTS_Live_UI.ViewModels;
using System;
using System.ComponentModel;
using Windows.System;

namespace TTS_Live_UI.Views;

/// <summary>
/// Thin code-behind for MainPage. All business logic lives in MainViewModel.
/// Only UI-specific operations (file pickers, launcher, word highlighting) remain here.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel { get; }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();

        Unloaded += OnUnloaded;

        // Subscribe to word highlight changes for RichEditBox highlighting
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;

        // Sync initial text
        Text_Start.Document.SetText(TextSetOptions.None, ViewModel.InputText);
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.HighlightStart) || e.PropertyName == nameof(ViewModel.HighlightLength))
        {
            ApplyWordHighlight();
        }
    }

    /// <summary>
    /// Highlights the currently spoken word in the RichEditBox.
    /// </summary>
    private void ApplyWordHighlight()
    {
        try
        {
            var doc = Text_Start.Document;

            // Clear all formatting first
            var fullRange = doc.GetRange(0, TextConstants.MaxUnitCount);
            fullRange.CharacterFormat.BackgroundColor = Colors.Transparent;
            fullRange.CharacterFormat.ForegroundColor = 
                ActualTheme == Microsoft.UI.Xaml.ElementTheme.Dark ? Colors.White : Colors.Black;

            // Apply highlight to current word
            if (ViewModel.HighlightLength > 0)
            {
                var highlightRange = doc.GetRange(
                    ViewModel.HighlightStart,
                    ViewModel.HighlightStart + ViewModel.HighlightLength);
                highlightRange.CharacterFormat.BackgroundColor = Colors.Yellow;
                highlightRange.CharacterFormat.ForegroundColor = Colors.Black;
            }
        }
        catch
        {
            // Ignore highlight errors — non-critical UI feature
        }
    }

    private void Start_TTS_Click(SplitButton sender, SplitButtonClickEventArgs args)
    {
        SyncTextToViewModel();
        ViewModel.SpeakCommand.Execute(null);
    }

    private void Cancel_TTS(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.CancelSpeechCommand.Execute(null);
    }

    private async void SaveToFileWav(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await SaveToFileAsync(".wav", "Arquivo de Áudio WAV");
    }

    private async void SaveToFileMp3(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await SaveToFileAsync(".mp3", "Arquivo de Áudio MP3");
    }

    private async Task SaveToFileAsync(string extension, string description)
    {
        SyncTextToViewModel();
        if (string.IsNullOrWhiteSpace(ViewModel.InputText)) return;

        try
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add(description, new System.Collections.Generic.List<string>() { extension });
            savePicker.SuggestedFileName = "Áudio_TTS";

            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                await ViewModel.SaveToFileCommand.ExecuteAsync(file.Path);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SaveToFile error: {ex.Message}");
        }
    }

    private async void Text_Start_TextChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        SyncTextToViewModel();
        await ViewModel.OnTextChangedAsync();
    }

    /// <summary>
    /// Syncs the RichEditBox text content to the ViewModel's InputText property.
    /// </summary>
    private void SyncTextToViewModel()
    {
        Text_Start.Document.GetText(TextGetOptions.UseLf, out var text);
        ViewModel.InputText = text?.TrimEnd('\r', '\n') ?? string.Empty;
    }

    private async void Alterar_Voz(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new Uri("ms-settings:speech"));
    }

    private void OnUnloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        ViewModel.Dispose();
    }
}
