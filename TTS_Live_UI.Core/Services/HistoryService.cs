using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TTS_Live_UI.Core.Contracts.Services;
using TTS_Live_UI.Core.Models;

namespace TTS_Live_UI.Core.Services;

/// <summary>
/// Persists TTS conversion history to a JSON file in the app's local data folder.
/// </summary>
public class HistoryService : IHistoryService
{
    private readonly string _historyFilePath;
    private readonly ILogger<HistoryService> _logger;
    private List<ConversionHistoryItem> _cache = new();
    private bool _loaded;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public HistoryService(ILogger<HistoryService> logger)
    {
        _logger = logger;
        var appData = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TTS_Live_UI");
        Directory.CreateDirectory(appData);
        _historyFilePath = Path.Combine(appData, "conversion_history.json");
    }

    public async Task<IReadOnlyList<ConversionHistoryItem>> GetAllAsync()
    {
        await EnsureLoadedAsync();
        return _cache.OrderByDescending(h => h.Timestamp).ToList().AsReadOnly();
    }

    public async Task AddAsync(ConversionHistoryItem item)
    {
        await EnsureLoadedAsync();
        await _lock.WaitAsync();
        try
        {
            _cache.Add(item);

            // Keep only last 500 entries
            if (_cache.Count > 500)
            {
                _cache = _cache.OrderByDescending(h => h.Timestamp).Take(500).ToList();
            }

            await SaveAsync();
            _logger.LogDebug("Added history entry: {Id}", item.Id);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task RemoveAsync(string id)
    {
        await EnsureLoadedAsync();
        await _lock.WaitAsync();
        try
        {
            _cache.RemoveAll(h => h.Id == id);
            await SaveAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task ClearAllAsync()
    {
        await _lock.WaitAsync();
        try
        {
            _cache.Clear();
            await SaveAsync();
            _logger.LogInformation("History cleared");
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task EnsureLoadedAsync()
    {
        if (_loaded) return;

        await _lock.WaitAsync();
        try
        {
            if (_loaded) return;

            if (File.Exists(_historyFilePath))
            {
                try
                {
                    var json = await File.ReadAllTextAsync(_historyFilePath);
                    _cache = JsonConvert.DeserializeObject<List<ConversionHistoryItem>>(json) ?? new();
                    _logger.LogInformation("Loaded {Count} history entries", _cache.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load history, starting fresh");
                    _cache = new();
                }
            }
            _loaded = true;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            var json = JsonConvert.SerializeObject(_cache, Formatting.Indented);
            await File.WriteAllTextAsync(_historyFilePath, json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save history");
        }
    }
}
