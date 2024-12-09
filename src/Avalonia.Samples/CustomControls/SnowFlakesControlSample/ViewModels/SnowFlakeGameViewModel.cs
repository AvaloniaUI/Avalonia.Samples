using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowFlakesControlSample.Models;

namespace SnowFlakesControlSample.ViewModels;

public partial class SnowFlakeGameViewModel : ViewModelBase
{
    public SnowFlakeGameViewModel()
    {
        _gameTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Background, OnGameTimerTick);
    }

    private void OnGameTimerTick(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(MilliSecondsRemaining));

        if (MilliSecondsRemaining <= 0)
        {
            _gameTimer.Stop();
            _stopwatch.Stop();
            IsGameRunning = false;
        }
    }

    private readonly DispatcherTimer _gameTimer;
    private readonly Stopwatch _stopwatch = new Stopwatch();
    
    public ObservableCollection<SnowFlake> SnowFlakes { get; } = new();
    
    [ObservableProperty] private bool _isGameRunning;
    [ObservableProperty] private int _score;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(MilliSecondsGameDuration))]
    private TimeSpan _gameDuration = TimeSpan.Zero;

    public double MilliSecondsRemaining => (GameDuration - _stopwatch.Elapsed).TotalMilliseconds;
    public double MilliSecondsGameDuration => GameDuration.TotalMilliseconds;

    [RelayCommand]
    private void StartGame()
    {
        SnowFlakes.Clear();
        Score = 0;
        
        for (int i = 0; i < 200; i++)
        {
            SnowFlakes.Add(new SnowFlake(
                Random.Shared.NextDouble(), 
                Random.Shared.NextDouble(), 
                Random.Shared.NextDouble() * 5 + 2,
                Random.Shared.NextDouble() / 5 + 0.1));
        }
        
        _stopwatch.Restart();
        GameDuration = TimeSpan.FromMinutes(1);
        IsGameRunning = true;
        _gameTimer.Start();
    }
}