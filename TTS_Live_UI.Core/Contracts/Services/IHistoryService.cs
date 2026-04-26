using TTS_Live_UI.Core.Models;

namespace TTS_Live_UI.Core.Contracts.Services;

/// <summary>
/// Manages TTS conversion history persistence.
/// </summary>
public interface IHistoryService
{
    /// <summary>
    /// Gets all history items, most recent first.
    /// </summary>
    Task<IReadOnlyList<ConversionHistoryItem>> GetAllAsync();

    /// <summary>
    /// Adds a new conversion to the history.
    /// </summary>
    Task AddAsync(ConversionHistoryItem item);

    /// <summary>
    /// Removes a specific entry by ID.
    /// </summary>
    Task RemoveAsync(string id);

    /// <summary>
    /// Clears all history.
    /// </summary>
    Task ClearAllAsync();
}
