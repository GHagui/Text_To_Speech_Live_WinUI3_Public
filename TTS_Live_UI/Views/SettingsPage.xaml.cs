using Microsoft.UI.Xaml.Controls;

using TTS_Live_UI.ViewModels;
using Windows.System;

namespace TTS_Live_UI.Views;

// TODO: Set the URL for your privacy policy by updating SettingsPage_PrivacyTermsLink.NavigateUri in Resources.resw.
public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel
    {
        get;
    }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
    }
    private async void Privacy_policy(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new Uri("https://www.freeprivacypolicy.com/live/71694196-f890-498a-b12f-13b7f123a3b4"));
    }
    private async void Acionar_GitHub(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await Launcher.LaunchUriAsync(new Uri("https://github.com/CrashXBETAX/Text_To_Speech_Live_WinUI3_Public"));
    }
}
