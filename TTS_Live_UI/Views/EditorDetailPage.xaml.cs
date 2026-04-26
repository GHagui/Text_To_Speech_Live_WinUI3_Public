using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using TTS_Live_UI.ViewModels;

namespace TTS_Live_UI.Views;

public sealed partial class EditorDetailPage : Page
{
    public EditorDetailViewModel ViewModel { get; }

    public EditorDetailPage()
    {
        ViewModel = App.GetService<EditorDetailViewModel>();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
    }
}
