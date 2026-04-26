namespace TTS_Live_UI.Contracts.Services;

/// <summary>
/// Represents a registered global hotkey action.
/// </summary>
public record HotkeyDefinition(int Id, string Name, uint Modifiers, uint VirtualKey);

/// <summary>
/// Manages system-wide (global) hotkeys via Win32 RegisterHotKey.
/// </summary>
public interface IGlobalHotkeyService : IDisposable
{
    /// <summary>
    /// Registers a global hotkey. Returns true if successful.
    /// </summary>
    bool RegisterHotkey(HotkeyDefinition hotkey);

    /// <summary>
    /// Unregisters a previously registered hotkey.
    /// </summary>
    void UnregisterHotkey(int id);

    /// <summary>
    /// Raised when a registered hotkey is pressed.
    /// </summary>
    event EventHandler<int>? HotkeyPressed;
}
