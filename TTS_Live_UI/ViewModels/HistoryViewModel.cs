using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using TTS_Live_UI.Core.Contracts.Services;
using TTS_Live_UI.Core.Models;

namespace TTS_Live_UI.ViewModels;

/// <summary>
/// ViewModel for the conversion history page.
/// </summary>
public partial class HistoryViewModel : ObservableRecipient
{
    private readonly IHistoryService _historyService;
    private readonly ISpeechService _speechService;
    private readonly ILogger<HistoryViewModel> _logger;

    public ObservableCollection<ConversionHistoryItem> Items { get; } = new();

    [ObservableProperty]
    private ConversionHistoryItem? _selectedItem;

    [ObservableProperty]
    private bool _isEmpty = true;

    public HistoryViewModel(
        IHistoryService historyService,
        ISpeechService speechService,
        ILogger<HistoryViewModel> logger)
    {
        _historyService = historyService;
        _speechService = speechService;
        _logger = logger;
    }

    [RelayCommand]
    public async Task LoadHistoryAsync()
    {
        try
        {
            var items = await _historyService.GetAllAsync();
            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
            IsEmpty = Items.Count == 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load history");
        }
    }

    [RelayCommand]
    private async Task ReplayAsync(ConversionHistoryItem? item)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.Text)) return;

        try
        {
            _logger.LogInformation("Replaying history item: {Id}", item.Id);
            await _speechService.SpeakAsync(item.Text);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to replay history item");
        }
    }

    [RelayCommand]
    private async Task RemoveAsync(ConversionHistoryItem? item)
    {
        if (item == null) return;

        await _historyService.RemoveAsync(item.Id);
        Items.Remove(item);
        IsEmpty = Items.Count == 0;
    }

    [RelayCommand]
    private async Task ClearAllAsync()
    {
        await _historyService.ClearAllAsync();
        Items.Clear();
        IsEmpty = true;
        _logger.LogInformation("History cleared by user");
    }
}
