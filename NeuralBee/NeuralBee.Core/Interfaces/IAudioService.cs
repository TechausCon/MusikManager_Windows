namespace NeuralBee.Core.Interfaces;

/// <summary>
/// Defines the contract for an audio playback service.
/// </summary>
public interface IAudioService
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
    /// Occurs when the playback state changes.
    /// </summary>
    event Action<PlaybackState> PlaybackStateChanged;

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
