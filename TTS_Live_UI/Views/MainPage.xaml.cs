using Microsoft.UI.Xaml.Controls;
using TTS_Live_UI.ViewModels;
namespace TTS_Live_UI.Views;
using System.Speech.Synthesis;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using Windows.System;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }
    private readonly SpeechSynthesizer _synth;
    private string anterior = "";
    public List<int> FontSizes { get; set; } = new List<int>() { 12, 14, 16, 18, 20, 24, 28, 32, 36, 40, 64 };
    public MainPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<MainViewModel>();
        _synth = new SpeechSynthesizer();
        _synth.SetOutputToDefaultAudioDevice();
        EventHandler<SpeakCompletedEventArgs> synth_SpeakCompleted = Synth_SpeakCompleted;
        _synth.SpeakCompleted += synth_SpeakCompleted;
        Text_Start.TextChanged += Text_Start_TextChanged;
        OrLive.Toggled += (s, e) =>
        {
            if (OrLive.IsOn)
            {
                Start_TTS.Content = "Ao vivo";
                Start_TTS.IsEnabled = false;
            }
            else
            {
                Start_TTS.Content = "Converter em áudio";
                Start_TTS.IsEnabled = true;
            }
        };
    }

    private void Start_TTS_Click(SplitButton sender, SplitButtonClickEventArgs args)
    {
        ProgressBarStart.ShowError = false;
        ButtonInterromper.IsEnabled = true;
        Start_TTS.Content = "Reproduzindo";
        ProgressBarStart.IsIndeterminate = true;
        Start_TTS.IsEnabled = false;
        var text = new Prompt(Text_Start.Text);
        _synth.SpeakAsync(text);
    }

    private void Cancel_TTS(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        _synth.SpeakAsyncCancelAll();
    }

    private void SaveToFile(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {

    }
    private async void Text_Start_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (OrLive.IsOn)
        {
            await Task.Delay(3000);
            var atual = Text_Start.Text;
            var difference = GetDifference(atual, anterior);
            if (difference.Contains('.') || difference.Contains('?') || difference.Contains('!'))
            {
                ProgressBarStart.ShowError = false;
                ButtonInterromper.IsEnabled = true;
                ProgressBarStart.IsIndeterminate = true;
                _synth.SpeakAsync(new Prompt(difference));
                anterior = atual;
            }
        }

    }
    private void Synth_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
    {
        if (e.Error != null)
        {
            ProgressBarStart.ShowError = true;
        }
        else
        {
            if(OrLive.IsOn == false)
            {
                Start_TTS.Content = "Converter em áudio";
                Start_TTS.IsEnabled = true;

            }
            else
            {
                
                Start_TTS.IsEnabled = false;
            }
            ProgressBarStart.IsIndeterminate = false;
            ButtonInterromper.IsEnabled = false;
        }
    }
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        double fontSize;
        if (double.TryParse(e.AddedItems[0].ToString(), out fontSize))
        {
            Text_Start.FontSize = fontSize;
        }
    }
    public static string GetDifference(string text1, string text2)
    {
        var diff = new StringBuilder();
        var d = new List<string>(text1.Split(' '));
        var e = new List<string>(text2.Split(' '));
        var result = d.Except(e);
        foreach (var s in result)
        {
            diff.Append(s + " ");
        }
        return diff.ToString().Trim();
    }

    private async void Alterar_Voz(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new Uri("ms-settings:speech"));
    }
}
