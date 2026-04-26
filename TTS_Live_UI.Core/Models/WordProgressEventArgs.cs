namespace TTS_Live_UI.Core.Models;

/// <summary>
/// Event arguments for word-by-word speech progress tracking.
/// Used to highlight the current word being spoken.
/// </summary>
public class WordProgressEventArgs : EventArgs
{
    public int CharacterPosition { get; }
    public int CharacterCount { get; }
    public string CurrentWord { get; }

    public WordProgressEventArgs(int characterPosition, int characterCount, string currentWord)
    {
        CharacterPosition = characterPosition;
        CharacterCount = characterCount;
        CurrentWord = currentWord;
    }
}
