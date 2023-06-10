using Microsoft.UI.Xaml.Controls;

using TTS_Live_UI.ViewModels;

namespace TTS_Live_UI.Views;

public sealed partial class IntroduçãoPage : Page
{
    public IntroduçãoViewModel ViewModel
    {
        get;
    }

    public IntroduçãoPage()
    {
        ViewModel = App.GetService<IntroduçãoViewModel>();
        InitializeComponent();
    }
}
