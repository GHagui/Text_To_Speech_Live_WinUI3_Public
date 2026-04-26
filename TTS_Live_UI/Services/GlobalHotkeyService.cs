using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using TTS_Live_UI.Contracts.Services;

namespace TTS_Live_UI.Services;

/// <summary>
/// Manages system-wide hotkeys using Win32 RegisterHotKey/UnregisterHotKey.
/// Uses a hidden message window to receive WM_HOTKEY messages.
/// </summary>
public class GlobalHotkeyService : IGlobalHotkeyService
{
    private readonly ILogger<GlobalHotkeyService> _logger;
    private readonly Dictionary<int, HotkeyDefinition> _registeredHotkeys = new();
    private bool _disposed;
    private IntPtr _hwnd;
    private System.Threading.Timer? _pollTimer;

    // Win32 constants
    private const int WM_HOTKEY = 0x0312;

    // Modifier keys
    public const uint MOD_ALT = 0x0001;
    public const uint MOD_CONTROL = 0x0002;
    public const uint MOD_SHIFT = 0x0004;
    public const uint MOD_WIN = 0x0008;
    public const uint MOD_NOREPEAT = 0x4000;

    // Virtual key codes
    public const uint VK_S = 0x53;
    public const uint VK_L = 0x4C;
    public const uint VK_C = 0x43;
    public const uint VK_H = 0x48;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public event EventHandler<int>? HotkeyPressed;

    public GlobalHotkeyService(ILogger<GlobalHotkeyService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Sets the window handle to use for hotkey messages.
    /// Must be called after window creation.
    /// </summary>
    public void SetWindowHandle(IntPtr hwnd)
    {
        _hwnd = hwnd;
        _logger.LogDebug("Window handle set for global hotkeys: {Handle}", hwnd);
    }

    public bool RegisterHotkey(HotkeyDefinition hotkey)
    {
        if (_hwnd == IntPtr.Zero)
        {
            _logger.LogWarning("Cannot register hotkey — window handle not set");
            return false;
        }

        try
        {
            var result = RegisterHotKey(_hwnd, hotkey.Id, hotkey.Modifiers | MOD_NOREPEAT, hotkey.VirtualKey);
            if (result)
            {
                _registeredHotkeys[hotkey.Id] = hotkey;
                _logger.LogInformation("Registered global hotkey: {Name} (ID: {Id})", hotkey.Name, hotkey.Id);
                return true;
            }
            else
            {
                var error = Marshal.GetLastWin32Error();
                _logger.LogWarning("Failed to register hotkey {Name}: Win32 error {Error}", hotkey.Name, error);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception registering hotkey {Name}", hotkey.Name);
            return false;
        }
    }

    public void UnregisterHotkey(int id)
    {
        if (_hwnd == IntPtr.Zero) return;

        try
        {
            UnregisterHotKey(_hwnd, id);
            _registeredHotkeys.Remove(id);
            _logger.LogDebug("Unregistered hotkey ID: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unregister hotkey ID: {Id}", id);
        }
    }

    /// <summary>
    /// Process a Windows message. Call this from the window's message handler.
    /// Returns true if the message was a hotkey message.
    /// </summary>
    public bool ProcessMessage(uint msg, IntPtr wParam)
    {
        if (msg == WM_HOTKEY)
        {
            var id = (int)wParam;
            if (_registeredHotkeys.ContainsKey(id))
            {
                _logger.LogDebug("Hotkey pressed: {Name}", _registeredHotkeys[id].Name);
                HotkeyPressed?.Invoke(this, id);
                return true;
            }
        }
        return false;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            foreach (var id in _registeredHotkeys.Keys.ToList())
            {
                UnregisterHotkey(id);
            }
            _pollTimer?.Dispose();
            _disposed = true;
        }
    }
}
