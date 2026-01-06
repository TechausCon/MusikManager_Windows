namespace NeuralBee.Core.Interfaces;

/// <summary>
/// Defines the contract for an audio playback service.
/// </summary>
public interface IAudioService : IDisposable
{
    /// <summary>
    /// Gets the current playback state.
    /// </summary>
    PlaybackState CurrentState { get; }

    /// <summary>
    /// Gets or sets the current playback volume (0.0 to 1.0).
    /// </summary>
    double Volume { get; set; }

    /// <summary>
    /// Gets or sets the current playback position.
    /// </summary>
    TimeSpan Position { get; set; }

    /// <summary>
    /// Gets the total duration of the currently loaded track.
    /// </summary>
    TimeSpan Duration { get; }

    /// <summary>
    /// Occurs when the playback state changes.
    /// </summary>
    event Action<PlaybackState> PlaybackStateChanged;

    /// <summary>
    /// Occurs when the media has finished playing.
    /// </summary>
    event Action MediaEnded;

    /// <summary>
    /// Loads a track for playback.
    /// </summary>
    /// <param name="trackPath">The file path of the track to load.</param>
    Task LoadAsync(string trackPath);

    /// <summary>
    /// Starts or resumes playback.
    /// </summary>
    void Play();

    /// <summary>
    /// Pauses playback.
    /// </summary>
    void Pause();

    /// <summary>
    /// Stops playback and unloads the current track.
    /// </summary>
    void Stop();
}

public enum PlaybackState
{
    Stopped,
    Playing,
    Paused
}
