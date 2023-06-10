using Microsoft.UI.Xaml.Controls;

using TTS_Live_UI.ViewModels;

namespace TTS_Live_UI.Views;

public sealed partial class EditorPage : Page
{
    public EditorViewModel ViewModel
    {
        get;
    }

    public EditorPage()
    {
        ViewModel = App.GetService<EditorViewModel>();
        InitializeComponent();
    }
}
