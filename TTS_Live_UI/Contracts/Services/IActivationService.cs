namespace TTS_Live_UI.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
