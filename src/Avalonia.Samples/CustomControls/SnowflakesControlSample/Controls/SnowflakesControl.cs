using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using SnowflakesControlSample.Models;

namespace SnowflakesControlSample.Controls;

/// <summary>
/// A control to render some <see cref="Snowflake">Snowflakes</see>. This control also adds the needed interaction logic
/// for the game to operate.
/// </summary>
public class SnowflakesControl : Control
{
    static SnowflakesControl()
    {
        // Make sure Render is updated whenever one of these properties changes
        AffectsRender<SnowflakesControl>(IsRunningProperty, SnowflakesProperty, ScoreProperty);
    }

    // We use a stopwatch to measure elapsed time between two render loops as it has higher precision compared to 
    // other options. See: https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.stopwatch
    private readonly Stopwatch _stopwatch = new();

    // A collection which holds all current visible score infos to render.
    private readonly ICollection<ScoreHint> _scoreHintsCollection = new Collection<ScoreHint>();

    /// <summary>
    /// Defines the <see cref="Snowflakes"/> property.
    /// </summary>
    public static readonly DirectProperty<SnowflakesControl, IList<Snowflake>> SnowflakesProperty =
        AvaloniaProperty.RegisterDirect<SnowflakesControl, IList<Snowflake>>(
            nameof(Snowflakes),
            o => o.Snowflakes,
            (o, v) => o.Snowflakes = v);

    /// <summary>
    /// Defines the <see cref="Score"/> property.
    /// </summary>
    public static readonly DirectProperty<SnowflakesControl, int> ScoreProperty =
        AvaloniaProperty.RegisterDirect<SnowflakesControl, int>(
            nameof(Score),
            o => o.Score,
            (o, v) => o.Score = v,
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Defines the <see cref="IsRunning"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> IsRunningProperty =
        AvaloniaProperty.Register<SnowflakesControl, bool>(nameof(IsRunning));

    private IList<Snowflake> _snowflakes = [];

    /// <summary>
    /// Gets or sets a List of  <see cref="Snowflake">Snowflakes</see> to render.
    /// </summary>
    public IList<Snowflake> Snowflakes
    {
        get => _snowflakes;
        set => SetAndRaise(SnowflakesProperty, ref _snowflakes, value);
    }

    private int _score;

    /// <summary>
    /// Gets or sets the current user score.
    /// </summary>
    public int Score
    {
        get => _score;
        set => SetAndRaise(ScoreProperty, ref _score, value);
    }

    /// <summary>
    /// Gets or sets a bool indicating if the Game whether the game is currently running.
    /// </summary>
    public bool IsRunning
    {
        get { return GetValue(IsRunningProperty); }
        set { SetValue(IsRunningProperty, value); }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        // If IsRunning is updated, we need to start or stop the stopwatch accordingly.
        if (change.Property == IsRunningProperty)
        {
            if (change.GetNewValue<bool>())
            {
                // Resets and starts the stopwatch
                _stopwatch.Restart();

                // Clear any previous score hints
                _scoreHintsCollection.Clear();
            }
            else
            {
                _stopwatch.Stop();
            }
        }
    }

    /// <inheritdoc />
    public override void Render(DrawingContext context)
    {
        // Figure out how much time elapsed since last render loop
        var elapsedMilliseconds = _stopwatch.Elapsed.TotalMilliseconds;

        foreach (var snowFlake in Snowflakes)
        {
            // If the game is running, move each flake to it's updated position
            if (IsRunning)
            {
                snowFlake.Move(elapsedMilliseconds);
            }

            // Draw the snowflake (we use a simple circle here)
            context.DrawEllipse(
                Brushes.White,
                null,
                snowFlake.GetCenterForViewport(Bounds),
                snowFlake.Radius,
                snowFlake.Radius);
        }

        // Bonus 1: Use a custom renderer using Skia-API to display the total score
        context.Custom(new ScoreRenderer(Bounds, $"Your score: {Score:N0}"));

        // Bonus 2: Render the score hint if any available.
        foreach (var scoreHint in _scoreHintsCollection.ToArray())
        {
            // If the game is running, move each flake to it's updated position
            if (IsRunning)
            {
                scoreHint.Update(elapsedMilliseconds);
            }

            // Use a formatted text to render the score hint.
            var formattedText =
                new FormattedText(
                    scoreHint.ToString(),
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    Typeface.Default,
                    20,
                    new SolidColorBrush(Colors.Yellow, scoreHint.Opacity));

            context.DrawText(formattedText,
                scoreHint.GetTopLeftForViewport(Bounds, new Size(formattedText.Width, formattedText.Height)));
        }

        base.Render(context);

        // Request the next frame as soon as possible if the game is running. Remember to reset the stopwatch.
        if (IsRunning)
        {
            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
            _stopwatch.Restart();
        }
    }

    /// <inheritdoc />
    protected override void OnPointerEntered(PointerEventArgs e)
    {
        HitTestSnowFlakes(e.GetPosition(this));

        base.OnPointerEntered(e);
    }

    /// <inheritdoc />
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        HitTestSnowFlakes(e.GetPosition(this));

        base.OnPointerMoved(e);
    }

    /// <summary>
    /// This method will check if the pointer has hit any snowflake. If so, it will remove the snowflake from the list
    /// and update the score.
    /// </summary>
    /// <param name="point">the pointer point to test.</param>
    private void HitTestSnowFlakes(Point point)
    {
        // if the game is not running, we don't need to do anything.
        if (!IsRunning) return;

        // loop through all snowflakes and check if the pointer is inside one of them.
        // Copy the list to avoid concurrent modification exceptions.
        foreach (var snowFlake in Snowflakes.ToArray())
        {
            if (snowFlake.IsHit(point, Bounds))
            {
                Snowflakes.Remove(snowFlake);
                Score += snowFlake.GetHitScore();

                // Add a text hint about the earned score. We also hand over the containing collection,
                // so it can auto-remove itself after 1 second.
                _scoreHintsCollection.Add(new ScoreHint(snowFlake, _scoreHintsCollection));
            }
        }
    }
}