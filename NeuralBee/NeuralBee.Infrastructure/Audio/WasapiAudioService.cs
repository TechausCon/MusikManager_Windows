using NeuralBee.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace NeuralBee.Infrastructure.Audio;

public class WasapiAudioService : IAudioService
{
    public PlaybackState CurrentState { get; private set; } = PlaybackState.Stopped;

    public double Volume { get; set; } = 1.0;

    public event Action<PlaybackState> PlaybackStateChanged;

    public Task LoadAsync(string trackPath)
    {
        // Placeholder for loading logic
        return Task.CompletedTask;
    }

    public void Pause()
    {
        if (CurrentState == PlaybackState.Playing)
        {
            CurrentState = PlaybackState.Paused;
            PlaybackStateChanged?.Invoke(CurrentState);
        }
    }

    public void Play()
    {
        if (CurrentState == PlaybackState.Paused || CurrentState == PlaybackState.Stopped)
        {
            CurrentState = PlaybackState.Playing;
            PlaybackStateChanged?.Invoke(CurrentState);
        }
    }

    public void Stop()
    {
        if (CurrentState != PlaybackState.Stopped)
        {
            CurrentState = PlaybackState.Stopped;
            PlaybackStateChanged?.Invoke(CurrentState);
        }
    }
}
