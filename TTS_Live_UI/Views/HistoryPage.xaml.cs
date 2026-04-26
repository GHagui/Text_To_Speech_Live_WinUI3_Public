using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TTS_Live_UI.Core.Models;
using TTS_Live_UI.ViewModels;

namespace TTS_Live_UI.Views;

public sealed partial class HistoryPage : Page
{
    public HistoryViewModel ViewModel { get; }

    public HistoryPage()
    {
        ViewModel = App.GetService<HistoryViewModel>();
        InitializeComponent();
    }

    private async void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadHistoryAsync();
    }

    private void ReplayItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ConversionHistoryItem item)
        {
            ViewModel.ReplayCommand.Execute(item);
        }
    }

    private void RemoveItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ConversionHistoryItem item)
        {
            ViewModel.RemoveCommand.Execute(item);
        }
    }
}
