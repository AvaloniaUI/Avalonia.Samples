using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SnowflakesControlSample.Models;

namespace SnowflakesControlSample.ViewModels;

/// <summary>
/// The ViewModel to control the game.
/// </summary>
public partial class SnowflakeGameViewModel : ViewModelBase
{
    private readonly DispatcherTimer _gameTimer;
    private readonly Stopwatch _stopwatch = new Stopwatch();
    
    public SnowflakeGameViewModel()
    {
        // Create a DispatcherTimer to update the game progress every 100 ms. 
        _gameTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(100), DispatcherPriority.Background, OnGameTimerTick);
    }
    
    private void OnGameTimerTick(object? sender, EventArgs e)
    {
        // Update the remaining milliseconds left. 
        OnPropertyChanged(nameof(MillisecondsRemaining));

        // If no more time is left, stop the timer and stopwatch and stop the game.  
        if (MillisecondsRemaining <= 0)
        {
            _gameTimer.Stop();
            _stopwatch.Stop();
            IsGameRunning = false;
        }
    }
    
    /// <summary>
    /// Gets a collection of initial Snowflakes available. 
    /// </summary>
    public ObservableCollection<Snowflake> Snowflakes { get; } = new();
    
    /// <summary>
    /// Gets or sets if the game is running or stopped. 
    /// </summary>
    [ObservableProperty] private bool _isGameRunning;
    
    /// <summary>
    /// Gets or sets the users hit-score. 
    /// </summary>
    [ObservableProperty] private int _score;

    /// <summary>
    /// Gets or sets the TimeSpan that the game is running. 
    /// </summary>
    /// <remarks>Also updates <see cref="GameDurationMilliseconds"/> if this property is updated.</remarks>
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(GameDurationMilliseconds))]
    private TimeSpan _gameDuration = TimeSpan.Zero;

    /// <summary>
    /// Gets the calculated time remaining in ms.
    /// </summary>
    public double MillisecondsRemaining => (GameDuration - _stopwatch.Elapsed).TotalMilliseconds;
    
    /// <summary>
    /// Gets the total game time in ms. 
    /// </summary>
    public double GameDurationMilliseconds => GameDuration.TotalMilliseconds;

    /// <summary>
    /// This command starts a new game session.
    /// </summary>
    [RelayCommand]
    private void StartGame()
    {
        // Clear all snowflakes.
        Snowflakes.Clear();
        
        // Reset game score.
        Score = 0;
        
        // Add some snowflakes at random positions, having random diameters and speed. 
        for (int i = 0; i < 200; i++)
        {
            Snowflakes.Add(new Snowflake(
                Random.Shared.NextDouble(), 
                Random.Shared.NextDouble(), 
                Random.Shared.NextDouble() * 5 + 2,
                Random.Shared.NextDouble() / 5 + 0.1));
        }
        
        // Set game time.
        GameDuration = TimeSpan.FromMinutes(1);
        
        // Indicate that game has started.
        IsGameRunning = true;
        
        // Start the timer and stopwatch.
        _stopwatch.Restart();
        _gameTimer.Start();
    }
}