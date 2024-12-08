using System;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowFlakesControlSample.Models;

namespace SnowFlakesControlSample.ViewModels;

public partial class SnowFlakeGameViewModel : ViewModelBase
{
    public SnowFlakeGameViewModel()
    {
        _gameTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(50), DispatcherPriority.Background, OnGameTimerTick);
    }

    private void OnGameTimerTick(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(TimeRemaining));
        OnPropertyChanged(nameof(SecondsRemaining));

        if (TimeRemaining < TimeSpan.Zero)
        {
            _gameTimer.Stop();
            IsGameRunning = false;
        }
    }

    private readonly DispatcherTimer _gameTimer;
    private DateTime _gameStartTime = DateTime.MinValue;

    public ObservableCollection<SnowFlake> SnowFlakes { get; } = new();
    
    [ObservableProperty] private bool _isGameRunning;
    [ObservableProperty] private int _Score;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(SecondsGameDuration))]
    TimeSpan _GameDuration = TimeSpan.Zero;

    public TimeSpan TimeRemaining => _gameStartTime + GameDuration - DateTime.Now;
    public double SecondsRemaining => TimeRemaining.TotalSeconds;
    public double SecondsGameDuration => GameDuration.TotalSeconds;

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
        
        _gameStartTime = DateTime.Now;
        GameDuration = TimeSpan.FromMinutes(1);
        IsGameRunning = true;
        _gameTimer.Start();
    }
}