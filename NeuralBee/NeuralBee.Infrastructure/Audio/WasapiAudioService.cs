using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using NeuralBee.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

// Alias to resolve ambiguity
using CorePlaybackState = NeuralBee.Core.Interfaces.PlaybackState;

namespace NeuralBee.Infrastructure.Audio;

public class WasapiAudioService : IAudioService
{
    private ISoundOut? _soundOut;
    private IWaveSource? _waveSource;
    private CorePlaybackState _currentState = CorePlaybackState.Stopped;
    private float _volume = 1.0f;
    private readonly SemaphoreSlim _loadingLock = new(1, 1);

    public CorePlaybackState CurrentState
    {
        get => _currentState;
        private set
        {
            if (_currentState != value)
            {
                _currentState = value;
                PlaybackStateChanged?.Invoke(_currentState);
            }
        }
    }

    public double Volume
    {
        get => _volume;
        set
        {
            _volume = (float)Math.Clamp(value, 0.0, 1.0);
            if (_soundOut != null)
            {
                _soundOut.Volume = _volume;
            }
        }
    }

    public TimeSpan Position
    {
        get => _waveSource?.GetPosition() ?? TimeSpan.Zero;
        set
        {
            if (_waveSource != null)
            {
                // Clamp position to duration
                if (value > Duration) value = Duration;
                if (value < TimeSpan.Zero) value = TimeSpan.Zero;

                _waveSource.SetPosition(value);
            }
        }
    }

    public TimeSpan Duration => _waveSource?.GetLength() ?? TimeSpan.Zero;

    public event Action<CorePlaybackState>? PlaybackStateChanged;
    public event Action? MediaEnded;

    public async Task LoadAsync(string trackPath)
    {
        await _loadingLock.WaitAsync();
        try
        {
            StopInternal(); // Stop any existing playback synchronously
            Cleanup();      // Clean up old resources

            // Run codec creation on a background thread
            await Task.Run(() =>
            {
                try
                {
                    _waveSource = CodecFactory.Instance.GetCodec(trackPath)
                        .ToSampleSource()
                        .ToWaveSource();

                    _soundOut = new WasapiOut();
                    _soundOut.Initialize(_waveSource);
                    _soundOut.Volume = _volume;
                    _soundOut.Stopped += OnPlaybackStopped;
                }
                catch (Exception)
                {
                    Cleanup();
                    throw;
                }
            });
        }
        finally
        {
            _loadingLock.Release();
        }
    }

    private void OnPlaybackStopped(object? sender, PlaybackStoppedEventArgs e)
    {
        // Differentiate between user-initiated Stop() and natural end of track
        if (CurrentState == CorePlaybackState.Playing && _waveSource != null && _waveSource.GetPosition() >= _waveSource.GetLength())
        {
            MediaEnded?.Invoke();
        }

        CurrentState = CorePlaybackState.Stopped;
    }

    public void Play()
    {
        if (_soundOut != null && CurrentState != CorePlaybackState.Playing)
        {
            _soundOut.Play();
            CurrentState = CorePlaybackState.Playing;
        }
    }

    public void Pause()
    {
        if (_soundOut != null && CurrentState == CorePlaybackState.Playing)
        {
            _soundOut.Pause();
            CurrentState = CorePlaybackState.Paused;
        }
    }

    public void Stop()
    {
        StopInternal();
    }

    private void StopInternal()
    {
        if (_soundOut != null)
        {
            _soundOut.Stop();
            // CurrentState update is handled in OnPlaybackStopped
        }
        else
        {
            CurrentState = CorePlaybackState.Stopped;
        }
    }

    private void Cleanup()
    {
        if (_soundOut != null)
        {
            _soundOut.Stopped -= OnPlaybackStopped;
            _soundOut.Dispose();
            _soundOut = null;
        }

        if (_waveSource != null)
        {
            _waveSource.Dispose();
            _waveSource = null;
        }
    }

    public void Dispose()
    {
        Stop();
        Cleanup();
        _loadingLock.Dispose();
        GC.SuppressFinalize(this);
    }
}
