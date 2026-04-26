namespace TTS_Live_UI.Core.Models;

/// <summary>
/// Represents the current state of the speech engine.
/// </summary>
public enum SpeechState
{
    Idle,
    Speaking,
    Paused,
    Error
}

/// <summary>
/// Event arguments for speech state transitions.
/// </summary>
public class SpeechStateChangedEventArgs : EventArgs
{
    public SpeechState State { get; }
    public string? ErrorMessage { get; }

    public SpeechStateChangedEventArgs(SpeechState state, string? errorMessage = null)
    {
        State = state;
        ErrorMessage = errorMessage;
    }
}
