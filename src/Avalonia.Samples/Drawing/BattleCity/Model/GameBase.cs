using System;
using Avalonia.Threading;

namespace BattleCity.Model;

public abstract class GameBase
{
    public const int TicksPerSecond = 60;
    private readonly DispatcherTimer _timer = new() { Interval = new TimeSpan(0, 0, 0, 0, 1000 / TicksPerSecond) };

    protected GameBase()
    {
        _timer.Tick += delegate { DoTick(); };
    }

    public long CurrentTick { get; private set; }


    private void DoTick()
    {
        Tick();
        CurrentTick++;
    }

    protected abstract void Tick();

    public void Start()
    {
        _timer.IsEnabled = true;
    }

    public void Stop()
    {
        _timer.IsEnabled = false;
    }
}