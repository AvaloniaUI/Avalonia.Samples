using System;
using Avalonia.Threading;

namespace Avalonia.BattleCity.Model
{
    public abstract class GameBase
    {
        public const int TicksPerSecond = 60;
        public long CurrentTick { get; private set; }
        private readonly DispatcherTimer _timer = new DispatcherTimer() {Interval = new TimeSpan(0, 0, 0, 0, 1000/TicksPerSecond)};


        void DoTick()
        {
            Tick();
            CurrentTick++;
        }

        protected abstract void Tick();

        protected GameBase()
        {
            _timer.Tick += delegate { DoTick(); };
        }

        public void Start() => _timer.IsEnabled = true;
        public void Stop() => _timer.IsEnabled = false;
    }
}