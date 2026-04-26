namespace TTS_Live_UI.Core.Models;

/// <summary>
/// Represents an installed text-to-speech voice.
/// </summary>
public class VoiceDetail
{
    public string Name { get; set; } = string.Empty;
    public string Culture { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;

    public override string ToString() => $"{Name} ({Culture})";
}
