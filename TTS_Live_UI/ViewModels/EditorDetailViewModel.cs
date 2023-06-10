using CommunityToolkit.Mvvm.ComponentModel;

using TTS_Live_UI.Contracts.ViewModels;
using TTS_Live_UI.Core.Contracts.Services;
using TTS_Live_UI.Core.Models;

namespace TTS_Live_UI.ViewModels;

public class EditorDetailViewModel : ObservableRecipient, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;
    private SampleOrder? _item;

    public SampleOrder? Item
    {
        get => _item;
        set => SetProperty(ref _item, value);
    }

    public EditorDetailViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        if (parameter is long orderID)
        {
            var data = await _sampleDataService.GetContentGridDataAsync();
            Item = data.First(i => i.OrderID == orderID);
        }
    }

    public void OnNavigatedFrom()
    {
    }
}
