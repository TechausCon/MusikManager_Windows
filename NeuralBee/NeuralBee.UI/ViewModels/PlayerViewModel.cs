using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NeuralBee.Core.Interfaces;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace NeuralBee.UI.ViewModels;

public partial class PlayerViewModel : ObservableObject, IDisposable
{
    private readonly IAudioService _audioService;
    private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;
    private readonly System.Timers.Timer _positionTimer;
    private bool _isDraggingSeek;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PlayPauseIcon))]
    private bool _isPlaying;

    [ObservableProperty]
    private double _volume = 1.0;

    [ObservableProperty]
    private TimeSpan _currentPosition;

    [ObservableProperty]
    private TimeSpan _totalDuration;

    [ObservableProperty]
    private string _trackTitle = "No Track Loaded";

    public string PlayPauseIcon => IsPlaying ? "\uE769" : "\uE768";

    public PlayerViewModel(IAudioService audioService)
    {
        _audioService = audioService;
        _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

        _audioService.PlaybackStateChanged += OnPlaybackStateChanged;
        _audioService.MediaEnded += OnMediaEnded;

        _positionTimer = new System.Timers.Timer(500);
        _positionTimer.Elapsed += OnPositionTimerTick;
    }

    private void OnPositionTimerTick(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (_isDraggingSeek || _audioService.CurrentState != PlaybackState.Playing) return;

        var pos = _audioService.Position;
        _dispatcherQueue.TryEnqueue(() =>
        {
            CurrentPosition = pos;
        });
    }

    private void OnPlaybackStateChanged(PlaybackState state)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            IsPlaying = state == PlaybackState.Playing;
            if (IsPlaying) _positionTimer.Start();
            else _positionTimer.Stop();
        });
    }

    private void OnMediaEnded()
    {
         _dispatcherQueue.TryEnqueue(() =>
        {
            IsPlaying = false;
            CurrentPosition = TimeSpan.Zero;
            _positionTimer.Stop();
        });
    }

    [RelayCommand]
    private void PlayPause()
    {
        if (_audioService.CurrentState == PlaybackState.Playing)
        {
            _audioService.Pause();
        }
        else
        {
            _audioService.Play();
        }
    }

    [RelayCommand]
    private void Stop()
    {
        _audioService.Stop();
    }

    [RelayCommand]
    private async Task OpenFile()
    {
        var window = App.MainWindow;
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        var picker = new FileOpenPicker();
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        picker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
        picker.FileTypeFilter.Add(".mp3");
        picker.FileTypeFilter.Add(".wav");
        picker.FileTypeFilter.Add(".flac");
        picker.FileTypeFilter.Add(".m4a");

        var file = await picker.PickSingleFileAsync();
        if (file != null)
        {
            try
            {
                TrackTitle = file.Name;
                await _audioService.LoadAsync(file.Path);
                TotalDuration = _audioService.Duration;
                CurrentPosition = TimeSpan.Zero;
                _audioService.Play();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading file: {ex.Message}");
            }
        }
    }

    partial void OnVolumeChanged(double value)
    {
        _audioService.Volume = value;
    }

    public void StartSeek()
    {
        _isDraggingSeek = true;
    }

    public void EndSeek(double newSeconds)
    {
        _isDraggingSeek = false;
        _audioService.Position = TimeSpan.FromSeconds(newSeconds);
        CurrentPosition = TimeSpan.FromSeconds(newSeconds);
    }

    public void Dispose()
    {
        _positionTimer.Stop();
        _positionTimer.Dispose();
        _audioService.PlaybackStateChanged -= OnPlaybackStateChanged;
        _audioService.MediaEnded -= OnMediaEnded;
        GC.SuppressFinalize(this);
    }
}
